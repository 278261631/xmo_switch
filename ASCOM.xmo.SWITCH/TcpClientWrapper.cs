using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.Text;


namespace ASCOM.xmo
{
    public class TcpClientWrapper : IDisposable
    {
        private TcpClient client;
        private NetworkStream stream;

        public TcpClientWrapper(string serverIp, int serverPort)
        {
            client = new TcpClient();
            client.Connect(serverIp, serverPort);
            stream = client.GetStream();
        }

        public void Send(string message)
        {
            byte[] dataToSend = Encoding.ASCII.GetBytes(message);
            stream.Write(dataToSend, 0, dataToSend.Length);
        }

        public string Receive()
        {
            byte[] dataReceived = new byte[1024];
            int bytesRead = stream.Read(dataReceived, 0, dataReceived.Length);
            return Encoding.ASCII.GetString(dataReceived, 0, bytesRead);
        }

        public void Disconnect()
        {
            stream.Close();
            client.Close();
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
