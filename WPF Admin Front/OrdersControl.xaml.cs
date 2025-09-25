using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for OrdersControl.xaml
    /// </summary>
    public partial class OrdersControl : UserControl
    {
        private ServerConnection connection;
        private List<Orders> allOrders = new List<Orders>();
        private List<Partner> partners = new List<Partner>();
        private List<User> users = new List<User>();
        private List<Product> allProducts = new List<Product>();
        private List<Invoices> invoices = new List<Invoices>();

        public OrdersControl(ServerConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
            Loaded += OrdersControl_Loaded;

        }
        private async void OrdersControl_Loaded(object s, RoutedEventArgs e)
        {
            await LoadPartnersAsync();
            await LoadUsersAsync();
            await LoadInvoicesAsync();
            await LoadOrdersAsync();
            await LoadProductsAsync();
        }
        public async Task LoadOrdersAsync()
        {
            OrdersDataGrid.ItemsSource = null;
            allOrders = await connection.GetOrders();

            foreach (var order in allOrders)
            {
                var partner = partners.FirstOrDefault(p => p.id == order.partnerId);
                order.partnerName = partner != null ? partner.name : "Ismeretlen partner";

                var user = users.FirstOrDefault(u => u.id == order.userId);
                order.userName = user != null ? user.name : "Ismeretlen felhasználó";

                var invoice = invoices.FirstOrDefault(i => i.id == order.invoiceId);
                order.invoiceNumber = invoice != null ? invoice.invoiceNumber : "Nincs számla";
            }

            OrdersDataGrid.ItemsSource = allOrders;
        }
        private void SearchBox_TextChanged(object s, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText)) OrdersDataGrid.ItemsSource = allOrders;
            else
            {
                var filtered = allOrders.Where(u =>
                    (u.orderNumber != null && u.orderNumber.ToLower().Contains(searchText)) ||
                    (u.status != null && u.status.ToLower().Contains(searchText)) ||
                    (u.partnerName != null && u.partnerName.ToLower().Contains(searchText))).ToList();
                OrdersDataGrid.ItemsSource = filtered;
            }
        }
        private async Task LoadPartnersAsync() => partners = await connection.GetPartners();        
        private async Task LoadUsersAsync() => users = await connection.GetUsers();
        private async Task LoadProductsAsync() => allProducts = await connection.GetProduct();
        private async Task LoadInvoicesAsync() => invoices = await connection.GetInvoices();
        private async void OrdersDataGrid_MouseDoubleClick(object s, MouseButtonEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is Orders selectedOrder)
            {
                var orderItems = await connection.GetOrderItems(selectedOrder.id);

                // Termék nevének hozzárendelése
                foreach (var item in orderItems)
                {
                    var product = allProducts.FirstOrDefault(p => p.id == item.productId);
                    item.productName = product != null ? product.name : "Ismeretlen termék";
                }

                OrderItemsDataGrid.ItemsSource = orderItems;
            }
        }
        private async void OrderCompleted_Click(object s, EventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is Orders selectedOrder)
            {
                selectedOrder.status = "Completed";
                OrdersDataGrid.Items.Refresh();

                try
                {
                    await connection.OrderAsCompletedAsync(selectedOrder.id);
                    MessageBox.Show("A rendelés státusza 'Completed'-re állítva és a számla generálva.", "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba történt: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Kérlek, válassz ki egy rendelést a listából!", "Figyelem", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }


    }
    
}
