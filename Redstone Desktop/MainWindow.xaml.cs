using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Windows.Documents;
using prestja;
using System.Windows.Controls;

namespace prestja {
    public partial class MainWindow : Window {
        private static TcpClient client;
        private static bool connected = false;
        private static string[] connectedPlayers;
        private static string address = "127.0.0.1";
        private static int port = 7070;


        public MainWindow() {
            // initializing the user interface
            InitializeComponent();
            field_port.Text = "" + port;
            field_address.Text = address;
            field_password.Password = "Password";
            localConsole.IsReadOnly = true;
            //playerList.FontFamily = new FontFamily("Minecraftia");


            try {
                client = new TcpClient(address, port);
            } catch (SocketException e) {

            }
        }
        private void Log(string line, Color color) {
            Paragraph para = new Paragraph(new Run(line));
            para.Foreground = new SolidColorBrush(color);
            para.Margin = new Thickness(0);
            localConsole.Document.Blocks.Add(para);
            localConsole.ScrollToEnd();
        }
        private void UpdatePlayerList() {
            playerList.Items.Clear();
            for (int i = 0; i < connectedPlayers.Length; i++) {
                playerList.Items.Add(connectedPlayers[i]);
            }
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
                Command(input.Text);
                input.Text = "";
            }
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void button_connect_Click(object sender, RoutedEventArgs e) {
            address = field_address.Text;
            try {
                Int32.TryParse(field_port.Text, out port);
            } catch (Exception ex) {
                Log("Malformed port", GameColors.error);
                return;
            }
            try {
                client = new TcpClient(address, port);
            } catch (SocketException ex) {
                Log(ex.Message, GameColors.error);
                return;
            }
            AttemptConnection();
        }
    }
}
