using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Windows.Documents;
using prestja;

namespace prestja {
    public partial class MainWindow : Window {
        private static TcpClient client;
        private static bool connected = false;
        private static string[] onlinePlayers;

        public MainWindow() {
            InitializeComponent();
            client = new TcpClient("158.69.52.181", 7070);
            localConsole.IsReadOnly = true; // initializing the user interface
            PerformInitialConnection();
            GetPlayers();


        }
        private void Log(string line, Color color) {
            Paragraph para = new Paragraph(new Run(line));
            para.Foreground = new SolidColorBrush(color);
            para.Margin = new Thickness(0);
            localConsole.Document.Blocks.Add(para);
            localConsole.ScrollToEnd();
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
    }
}
