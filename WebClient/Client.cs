using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace WebClient
{
    public class Client
    {
        private readonly string _serverIp;
        private readonly int _serverPort;

        public Client(string serverIp, int serverPort)
        {
            _serverIp = serverIp;
            _serverPort = serverPort;
        }

        public async Task Start()
        {
            try
            {
                Console.WriteLine("Connecting to the server...");
                using (var tcpClient = new TcpClient(_serverIp, _serverPort))
                {
                    Console.WriteLine("Connected");

                    await SendImage(tcpClient.GetStream(), "../../../Images/image.jpg");

                    await ReceiveMessage(tcpClient.GetStream());

                    while (true)
                    {
                        Console.Write("Enter the string to be transmitted (or 'exit' to quit): ");
                        string message = Console.ReadLine() ?? throw new Exception();
                        if (message.ToLower() == "exit")
                            break;

                        await SendMessage(tcpClient.GetStream(), message);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }

        private async Task SendMessage(NetworkStream stream, string message)
        {
            try
            {
                Console.WriteLine("Transmitting message...");
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                await stream.WriteAsync(buffer, 0, buffer.Length);

                await ReceiveMessage(stream);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending message: " + ex.Message);
            }
        }

        private async Task ReceiveMessage(NetworkStream stream)
        {
            byte[] responseBuffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(responseBuffer, 0, responseBuffer.Length);
            string response = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);
            Console.WriteLine("Response from server: " + response);
        }

        private async Task SendImage(NetworkStream stream, string path)
        {
            MemoryStream ms = new MemoryStream();
            try
            {
                Console.WriteLine("Sending image to server...");
                using (FileStream fs = File.OpenRead(path))
                {
                    fs.CopyTo(ms);
                }
                await stream.WriteAsync(ms.ToArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending image: " + ex.Message);
            }
        }
    }
}
