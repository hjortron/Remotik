using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        private const uint KeyeventfKeyup = 0x0002;
        private const uint KeyeventfExtendedkey = 0x0001;

        static void Main(string[] args)
        {
            var hostEntry = Dns.GetHostEntry(Dns.GetHostName());

            if (hostEntry.AddressList.Length == 0) return;
            var ip = hostEntry.AddressList[2].ToString();          

            Console.WriteLine("Starting server...");         
            Task.Run(() => Server.StartServer(ip));
        }

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        public static void Press(byte key)
        {
            keybd_event(key, 0, KeyeventfExtendedkey | 0, 0);
        }
    }
}
