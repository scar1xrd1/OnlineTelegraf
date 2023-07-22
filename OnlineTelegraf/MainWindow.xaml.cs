using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using NAudio;
using NAudio.Wave;

namespace OnlineTelegraf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WaveOutEvent waveOut;

        public MainWindow()
        {
            InitializeComponent();
            SetImage("Images/Telegraf.png");
        }

        void PlaySound()
        {
            var audioFile = new AudioFileReader("путь_к_вашему_аудиофайлу.mp3");
            waveOut = new WaveOutEvent();
            waveOut.Init(audioFile);
            waveOut.Play();
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

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                SetImage("Images/TelegrafBeep.png");
            }
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                SetImage("Images/Telegraf.png");
            }
        }
    }
}
