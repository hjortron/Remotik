using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfApplication1
{
    public partial class MainWindow
    {
        private const uint KeyeventfKeyup = 0x0002;
        private const uint KeyeventfExtendedkey = 0x0001;
        private const byte VkVolumeMute = 0xAD; //Volume Mute key
        private const byte VkVolumeUp = 0xAF; //Volume Up key
        private const byte VkVolumeDown = 0xAE; //Volume Down key
        private const byte VkMediaNextTrack = 0xB0; //Next Track key
        private const byte VkMediaPrevTrack = 0xB1; //Previous Track key
        private const byte VkMediaStop = 0xB2; //Stop Media key
        private const byte VkMediaPlayPause = 0xB3; //Play/Pause Media key

        public MainWindow()
        {
            InitializeComponent();           

            var hostEntry = Dns.GetHostEntry(tbServerUri.Text);

            if (hostEntry.AddressList.Length == 0) return;
            var ip = hostEntry.AddressList[2].ToString();
            label.Content = ip;

            Client.ConnectAsync(ip);
        }       

        private void button_prev_Click(object sender, RoutedEventArgs e)
        {
            Client.HubProxy.Invoke("Send", VkMediaPrevTrack);
        }

        private void button_next_Click(object sender, RoutedEventArgs e)
        {
            Client.HubProxy.Invoke("Send", VkMediaNextTrack);
        }

        private void button_play_Click(object sender, RoutedEventArgs e)
        {
            Client.HubProxy.Invoke("Send", VkMediaPlayPause);
        }

        private void button_stop_Click(object sender, RoutedEventArgs e)
        {
            Client.HubProxy.Invoke("Send", VkMediaStop);
        }                         

        public void WriteToConsole(string message)
        {
            if (!(textBlock.CheckAccess()))
            {
                Dispatcher.Invoke(() =>
                    WriteToConsole(message)
                );
                return;
            }
            textBlock.Text += message + "\r";
        }
    }
}
