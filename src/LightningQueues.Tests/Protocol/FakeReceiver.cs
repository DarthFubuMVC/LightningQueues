using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using LightningQueues.Protocol;

namespace LightningQueues.Tests.Protocol
{
    public class FakeReceiver : IDisposable
    {
        private readonly TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Loopback, 23456));

        public bool DisconnectAfterConnect;
        public bool DisconnectDuringMessageSend;
        public bool DisconnectAfterMessageSend;
        public bool SendBadResponse;
        public bool DisconnectAfterSendingReciept;
        public bool FailOnAcknowledgement;
        public bool AcceptConnections = true;
        public bool TimeoutOnReceive;

        public void Start()
        {
            if (AcceptConnections)
            {
                listener.Start();
                listener.BeginAcceptTcpClient(BeginAcceptTcpClientCallback, null);
            }
        }

        private void BeginAcceptTcpClientCallback(IAsyncResult result)
        {
            using (var client = listener.EndAcceptTcpClient(result))
            {
                if (DisconnectAfterConnect)
                    return;

                if (TimeoutOnReceive)
                    Thread.Sleep(1000);

                using (var stream = client.GetStream())
                {
                    var buffer = new byte[4];
                    var index = 0;
                    while (index < buffer.Length)
                    {
                        var read = stream.Read(buffer, index, buffer.Length - index);
                        index += read;
                    }
                    var len = BitConverter.ToInt32(buffer, 0);

                    if (DisconnectDuringMessageSend)
                        return;

                    buffer = new byte[len];
                    index = 0;
                    while (index < buffer.Length)
                    {
                        var read = stream.Read(buffer, index, buffer.Length - index);
                        index += read;
                    }

                    if (DisconnectAfterMessageSend)
                        return;

                    if (SendBadResponse)
                    {
                        buffer = Encoding.Unicode.GetBytes("BAD_DATA");
                        stream.Write(buffer, 0, buffer.Length);
                        return;
                    }

                    stream.Write(ProtocolConstants.RecievedBuffer, 0, ProtocolConstants.RecievedBuffer.Length);

                    if (DisconnectAfterSendingReciept)
                        return;

                    buffer = new byte[ProtocolConstants.AcknowledgedBuffer.Length];
                    stream.Read(buffer, 0, buffer.Length);

                    if (FailOnAcknowledgement)
                    {
                        stream.Write(ProtocolConstants.RevertBuffer, 0, ProtocolConstants.RevertBuffer.Length);
                    }
                }
            }
        }

        public void Dispose()
        {
            listener.Stop();
        }
    }
}