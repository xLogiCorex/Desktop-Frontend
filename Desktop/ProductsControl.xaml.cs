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
        private List<Subcategory> subcategories = new List<Subcategory>();

        public ProductsControl(ServerConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
            Loaded += ProductsControl_Loaded;
        }

        private async void ProductsControl_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadCategoriesAsync();
            await LoadSubcategoriesAsync();
            await LoadProductsAsync();
        }

        public async Task LoadProductsAsync()
        {
            ProductsDataGrid.ItemsSource = null;
            allProduct = await connection.GetProduct();

            foreach (var product in allProduct)
            {
                var cat = categories.FirstOrDefault(c => c.id == product.categoryId);
                product.categoryName = cat != null ? cat.name : "Ismeretlen";

                var subcat = subcategories.FirstOrDefault(sc => sc.id == product.subcategoryId);
                product.subcategoryName = subcat != null ? subcat.name : "Ismeretlen";
            }

            ProductsDataGrid.ItemsSource = allProduct;
        }

        private async Task LoadCategoriesAsync()
        {
            categories = await connection.GetCategories();
        }

        private async Task LoadSubcategoriesAsync()
        {
            subcategories = await connection.GetSubcategories();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText)) ProductsDataGrid.ItemsSource = allProduct;
            else
            {
                var filtered = allProduct.Where(u =>
                    (u.name != null && u.name.ToLower().Contains(searchText)) ||
                    (u.sku != null && u.sku.ToLower().Contains(searchText))).ToList();
                ProductsDataGrid.ItemsSource = filtered;
            }
        }

        private void ShowProductPanel_Click(object sender, RoutedEventArgs e)
        {
            var panel = new PostProductControl(connection, categories, subcategories);
            panel.ProductAdded += async (s, ev) => await LoadProductsAsync(); // frissül a bal oldal minden gombnyomésra
            RightPanelContent.Content = panel;
        }

        private void ShowCategoryPanel_Click(object sender, RoutedEventArgs e)
        {
            var panel = new PostCategoryControl(connection, categories);
            panel.CategoryAdded += async (s, ev) => await LoadCategoriesAsync();
            RightPanelContent.Content = panel;
        }

        private void ShowSubcategoryPanel_Click(object sender, RoutedEventArgs e)
        {
            var panel = new PostSubcategoryControl(connection, categories);
            panel.SubcategoryAdded += async (s, ev) => await LoadSubcategoriesAsync();
            RightPanelContent.Content = panel;
        }

    }
}
