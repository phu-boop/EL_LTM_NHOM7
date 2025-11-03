using System;

namespace UdpDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Chọn chế độ chạy: ");
            Console.WriteLine("1. Server");
            Console.WriteLine("2. Client");
            Console.Write("Nhập lựa chọn (1/2): ");
            string choice = Console.ReadLine();

            if (choice == "1")
                Server.Start();
            else if (choice == "2")
                Client.Start();
            else
                Console.WriteLine("Lựa chọn không hợp lệ!");
        }
    }
}
