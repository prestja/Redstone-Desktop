using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Collections.Generic;
using WisdomOtter;
using System.Windows.Documents;

namespace Redstone_Desktop {
    public partial class MainWindow : Window {
        private static TcpClient client;
        private static bool connected = false;
        private static string[] onlinePlayers;

        public MainWindow() {
            InitializeComponent();
            client = new TcpClient("158.69.52.181", 7070);
            // initializing the user interface
            localConsole.IsReadOnly = true;
            PerformInitialConnection();
            GetPlayers();


        }

        #region Log
        private void Log(string line, Color color) {
            Paragraph para = new Paragraph(new Run(line));
            para.Foreground = new SolidColorBrush(color);
            para.Margin = new Thickness(0);
            localConsole.Document.Blocks.Add(para);
            localConsole.ScrollToEnd();
        }
        #endregion





        private async void PerformInitialConnection() {
            Log("Connecting to server at " + client.Client.RemoteEndPoint, GameColors.warning);
            bool success = await ConnectAsync();
            if (success)
                Log("Connection successful", GameColors.green);
            else Log("Failed to connect to server", GameColors.error);
        }
        private async void Reconnect() {
            throw new NotImplementedException(); // todo: implement
        }
        private async void GetPlayers () {
            string response = await SendCommandAsync("list");
            int index = response.IndexOf(':');
            response = response.Remove(0, index + 1);
            onlinePlayers = response.Split(',');
            for (int i = 0; i < onlinePlayers.Length; i++) {
                Log(onlinePlayers[i], GameColors.green);
            }
            // I did something
        }

        private async void Command(String message) {
            string response = await SendCommandAsync(message);
            Log(response, GameColors.standard);
        }

        static async Task<bool> ConnectAsync() {
            NetworkStream stream = client.GetStream();
            Packet login = Packet.FormatLogin("hugs");
            byte[] outgoing = login.Serialize();
            byte[] incoming = new byte[1234];
            if (stream == null)
                return false;
            stream.Write(outgoing, 0, outgoing.Length);
            await stream.ReadAsync(incoming, 0, incoming.Length);
            Packet response = Packet.DeSerialize(incoming);
            if (response.requestID == login.requestID) {
                Console.WriteLine("Connection successful");
                connected = true;
                return connected;
            } else {
                Console.WriteLine("Connection failed");
                connected = false;
                return connected;
            }
        }

        static async Task<string> SendCommandAsync(string message) {
            NetworkStream stream = client.GetStream();
            Packet request = Packet.Format(PacketType.command, message);
            byte[] outgoing = request.Serialize();
            byte[] incoming = new byte[1234];
            if (stream == null)
                return "";
            stream.Write(outgoing, 0, outgoing.Length);
            await stream.ReadAsync(incoming, 0, incoming.Length);
            Packet response = Packet.DeSerialize(incoming);
            return Encoding.ASCII.GetString(response.payload);
        }

        private async void send_Click(object sender, RoutedEventArgs e) {
            try {
                string response = await SendCommandAsync(input.Text);
                Console.WriteLine(response);
            } catch (Exception ex) {
                
            } // todo: remove?
        }


        private void EnterKeyHandler(object sender, KeyEventArgs e) {
            if (e.Key == Key.Return) {
                //string response = SendMessageAsync("list").Result;
                Command(input.Text);
                input.Text = "";
            }
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }


    }
}
