using MusicLover.models;
using System;
using System.IO;
using System.Collections.Generic;
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
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace MusicLover.ViewModels
{
    /// <summary>
    /// Логика взаимодействия для Search.xaml
    /// </summary>
    public partial class Search : Page
    {
        public Search()
        {
            InitializeComponent();
            CollapseElements();
        }

        private void CollapseElements()
        {
            ArtistLabel.Visibility = Visibility.Collapsed;
            AlbumLabel.Visibility = Visibility.Collapsed;
            SongsScroll.Visibility = Visibility.Collapsed;
        }

        private void ShowElements()
        {
            ArtistLabel.Visibility = Visibility.Visible;
            AlbumLabel.Visibility = Visibility.Visible;
            SongsScroll.Visibility = Visibility.Visible;
        }

        private void ClearLists()
        {
            for (int i = 0; i < artistsList.Count; i++)
            {
                ArtistsItemsContol.Items.Remove(artistsList[i]);
            }
            artistsList.Clear();
            for (int i = 0; i < albums.Count; i++)
            {
                ArtistsItemsContol.Items.Remove(albums[i]);
            }
            albums.Clear();
            for (int i = 0; i < chartsList.Count; i++)
            {
                ArtistsItemsContol.Items.Remove(chartsList[i]);
            }
            chartsList.Clear();
        }

        string conn = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
        //SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-TJLQPJN\SQLEXPRESS;Initial Catalog = music_lover; Integrated Security = True");
        //SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-TJLQPJN\SQLEXPRESS;Initial Catalog = music_lover; Integrated Security = True");

        List<Artists> artistsList = new List<Artists>();

        List<Album> albums = new List<Album>();

        List<Charts> chartsList = new List<Charts>();


        bool artistFlag = true;
        bool albumFlag = true;
        bool songsFlag = true;
        private void ConnectToDB(string str)
        {
            ShowElements();
            ClearLists();
            SqlConnection connection = new SqlConnection(conn);
            using (connection)
            {
                try
                {
                    connection.Open();

                    //музыканты
                    string queryArtists = "SelectArtists";
                    SqlCommand sqlCommandArtists = new SqlCommand(queryArtists, connection);
                    sqlCommandArtists.CommandType = CommandType.StoredProcedure;
                    sqlCommandArtists.Parameters.AddWithValue("@str", SqlDbType.NVarChar).Value = str;
                    SqlDataReader readerArtists = sqlCommandArtists.ExecuteReader();
                    while (readerArtists.Read())
                    {
                        Artists artists = new Artists();
                        artists.artistName = Convert.ToString(readerArtists[1]);
                        artists.artistId = (Int32)readerArtists[0];
                        artistsList.Add(artists);
                    }
                    if (readerArtists.HasRows == false)
                    {
                        artistFlag = false;
                        ArtistLabel.Visibility = Visibility.Collapsed;
                        Grid.SetRowSpan(AlbumScroll, 2);
                    }
                    readerArtists.Close();
                    //ArtistsItemsContol.ItemsSource = artistsList;
                    for (int i = 0; i < artistsList.Count; i++)
                    {
                        ArtistsItemsContol.Items.Add(artistsList[i]);
                    }

                    //альбомы
                    string queryAlbum = "SelectSearchingAlbum";
                    SqlCommand sqlCommandAlbum = new SqlCommand(queryAlbum, connection);
                    sqlCommandAlbum.CommandType = CommandType.StoredProcedure;
                    sqlCommandAlbum.Parameters.AddWithValue("@str", SqlDbType.NVarChar).Value = str;
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
                        albumFlag = false;
                        AlbumLabel.Visibility = Visibility.Collapsed;
                        Grid.SetRowSpan(SongsLV, 2);
                    }
                    readerAlbum.Close();
                    AlbumsItemsContol.ItemsSource = albums;

                    //песни
                    string querySongs = "SelectSongs";
                    SqlCommand sqlCommandSongs = new SqlCommand(querySongs, connection);
                    sqlCommandSongs.CommandType = CommandType.StoredProcedure;
                    sqlCommandSongs.Parameters.AddWithValue("@song_name", SqlDbType.NVarChar).Value = str;
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
                        songsFlag = false;
                        SongsScroll.Visibility = Visibility.Collapsed;
                    }
                    readerSongs.Close();
                    SongsLV.ItemsSource = chartsList;

                    if (!songsFlag && !albumFlag && !artistFlag)
                    {
                        NoSearchResults.Visibility = Visibility.Visible;
                    }
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

        private void OpenArtists_Click(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                Button btn = sender as Button;
                var dataObject = btn.DataContext as Artists;
                int artistId = dataObject.artistId;
                ChosenArtistPage chosenArtistPage = new ChosenArtistPage(artistId);
                NavigationService.Navigate(chosenArtistPage);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(SearchTB.Text))
            {
                ConnectToDB(SearchTB.Text);
            }
        }
    }
}
