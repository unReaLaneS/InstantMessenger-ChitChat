using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
            Console.WriteLine();
            Console.WriteLine("Press enter to close program.");
            Console.ReadLine();
        }

        private IPAddress ip = IPAddress.Parse("127.0.0.1");
        private int port = 2000;
        private bool running = true;
        private TcpListener server;
        private List<UserController> userControllers = new List<UserController>();

        public List<UserController> UserControllers
        {
            get { return userControllers; }
            set { userControllers = value; }
        }

        public Program()
        {
            Console.Title = "InstantMessenger Server";
            Console.WriteLine("----- InstantMessenger Server -----");
            Console.WriteLine("[{0}] Starting server...", DateTime.Now);

            server = new TcpListener(ip, port);
            server.Start();
            Console.WriteLine("[{0}] Server is running properly!", DateTime.Now);
            
            Listen();
        }

        private void Listen() 
        {
            while (running)
            {
                TcpClient tcpClient = server.AcceptTcpClient();
                UserController client = new UserController(this, tcpClient);
                userControllers.Add(client);
            }

            server.Stop();
        }

    }
}
