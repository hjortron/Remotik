using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WPFServer
{
    /// <summary>
    /// WPF host for a SignalR server. The host can stop and start the SignalR
    /// server, report errors when trying to start the server on a URI where a
    /// server is already being hosted, and monitor when clients connect and disconnect. 
    /// The hub used in this server is a simple echo service, and has the same 
    /// functionality as the other hubs in the SignalR Getting Started tutorials.
    /// For simplicity, MVVM will not be used for this sample.
    /// </summary>
    public partial class MainWindow
    {        
        const int KEYEVENTF_KEYDOWN = 0x0;
        const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        const uint KEYEVENTF_KEYUP = 0x0002;
        public IDisposable SignalR { get; set; }       

        public MainWindow()
        {
            InitializeComponent();
          
        }
        [DllImport("user32.dll")]
        static extern UInt32 SendInput(UInt32 nInputs, [MarshalAs(UnmanagedType.LPArray, SizeConst = 1)] INPUT[] pInputs, Int32 cbSize);
        [DllImport("user32.dll", EntryPoint = "keybd_event", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern void keybd_event(byte vk, byte bscan, uint flags, int extrainfo);     
        
        public void Press(byte key)
         {
            INPUT[] InputData = new INPUT[1];

            InputData[0].Type = (UInt32)InputType.KEYBOARD;
            //InputData[0].Vk = (ushort)DirectInputKeyScanCode;  //Virtual key is ignored when sending scan code
            InputData[0].Scan = (ushort)DirectInputKeyScanCode;
            InputData[0].Flags = (uint)KeyboardFlag.KEYUP | (uint)KeyboardFlag.SCANCODE;
            InputData[0].Time = 0;
            InputData[0].ExtraInfo = IntPtr.Zero;

            // Send Keyup flag "OR"ed with Scancode flag for keyup to work properly
            SendInput(1, InputData, Marshal.SizeOf(typeof (INPUT)));
         }

    /// <summary>
    /// Calls the StartServer method with Task.Run to not
    /// block the UI thread. 
    /// </summary>
    private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            var hostEntry = Dns.GetHostEntry(Dns.GetHostName());

            if (hostEntry.AddressList.Length == 0) return;
            var ip = hostEntry.AddressList[2].ToString();
            WriteToConsole("Starting server...");
            ButtonStart.IsEnabled = false;            
            Task.Run(() => StartServer(ip));
        }

        /// <summary>
        /// Stops the server and closes the form. Restart functionality omitted
        /// for clarity.
        /// </summary>
        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            SignalR.Dispose();
            Close();
        }

        /// <summary>
        /// Starts the server and checks for error thrown when another server is already 
        /// running. This method is called asynchronously from Button_Start.
        /// </summary>
        private void StartServer(string serverIp)
        {
            try
            {
                SignalR = WebApp.Start($"http://{serverIp}:8080");
            }
            catch (TargetInvocationException)
            {
                WriteToConsole("A server is already running at " + serverIp);
                Dispatcher.Invoke(() => ButtonStart.IsEnabled = true);
                return;
            }
            Dispatcher.Invoke(() => ButtonStop.IsEnabled = true);
            WriteToConsole("Server started at " + serverIp);
        }
        ///This method adds a line to the RichTextBoxConsole control, using Dispatcher.Invoke if used
        /// from a SignalR hub thread rather than the UI thread.
        public void WriteToConsole(string message)
        {
            if (!(RichTextBoxConsole.CheckAccess()))
            {
                Dispatcher.Invoke(() =>
                    WriteToConsole(message)
                );
                return;
            }
            RichTextBoxConsole.AppendText(message + "\r");
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            Press(0xB0);
        }
    }
    /// <summary>
    /// Used by OWIN's startup process. 
    /// </summary>
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }
    /// <summary>
    /// Echoes messages sent using the Send message by calling the
    /// addMessage method on the client. Also reports to the console
    /// when clients connect and disconnect.
    /// </summary>
    public class MyHub : Hub
    {
        public void Send(byte keyCode)
        {
            Application.Current.Dispatcher.Invoke(() =>
                ((MainWindow)Application.Current.MainWindow).Press(keyCode));
        }
        public override Task OnConnected()
        {
            //Use Application.Current.Dispatcher to access UI thread from outside the MainWindow class
            Application.Current.Dispatcher.Invoke(() => 
                ((MainWindow)Application.Current.MainWindow).WriteToConsole("Client connected: " + Context.ConnectionId));

            return base.OnConnected();
        }
        public override Task OnDisconnected()
        {
            //Use Application.Current.Dispatcher to access UI thread from outside the MainWindow class
            Application.Current.Dispatcher.Invoke(() => 
                ((MainWindow)Application.Current.MainWindow).WriteToConsole("Client disconnected: " + Context.ConnectionId));

            return base.OnDisconnected();
        }

    }
}
