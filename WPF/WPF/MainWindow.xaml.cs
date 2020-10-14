using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double totalDuration;

        public MainWindow()
        {
            InitializeComponent();

            Label1.Content = "No file selected yet";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog aDialog = new OpenFileDialog();

            // if no file selected just stop here
            if (aDialog.ShowDialog() == false) return;

            // update the label
            Label1.Content = "Media file currently in use: [ " + aDialog.FileName + " ]";

            MediaElement1.Source = new Uri(aDialog.FileName);
            MediaElement1.Play();

            Thread cursorUpdate = new Thread(updateProgressBar);
            cursorUpdate.Start();
        }

        private void updateProgressBar()
        {
            bool b = false;

            // wait for the media file to be loaded
            while (!b)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    b = MediaElement1.NaturalDuration.HasTimeSpan;
                }));

                Thread.Sleep(100);
            }
            
            // set the progress bar max value
            Dispatcher.Invoke(new Action(() =>
            {
                totalDuration = MediaElement1.NaturalDuration.TimeSpan.TotalMilliseconds;

                pbProgress.Maximum = totalDuration;

                //Console.WriteLine("Total Duration {0}", totalDuration);
            }));

            bool progress = true;

            // update the progress bar every 100ms
            while (progress)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    double current = MediaElement1.Position.TotalMilliseconds;

                    pbProgress.Value = current;

                    //Console.WriteLine(current);
                }));

                Thread.Sleep(100);

                Dispatcher.Invoke(new Action(() =>
                {
                    if (pbProgress.Value == pbProgress.Maximum) progress = false;
                }));
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MediaElement1.Play();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MediaElement1.Pause();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MediaElement1.Stop();
        }

        private void MediaElement1_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show("Media loaded unsuccessfuly. "+e.ErrorException.Message);
        }
    }
}
