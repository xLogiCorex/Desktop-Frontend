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

    public partial class MainWindow : Window
    {
<<<<<<< Updated upstream
        public MainWindow()
=======
        ServerConnection connection;
        public MainWindow(ServerConnection connection)
>>>>>>> Stashed changes
        {
            InitializeComponent();
            this.connection = connection;
        }
<<<<<<< Updated upstream
=======
        void Users(object s, RoutedEventArgs e) { MainContent.Content = new UsersControl(connection); }
        void Products(object sender, RoutedEventArgs e) { MainContent.Content = new ProductsControl(connection); }
        void Partners(object sender, RoutedEventArgs e) { MainContent.Content = new PartnersControl(connection); }
        void Orders(object sender, RoutedEventArgs e) { }
        void Invoices(object sender, RoutedEventArgs e) { }
        void Logs(object sender, RoutedEventArgs e) { }
>>>>>>> Stashed changes
    }
}