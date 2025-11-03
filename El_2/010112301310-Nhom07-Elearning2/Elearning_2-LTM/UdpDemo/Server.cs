using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UdpDemo
{
    internal class Server
    {
        private const int Port = 5005;
        private const double PacketLossRate = 0.2; // 20% giả lập mất gói

        public static void Start()
        {
            using var server = new UdpClient(Port);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[SERVER] Listening on port {Port}...");
            Console.ResetColor();

            var clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                try
                {
                    byte[] receivedData = server.Receive(ref clientEndPoint);
                    string rawMessage = Encoding.UTF8.GetString(receivedData);

                    if (string.IsNullOrWhiteSpace(rawMessage))
                        continue;

                    string[] parts = rawMessage.Split('|');
                    if (parts.Length < 2)
                        continue;

                    int seqNumber = int.Parse(parts[0]);
                    string payload = parts[1];

                    // Mô phỏng mất gói
                    if (Random.Shared.NextDouble() < PacketLossRate)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"[LOSS] Gói {seqNumber} bị bỏ qua.");
                        Console.ResetColor();
                        continue;
                    }

                    // Hiển thị dữ liệu nhận được
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[RECV] Gói {seqNumber}: {payload}");
                    Console.ResetColor();

                    // Gửi ACK lại client
                    string ackMsg = $"ACK|{seqNumber}";
                    byte[] ackData = Encoding.UTF8.GetBytes(ackMsg);
                    server.Send(ackData, ackData.Length, clientEndPoint);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[ERROR] {ex.Message}");
                    Console.ResetColor();
                }
            }
        }
    }
}
