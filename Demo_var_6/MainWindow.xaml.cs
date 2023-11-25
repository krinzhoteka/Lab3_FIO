using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
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
using EasyCaptcha.Wpf;
using Demo_var_6.VMs;

namespace Demo_var_6
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string connectionString = "Data Source=KRINZHOTEKA\\KRINZHOTEKA;Initial Catalog=Demo_var_6;Integrated Security=True";
        private string captchaText;

        public MainWindow()
        {
            InitializeComponent();
            GenerateCaptcha();
        }

        public void GenerateCaptcha()
        {
            Captcha.CreateCaptcha(Captcha.LetterOption.Alphanumeric, 5);
            captchaText = Captcha.CaptchaText;
            CaptchaTextBox.Clear();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль.");
                return;
            }

            if (Login(username, password))
            {
                Product productWindow = new Product();
                productWindow.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Неправильный логин или пароль.");
            }
        }

        public bool Login(string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Users WHERE UserLogin = @Username AND UserPassword = @Password", connection);
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    int userCount = (int)command.ExecuteScalar();

                    return userCount > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка: " + ex.Message);
                    return false;
                }
            }
        }
    }
}
