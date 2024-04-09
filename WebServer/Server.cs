using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Drawing;
using System.IO;
using System;

namespace WebServer
{
    public class Server
    {
        private readonly TcpListener _tcpListener;

        public Server(string ipAdress, int port)
        {
            var hostAddress = IPAddress.Parse(ipAdress);
            _tcpListener = new TcpListener(hostAddress, port);
        }

        public async Task Start()
        {
            _tcpListener.Start();
            Console.WriteLine($"Server running at {_tcpListener.LocalEndpoint}");

            while (true)
            {
                TcpClient client = await _tcpListener.AcceptTcpClientAsync();
                Console.WriteLine("Client connected.");

                await HandleClientAsync(client);
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            using (NetworkStream stream = client.GetStream())
            {
                byte[] buffer = new byte[4096];
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await RecieveImage(buffer, bytesRead);
                    await SendMessageToClient(stream, "Done");
                }
            }

            await SendMessageToClient(client.GetStream(), "Message Received");
        }


        private async Task RecieveImage(byte[] buffer, int bytesRead)
        {
            using (MemoryStream ms = new MemoryStream(buffer, 0, bytesRead))
            {
                using (FileStream file = new FileStream("../../../Images/image.jpg", FileMode.Append, FileAccess.Write))
                {
                    await ms.CopyToAsync(file);
                }
            }
        }

        private async Task SendMessageToClient(NetworkStream stream, string message)
        {
            byte[] responseBuffer = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(responseBuffer);
        }
    }
}
