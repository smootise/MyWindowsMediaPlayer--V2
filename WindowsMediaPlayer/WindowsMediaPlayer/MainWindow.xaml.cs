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

//test
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

//for messagebox
using MessageBox = System.Windows.MessageBox;
using ListBox = System.Windows.Forms.ListBox;


namespace WindowsMediaPlayer
{
    public partial class MainWindow : Window
    {
        private bool            MediaReader_Playing = false;
        private bool            userIsDraggingSlider = false;
        private List<string>    _playlist_items = new List<string>();
        private LibraryStocker  LibStocker = new LibraryStocker();

        private ImageBrush brush_play;
        private ImageBrush brush_pause;
        private ImageBrush brush_load;
        private ImageBrush brush_stop;
        private ImageBrush brush_next;
        private ImageBrush brush_prev;
        private ImageBrush brush_delete;
        private ImageBrush brush_send;

        public MainWindow()
        {
            InitializeComponent();

            brush_play = new ImageBrush();
            brush_pause = new ImageBrush();
            brush_load = new ImageBrush();
            brush_stop = new ImageBrush();
            brush_next = new ImageBrush();
            brush_prev = new ImageBrush();
            brush_delete = new ImageBrush();
            brush_send = new ImageBrush();

            brush_play.ImageSource = new BitmapImage(new Uri("play_button.png", UriKind.Relative));
            brush_pause.ImageSource = new BitmapImage(new Uri("pause_button.png", UriKind.Relative));
            brush_load.ImageSource = new BitmapImage(new Uri("load_button.png", UriKind.Relative));
            brush_stop.ImageSource = new BitmapImage(new Uri("stop_button.png", UriKind.Relative));
            brush_next.ImageSource = new BitmapImage(new Uri("next_button.png", UriKind.Relative));
            brush_prev.ImageSource = new BitmapImage(new Uri("previous_button.png", UriKind.Relative));
            brush_delete.ImageSource = new BitmapImage(new Uri("delete_button.png", UriKind.Relative));
            brush_send.ImageSource = new BitmapImage(new Uri("send_button.png", UriKind.Relative));

            My_Button_Play.Background = brush_play;
            My_Button_Load.Background = brush_load;
            My_Button_Stop.Background = brush_stop;
            My_Button_Next.Background = brush_next;
            My_Button_Prev.Background = brush_prev;
            My_Button_Delete.Background = brush_delete;
            My_Button_Send.Background = brush_send;
            if (My_Playist.Items.Count == 0)
                My_Text_No_Playlist.Content = "Your playlist" + Environment.NewLine + "   is empty.";

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();

            Bibli_reset();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (_playlist_items.Count >= 1)
                My_Text_No_Playlist.Content = "";

            if ((MediaReader.Source != null) && (MediaReader.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
            {
                sliProgress.Minimum = 0;
                sliProgress.Maximum = MediaReader.NaturalDuration.TimeSpan.TotalSeconds;
                sliProgress.Value = MediaReader.Position.TotalSeconds;

                //s'il n'a pas bien démaré
                if (MediaReader.Position.TotalSeconds == 0 && MediaReader_Playing == true)
                    MediaReader.Play();

                // si la vidéo est finie
                if (MediaReader.Position.TotalSeconds == MediaReader.NaturalDuration.TimeSpan.TotalSeconds)
                {
                    string  sel_item = My_Playist.SelectedItem.ToString();
                    int     pos = My_Playist.Items.IndexOf(sel_item);
                    //s'il reste au moins un truc a lire dans la playlist
                    if (pos < (My_Playist.Items.Count - 1))
                    {
                        MediaReader.Stop();
                        MediaReader.Source = new Uri(_playlist_items.ElementAt(pos + 1));
                        MediaReader_Playing = true;
                        MediaReader.Play();
                        My_Playist.SelectedIndex = pos + 1;
                    }
                }
            }
        }

        private void Button_Load(object sender, RoutedEventArgs e)
        {

            OpenFileDialog  ofd;

            ofd = new OpenFileDialog();
            ofd.AddExtension = true;
            ofd.DefaultExt = "*.*";
            ofd.Filter = "Media (*.*) | *.*";
            ofd.ShowDialog();

            if (_playlist_items.Contains(ofd.FileName) != false)
                return ;
            MediaReader.Source = new Uri(ofd.FileName);

            //lucas related
            LibStocker.addMedia(ofd.FileName);
            this.Bibli_reset();
        }

        private void Button_Play(object sender, RoutedEventArgs e)
        {
            if (MediaReader.Source == null)
                return ;
            if (MediaReader_Playing == false)
            {
                this.Blibli_Set_Invis();
                MediaReader_Playing = true;
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
            My_Button_Play.Background = brush_play;
            this.Blibli_Set_Visible();
            MediaReader.Stop();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string      sel_item = My_Playist.SelectedItem.ToString();

            //load le media
            MediaReader.Source = new Uri(_playlist_items.ElementAt(My_Playist.Items.IndexOf(sel_item)));
            MediaReader_Playing = false;
            My_Button_Play.Background = brush_play;
        }

        private void sliProgress_DragStarted(object sender, DragStartedEventArgs e)
        {
            userIsDraggingSlider = true;
        }

        private void sliProgress_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            userIsDraggingSlider = false;
            MediaReader.Position = TimeSpan.FromSeconds(sliProgress.Value);
        }

        private void sliProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblProgressStatus.Text = TimeSpan.FromSeconds(sliProgress.Value).ToString(@"hh\:mm\:ss");
        }

        private void Button_Prev(object sender, RoutedEventArgs e)
        {
            string  sel_item = My_Playist.SelectedItem.ToString();
            int     pos = My_Playist.Items.IndexOf(sel_item);

            if (pos >= 1)
            {
                MediaReader.Source = new Uri(_playlist_items.ElementAt(pos - 1));
                My_Playist.SelectedIndex = pos - 1;
            }
        }

        private void Button_Next(object sender, RoutedEventArgs e)
        {
            string sel_item = My_Playist.SelectedItem.ToString();
            int pos = My_Playist.Items.IndexOf(sel_item);

            //s'il reste au moins un truc a lire dans la playlist
            if (pos < (My_Playist.Items.Count - 1))
            {
                MediaReader.Source = new Uri(_playlist_items.ElementAt(pos + 1));
                My_Playist.SelectedIndex = pos + 1;
            }
        }

        private void    Blibli_Set_Invis()
        {
            My_Text_Musiques.Visibility = Visibility.Hidden;
            My_Text_Films.Visibility = Visibility.Hidden;
            My_Text_Images.Visibility = Visibility.Hidden;

            My_ListBox_Musiques.Visibility = Visibility.Hidden;
            My_ListBox_Films.Visibility = Visibility.Hidden;
            My_ListBox_Images.Visibility = Visibility.Hidden;
        }

        private void Blibli_Set_Visible()
        {
            Bibli_reset();
            My_Text_Musiques.Visibility = Visibility.Visible;
            My_Text_Films.Visibility = Visibility.Visible;
            My_Text_Images.Visibility = Visibility.Visible;

            My_ListBox_Musiques.Visibility = Visibility.Visible;
            My_ListBox_Films.Visibility = Visibility.Visible;
            My_ListBox_Images.Visibility = Visibility.Visible;
        }

        private void Bibli_reset()
        {
            //on clear les listbox
            My_ListBox_Musiques.Items.Clear();
            My_ListBox_Films.Items.Clear();
            My_ListBox_Images.Items.Clear();

            //on ajoute par rapport aux bails de lucas
            List<string> my_list = LibStocker.getMusic();
            for (int i = 0; i < my_list.Count(); i++)
                My_ListBox_Musiques.Items.Add(my_list.ElementAt(i).Substring(my_list.ElementAt(i).LastIndexOf('\\') + 1));

            my_list = LibStocker.getVideo();
            for (int i = 0; i < my_list.Count(); i++)
                My_ListBox_Films.Items.Add(my_list.ElementAt(i).Substring(my_list.ElementAt(i).LastIndexOf('\\') + 1));

            my_list = LibStocker.getPic();
            for (int i = 0; i < my_list.Count(); i++)
                My_ListBox_Images.Items.Add(my_list.ElementAt(i).Substring(my_list.ElementAt(i).LastIndexOf('\\') + 1));
        }

        private void Button_Delete(object sender, RoutedEventArgs e)
        {
            if (My_ListBox_Musiques.SelectedIndex != -1)
            {
                string sel_item = My_ListBox_Musiques.SelectedItem.ToString();

                LibStocker.removeMedia(sel_item);
            }
            else if (My_ListBox_Films.SelectedIndex != -1)
            {
                string sel_item = My_ListBox_Films.SelectedItem.ToString();

                LibStocker.removeMedia(sel_item);
            }
            else if (My_ListBox_Images.SelectedIndex != -1)
            {
                string sel_item = My_ListBox_Images.SelectedItem.ToString();

                LibStocker.removeMedia(sel_item);
            }
            this.Bibli_reset();
        }


        private void My_ListBox_Images_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            My_ListBox_Musiques.SelectedIndex = -1;
            My_ListBox_Films.SelectedIndex = -1;

            if (My_ListBox_Images.SelectedIndex == -1)
                return;

            string sel_item = My_ListBox_Images.SelectedItem.ToString();
            //load le media
            MediaReader.Source = new Uri(LibStocker.getfullpath(sel_item));
        }

