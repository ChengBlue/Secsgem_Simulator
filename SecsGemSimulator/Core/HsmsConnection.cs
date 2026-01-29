using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SecsGemSimulator.Config;
using SecsGemSimulator.Helpers;

namespace SecsGemSimulator.Core
{
    public class HsmsConnection
    {
        private TcpClient _client;
        private TcpListener _listener;
        private NetworkStream _stream;
        private bool _isRunning;
        private Thread _receiveThread;
        private AppConfig _config;

        public event Action<SecsMessage> OnMessageReceived;
        public event Action<string> OnConnected;
        public event Action<string> OnDisconnected;
        public event Action<string> OnError;

        public bool IsConnected => _client != null && _client.Connected;

        public HsmsConnection(AppConfig config)
        {
            _config = config;
        }

        public async Task StartAsync()
        {
            _isRunning = true;
            try
            {
                if (_config.IsActiveMode)
                {
                    LogHelper.Info($"Connecting to {_config.IpAddress}:{_config.Port}...");
                    _client = new TcpClient();
                    await _client.ConnectAsync(_config.IpAddress, _config.Port);
                    HandleConnection(_client);
                }
                else
                {
                    LogHelper.Info($"Listening on {_config.IpAddress}:{_config.Port}...");
                    IPAddress ip = _config.IpAddress == "0.0.0.0" ? IPAddress.Any : IPAddress.Parse(_config.IpAddress);
                    _listener = new TcpListener(ip, _config.Port);
                    _listener.Start();
                    
                    // Accept loop for passive mode
                    _ = Task.Run(async () =>
                    {
                        while (_isRunning)
                        {
                            try
                            {
                                var client = await _listener.AcceptTcpClientAsync();
                                LogHelper.Info($"Client connected from {client.Client.RemoteEndPoint}");
                                // For simplicity, we only handle one client at a time or the latest one
                                if (_client != null && _client.Connected)
                                {
                                    LogHelper.Warning("Dropping previous connection.");
                                    _client.Close();
                                }
                                HandleConnection(client);
                            }
                            catch (Exception ex)
                            {
                                if (_isRunning) LogHelper.Error("Listener error", ex);
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _isRunning = false;
                try { _listener?.Stop(); } catch { }
                OnError?.Invoke(ex.Message);
                LogHelper.Error("Start failed", ex);
            }
        }

        private void HandleConnection(TcpClient client)
        {
            _client = client;
            _stream = client.GetStream();
            OnConnected?.Invoke(client.Client.RemoteEndPoint?.ToString());
            
            _receiveThread = new Thread(ReceiveLoop);
            _receiveThread.IsBackground = true;
            _receiveThread.Start();

            // Send Select Request if Active
            if (_config.IsActiveMode)
            {
                SendSelectRequest();
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _listener?.Stop();
            _client?.Close();
            _stream?.Close();
            _listener = null;
            _client = null;
            _stream = null;
            OnDisconnected?.Invoke("Stopped");
        }

        public void Send(SecsMessage msg)
        {
            if (!IsConnected) return;
            try
            {
                byte[] data = msg.ToBytes();
                _stream.Write(data, 0, data.Length);
                LogHelper.Log(LogLevel.Tx, msg.ToString(), BitConverter.ToString(data));
            }
            catch (Exception ex)
            {
                LogHelper.Error("Send failed", ex);
                OnError?.Invoke("Send failed: " + ex.Message);
                Disconnect();
            }
        }

        private void SendSelectRequest()
        {
             var msg = new SecsMessage
             {
                 DeviceId = _config.DeviceId,
                 Stream = 0,
                 Function = 0,
                 ReplyExpected = false,
                 SType = 1, // Select.Req
                 SystemBytes = (uint)new Random().Next()
             };
             Send(msg);
        }

        private void SendSelectResponse(SecsMessage req)
        {
             var msg = new SecsMessage
             {
                 DeviceId = req.DeviceId,
                 Stream = 0,
                 Function = 0,
                 ReplyExpected = false,
                 SType = 2, // Select.Rsp
                 SystemBytes = req.SystemBytes
             };
             Send(msg);
        }

        private void ReceiveLoop()
        {
            byte[] lengthBuffer = new byte[4];
            while (_isRunning && IsConnected)
            {
                try
                {
                    // Read Length (4 bytes)
                    int bytesRead = 0;
                    while (bytesRead < 4)
                    {
                        int read = _stream.Read(lengthBuffer, bytesRead, 4 - bytesRead);
                        if (read == 0) throw new Exception("Remote closed connection");
                        bytesRead += read;
                    }

                    if (BitConverter.IsLittleEndian) Array.Reverse(lengthBuffer);
                    int length = BitConverter.ToInt32(lengthBuffer, 0);

                    // Read Message
                    byte[] messageBuffer = new byte[length];
                    bytesRead = 0;
                    while (bytesRead < length)
                    {
                        int read = _stream.Read(messageBuffer, bytesRead, length - bytesRead);
                        if (read == 0) throw new Exception("Remote closed connection");
                        bytesRead += read;
                    }

                    // Parse
                    var msg = SecsMessage.FromBytes(messageBuffer);
                    LogHelper.Log(LogLevel.Rx, msg.ToString(), BitConverter.ToString(messageBuffer));
                    
                    // Handle HSMS Control Messages
                    if (msg.SType == 1) // Select.Req
                    {
                         SendSelectResponse(msg);
                         LogHelper.Info("Auto-replied to Select.Req");
                    }
                    else if (msg.SType == 5) // Linktest.Req
                    {
                        var rsp = new SecsMessage
                        {
                            DeviceId = msg.DeviceId,
                            Stream = 0,
                            Function = 0,
                            ReplyExpected = false,
                            SType = 6,
                            SystemBytes = msg.SystemBytes
                        };
                        Send(rsp);
                        LogHelper.Info("Auto-replied to Linktest.Req");
                    }
                    else if (msg.SType == 0) // SECS-II Data
                    {
                        // 自动回复 S1F1→S1F2、S1F13→S1F14、S2F41→S2F42、S5F1→S5F2 等
                        var reply = SecsAutoReply.TryBuildReply(msg);
                        if (reply != null)
                        {
                            Send(reply);
                            LogHelper.Info($"Auto-replied to S{msg.Stream}F{msg.Function} -> S{reply.Stream}F{reply.Function}");
                        }
                    }

                    OnMessageReceived?.Invoke(msg);
                }
                catch (Exception ex)
                {
                    if (_isRunning)
                    {
                        LogHelper.Error("Receive error", ex);
                        OnError?.Invoke("Connection lost");
                        Disconnect();
                    }
                    break;
                }
            }
        }
        
        private void Disconnect()
        {
            try { _client?.Close(); } catch { }
            OnDisconnected?.Invoke("Connection Closed");
        }
    }
}
