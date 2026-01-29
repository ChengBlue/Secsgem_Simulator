using System;
using System.Collections.Generic;

namespace SecsGemSimulator.Core
{
    /// <summary>
    /// SECS-II 请求的自动回复规则。收到带 W-bit 的请求时，根据 Stream/Function 构造并发送对应回复。
    /// </summary>
    public static class SecsAutoReply
    {
        /// <summary>
        /// 设备型号 / 软件版本，可用于 S1F2 等回复（当前 S1F2 仅返回 "ONLINE"）。
        /// </summary>
        public static string EquipmentModel { get; set; } = "SecsGemSimulator";
        public static string SoftwareRevision { get; set; } = "V1.0";

        /// <summary>
        /// 若为支持的请求则构造回复消息并返回，否则返回 null。不发送，由调用方 Send。
        /// </summary>
        public static SecsMessage TryBuildReply(SecsMessage req)
        {
            if (req == null || req.SType != 0) return null;
            if (!req.ReplyExpected) return null;

            byte stream = req.Stream;
            byte function = req.Function;
            // 仅处理 primary（奇 function）请求
            if ((function & 1) == 0) return null;

            byte replyFunction = (byte)(function + 1);
            SecsItem body = null;

            if (stream == 1 && function == 1)
            {
                // S1F1 Are You There -> S1F2 Online Data: <L <A "ONLINE"> >
                body = new SecsList(new List<SecsItem> { new SecsAscii("ONLINE") });
            }
            else if (stream == 1 && function == 13)
            {
                // S1F13 Establish Communication Request -> S1F14 Establish Communication Acknowledge
                // HMACK: 0=Accepted, 1=Denied, 2=Not supported. 使用 0。
                body = new SecsList(new List<SecsItem> { new SecsBinary(new byte[] { 0 }) });
            }
            else if (stream == 1 && function == 15)
            {
                // S1F15 Request Offline -> S1F16 Offline Acknowledge. MDACK: 0=Accepted.
                body = new SecsList(new List<SecsItem> { new SecsBinary(new byte[] { 0 }) });
            }
            else if (stream == 2 && function == 41)
            {
                // S2F41 Host Command Send -> S2F42 Host Command Acknowledge. HCACK=0, 0.
                body = new SecsList(new List<SecsItem>
                {
                    new SecsBinary(new byte[] { 0 }),
                    new SecsBinary(new byte[] { 0 })
                });
            }
            else if (stream == 5 && function == 1)
            {
                // S5F1 Alarm Report Send -> S5F2 Alarm Report Acknowledge.
                body = new SecsList(new List<SecsItem> { new SecsBinary(new byte[] { 0 }) });
            }
            else
            {
                return null;
            }

            var rsp = new SecsMessage
            {
                DeviceId = req.DeviceId,
                Stream = stream,
                Function = replyFunction,
                ReplyExpected = false,
                SType = 0,
                SystemBytes = req.SystemBytes,
                Root = body
            };
            return rsp;
        }
    }
}
