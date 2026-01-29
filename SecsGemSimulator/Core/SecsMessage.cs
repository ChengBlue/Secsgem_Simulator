using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecsGemSimulator.Core
{
    public class SecsMessage
    {
        public ushort DeviceId { get; set; }
        public byte Stream { get; set; }
        public byte Function { get; set; }
        public bool ReplyExpected { get; set; }
        public byte SType { get; set; } = 0; // 0=Data, 1=Select.Req, 2=Select.Rsp, etc.
        public uint SystemBytes { get; set; }
        public SecsItem Root { get; set; }

        // HSMS Header is 10 bytes
        // SessionID (2) | HeaderByte2 (1) | HeaderByte3 (1) | PType (1) | SType (1) | SystemBytes (4)
        
        // HeaderByte2 = W-bit (MSB) + Stream (7 bits)
        // HeaderByte3 = Function

        public byte[] ToBytes()
        {
            List<byte> buffer = new List<byte>();
            
            // 1. Session ID (Device ID)
            byte[] devId = BitConverter.GetBytes(DeviceId);
            if (BitConverter.IsLittleEndian) Array.Reverse(devId);
            buffer.AddRange(devId);

            // 2. Header Byte 2 (W + Stream)
            byte hb2 = (byte)(Stream & 0x7F);
            if (ReplyExpected) hb2 |= 0x80;
            buffer.Add(hb2);

            // 3. Header Byte 3 (Function)
            buffer.Add(Function);

            // 4. PType (0 for SECS-II)
            buffer.Add(0);

            // 5. SType (0 for Data)
            buffer.Add(SType);

            // 6. System Bytes
            byte[] sysBytes = BitConverter.GetBytes(SystemBytes);
            if (BitConverter.IsLittleEndian) Array.Reverse(sysBytes);
            buffer.AddRange(sysBytes);

            // Body
            if (Root != null)
            {
                buffer.AddRange(Root.ToBytes());
            }

            // HSMS Message Length (4 bytes) - Does NOT include the length bytes themselves
            int totalLength = buffer.Count;
            byte[] lenBytes = BitConverter.GetBytes(totalLength);
            if (BitConverter.IsLittleEndian) Array.Reverse(lenBytes);
            
            List<byte> finalMessage = new List<byte>();
            finalMessage.AddRange(lenBytes);
            finalMessage.AddRange(buffer);
            
            return finalMessage.ToArray();
        }

        public static SecsMessage FromBytes(byte[] data)
        {
            // Data includes Length (4 bytes) + Header (10 bytes) + Body
            // Assume the input 'data' starts with the Header (10 bytes). Length should be handled by the receiver.
            // If the input has length bytes at 0-3, we skip them.
            
            int offset = 0;
            if (data.Length < 10) throw new ArgumentException("Data too short for SECS message");

            // Check if it includes length bytes (HSMS packet usually does on wire, but higher level might strip it)
            // Let's assume we pass the full packet including length if it looks like one, or just header+body.
            // A heuristic: if data.Length > 10 and data[0..3] as int equals data.Length-4
            
            // For simplicity, let's assume the passed data STARTS with the 10-byte header.
            // The receiver loop should strip the 4-byte length before calling this.
            
            SecsMessage msg = new SecsMessage();
            
            byte[] devIdBytes = new byte[2];
            Array.Copy(data, offset, devIdBytes, 0, 2);
            if (BitConverter.IsLittleEndian) Array.Reverse(devIdBytes);
            msg.DeviceId = BitConverter.ToUInt16(devIdBytes, 0);
            offset += 2;

            byte hb2 = data[offset++];
            msg.ReplyExpected = (hb2 & 0x80) != 0;
            msg.Stream = (byte)(hb2 & 0x7F);

            msg.Function = data[offset++];

            byte ptype = data[offset++];
            msg.SType = data[offset++];

            byte[] sysBytes = new byte[4];
            Array.Copy(data, offset, sysBytes, 0, 4);
            if (BitConverter.IsLittleEndian) Array.Reverse(sysBytes);
            msg.SystemBytes = BitConverter.ToUInt32(sysBytes, 0);
            offset += 4;

            // Body
            if (offset < data.Length)
            {
                // Decode remaining
                // We need to pass the array from offset
                byte[] body = new byte[data.Length - offset];
                Array.Copy(data, offset, body, 0, body.Length);
                
                int bodyOffset = 0;
                msg.Root = SecsItem.Decode(body, ref bodyOffset);
            }

            return msg;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"S{Stream}F{Function} {(ReplyExpected ? "W" : "")} [SystemBytes={SystemBytes}]");
            if (Root != null)
            {
                sb.Append(Root.ToString());
            }
            // 整体消息以句点结束（例如最后一行形如 ">."）
            if (sb.Length > 0 && sb[sb.Length - 1] != '.')
            {
                sb.Append(".");
            }
            return sb.ToString();
        }
    }
}