        private void My_ListBox_Films_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            My_ListBox_Musiques.SelectedIndex = -1;
            My_ListBox_Images.SelectedIndex = -1;

            if (My_ListBox_Films.SelectedIndex == -1)
                return;

            string sel_item = My_ListBox_Films.SelectedItem.ToString();
            //load le media
            MediaReader.Source = new Uri(LibStocker.getfullpath(sel_item));
        }

        private void My_ListBox_Musiques_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            My_ListBox_Images.SelectedIndex = -1;
            My_ListBox_Films.SelectedIndex = -1;

            if (My_ListBox_Musiques.SelectedIndex == -1)
                return;

            string sel_item = My_ListBox_Musiques.SelectedItem.ToString();
            //load le media
            MediaReader.Source = new Uri(LibStocker.getfullpath(sel_item));
        }

        private void Button_Send(object sender, RoutedEventArgs e)
        {
            if (MediaReader_Playing == false)
            {
                if (My_ListBox_Musiques.SelectedIndex != -1)
                {
                    string sel_item = My_ListBox_Musiques.SelectedItem.ToString();

                    //playlist related
                    My_Playist.Items.Insert(0, sel_item);
                    _playlist_items.Insert(0, LibStocker.getfullpath(sel_item));
                    My_Playist.SelectedIndex = 0;
                }
                else if (My_ListBox_Films.SelectedIndex != -1)
                {
                    string sel_item = My_ListBox_Films.SelectedItem.ToString();

                    My_Playist.Items.Insert(0, sel_item);
                    _playlist_items.Insert(0, LibStocker.getfullpath(sel_item));
                    My_Playist.SelectedIndex = 0;
                }
                else if (My_ListBox_Images.SelectedIndex != -1)
                {
                    string sel_item = My_ListBox_Images.SelectedItem.ToString();

                    My_Playist.Items.Insert(0, sel_item);
                    _playlist_items.Insert(0, LibStocker.getfullpath(sel_item));
                    My_Playist.SelectedIndex = 0;
                }
            }
        }

    }
}
