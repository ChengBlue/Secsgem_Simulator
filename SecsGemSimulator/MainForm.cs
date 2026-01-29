using System;
using System.Drawing;
using System.Windows.Forms;
using SecsGemSimulator.Config;
using SecsGemSimulator.Core;
using SecsGemSimulator.Helpers;

namespace SecsGemSimulator
{
    public partial class MainForm : Form
    {
        private AppConfig _config;
        private HsmsConnection _hsms;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Load Config
            _config = AppConfig.Load();
            txtIp.Text = _config.IpAddress;
            txtPort.Text = _config.Port.ToString();
            txtDeviceId.Text = _config.DeviceId.ToString();
            chkActive.Checked = _config.IsActiveMode;

            // Setup Log
            LogHelper.OnLog += LogHelper_OnLog;

            // Setup Templates
            cmbTemplates.Items.Add("S1F1 Are You There");
            cmbTemplates.Items.Add("S1F13 Establish Comm");
            cmbTemplates.Items.Add("S2F41 Host Command");
            cmbTemplates.Items.Add("S5F1 Alarm Report Send");
            cmbTemplates.Items.Add("Custom");
            cmbTemplates.SelectedIndex = 0;
        }

        private void LogHelper_OnLog(LogEntry entry)
        {
            if (IsDisposed) return;
            Invoke(new Action(() =>
            {
                if (entry.Level == LogLevel.Tx)
                {
                    AppendLog($"[TX] {entry.Timestamp:HH:mm:ss.fff}: {entry.Message}", Color.Cyan);
                    if (chkHexMode.Checked && entry.Details != null) AppendLog($"     {entry.Details}", Color.DarkCyan);
                }
                else if (entry.Level == LogLevel.Rx)
                {
                    AppendLog($"[RX] {entry.Timestamp:HH:mm:ss.fff}: {entry.Message}", Color.Lime);
                    if (chkHexMode.Checked && entry.Details != null) AppendLog($"     {entry.Details}", Color.Green);
                }
                else if (entry.Level == LogLevel.Error)
                {
                    AppendLog($"[ERR] {entry.Message}", Color.Red);
                    if (!string.IsNullOrEmpty(entry.Details))
                        AppendLog($"     {entry.Details}", Color.OrangeRed);
                }
                else if (entry.Level == LogLevel.Warning)
                {
                    AppendLog($"[WRN] {entry.Message}", Color.Orange);
                }
                else
                {
                    AppendLog($"[INF] {entry.Message}", Color.White);
                }
            }));
        }

        private void AppendLog(string text, Color color)
        {
            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.SelectionLength = 0;
            rtbLog.SelectionColor = color;
            rtbLog.AppendText(text + "\r\n");
            rtbLog.ScrollToCaret();
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            // Passive 模式下“已启动监听但尚未有设备连接”时 IsConnected=false，
            // 仍应允许 Stop，而不是再次 Start 导致端口重复绑定(10048)。
            if (_hsms != null)
            {
                // Disconnect
                _hsms.Stop();
                _hsms = null;
                btnConnect.Text = "Start/Connect";
                SetNetworkControls(true);
            }
            else
            {
                // Connect
                // Update Config from UI
                _config.IpAddress = txtIp.Text;
                if (int.TryParse(txtPort.Text, out int port)) _config.Port = port;
                if (ushort.TryParse(txtDeviceId.Text, out ushort devId)) _config.DeviceId = devId;
                _config.IsActiveMode = chkActive.Checked;

                _hsms = new HsmsConnection(_config);
                _hsms.OnConnected += (ep) => LogHelper.Info($"Connected to {ep}");
                _hsms.OnError += (err) =>
                {
                    LogHelper.Error(err);
                    // 启动失败/监听失败时，回滚 UI 状态，避免按钮停留在 Stop
                    if (IsDisposed) return;
                    Invoke(new Action(() =>
                    {
                        btnConnect.Text = "Start/Connect";
                        SetNetworkControls(true);
                    }));
                    try { _hsms?.Stop(); } catch { }
                    _hsms = null;
                };
                _hsms.OnDisconnected += (reason) => 
                {
                     LogHelper.Info($"Disconnected: {reason}");
                     Invoke(new Action(() => 
                     {
                         btnConnect.Text = "Start/Connect";
                         SetNetworkControls(true);
                     }));
                };
                
                SetNetworkControls(false);
                btnConnect.Text = "Stop";
                
                await _hsms.StartAsync();
            }
        }

        private void SetNetworkControls(bool enabled)
        {
            txtIp.Enabled = enabled;
            txtPort.Enabled = enabled;
            txtDeviceId.Enabled = enabled;
            chkActive.Enabled = enabled;
        }

        private void btnSaveConfig_Click(object sender, EventArgs e)
        {
            _config.IpAddress = txtIp.Text;
            if (int.TryParse(txtPort.Text, out int port)) _config.Port = port;
            if (ushort.TryParse(txtDeviceId.Text, out ushort devId)) _config.DeviceId = devId;
            _config.IsActiveMode = chkActive.Checked;
            _config.Save();
            MessageBox.Show("Configuration Saved!");
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (_hsms == null || !_hsms.IsConnected)
            {
                MessageBox.Show("Not Connected!");
                return;
            }

            try
            {
                byte stream = byte.Parse(txtStream.Text);
                byte function = byte.Parse(txtFunction.Text);
                bool reply = chkReply.Checked;
                
                SecsItem root = null;
                string sml = txtBody.Text.Trim();
                if (!string.IsNullOrEmpty(sml))
                {
                    root = SmlParser.Parse(sml);
                }

                var msg = new SecsMessage
                {
                    DeviceId = _config.DeviceId,
                    Stream = stream,
                    Function = function,
                    ReplyExpected = reply,
                    SType = 0,
                    SystemBytes = (uint)Environment.TickCount, // Simple ID
                    Root = root
                };

                _hsms.Send(msg);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating message: " + ex.Message);
            }
        }

        private void cmbTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sel = cmbTemplates.SelectedItem.ToString();
            if (sel.Contains("S1F1 "))
            {
                txtStream.Text = "1"; txtFunction.Text = "1"; chkReply.Checked = true;
                txtBody.Text = ""; // Empty body usually
            }
            else if (sel.Contains("S1F13"))
            {
                txtStream.Text = "1"; txtFunction.Text = "13"; chkReply.Checked = true;
                txtBody.Text = "<L\n  <A \"SecsGemSimulator\">\n  <A \"V1.0\">\n>";
            }
            else if (sel.Contains("S2F41"))
            {
                txtStream.Text = "2"; txtFunction.Text = "41"; chkReply.Checked = true;
                txtBody.Text = "<L\n  <A \"START\">\n  <L\n    <L\n      <A \"PARAM1\">\n      <A \"VAL1\">\n    >\n  >\n>";
            }
            else if (sel.Contains("S5F1"))
            {
                txtStream.Text = "5"; txtFunction.Text = "1"; chkReply.Checked = true;
                txtBody.Text = "<L\n  <U1 128>\n  <U4 1001>\n  <A \"Alarm Text\">\n>";
            }
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            rtbLog.Clear();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            LogHelper.OnLog -= LogHelper_OnLog;
            _hsms?.Stop();
        }
    }
}
