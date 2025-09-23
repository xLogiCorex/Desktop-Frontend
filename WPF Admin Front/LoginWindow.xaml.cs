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
using System.Windows.Shapes;

namespace WPF_Admin_Front
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        ServerConnection connection = new ServerConnection("http://127.1.1.1:3000");
        public LoginWindow()
        {
            InitializeComponent();
        }
        async void Login(object s, RoutedEventArgs e)
        {
            var userLogin = await connection.Login(emailInput.Text, passwordInput.Password);
            if (userLogin == null || userLogin.email == null)
            {
                MessageBox.Show("Sikertelen bejelentkezés!");
                return;
            }

            if (userLogin.role == "admin")
            {

                var adminWin = new MainWindow(connection);
                adminWin.Show();
                this.Close();
            }

            else
            {
                MessageBox.Show("Nincs jogosultságod belépni!");
            }
            this.Close();
        }
    }
}
