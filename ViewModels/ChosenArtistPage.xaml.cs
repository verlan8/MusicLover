using MusicLover.models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
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

namespace MusicLover.ViewModels
{
    /// <summary>
    /// Логика взаимодействия для ChosenArtistPage.xaml
    /// </summary>
    public partial class ChosenArtistPage : Page
    {
        public ChosenArtistPage(int artist)
        {
            InitializeComponent();
            ConnectToDB(artist);
        }

        string conn = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
        //SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-TJLQPJN\SQLEXPRESS;Initial Catalog = music_lover; Integrated Security = True");
        //SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-TJLQPJN\SQLEXPRESS;Initial Catalog = music_lover; Integrated Security = True");

        List<Album> albums = new List<Album>();

        List<Charts> chartsList = new List<Charts>();


        private void ConnectToDB(int id_artist)
        {
            SqlConnection connection = new SqlConnection(conn);
            using (connection)
            {
                try
                {
                    connection.Open();

                    //альбомы
                    string queryAlbum = "SelectArtistsAlbums";
                    SqlCommand sqlCommandAlbum = new SqlCommand(queryAlbum, connection);
                    sqlCommandAlbum.CommandType = CommandType.StoredProcedure;
                    sqlCommandAlbum.Parameters.AddWithValue("@id_artists", SqlDbType.NVarChar).Value = id_artist;
                    SqlDataReader readerAlbum = sqlCommandAlbum.ExecuteReader();
                    while (readerAlbum.Read())
                    {
                        Album album = new Album();
                        byte[] imageBytes = (byte[])readerAlbum[2];
                        MemoryStream ms = new MemoryStream();
                        ms.Write(imageBytes, 0, imageBytes.Length);
                        BitmapImage bmp = new BitmapImage();
                        bmp.BeginInit();
                        bmp.StreamSource = ms;
                        bmp.EndInit();
                        album.albumImage = bmp;
                        album.albumName = Convert.ToString(readerAlbum[1]);
                        album.albumId = (Int32)readerAlbum[0];
                        albums.Add(album);
                    }
                    if (readerAlbum.HasRows == false)
                    {
                        AlbumLabel.Visibility = Visibility.Collapsed;
                        Grid.SetRowSpan(SongsLV, 2);
                    }
                    readerAlbum.Close();
                    AlbumsItemsContol.ItemsSource = albums;

                    //песни
                    string querySongs = "SelectArtistsSongs";
                    SqlCommand sqlCommandSongs = new SqlCommand(querySongs, connection);
                    sqlCommandSongs.CommandType = CommandType.StoredProcedure;
                    sqlCommandSongs.Parameters.AddWithValue("@id_artists", SqlDbType.NVarChar).Value = id_artist;
                    SqlDataReader readerSongs = sqlCommandSongs.ExecuteReader();
                    int pos_music = 1;
                    while (readerSongs.Read())
                    {
                        Charts charts = new Charts();
                        charts.chartPos = pos_music;
                        charts.musicArtist = readerSongs[1].ToString();
                        charts.musicName = readerSongs[2].ToString();
                        string min, sec, duration;
                        min = Convert.ToString((Int32)readerSongs[3] / 100);
                        sec = Convert.ToString((Int32)readerSongs[3] % 100);
                        if (min.Length == 1) min = "0" + min;
                        if (sec.Length == 1) sec = "0" + sec;
                        duration = min + ":" + sec;
                        charts.musicDuration = duration;
                        charts.musicId = (Int32)readerSongs[0];
                        chartsList.Add(charts);
                        pos_music++;

                    }
                    if (readerSongs.HasRows == false)
                    {
                        SongsScroll.Visibility = Visibility.Collapsed;
                    }
                    readerSongs.Close();
                    SongsLV.ItemsSource = chartsList;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


            }
        }

        private void Tracks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var main = Window.GetWindow(this);
            MainPage mainPage = (MainPage)main;
            mainPage.SetListOfSongs(chartsList, SongsLV.SelectedIndex);
        }

        private void OpenAlbums_Click(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                Button btn = sender as Button;
                var dataObject = btn.DataContext as Album;
                int albumId = dataObject.albumId;
                ChosenAlbumPage albumPage = new ChosenAlbumPage(albumId);
                NavigationService.Navigate(albumPage);
            }
        }
    }
}
