using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Storage.Utils;

namespace Storage.Network
{
    public class Message : ISerializable
    {
        private const int PayloadMaxSize = 0x02000000;

        public static readonly uint Magic = Settings.Default.Magic;
        public string Command;
        public uint Checksum;
        public byte[] Payload;

        public int Size => sizeof(uint) + 12 + sizeof(int) + sizeof(uint) + Payload.Length;

        public static Message Create(string command, ISerializable payload = null)
        {
            return Create(command, payload == null ? new byte[0] : payload.ToArray());
        }

        public static Message Create(string command, byte[] payload)
        {
            return new Message
            {
                Command = command,
                Checksum = GetChecksum(payload),
                Payload = payload
            };
        }

        void ISerializable.Deserialize(BinaryReader reader)
        {
            if (reader.ReadUInt32() != Magic)
                throw new FormatException();
            this.Command = reader.ReadFixedString(12);
            uint length = reader.ReadUInt32();
            if (length > PayloadMaxSize)
                throw new FormatException();
            this.Checksum = reader.ReadUInt32();
            this.Payload = reader.ReadBytes((int)length);
            if (GetChecksum(Payload) != Checksum)
                throw new FormatException();
        }

        public static async Task<Message> DeserializeFromAsync(Stream stream, CancellationToken cancellationToken)
        {
            uint payloadLength;
            byte[] buffer = await FillBufferAsync(stream, 24, cancellationToken);
            Message message = new Message();
            using (MemoryStream ms = new MemoryStream(buffer, false))
            using (BinaryReader reader = new BinaryReader(ms, Encoding.UTF8))
            {
                if (reader.ReadUInt32() != Magic)
                    throw new FormatException();
                message.Command = reader.ReadFixedString(12);
                payloadLength = reader.ReadUInt32();
                if (payloadLength > PayloadMaxSize)
                    throw new FormatException();
                message.Checksum = reader.ReadUInt32();
            }
            if (payloadLength > 0)
                message.Payload = await FillBufferAsync(stream, (int)payloadLength, cancellationToken);
            else
                message.Payload = new byte[0];
            if (GetChecksum(message.Payload) != message.Checksum)
                throw new FormatException();
            return message;
        }

        public static async Task<Message> DeserializeFromAsync(WebSocket socket, CancellationToken cancellationToken)
        {
            uint payloadLength;
            byte[] buffer = await FillBufferAsync(socket, 24, cancellationToken);
            Message message = new Message();
            using (MemoryStream ms = new MemoryStream(buffer, false))
            using (BinaryReader reader = new BinaryReader(ms, Encoding.UTF8))
            {
                if (reader.ReadUInt32() != Magic)
                    throw new FormatException();
                message.Command = reader.ReadFixedString(12);
                payloadLength = reader.ReadUInt32();
                if (payloadLength > PayloadMaxSize)
                    throw new FormatException();
                message.Checksum = reader.ReadUInt32();
            }
            if (payloadLength > 0)
                message.Payload = await FillBufferAsync(socket, (int)payloadLength, cancellationToken);
            else
                message.Payload = new byte[0];
            if (GetChecksum(message.Payload) != message.Checksum)
                throw new FormatException();
            return message;
        }

        private static async Task<byte[]> FillBufferAsync(Stream stream, int buffer_size, CancellationToken cancellationToken)
        {
            var maxSize = 1024;
            byte[] buffer = new byte[buffer_size < maxSize ? buffer_size : maxSize];
            using (MemoryStream ms = new MemoryStream())
            {
                while (buffer_size > 0)
                {
                    int count = buffer_size < maxSize ? buffer_size : maxSize;
                    count = await stream.ReadAsync(buffer, 0, count, cancellationToken);
                    if (count <= 0) throw new IOException();
                    ms.Write(buffer, 0, count);
                    buffer_size -= count;
                }
                return ms.ToArray();
            }
        }

        private static async Task<byte[]> FillBufferAsync(WebSocket socket, int buffer_size, CancellationToken cancellationToken)
        {
            var maxSize = 1024;
            byte[] buffer = new byte[buffer_size < maxSize ? buffer_size : maxSize];
            using (MemoryStream ms = new MemoryStream())
            {
                while (buffer_size > 0)
                {
                    int count = buffer_size < maxSize ? buffer_size : maxSize;
                    ArraySegment<byte> segment = new ArraySegment<byte>(buffer, 0, count);
                    WebSocketReceiveResult result = await socket.ReceiveAsync(segment, cancellationToken);
                    if (result.Count <= 0 || result.MessageType != WebSocketMessageType.Binary)
                        throw new IOException();
                    ms.Write(buffer, 0, result.Count);
                    buffer_size -= result.Count;
                }
                return ms.ToArray();
            }
        }

        private static uint GetChecksum(byte[] value)
        {
            //return Crypto.Default.Hash256(value).ToUInt32(0);
            throw new NotImplementedException();
        }

        void ISerializable.Serialize(BinaryWriter writer)
        {
            writer.Write(Magic);
            writer.WriteFixedString(Command, 12);
            writer.Write(Payload.Length);
            writer.Write(Checksum);
            writer.Write(Payload);
        }
    }
}
