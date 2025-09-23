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
    /// Interaction logic for InvoicesControl.xaml
    /// </summary>
    public partial class InvoicesControl : UserControl
    {
        private ServerConnection connection;
        private List<Orders> allOrders = new List<Orders>();
        private List<Partner> partners = new List<Partner>();
        private List<User> users = new List<User>();
        private List<Invoices> allInvoices = new List<Invoices>();


        public InvoicesControl(ServerConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
            Loaded += InvoicesControl_Loaded;
        }
        private async void InvoicesControl_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadPartnersAsync();
            await LoadUsersAsync();
            await LoadOrdersAsync();
            await LoadInvoicesAsync();
        }
        public async Task LoadInvoicesAsync()
        {
            InvoicesDataGrid.ItemsSource = null;
            allInvoices = await connection.GetInvoices();

            foreach (var invoice in allInvoices)
            {
                var order = allOrders.FirstOrDefault(o => o.id == invoice.id);
                invoice.orderNumber = order != null ? order.orderNumber : "Ismeretlen rendelés";

                var partner = partners.FirstOrDefault(p => p.id == invoice.partnerId);
                invoice.partnerName = partner != null ? partner.name : "Ismeretlen partner";

                var user = users.FirstOrDefault(u => u.id == invoice.userId); 
                invoice.username = user != null ? user.name : "Ismeretlen felhasználó";
            }
            InvoicesDataGrid.ItemsSource = allInvoices;
        }
        private async Task LoadPartnersAsync() => partners = await connection.GetPartners();
        private async Task LoadUsersAsync() => users = await connection.GetUsers();
        private async Task LoadOrdersAsync() => allOrders = await connection.GetOrders();
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText)) InvoicesDataGrid.ItemsSource = allInvoices;
            else
            {
                var filtered = allInvoices.Where(u =>
                    (u.partnerName != null && u.partnerName.ToLower().Contains(searchText)) ||
                    (u.issueDate != null && u.issueDate.ToString().Contains(searchText))).ToList();
                InvoicesDataGrid.ItemsSource = filtered;
            }
        }
    }
}
