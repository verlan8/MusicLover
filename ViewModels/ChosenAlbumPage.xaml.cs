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


namespace MusicLover.ViewModels
{
    /// <summary>
    /// Логика взаимодействия для ChosenAlbumPage.xaml
    /// </summary>
    public partial class ChosenAlbumPage : Page
    {
        public ChosenAlbumPage(int albumId)
        {
            InitializeComponent();
            ConnectToDB(albumId);
        }

        //SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-TJLQPJN\SQLEXPRESS;Initial Catalog = music_lover; Integrated Security = True");
        SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-PMJ80NN\SQLEXPRESS;Initial Catalog = music_lover; Integrated Security = True");

        List<Album> albums = new List<Album>();

        List<Charts> chartsList = new List<Charts>();

        private void ConnectToDB(int albumId)
        {
            using (connection)
            {
                try
                {
                    connection.Open();

                    //альбомы
                    string queryAlbum = "SelectOneAlbum";
                    SqlCommand sqlCommandAlbum = new SqlCommand(queryAlbum, connection);
                    sqlCommandAlbum.CommandType = CommandType.StoredProcedure;
                    sqlCommandAlbum.Parameters.AddWithValue("@id_album", SqlDbType.Int).Value = albumId;
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
                    string queryCharts = "SelectTracksWithAlbums";
                    DataTable dtCharts = new DataTable();
                    SqlCommand sqlCommandCharts = new SqlCommand(queryCharts, connection);
                    sqlCommandCharts.CommandType = CommandType.StoredProcedure;
                    sqlCommandCharts.Parameters.AddWithValue("@id_album", SqlDbType.Int).Value = albumId;
                    SqlDataReader readerCharts = sqlCommandCharts.ExecuteReader();
                    int pos_music = 1;
                    while (readerCharts.Read())
                    {
                        Charts charts = new Charts();
                        charts.chartPos = pos_music;
                        charts.musicArtist = readerCharts[1].ToString();
                        charts.musicName = readerCharts[2].ToString();
                        string min, sec, duration;
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
            var main = Window.GetWindow(this);
            MainPage mainPage = (MainPage)main;
            mainPage.SetListOfSongs(chartsList, 0);

        }
    }
}
