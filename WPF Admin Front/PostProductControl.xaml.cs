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
    /// Interaction logic for PostProductControl.xaml
    /// </summary>
    public partial class PostProductControl : UserControl
    {
        private ServerConnection connection;
        private List<Category> categories = new List<Category>();
        private List<Subcategory> subcategories = new List<Subcategory>();
        private List<string> units = new List<string> { "db", "kg", "csomag", "tálca", "doboz", "zacskó", "karton", "tekercs", "lap" };

        public event EventHandler ProductAdded;

        public PostProductControl(ServerConnection connection, List<Category> categories, List<Subcategory> subcategories)
        {
            InitializeComponent();
            Init(connection, categories, subcategories);
        }

        // Ezt a metódust hívja meg a ProductsControl a szükséges adatok átadásához
        public void Init(ServerConnection connection, List<Category> categories, List<Subcategory> subcategories)
        {
            this.connection = connection;
            this.categories = categories;
            this.subcategories = subcategories;

            CategoryBox.ItemsSource = categories;
            CategoryBox.SelectedIndex = -1;
            SubcategoryBox.ItemsSource = subcategories;
            SubcategoryBox.SelectedIndex = -1;
            UnitBox.ItemsSource = units;
            UnitBox.SelectedIndex = -1;
        }

        private void CategoryBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCategory = CategoryBox.SelectedItem as Category;
            if (selectedCategory != null)
            {
                var filteredSubcategories = subcategories
                    .Where(sc => sc.categoryId == selectedCategory.id).ToList();
                SubcategoryBox.ItemsSource = filteredSubcategories;
                SubcategoryBox.SelectedIndex = -1;
            }
        }

        private async void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            string sku = SkuBox.Text.Trim();
            string name = NameBox.Text.Trim();
            bool isActive = IsActiveBox.IsChecked == true;

            Category selectedCategory = CategoryBox.SelectedItem as Category;
            if (selectedCategory == null)
            {
                MessageBox.Show("Válassz kategóriát!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            int categoryId = selectedCategory.id;

            string unit = UnitBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(unit))
            {
                MessageBox.Show("Válassz mértékegységet!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            int subcategoryId = 0;
            Subcategory selectedSubcategory = SubcategoryBox.SelectedItem as Subcategory;
            if (selectedSubcategory != null)
                subcategoryId = selectedSubcategory.id;

            if (string.IsNullOrWhiteSpace(sku) || string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("SKU és név megadása kötelező!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(PriceBox.Text.Trim(), out int price))
            {
                MessageBox.Show("Az ár csak egész szám lehet!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(StockQuantityBox.Text.Trim(), out int stockQuantity))
            {
                MessageBox.Show("A mennyiség csak egész szám lehet!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(MinStockLevelBox.Text.Trim(), out int minStockLevel))
            {
                MessageBox.Show("A minimális mennyiség csak egész szám lehet!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newProduct = new Product
            {
                sku = sku,
                name = name,
                categoryId = categoryId,
                subcategoryId = subcategoryId,
                unit = unit,
                price = price,
                stockQuantity = stockQuantity,
                minStockLevel = minStockLevel,
                isActive = isActive
            };

            bool success = await connection.PostProduct(newProduct);

            if (success)
            {
                ProductAdded?.Invoke(this, EventArgs.Empty);

                // Mezők ürítése
                SkuBox.Text = "";
                NameBox.Text = "";
                CategoryBox.SelectedIndex = -1;
                SubcategoryBox.Text = "";
                UnitBox.SelectedIndex = -1;
                PriceBox.Text = "";
                StockQuantityBox.Text = "";
                MinStockLevelBox.Text = "";
                IsActiveBox.IsChecked = true;
            }
        }
    }
}
