using System;
using System.Windows;
using System.Windows.Media;
using System.Threading.Tasks;


namespace WpfLayoutPractice
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            UpdateConnectionUI();
        }

        public enum ConnectionState
        {
            Disconnected,
            Connecting,
            Connected,
            Error
        }

        private ConnectionState _connectionState
            = ConnectionState.Disconnected;

        private void UpdateConnectionUI()
        {
            // switch는 값에 따라 실행할 코드를 선택합니다.
            switch (_connectionState)
            {
                case ConnectionState.Disconnected:
                    StatusTextBlock.Text = "● 연결 안 됨";
                    StatusTextBlock.Foreground = Brushes.Gray;
                    break;

                case ConnectionState.Connecting:
                    StatusTextBlock.Text = "● 연결 중";
                    StatusTextBlock.Foreground = Brushes.Orange;
                    break;

                case ConnectionState.Connected:
                    StatusTextBlock.Text = "● 연결됨";
                    StatusTextBlock.Foreground = Brushes.Green;
                    break;

                case ConnectionState.Error:
                    StatusTextBlock.Text = "● 연결 오류";
                    StatusTextBlock.Foreground = Brushes.Red;
                    break;
            }

            bool canConnect =
                _connectionState == ConnectionState.Disconnected ||
                _connectionState == ConnectionState.Error;

            ConnectButton.IsEnabled = canConnect;

            DisconnectButton.IsEnabled =
                _connectionState == ConnectionState.Connected;

            ServerAddressTextBox.IsEnabled = canConnect;
        }


        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            string address = ServerAddressTextBox.Text.Trim();

            // 빈 주소는 연결을 시도하기 전에 확인합니다.
            if (string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("서버 주소를 입력하세요.");
                return;
            }

            // 모의 연결: 2초 동안 연결 중 상태를 표시합니다.
            _connectionState = ConnectionState.Connecting;
            UpdateConnectionUI();
            DataTextBox.Text = "서버에 연결하는 중입니다.";

            await Task.Delay(2000);

            if (address == "test")
            {
                _connectionState = ConnectionState.Connected;

                DataTextBox.Text =
                    $"서버 주소: {address}\n" +
                    $"수신 시간: {DateTime.Now:HH:mm:ss}\n\n" +
                    "온도: 24.5 °C\n" +
                    "습도: 58 %\n" +
                    "장비 상태: 정상";
            }
            else
            {
                _connectionState = ConnectionState.Error;
                DataTextBox.Text = "연결에 실패했습니다. 주소에 test를 입력해 보세요.";
            }

            // 성공·실패 분기에서 공통으로 실행합니다.
            UpdateConnectionUI();
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            _connectionState = ConnectionState.Disconnected;
            UpdateConnectionUI();

            DataTextBox.Text = "연결이 해제되었습니다.";
        }
    }
}