using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace UdpDemo
{
    internal class Client
    {
        private const string ServerIp = "127.0.0.1";
        private const int ServerPort = 5005;
        private const int MaxRetries = 3;
        private const int TimeoutMs = 1000;

        public static void Start()
        {
            using var client = new UdpClient();
            client.Client.ReceiveTimeout = TimeoutMs;

            string[] messages =
            {
                "Xin chào",
                "Tối ưu UDP",
                "Mất gói mô phỏng",
                "ACK/NACK minh họa",
                "Kết thúc truyền"
            };

            for (int seq = 0; seq < messages.Length; seq++)
            {
                bool acked = false;
                int retries = 0;

                while (!acked && retries < MaxRetries)
                {
                    string packet = $"{seq}|{messages[seq]}";
                    byte[] data = Encoding.UTF8.GetBytes(packet);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[SEND] Gửi gói {seq}: {messages[seq]}");
                    Console.ResetColor();

                    client.Send(data, data.Length, ServerIp, ServerPort);

                    try
                    {
                        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                        byte[] ackData = client.Receive(ref remoteEP);
                        string ackMsg = Encoding.UTF8.GetString(ackData);

                        if (ackMsg.StartsWith("ACK|") && ackMsg.EndsWith(seq.ToString()))
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"[ACK] Nhận ACK cho gói {seq}");
                            Console.ResetColor();
                            acked = true;
                        }
                    }
                    catch (SocketException)
                    {
                        retries++;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[RETRY] Gửi lại gói {seq} (lần {retries})...");
                        Console.ResetColor();
                    }

                    Thread.Sleep(400); // giảm tải CPU, giúp log dễ nhìn hơn
                }

                if (!acked)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"[FAIL] Không nhận được ACK cho gói {seq} sau {MaxRetries} lần gửi.");
                    Console.ResetColor();
                }
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[CLIENT] Đã gửi xong tất cả gói tin.");
            Console.ResetColor();
        }
    }
}
