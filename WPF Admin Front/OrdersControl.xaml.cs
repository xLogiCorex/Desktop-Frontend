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
    /// Interaction logic for OrdersControl.xaml
    /// </summary>
    public partial class OrdersControl : UserControl
    {
        private ServerConnection connection;
        private List<Orders> allOrders = new List<Orders>();
        private List<Partner> partners = new List<Partner>();
        private List<User> users = new List<User>();
        private List<Product> allProducts = new List<Product>();
        //private List<Invoice> invoices = new List<Invoice>();

        public OrdersControl(ServerConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
            Loaded += OrdersControl_Loaded;
        }
        private async void OrdersControl_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadPartnersAsync();
            await LoadUsersAsync();
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

                //var invoice = invoices.FirstOrDefault(i => i.id == order.invoiceId);
                //order.invoiceNumber = invoice != null ? invoice.invoiceNumber : "";
            }

            OrdersDataGrid.ItemsSource = allOrders;
        }
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText)) OrdersDataGrid.ItemsSource = allOrders;
            else
            {
                var filtered = allOrders.Where(u =>
                    (u.orderNumber != null && u.orderNumber.ToLower().Contains(searchText)) ||
                    (u.status != null && u.status.ToLower().Contains(searchText))).ToList();
                OrdersDataGrid.ItemsSource = filtered;
            }
        }
        private async Task LoadPartnersAsync()
        {
            partners = await connection.GetPartners();
        }
        private async Task LoadUsersAsync()
        {
            users = await connection.GetUsers();
        }
        private async Task LoadProductsAsync()
        {
            allProducts = await connection.GetProduct();
        }
        private async void OrdersDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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
    }
    
}
