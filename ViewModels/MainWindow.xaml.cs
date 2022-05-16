using System;
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


namespace MusicLover
{
    /// <summary>
    /// Прокофьев Иван ПКС-305
    /// </summary>
    /// DESKTOP-TJLQPJN\SQLEXPRESS
    public partial class MainWindow : Window
    {
       
        string conn = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
        char[] alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ErrorText()
        {
            ErrorLogPass.Visibility = Visibility.Visible;
            ErrorLogPass.Background = Brushes.LightPink;
            ErrorLogPass.Text = "Неправильный логин или пароль";
        }

        private void ErrorUserExists()
        {
            ErrorLogPass.Visibility = Visibility.Visible;
            ErrorLogPass.Background = Brushes.LightPink;
            ErrorLogPass.Text = "Пользователь с таким логином уже существует";
        }

        private bool Validation(string log, string pas)
        {
            if(log == "" && pas == "")
            {
                ErrorText();
                return false;
            }
            log.ToLower();
            for(int i = 0; i < log.Length; i++)
            {
                if(alphabet.Contains(log[i]) == false)
                {
                    ErrorText();
                    return false;
                }
            }
            pas.ToLower();
            for (int i = 0; i < pas.Length; i++)
            {
                if (alphabet.Contains(pas[i]) == false)
                {
                    ErrorText();
                    return false;
                }
            }
            return true;
        }
        
        private bool UserValidation(string log, string pas)
        {
            SqlConnection connection = new SqlConnection(conn);
            bool flag = true;
            using (connection)
            {
                try
                {
                    connection.Open();

                    string query = "Select login, password, id_user from [user] where (login = @log and password = @pas)";
                    SqlCommand sqlCommand = new SqlCommand(query, connection);
                    sqlCommand.Parameters.Add(new SqlParameter("log", log));
                    sqlCommand.Parameters.Add(new SqlParameter("pas", pas));
                    SqlDataReader reader = sqlCommand.ExecuteReader();
                    if (reader.HasRows == true)
                    {
                        ErrorUserExists();
                        flag = false;
                    }
                    else
                    {
                        reader.Close();
                        string queryIdUser = "Select top(1) id_user from [user] order by id_user DESC";
                        SqlCommand sqlCommandIdUser = new SqlCommand(queryIdUser, connection);
                        SqlDataReader readerIdUser = sqlCommandIdUser.ExecuteReader();
                        while (readerIdUser.Read())
                        {
                            id_user = (Int32)readerIdUser[0]+1;
                        }
                        readerIdUser.Close();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    flag = false;
                }

            }
            return flag;
        }
        
        private bool SignIn(string log, string pas)
        {
            SqlConnection connection = new SqlConnection(conn);
            bool flag = true;
            using (connection)
            {
                try
                {
                    connection.Open();

                    string query = "Select login, password, id_user from [user] where (login = @log and password = @pas)";
                    SqlCommand sqlCommand = new SqlCommand(query, connection);
                    sqlCommand.Parameters.Add(new SqlParameter("log", log));
                    sqlCommand.Parameters.Add(new SqlParameter("pas", pas));
                    SqlDataReader reader = sqlCommand.ExecuteReader();
                    if (reader.HasRows == false)
                    {
                        ErrorText();
                        flag = false;
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            id_user = (Int32)reader[2];
                        }
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    flag = false;
                }
                
            }
            return flag;
        }

        private bool SignUp(string log, string pas)
        {
            SqlConnection connection = new SqlConnection(conn);
            bool flag = true;
            using (connection)
            {
                try
                {
                    connection.Open();

                    string query = "Insert into [user] values(@log, @pas, @id)";
                    SqlCommand sqlCommand = new SqlCommand(query, connection);
                    //sqlCommand.Connection = connection;

                    SqlParameter loginUser = new SqlParameter();
                    loginUser.ParameterName = "@log";
                    loginUser.SqlDbType = SqlDbType.NVarChar;
                    loginUser.Direction = ParameterDirection.Input;

                    SqlParameter passwordUser = new SqlParameter();
                    passwordUser.ParameterName = "@pas";
                    passwordUser.SqlDbType = SqlDbType.NVarChar;
                    passwordUser.Direction = ParameterDirection.Input;

                    SqlParameter idUserType = new SqlParameter();
                    idUserType.ParameterName = "@id";
                    idUserType.SqlDbType = SqlDbType.Int;
                    idUserType.Direction = ParameterDirection.Input;

                    loginUser.Value = log;
                    passwordUser.Value = pas;
                    idUserType.Value = (Int32)2;

                    sqlCommand.Parameters.Add(loginUser);
                    sqlCommand.Parameters.Add(passwordUser);
                    sqlCommand.Parameters.Add(idUserType);

                    if (UserValidation(log, pas) == true)
                    {
                        sqlCommand.ExecuteNonQuery();
                    }
                    else
                    {
                        flag = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    flag = false;
                }
            }
                
            return flag;
        }

        int id_user;
        private void Registry_Button_Click(object sender, RoutedEventArgs e)
        {
            string login = Login.Text;
            string password = Password.Text;
            if (Validation(login, password) == true)
            {
                if (SignUp(login, password) == true)
                {
                    MainPage mainPage = new MainPage(login, id_user);
                    this.Close();
                    mainPage.ShowDialog();

                }
            }

        }

        private void Have_Acc_Click(object sender, RoutedEventArgs e)
        {
            string login = Login.Text;
            string password = Password.Text;
            if (Validation(login, password) == true)
            {
                if (SignIn(login, password) == true)
                {
                    MainPage mainPage = new MainPage(login, id_user);
                    this.Close();
                    mainPage.ShowDialog();

                }
            }
        }

        private void Login_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        int counterShowPassword = 0;
        private void ShowHidePassword_Click(object sender, RoutedEventArgs e)
        {
            if (counterShowPassword == 1)
            {
                PasswordPB.Password = Password.Text;
                Password.Visibility = Visibility.Collapsed;
                PasswordPB.Visibility = Visibility.Visible;
                counterShowPassword--;
            }
            else if (counterShowPassword == 0)
            {
                Password.Text = PasswordPB.Password;
                PasswordPB.Visibility = Visibility.Collapsed;
                Password.Visibility = Visibility.Visible;
                counterShowPassword++;
            }
        }

        private void Password_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void PasswordPB_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Password.Text = PasswordPB.Password;
        }
    }
}

