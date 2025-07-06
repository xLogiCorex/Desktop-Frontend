using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Desktop
{
    public partial class ProductsControl : UserControl
    {
        private ServerConnection connection;
        private List<Product> allProduct = new List<Product>();
        private List<Category> categories = new List<Category>();
        private List<string> units = new List<string>
        {
            "db", "kg", "dkg", "g", "liter", "ml", "csomag", "tálca",
            "üveg", "doboz", "zacskó", "karton", "tekercs", "láb", "pár", "lap"
        };

        public ProductsControl(ServerConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
            Loaded += ProductsControl_Loaded;
        }

        private async void ProductsControl_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadCategoriesAsync();
            await LoadProductsAsync();
           

            UnitBox.ItemsSource = units;
            UnitBox.SelectedIndex = -1;
        }

        public async Task LoadProductsAsync()
        {
            ProductsDataGrid.ItemsSource = null;
            allProduct = await connection.GetProduct();

            foreach (var product in allProduct)
            {
                var cat = categories.FirstOrDefault(c => c.id == product.categoryId);
                product.categoryName = cat != null ? cat.name : "Ismeretlen";
            }
            ProductsDataGrid.ItemsSource = allProduct;
        }

        private async Task LoadCategoriesAsync()
        {
            categories = await connection.GetCategories();
            CategoryBox.ItemsSource = categories;
            CategoryBox.SelectedIndex = -1;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                ProductsDataGrid.ItemsSource = allProduct;
            }
            else
            {
                var filtered = allProduct.Where(u =>
                    (u.name != null && u.name.ToLower().Contains(searchText)) ||
                    (u.sku != null && u.sku.ToLower().Contains(searchText))
                ).ToList();

                ProductsDataGrid.ItemsSource = filtered;
            }
        }

        private async void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            string sku = SkuBox.Text.Trim();
            string name = NameBox.Text.Trim();
            bool isActive = IsActiveBox.IsChecked == true;

            // Kategória validáció
            Category selectedCategory = CategoryBox.SelectedItem as Category;
            if (selectedCategory == null)
            {
                MessageBox.Show("Válassz kategóriát!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            int categoryId = selectedCategory.id;

            // Mértékegység validáció
            string unit = UnitBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(unit))
            {
                MessageBox.Show("Válassz mértékegységet!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Alkategória
            int? subcategoryId = null;
            if (!string.IsNullOrWhiteSpace(SubcategoryBox.Text))
            {
                if (int.TryParse(SubcategoryBox.Text.Trim(), out int scid))
                    subcategoryId = scid;
            }



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
                subcategoryId = subcategoryId ?? 0,
                unit = unit,
                price = price,
                stockQuantity = stockQuantity,
                minStockLevel = minStockLevel,
                isActive = isActive
            };

            // Küldés a szerverre
            bool success = await connection.PostProduct(newProduct);

            if (success)
            {
                await LoadProductsAsync();
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