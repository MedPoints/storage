using System;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Storage.Utils;

namespace Storage.Network
{
    internal class WebSocketRemoteNode : RemoteNode
    {
        private readonly WebSocket _socket;
        private readonly bool _connected;
        private int _disposed;

        public WebSocketRemoteNode(LocalNode localNode, WebSocket socket, IPEndPoint remoteEndpoint)
            : base(localNode)
        {
            _socket = socket;
            RemoteEndpoint = new IPEndPoint(remoteEndpoint.Address.MapToIPv6(), remoteEndpoint.Port);
            _connected = true;
        }

        public override void Disconnect(bool error)
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                _socket.Dispose();
                base.Disconnect(error);
            }
        }

        protected override async Task<Message> ReceiveMessageAsync(TimeSpan timeout)
        {
            CancellationTokenSource source = new CancellationTokenSource(timeout);
            try
            {
                return await Message.DeserializeFromAsync(_socket, source.Token);
            }
            catch (ArgumentException) { }
            catch (ObjectDisposedException) { }
            catch (Exception ex) when (ex is FormatException || ex is IOException || ex is WebSocketException || ex is OperationCanceledException)
            {
                Disconnect(false);
            }
            finally
            {
                source.Dispose();
            }
            return null;
        }

        protected override async Task<bool> SendMessageAsync(Message message)
        {
            if (!_connected) throw new InvalidOperationException();
            if (_disposed > 0) return false;
            ArraySegment<byte> segment = new ArraySegment<byte>(message.ToArray());
            CancellationTokenSource source = new CancellationTokenSource(10000);
            try
            {
                await _socket.SendAsync(segment, WebSocketMessageType.Binary, true, source.Token);
                return true;
            }
            catch (ObjectDisposedException) { }
            catch (Exception ex) when (ex is WebSocketException || ex is OperationCanceledException)
            {
                Disconnect(false);
            }
            finally
            {
                source.Dispose();
            }
            return false;
        }
    }
}
