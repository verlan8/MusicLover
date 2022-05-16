using MusicLover.models;
using MusicLover.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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
using System.Windows.Threading;

namespace MusicLover
{
    /// <summary>
    /// Прокофьев Иван ПКС-305
    /// </summary>
    public partial class MainPage : Window
    {
        private int id_user;
        public MainPage(string log, int user)
        {
            InitializeComponent();
            id_user = user;
            CheckForAdmin(user);
            LoginTB.Text = log;
            MainFrame.Source = new Uri("HomePage.xaml", UriKind.Relative);
        }

        string conn = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
        //SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-TJLQPJN\SQLEXPRESS;Initial Catalog = music_lover; Integrated Security = True");
        //SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-PMJ80NN\SQLEXPRESS;Initial Catalog = music_lover; Integrated Security = True");


        private void CheckForAdmin(int user)
        {
            if (user == 1)
            {
                AdminButton.Visibility = Visibility.Visible;
            }
        }


        private void MainButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Source = new Uri("HomePage.xaml", UriKind.Relative);
        }


        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow adminWindow = new AdminWindow();
            this.Close();
            adminWindow.ShowDialog();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.ShowDialog();

        }

        //подумать как воспроизвести снова(MediaElement.MediaEnded)
        int counterPausePlayMusic = 1;
        bool flag = true;
        private void PauseMusic_Click(object sender, RoutedEventArgs e)
        {
            if (counterPausePlayMusic == 1)
            {
                PlayMusic.Pause();
                flag = false;
                PauseMusic.Visibility = Visibility.Collapsed;
                PlayMusicButton.Visibility = Visibility.Visible;

            }
            else if (counterPausePlayMusic == 0)
            {
                PlayMusic.Play();
                flag = true;
                PlayMusicButton.Visibility = Visibility.Collapsed;
                PauseMusic.Visibility = Visibility.Visible;
            }
            switch (flag)
            {
                case true:
                    counterPausePlayMusic++;
                    break;
                case false:
                    counterPausePlayMusic--;
                    break;
            }

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            PlayerArea.Visibility = Visibility.Collapsed;
            PlayMusic.Stop();
        }

        private void CheckForLike(int idMusic)
        {
            SqlConnection connection = new SqlConnection(conn);
            using (connection)
            {
                try
                {
                    connection.Open();
                    string query = "Select id_user, id_music, score from scores where (id_music = @id_music and id_user = @id_user)";
                    SqlCommand sqlCommand = new SqlCommand(query, connection);
                    sqlCommand.Parameters.Add(new SqlParameter("id_music", idMusic));
                    sqlCommand.Parameters.Add(new SqlParameter("id_user", id_user));
                    SqlDataReader reader = sqlCommand.ExecuteReader();
                    if (reader.HasRows == true)
                    {
                        UnlikedMusicButton.Visibility = Visibility.Collapsed;
                        LikedMusicButton.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        UnlikedMusicButton.Visibility = Visibility.Visible;
                        LikedMusicButton.Visibility = Visibility.Collapsed;
                    }
                    reader.Close();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void PlusAudition()
        {
            SqlConnection connection = new SqlConnection(conn);
            using (connection)
            {
                try
                {
                    connection.Open();
                    string query = "UpdateNumberOfAuditions";
                    SqlCommand sqlCommand = new SqlCommand(query, connection);
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.Parameters.Add(new SqlParameter("@id_music", chartsList[index].musicId));

                    int i = sqlCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        int index = 0;
        List<Charts> chartsList = new List<Charts>();
        public void SetListOfSongs(List<Charts> qwe, int ind)
        {
            chartsList = qwe;
            index = ind;
            PlayerArea.Visibility = Visibility.Visible;
            PlayMusicButton.Visibility = Visibility.Collapsed;
            PauseMusic.Visibility = Visibility.Visible;
            MusicArtistTitle.Text = chartsList[index].musicArtist;
            MusicTitle.Text = chartsList[index].musicName;
            MusicArtistTitle.ToolTip = chartsList[index].musicArtist;
            MusicTitle.ToolTip = chartsList[index].musicName;
            CheckForLike(chartsList[index].musicId);
            //PlayMusic.Source = new Uri(@"C:\Users\Korela\Downloads\music\" + chartsList[index].musicArtist + " - " + chartsList[index].musicName + ".mp3", UriKind.Relative);
            PlayMusic.Source = new Uri(@"C:\Users\Korela\Downloads\music\" + chartsList[index].musicArtist + " - " + chartsList[index].musicName + ".mp3", UriKind.Relative);
            PlayMusic.Play();
            PlusAudition();
        }

        private void PlayMusic_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (repeatFlag)
            {
                PlayMusic.Stop();
                PlayMusic.Position = TimeSpan.Zero;
                counterPausePlayMusic = 0;
                PlayMusic.Play();
                PlusAudition();
            }
            else if (index != chartsList.Count - 1)
            {
                NextMusic_Click(sender, e);
            }
        }

        private void NextMusic_Click(object sender, RoutedEventArgs e)
        {
            if (!randomFlag && index != chartsList.Count-1)
            {
                index++;
            }
            else if (randomFlag)
            {
                SetRandomPos();
            }
            else if (repeatFlag)
            {
                PlayMusic.Stop();
                PlayMusic.Position = TimeSpan.Zero;
                counterPausePlayMusic = 0;
                PlayMusic.Play();
                PlusAudition();
            }
            if (chartsList.Count > index)
            {
                PlayMusicButton.Visibility = Visibility.Collapsed;
                PauseMusic.Visibility = Visibility.Visible;
                MusicArtistTitle.Text = chartsList[index].musicArtist;
                MusicTitle.Text = chartsList[index].musicName;
                //PlayMusic.Source = new Uri(@"C:\Users\Korela\Downloads\music\" + chartsList[index].musicArtist + " - " + chartsList[index].musicName + ".mp3", UriKind.Relative);
                PlayMusic.Source = new Uri(@"C:\Users\Korela\Downloads\music\" + chartsList[index].musicArtist + " - " + chartsList[index].musicName + ".mp3", UriKind.Relative);
                PlayMusic.Play();
                PlusAudition();
            }

        }

        private void PrevMusic_Click(object sender, RoutedEventArgs e)
        {
            if(index != 0)
            {
                index--;
            }
            if (index >=0)
            {
                PlayMusicButton.Visibility = Visibility.Collapsed;
                PauseMusic.Visibility = Visibility.Visible;
                MusicArtistTitle.Text = chartsList[index].musicArtist;
                MusicTitle.Text = chartsList[index].musicName;
                //PlayMusic.Source = new Uri(@"C:\Users\Korela\Downloads\music\" + chartsList[index].musicArtist + " - " + chartsList[index].musicName + ".mp3", UriKind.Relative);
                PlayMusic.Source = new Uri(@"C:\Users\Korela\Downloads\music\" + chartsList[index].musicArtist + " - " + chartsList[index].musicName + ".mp3", UriKind.Relative);
                PlayMusic.Play();
                PlusAudition();
            }
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.NavigationService.CanGoBack)
            {
                MainFrame.NavigationService.GoBack();
            }
        }


        int counterRepeatMusic = 0;
        bool repeatFlag = false;

        private void RepeatMusicButton_Click(object sender, RoutedEventArgs e)
        {
            if (counterRepeatMusic == 1)
            {
                RepeatMusicButton.BorderThickness = new Thickness(0, 0, 0, 0);
                counterRepeatMusic--;
                repeatFlag = false;
            }
            else if (counterRepeatMusic == 0)
            {
                RepeatMusicButton.BorderThickness = new Thickness(2, 2, 2, 2);
                counterRepeatMusic++;
                repeatFlag = true;
            }

        }

        private void SetRandomPos()
        {
            bool whileFlag = false;
            Random rnd = new Random();
            int currentPos = chartsList[index].chartPos - 1;
            int maxPos = chartsList.Count;
            while (!whileFlag)
            {
                int randomPos = rnd.Next(0, maxPos);
                if (randomPos != currentPos)
                {
                    index = randomPos;
                    whileFlag = true;
                }
            }
        }

        int counterRandomMusic = 0;
        bool randomFlag = false;
        private void RandomMusicButton_Click(object sender, RoutedEventArgs e)
        {
            if (counterRandomMusic == 1)
            {
                RandomMusicButton.BorderThickness = new Thickness(0, 0, 0, 0);
                counterRandomMusic--;
                randomFlag = false;
            }
            else if (counterRandomMusic == 0)
            {
                RandomMusicButton.BorderThickness = new Thickness (2, 2, 2, 2);
                counterRandomMusic++;
                randomFlag = true;
            }

        }

        private void UnlikedMusicButton_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection connection = new SqlConnection(conn);
            using (connection)
            {
                try
                {
                    connection.Open();
                    string query = "SetLike";
                    SqlCommand sqlCommand = new SqlCommand(query, connection);
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.Parameters.Add(new SqlParameter("@id_user",id_user));
                    sqlCommand.Parameters.Add(new SqlParameter("@id_music",chartsList[index].musicId));

                    int i = sqlCommand.ExecuteNonQuery();
                    if (i > 0)
                    {
                        UnlikedMusicButton.Visibility = Visibility.Collapsed;
                        LikedMusicButton.Visibility = Visibility.Visible;
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }

        private void LikedMusicButton_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection connection = new SqlConnection(conn);
            using (connection)
            {
                try
                {
                    connection.Open();
                    string query = "RemoveLike";
                    SqlCommand sqlCommand = new SqlCommand(query, connection);
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    sqlCommand.Parameters.Add(new SqlParameter("@id_user",id_user));
                    sqlCommand.Parameters.Add(new SqlParameter("@id_music",chartsList[index].musicId));

                    int i = sqlCommand.ExecuteNonQuery();
                    if (i > 0)
                    {
                        LikedMusicButton.Visibility = Visibility.Collapsed;
                        UnlikedMusicButton.Visibility = Visibility.Visible;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        int counterVolumeMusic = 1;
        bool flagVolume = true;
        private void VolumeMusicButton_Click(object sender, RoutedEventArgs e)
        {
            if (counterVolumeMusic == 1)
            {
                VolumeMusicButton.BorderThickness = new Thickness(2, 2, 2, 2);
                PlayMusic.Volume = 0;
                flagVolume = false;
            }
            else if (counterVolumeMusic == 0)
            {
                VolumeMusicButton.BorderThickness = new Thickness(0, 0, 0, 0);
                PlayMusic.Volume = VolumeSlider.Value;
                flagVolume = true;
            }
            switch (flagVolume)
            {
                case true:
                    counterVolumeMusic++;
                    break;
                case false:
                    counterVolumeMusic--;
                    break;
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (flagVolume)
            {
                PlayMusic.Volume = VolumeSlider.Value;
            }
        }


        private void PlayMusic_MediaOpened(object sender, RoutedEventArgs e)
        {
            MaxPosMusicTB.Text = chartsList[index].musicDuration;
            string[] duration = chartsList[index].musicDuration.Split(":");
            int min = Convert.ToInt32(duration[0]);
            min = min * 60;
            int sec = Convert.ToInt32(duration[1]) + min;
            TimelineSlider.Maximum = sec;

            // Create a timer that will update the counters and the time slider
            DispatcherTimer timerVideoTime = new DispatcherTimer();
            timerVideoTime.Interval = TimeSpan.FromSeconds(1);
            timerVideoTime.Tick += new EventHandler(timer_Tick);
            timerVideoTime.Start();
        }

        int min = 0;
        void timer_Tick(object sender, EventArgs e)
        {
            // Check if the movie finished calculate it's total time
            string sec;
            string duration;
            sec = Math.Round(PlayMusic.Position.TotalSeconds).ToString();
            int seconds = Convert.ToInt32(sec);
            if (seconds < 10)
            {
                if (min < 9)
                {
                    CurrentPosMusicTB.Text = "0"+min.ToString()+":0" + sec;
                }
            }
            else if (seconds >= 60)
            {
                if (seconds / 60 - min == 1)
                {
                    min++;
                }
                seconds = seconds - (min * 60);
                if (seconds < 10)
                {
                    sec = "0"+seconds.ToString();
                }
                else
                {
                    sec = seconds.ToString();
                }
                if (min > 9)
                {
                    duration = min.ToString() + ":" + sec;
                }
                else
                {
                    duration = "0" + min.ToString() + ":" + sec;
                }
                CurrentPosMusicTB.Text = duration;
            }
            else
            {
                CurrentPosMusicTB.Text = "00:"+sec;
            }

            int currentMediaTicks = (int)PlayMusic.Position.TotalSeconds;
            if (PlayMusic.NaturalDuration.HasTimeSpan)
            {
                long totalMediaTicks = PlayMusic.NaturalDuration.TimeSpan.Ticks;
                if (totalMediaTicks > 0)
                    TimelineSlider.Value = currentMediaTicks;
                else
                    TimelineSlider.Value = TimelineSlider.Maximum;
            }

            if (CurrentPosMusicTB.Text == chartsList[index].musicDuration)
            {
                TimelineSlider.Value = TimelineSlider.Maximum;
            }
        }

        //THINK
        private void ChartsButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Source = new Uri("ChartsPage.xaml", UriKind.Relative);
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            Search search = new Search();
            MainFrame.NavigationService.Navigate(search);
        }

        private void FavoritesButton_Click(object sender, RoutedEventArgs e)
        {
            FavoritesPage favoritesPage = new FavoritesPage(id_user);
            MainFrame.NavigationService.Navigate(favoritesPage);
        }


        /*private void TimelineSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (TotalTime.TotalSeconds > 0)
            {
                PlayMusic.Position = TimeSpan.FromSeconds(TimelineSlider.Value);
            }
        }*/
    }
}
