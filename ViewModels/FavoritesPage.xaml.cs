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
    /// Логика взаимодействия для FavoritesPage.xaml
    /// </summary>
    public partial class FavoritesPage : Page
    {
        private int id_user;
        public FavoritesPage(int user)
        {
            InitializeComponent();
            id_user = user;
            ConnectToDB();
        }

        string conn = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
        //SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-TJLQPJN\SQLEXPRESS;Initial Catalog = music_lover; Integrated Security = True");
        //SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-PMJ80NN\SQLEXPRESS;Initial Catalog = music_lover; Integrated Security = True");

        List<Charts> chartsList = new List<Charts>();

        private void ConnectToDB()
        {
            SqlConnection connection = new SqlConnection(conn);
            using (connection)
            {
                try
                {
                    connection.Open();

                    //чарты
                    string queryCharts = "SelectUserFavoritesSongs";
                    SqlCommand sqlCommandCharts = new SqlCommand(queryCharts, connection);
                    sqlCommandCharts.CommandType = CommandType.StoredProcedure;
                    sqlCommandCharts.Parameters.AddWithValue("@id_user", SqlDbType.Int).Value = id_user;
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

        private void SongsLV_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var main = Window.GetWindow(this);
            MainPage mainPage = (MainPage)main;
            mainPage.SetListOfSongs(chartsList, SongsLV.SelectedIndex);
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            DeleteChanges.Visibility = Visibility.Collapsed;
        }

        private void DeleteChanges_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection connection = new SqlConnection(conn);
            using (connection)
            {
                try
                {
                    connection.Open();
                    dynamic currentItem = SongsLV.SelectedItem;
                    int id_music = (Int32)currentItem.musicId;
                    string query = "RemoveLike";
                    SqlCommand sqlCommand = new SqlCommand(query, connection);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add(new SqlParameter("@id_user", id_user));
                    sqlCommand.Parameters.Add(new SqlParameter("@id_music", id_music));

                    int i = sqlCommand.ExecuteNonQuery();
                    if (i > 0)
                    {
                        FavoritesPage favoritesPage = new FavoritesPage(id_user);
                        NavigationService.Navigate(favoritesPage);
                    }
                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            DeleteChanges.Visibility = Visibility.Visible;
        }
    }
}
