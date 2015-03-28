using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const uint KEYEVENTF_KEYUP = 0x0002;
        const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        const byte VK_VOLUME_MUTE      = 0xAD; //Volume Mute key
        const byte VK_VOLUME_UP        = 0xAF; //Volume Up key
        const byte VK_VOLUME_DOWN      = 0xAE; //Volume Down key
        const byte VK_MEDIA_NEXT_TRACK = 0xB0; //Next Track key
        const byte VK_MEDIA_PREV_TRACK = 0xB1; //Previous Track key
        const byte VK_MEDIA_STOP       = 0xB2; //Stop Media key
        const byte VK_MEDIA_PLAY_PAUSE = 0xB3; //Play/Pause Media key
        public MainWindow()
        {            
            InitializeComponent(); const uint KEYEVENTF_KEYUP = 0x0002;
            const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
            IPHostEntry hostEntry;

            hostEntry = Dns.GetHostEntry(textBox.Text);
          
            if (hostEntry.AddressList.Length > 0)
            {
                var ip = hostEntry.AddressList[0];
                label.Content = ip.ToString();
            }
        }
      
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        int press(byte key)
        {           
            keybd_event(key, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
            return 0;

        }
        
        private void button_prev_Click(object sender, RoutedEventArgs e)
        {
            press(VK_MEDIA_PREV_TRACK);           
        }

        private void button_next_Click(object sender, RoutedEventArgs e)
        {
            press(VK_MEDIA_NEXT_TRACK);           
        }

        private void button_play_Click(object sender, RoutedEventArgs e)
        {
            press(VK_MEDIA_PLAY_PAUSE);
        }

        private void button_stop_Click(object sender, RoutedEventArgs e)
        {
            press(VK_MEDIA_STOP);
        }
    }
}
