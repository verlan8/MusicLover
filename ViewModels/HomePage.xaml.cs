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
using MusicLover.models;
using System.Configuration;

namespace MusicLover.ViewModels
{
    /// <summary>
    /// Логика взаимодействия для HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            ConnectToDB();
            
        }
        string conn = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
        //SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-TJLQPJN\SQLEXPRESS;Initial Catalog = music_lover; Integrated Security = True");
        //SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-PMJ80NN\SQLEXPRESS;Initial Catalog = music_lover; Integrated Security = True");

        List<Genre> genreList = new List<Genre>();

        List<Album> albums = new List<Album>();

        List<Charts> chartsList = new List<Charts>();
        
        
        private void ConnectToDB()
        {
            SqlConnection connection = new SqlConnection(conn);
            using (connection)
            {
                connection.Open();
                
                //жанры
                string query = "SelectGenres";
                //DataTable dt = new DataTable();
                SqlCommand sqlCommand = new SqlCommand(query, connection);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = sqlCommand.ExecuteReader();
                while (reader.Read())
                {
                    Genre genre = new Genre();
                    byte[] imageBytes = (byte[])reader[2];
                    MemoryStream ms = new MemoryStream();
                    ms.Write(imageBytes, 0, imageBytes.Length);
                    BitmapImage bmp = new BitmapImage();
                    bmp.BeginInit();
                    bmp.StreamSource = ms;
                    bmp.EndInit();
                    genre.genreImage = bmp;
                    genre.genreName = Convert.ToString(reader[1]);
                    genreList.Add(genre);
                }
                reader.Close();
                GenresItemsContol.ItemsSource = genreList;
                
                //альбомы
                string queryAlbum = "SelectAlbums";
                SqlCommand sqlCommandAlbum = new SqlCommand(queryAlbum, connection);
                sqlCommandAlbum.CommandType = CommandType.StoredProcedure;
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
                    album.albumId = (Int32)readerAlbum[0];
                    album.albumName = Convert.ToString(readerAlbum[1]);
                    albums.Add(album);
                }
                readerAlbum.Close();
                AlbumsItemsContol.ItemsSource = albums;


                //чарты
                string queryCharts = "SelectChartsAuditions";
                DataTable dtCharts = new DataTable();
                SqlCommand sqlCommandCharts = new SqlCommand(queryCharts, connection);
                sqlCommandCharts.CommandType = CommandType.StoredProcedure;
                SqlDataReader readerCharts = sqlCommandCharts.ExecuteReader();
                int pos_music = 1;
                while (readerCharts.Read())
                {
                    Charts charts = new Charts();
                    charts.chartPos = pos_music;
                    charts.musicArtist = readerCharts[1].ToString();
                    charts.musicName = readerCharts[2].ToString();
                    string min,sec,duration;
                    min = Convert.ToString((Int32)readerCharts[3] / 100);
                    sec = Convert.ToString((Int32)readerCharts[3] % 100);
                    if (min.Length == 1) min = "0" + min;
                    if (sec.Length == 1) sec = "0" + sec;
                    duration = min + ":" + sec;
                    charts.musicDuration = duration;
                    charts.musicId = (Int32)readerCharts[0];
                    chartsList.Add(charts);
                    pos_music++;

                }
                readerCharts.Close();
                AuditionsCharts.ItemsSource = chartsList;
                
            }
        }

        private void OpenGenre_Click(object sender, RoutedEventArgs e)
        {

            if (sender != null)
            {
                Button btn = sender as Button;
                var dataObject = btn.DataContext as Genre;
                string genreTitle = dataObject.genreName;
                ChosenGenrePage genrePage = new ChosenGenrePage(genreTitle);
                NavigationService.Navigate(genrePage);
            }
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

        private void AuditionsCharts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var main = Window.GetWindow(this);
            MainPage mainPage = (MainPage)main;
            mainPage.SetListOfSongs(chartsList, AuditionsCharts.SelectedIndex);
        }
    }
}
