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
    /// Interaction logic for PostSubcategoryControl.xaml
    /// </summary>
    public partial class PostSubcategoryControl : UserControl
    {
        private ServerConnection connection;
        private List<Category> categories;

        public event EventHandler SubcategoryAdded;

        public PostSubcategoryControl(ServerConnection connection, List<Category> categories)
        {
            InitializeComponent();
            this.connection = connection;
            this.categories = categories;

            // Feltöltjük a ComboBox-ot a kategóriákkal
            CategoryBox.ItemsSource = categories;
            CategoryBox.DisplayMemberPath = "name";
            CategoryBox.SelectedValuePath = "id";
            CategoryBox.SelectedIndex = -1;
        }

        private async void AddSubcategory_Click(object sender, RoutedEventArgs e)
        {
            string name = NameBox.Text.Trim();
            var selectedCategory = CategoryBox.SelectedItem as Category;

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Az alkategória neve kötelező!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selectedCategory == null)
            {
                MessageBox.Show("Válassz kategóriát!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int categoryId = selectedCategory.id;

            var newSubcategory = new Subcategory
            {
                name = name,
                categoryId = categoryId
            };

            // Feltételezzük, hogy van ilyen metódusod a szerver oldalon:
            bool success = await connection.PostSubcategory(newSubcategory);

            if (success)
            {
                SubcategoryAdded?.Invoke(this, EventArgs.Empty);
                NameBox.Text = "";
                CategoryBox.SelectedIndex = -1;
                MessageBox.Show("Alkategória sikeresen hozzáadva!", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Az alkategória hozzáadása sikertelen.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}