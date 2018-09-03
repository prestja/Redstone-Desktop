using System;
using System.Collections.Generic;
using System.Text;

namespace prestja
{
    public enum PacketType
    {
        response = 0,
        command = 2,
        login = 3
    }
    public class Packet
    {
        public int requestID;
        public PacketType type;
        public byte[] payload;
        public short pad;
        public int length
        {
            get
            {
                return 10 + payload.Length; // 4 bytes for request ID, 4 bytes for type, 2 bytes for pad
            }
        }
        public int serializedLength
        {
            get
            {
                return length + 4;
            }
        }

        public Packet(int requestID, PacketType type, byte[] payload) {
            this.requestID = requestID;
            this.type = type;
            this.payload = payload;
            pad = 0;          
        }

        public static Packet Format (PacketType type, string message)
        {
            Packet packet = new Packet(PacketUtils.nextRequestID, type, Encoding.ASCII.GetBytes(message));
            return packet;
        }

        public static Packet FormatLogin (string password)
        {
            return new Packet(PacketUtils.nextRequestID, PacketType.login, Encoding.ASCII.GetBytes(password));
        }

        public byte[] Serialize()
        {
            List<byte> bytes = new List<byte>(serializedLength);
            bytes.AddRange(BitConverter.GetBytes(length));
            bytes.AddRange(BitConverter.GetBytes(requestID));
            bytes.AddRange(BitConverter.GetBytes((int)type));
            bytes.AddRange(payload);
            bytes.AddRange(BitConverter.GetBytes(pad));
            return bytes.ToArray();
        }

        public static Packet DeSerialize(byte[] bytes)
        {
            byte[] a_length = new byte[4];
            Array.Copy(bytes, 0, a_length, 0, 4);
            int remainder = BitConverter.ToInt32(a_length, 0);
            byte[] a_requestID = new byte[4];
            byte[] a_type = new byte[4];
            byte[] payload = new byte[remainder - 10]; 
            Array.Copy(bytes, 4, a_requestID, 0, 4); // start at index 4, b/c length is not copied and can be implied
            Array.Copy(bytes, 8, a_type, 0, 4);
            Array.Copy(bytes, 12, payload, 0, payload.Length);
            int requestID = BitConverter.ToInt32(a_requestID, 0);
            PacketType type = (PacketType)BitConverter.ToInt32(a_type, 0);
            return new Packet(requestID, type, payload);
        }

        public override String ToString()
        {
            string r = "";
            byte[] serialized = Serialize();
            for (int i = 0; i < serialized.Length; i++)
            {
                r += i + ": " + serialized[i] + "\n";
            }
            return r;
        }
    }
}
