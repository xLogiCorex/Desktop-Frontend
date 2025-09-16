using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_Admin_Front
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() { }
        ServerConnection connection;
        public MainWindow(ServerConnection connection)

        {
            InitializeComponent();
            this.connection = connection;
        }

        void Users(object s, RoutedEventArgs e) { MainContent.Content = new UsersControl(connection); }
        void Products(object sender, RoutedEventArgs e) { MainContent.Content = new ProductsControl(connection); }
        void Partners(object sender, RoutedEventArgs e) { MainContent.Content = new PartnersControl(connection); }
        void Orders(object sender, RoutedEventArgs e) { }
        void Invoices(object sender, RoutedEventArgs e) { }
        void Logs(object sender, RoutedEventArgs e) { }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            connection.Logout(); // Token és adatok törlése

            // Login ablak megnyitása
            var loginWindow = new LoginWindow();
            loginWindow.Show();

            // Főablak bezárása
            this.Close();
        }
    }
}