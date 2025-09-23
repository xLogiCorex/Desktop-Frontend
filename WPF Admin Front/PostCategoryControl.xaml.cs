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

namespace WPF_Admin_Front
{
    /// <summary>
    /// Interaction logic for PostCategoryControl.xaml
    /// </summary>
    public partial class PostCategoryControl : UserControl
    {
        ServerConnection connection;
        private List<Category> categories = new List<Category>();
        public event EventHandler CategoryAdded;
        public PostCategoryControl(ServerConnection connection, List<Category> categories)
        {
            InitializeComponent();
            this.connection = connection;
            this.categories = categories;
        }
        private async void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            string name = NameBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("A kategória neve kötelező!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newCategory = new Category
            {
                name = name
            };


            bool success = await connection.PostCategory(newCategory);

            if (success)
            {
                CategoryAdded?.Invoke(this, EventArgs.Empty);
                NameBox.Text = "";
                MessageBox.Show("Kategória sikeresen hozzáadva!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("A kategória hozzáadása sikertelen.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
