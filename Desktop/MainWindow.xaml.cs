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

namespace Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ServerConnection connection = new ServerConnection("http://127.1.1.1:3000");
        public MainWindow()
        {
            InitializeComponent();
        }
        async void Login(object s, EventArgs e)
        {
            var userLogin = await connection.Login(emailInput.Text, passwordInput.Password);
            if (userLogin == null || userLogin.email == null)
            {
                MessageBox.Show("Sikertelen bejelentkezés!");
                return;
            }

            if (userLogin.role == "admin")
            {
                var adminWin = new AdminApp();
                adminWin.Show();
                this.Close();
            }

            else
            {
                MessageBox.Show("Nincs jogosultságod belépni!");
            }
            //AdminApp b = new AdminApp() { Top = this.Top, Left = this.Left, Visibility = Visibility.Visible };
            this.Close();
        }
    }
}
