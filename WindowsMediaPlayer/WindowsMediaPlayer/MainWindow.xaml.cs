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
//for messagebox
using MessageBox = System.Windows.Forms.MessageBox;

namespace WindowsMediaPlayer
{
    public partial class MainWindow : Window
    {
        private bool MediaReader_Playing = false;

        public MainWindow()
        {
            InitializeComponent();
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
                My_Button_Play.Content = "Pause"; 
            }
            else
            {
                MediaReader_Playing = false;
                MediaReader.Pause();
                My_Button_Play.Content = "Play"; 
            }
        }

    }
}
