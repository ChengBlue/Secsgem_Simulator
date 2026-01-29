using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SecsGemSimulator.Core
{
    public enum SecsFormat : byte
    {
        List = 0x00,
        Binary = 0x20,
        Boolean = 0x24,
        ASCII = 0x40,
        JIS8 = 0x44, // Not commonly used but standard
        I8 = 0x60,
        I1 = 0x64,
        I2 = 0x68,
        I4 = 0x70,
        F8 = 0x80,
        F4 = 0x90,
        U8 = 0xA0,
        U1 = 0xA4,
        U2 = 0xA8,
        U4 = 0xB0
    }

    public abstract class SecsItem
    {
        public SecsFormat Format { get; protected set; }
        
        public abstract byte[] ToBytes();
        
        public static SecsItem Decode(byte[] buffer, ref int offset)
        {
            if (offset >= buffer.Length) throw new IndexOutOfRangeException("Buffer ended unexpectedly.");

            byte formatByte = buffer[offset];
            int lengthBytes = formatByte & 0x03;
            SecsFormat format = (SecsFormat)(formatByte & 0xFC);
            offset++;

            int length = 0;
            if (lengthBytes == 1)
            {
                length = buffer[offset];
                offset++;
            }
            else if (lengthBytes == 2)
            {
                byte[] lenBytes = new byte[2];
                Array.Copy(buffer, offset, lenBytes, 0, 2);
                if (BitConverter.IsLittleEndian) Array.Reverse(lenBytes);
                length = BitConverter.ToUInt16(lenBytes, 0);
                offset += 2;
            }
            else if (lengthBytes == 3)
            {
                 byte[] lenBytes = new byte[4]; // Pad to 4 for Int32
                 Array.Copy(buffer, offset, lenBytes, 1, 3); // Copy 3 bytes to index 1,2,3
                 if (BitConverter.IsLittleEndian) Array.Reverse(lenBytes);
                 length = BitConverter.ToInt32(lenBytes, 0);
                 offset += 3;
            }

            if (format == SecsFormat.List)
            {
                List<SecsItem> items = new List<SecsItem>();
                for (int i = 0; i < length; i++)
                {
                    items.Add(Decode(buffer, ref offset));
                }
                return new SecsList(items);
            }

            byte[] data = new byte[length];
            Array.Copy(buffer, offset, data, 0, length);
            offset += length;

            switch (format)
            {
                case SecsFormat.ASCII:
                    return new SecsAscii(data);
                case SecsFormat.Binary:
                    return new SecsBinary(data);
                case SecsFormat.U1:
                    return new SecsU1(data);
                case SecsFormat.U2:
                    return new SecsU2(data);
                case SecsFormat.U4:
                    return new SecsU4(data);
                case SecsFormat.I1:
                    return new SecsI1(data);
                case SecsFormat.I2:
                    return new SecsI2(data);
                case SecsFormat.I4:
                    return new SecsI4(data);
                default:
                    return new SecsBinary(data) { CustomFormat = format };
            }
        }
        
        protected byte[] EncodeHeader(int length)
        {
            List<byte> header = new List<byte>();
            byte formatByte = (byte)Format;
            
            if (length <= 0xFF)
            {
                formatByte |= 0x01;
                header.Add(formatByte);
                header.Add((byte)length);
            }
            else if (length <= 0xFFFF)
            {
                formatByte |= 0x02;
                header.Add(formatByte);
                byte[] len = BitConverter.GetBytes((ushort)length);
                if (BitConverter.IsLittleEndian) Array.Reverse(len);
                header.AddRange(len);
            }
            else
            {
                formatByte |= 0x03;
                header.Add(formatByte);
                byte[] len = BitConverter.GetBytes(length);
                if (BitConverter.IsLittleEndian) Array.Reverse(len);
                header.Add(len[1]); // Only 3 bytes
                header.Add(len[2]);
                header.Add(len[3]);
            }
            return header.ToArray();
        }

        public abstract override string ToString();
    }

    public class SecsList : SecsItem
    {
        public List<SecsItem> Items { get; private set; } = new List<SecsItem>();

        public SecsList(byte[] data)
        {
            Format = SecsFormat.List;
            int offset = 0;
            while (offset < data.Length)
            {
                Items.Add(SecsItem.Decode(data, ref offset));
            }
        }
        
        public SecsList(IEnumerable<SecsItem> items)
        {
            Format = SecsFormat.List;
            Items = new List<SecsItem>(items);
        }

        public override byte[] ToBytes()
        {
            List<byte> data = new List<byte>();
            foreach (var item in Items)
            {
                data.AddRange(item.ToBytes());
            }
            
            var header = EncodeHeader(Items.Count); // For List, length is number of elements
            // Wait, standard says length is bytes for headers? No, for List, it's number of elements.
            // Actually, for List (Format 0), the length byte(s) specify the number of elements in the list.
            
            List<byte> result = new List<byte>();
            result.AddRange(header);
            result.AddRange(data);
            return result.ToArray();
        }
        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            // 按 SML 习惯：首行输出 "<L"，末行单独输出 ">"，不在这里加句点
            sb.AppendLine("<L");
            foreach(var item in Items)
            {
                sb.AppendLine("  " + item.ToString().Replace("\n", "\n  "));
            }
            sb.Append(">");
            return sb.ToString();
        }
    }

    public class SecsAscii : SecsItem
    {
        public string Value { get; private set; }

        public SecsAscii(byte[] data)
        {
            Format = SecsFormat.ASCII;
            Value = Encoding.ASCII.GetString(data);
        }
        
        public SecsAscii(string value)
        {
            Format = SecsFormat.ASCII;
            Value = value;
        }

        public override byte[] ToBytes()
        {
            byte[] data = Encoding.ASCII.GetBytes(Value);
            var header = EncodeHeader(data.Length);
            
            List<byte> result = new List<byte>();
            result.AddRange(header);
            result.AddRange(data);
            return result.ToArray();
        }

        public override string ToString()
        {
            return $"<A \"{Value}\">";
        }
    }

    public class SecsBinary : SecsItem
    {
        public byte[] Value { get; private set; }
        public SecsFormat CustomFormat { get; set; } = SecsFormat.Binary;

        public SecsBinary(byte[] data)
        {
            Format = SecsFormat.Binary;
            Value = data;
        }

        public override byte[] ToBytes()
        {
            Format = CustomFormat;
            var header = EncodeHeader(Value.Length);
            
            List<byte> result = new List<byte>();
            result.AddRange(header);
            result.AddRange(Value);
            return result.ToArray();
        }

        public override string ToString()
        {
            return $"<B 0x{BitConverter.ToString(Value).Replace("-", "")}>";
        }
    }
    
    // Simplified Implementations for other types
    public class SecsU1 : SecsItem
    {
        public byte[] Values { get; private set; }
        public SecsU1(byte[] data) { Format = SecsFormat.U1; Values = data; }
        public override byte[] ToBytes() { return EncodeHeader(Values.Length).Concat(Values).ToArray(); }
        public override string ToString() { return $"<U1 {string.Join(" ", Values)}>"; }
    }
    
    public class SecsU2 : SecsItem
    {
        public ushort[] Values { get; private set; }
        public SecsU2(byte[] data) 
        { 
            Format = SecsFormat.U2; 
            Values = new ushort[data.Length / 2];
            for(int i=0; i<Values.Length; i++) 
            {
                byte[] val = new byte[2];
                Array.Copy(data, i*2, val, 0, 2);
                if (BitConverter.IsLittleEndian) Array.Reverse(val);
                Values[i] = BitConverter.ToUInt16(val, 0);
            }
        }
        public SecsU2(ushort[] values)
        {
            Format = SecsFormat.U2;
            Values = values;
        }
        public override byte[] ToBytes() 
        { 
            List<byte> data = new List<byte>();
            foreach(var v in Values)
            {
                byte[] b = BitConverter.GetBytes(v);
                if (BitConverter.IsLittleEndian) Array.Reverse(b);
                data.AddRange(b);
            }
            return EncodeHeader(data.Count).Concat(data).ToArray(); 
        }
        public override string ToString() { return $"<U2 {string.Join(" ", Values)}>"; }
    }
    
    public class SecsU4 : SecsItem
    {
        public uint[] Values { get; private set; }
        public SecsU4(byte[] data) 
        { 
            Format = SecsFormat.U4; 
            Values = new uint[data.Length / 4];
            for(int i=0; i<Values.Length; i++) 
            {
                byte[] val = new byte[4];
                Array.Copy(data, i*4, val, 0, 4);
                if (BitConverter.IsLittleEndian) Array.Reverse(val);
                Values[i] = BitConverter.ToUInt32(val, 0);
            }
        }
        public SecsU4(uint[] values)
        {
            Format = SecsFormat.U4;
            Values = values;
        }
        public override byte[] ToBytes() 
        { 
            List<byte> data = new List<byte>();
            foreach(var v in Values)
            {
                byte[] b = BitConverter.GetBytes(v);
                if (BitConverter.IsLittleEndian) Array.Reverse(b);
                data.AddRange(b);
            }
            return EncodeHeader(data.Count).Concat(data).ToArray(); 
        }
        public override string ToString() { return $"<U4 {string.Join(" ", Values)}>"; }
    }

    public class SecsI1 : SecsItem
    {
        public sbyte[] Values { get; private set; }
        public SecsI1(byte[] data)
        {
            Format = SecsFormat.I1;
            Values = Array.ConvertAll(data, b => (sbyte)b);
        }
        public SecsI1(sbyte[] values)
        {
            Format = SecsFormat.I1;
            Values = values;
        }
        public override byte[] ToBytes()
        {
            byte[] data = Array.ConvertAll(Values, v => (byte)v);
            return EncodeHeader(data.Length).Concat(data).ToArray();
        }
        public override string ToString() { return $"<I1 {string.Join(" ", Values)}>"; }
    }

    public class SecsI2 : SecsItem
    {
        public short[] Values { get; private set; }
        public SecsI2(byte[] data)
        {
            Format = SecsFormat.I2;
            Values = new short[data.Length / 2];
            for (int i = 0; i < Values.Length; i++)
            {
                byte[] val = new byte[2];
                Array.Copy(data, i * 2, val, 0, 2);
                if (BitConverter.IsLittleEndian) Array.Reverse(val);
                Values[i] = BitConverter.ToInt16(val, 0);
            }
        }
        public SecsI2(short[] values)
        {
            Format = SecsFormat.I2;
            Values = values;
        }
        public override byte[] ToBytes()
        {
            List<byte> data = new List<byte>();
            foreach (var v in Values)
            {
                byte[] b = BitConverter.GetBytes(v);
                if (BitConverter.IsLittleEndian) Array.Reverse(b);
                data.AddRange(b);
            }
            return EncodeHeader(data.Count).Concat(data).ToArray();
        }
        public override string ToString() { return $"<I2 {string.Join(" ", Values)}>"; }
    }

    public class SecsI4 : SecsItem
    {
        public int[] Values { get; private set; }
        public SecsI4(byte[] data)
        {
            Format = SecsFormat.I4;
            Values = new int[data.Length / 4];
            for (int i = 0; i < Values.Length; i++)
            {
                byte[] val = new byte[4];
                Array.Copy(data, i * 4, val, 0, 4);
                if (BitConverter.IsLittleEndian) Array.Reverse(val);
                Values[i] = BitConverter.ToInt32(val, 0);
            }
        }
        public SecsI4(int[] values)
        {
            Format = SecsFormat.I4;
            Values = values;
        }
        public override byte[] ToBytes()
        {
            List<byte> data = new List<byte>();
            foreach (var v in Values)
            {
                byte[] b = BitConverter.GetBytes(v);
                if (BitConverter.IsLittleEndian) Array.Reverse(b);
                data.AddRange(b);
            }
            return EncodeHeader(data.Count).Concat(data).ToArray();
        }
        public override string ToString() { return $"<I4 {string.Join(" ", Values)}>"; }
    }
}
