using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using NAudio;
using NAudio.Wave;

namespace OnlineTelegraf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ServiceMorze.IServiceMorzeCallback
    {
        bool isConnected = false;
        ServiceMorze.ServiceMorzeClient client;
        int ID;

        WaveOutEvent waveOut;
        DispatcherTimer timer;

        int tick; int tickToStop = 0;
        bool timerToStop = false;
        bool canBeep = true;

        public MainWindow()
        {
            InitializeComponent();
            MyInitialize();
            SetImage("Images/Telegraf.png");            
        }

        void MyInitialize()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(!timerToStop) tick++;
            else
            {
                tickToStop++;
                if (tickToStop >= tick)
                {
                    ForciblyStopSound();
                    timerToStop = false;
                }
            }
        }

        void PlaySound()
        {
            var audioFile = new AudioFileReader("Sounds/Beep.mp3");
            waveOut = new WaveOutEvent(); // Громкость от 0.0 до 1.0
            waveOut.Init(audioFile);
            waveOut.Volume = (float)tbVolume.Value / 10;
            waveOut.Play();

            timer.Start();
        }  

        void StopSound()
        {
            if (waveOut != null)
            {
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
            }

            if (tick < 10) tick = 10;
            lblTIck.Content = tick.ToString();
            timer.Stop();
        } 
        
        void ForciblyStopSound()
        {
            if (waveOut != null)
            {
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
            }
        }

        void SetImage(string imagePath)
        {
            try
            {
                BitmapImage image = new BitmapImage();

                image.BeginInit();
                image.UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute);
                image.EndInit();

                imgTelegraf.Source = image;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e) { }
        private void Window_PreviewKeyUp(object sender, KeyEventArgs e) { }

        private void Window_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(canBeep)
            {
                SetImage("Images/TelegrafBeep.png");

                tick = 0;
                PlaySound();
            }           
        }

        private void Window_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(canBeep)
            {
                SetImage("Images/Telegraf.png");

                StopSound();

                if (client != null) client.SendSignal(tick, ID);
            }

           
            //Thread.Sleep(100);

            //timerToStop = true;
            //tickToStop = 0;
            //PlaySound();
        }

        private void btnGiveControl_Click(object sender, RoutedEventArgs e)
        {
            btnGiveControl.IsEnabled = false;
            canBeep = false;
        }

        void ConnectUser()
        {
            if (!isConnected)
            {
                client = new ServiceMorze.ServiceMorzeClient(new System.ServiceModel.InstanceContext(this));
                ID = client.Connect(tbUsername.Text);
                tbUsername.IsEnabled = false;
                btnConnect.Content = "Отключиться";
                isConnected = true;
            }
        }

        void DisconnectUser()
        {
            if(isConnected)
            {
                client.Disconnect(ID);
                client = null;
                tbUsername.IsEnabled = true;
                btnConnect.Content = "Подключиться";
                isConnected = false;
            }
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if(isConnected) DisconnectUser();
            else ConnectUser();
        }

        public void SignalCallback(int tick, int senderID)
        {
            //if(senderID != ID)
            //{
            if(senderID != ID)
            {
                this.tick = tick;
                lblTIck.Content = this.tick.ToString();
                timerToStop = true;
                tickToStop = 0;
                PlaySound();
            }
              
            //}            
        }

        public void MsgCallback(string msg, int senderID)
        {
            lbChat.Items.Add(msg);
            lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DisconnectUser();
        }

        private void tbChat_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                if(client != null)
                {
                    client.SendMsg(tbChat.Text, ID);
                    tbChat.Text = string.Empty;
                }             
            }
        }
    }
}
