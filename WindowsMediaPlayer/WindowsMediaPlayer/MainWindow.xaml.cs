using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Drawing;
using System.ComponentModel;
//for messagebox
using MessageBox = System.Windows.MessageBox;


namespace WindowsMediaPlayer
{
    public partial class MainWindow : Window
    {
        private bool            MediaReader_Playing = false;
        private ImageBrush      brush_play;
        private ImageBrush      brush_pause;
        private ImageBrush      brush_load;
        private ImageBrush      brush_stop;

        public MainWindow()
        {
            InitializeComponent();

            brush_play = new ImageBrush();
            brush_pause = new ImageBrush();
            brush_load = new ImageBrush();
            brush_stop = new ImageBrush();

            brush_play.ImageSource = new BitmapImage(new Uri("play_button.png", UriKind.Relative));
            brush_pause.ImageSource = new BitmapImage(new Uri("pause_button.png", UriKind.Relative));
            brush_load.ImageSource = new BitmapImage(new Uri("load_button.png", UriKind.Relative));
            brush_stop.ImageSource = new BitmapImage(new Uri("stop_button.png", UriKind.Relative));

            My_Button_Play.Background = brush_play;
            My_Button_Load.Background = brush_load;
            My_Button_Stop.Background = brush_stop;
            if (My_Playist.Items.Count == 0)
                My_Text_No_Playlist.Content = "Your playlist" + Environment.NewLine + "   is empty.";
        }

        private void Button_Load(object sender, RoutedEventArgs e)
        {
            OpenFileDialog  ofd;

            ofd = new OpenFileDialog();
            ofd.AddExtension = true;
            ofd.DefaultExt = "*.*";
            ofd.Filter = "Media (*.*) | *.*";
            ofd.ShowDialog();

            MediaReader.Source = new Uri(ofd.FileName);
        }

        private void Button_Play(object sender, RoutedEventArgs e)
        {
            if (MediaReader_Playing == false)
            {
                if (MediaReader.IsLoaded == false)
                {
                    MessageBox.Show("Veuillez selectionner un fichier a lire !");
                    return ;
                }
                MediaReader_Playing = true;
                MediaReader.Play();
                MediaReader.Play();
                My_Button_Play.Background = brush_pause;
            }
            else
            {
                MediaReader_Playing = false;
                MediaReader.Pause();
                My_Button_Play.Background = brush_play;
            }
        }

        private void Button_Stop(object sender, RoutedEventArgs e)
        {
            MediaReader_Playing = false;
            MediaReader.Stop();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

    }
}
