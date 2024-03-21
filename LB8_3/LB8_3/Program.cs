using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LB8_3
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener server = null;

            try
            {
                int port = 8050;

                server = new TcpListener(IPAddress.Any, port);
             
                server.Start();

                Console.WriteLine($"Сервер запущен на порту {port}");

                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();

                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                    clientThread.Start(client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            finally
            {
                server?.Stop();
            }
        }

        static void HandleClient(object obj)
        {
            TcpClient tcpClient = (TcpClient)obj;

            using (NetworkStream networkStream = tcpClient.GetStream())
            using (StreamReader reader = new StreamReader(networkStream))
            using (StreamWriter writer = new StreamWriter(networkStream))
            {
                string request = reader.ReadLine();
                Console.WriteLine($"Запрос от клиента: {request}");

                string htmlContent = "<!DOCTYPE html><html><body><h1>My first Heading</h1><p>My first paragraph</p></body></html>";
                string response = $"HTTP/1.1 200 OK\r\nContent-Type: text/html\r\nContent-Length: {htmlContent.Length}\r\n\r\n{htmlContent}";

                writer.WriteAsync(response);
                writer.Flush();
            }

            tcpClient.Close();
        }
    
    }
}
