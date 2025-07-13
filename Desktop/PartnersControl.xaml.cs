using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    /// Interaction logic for PartnersControl.xaml
    /// </summary>
    public partial class PartnersControl : UserControl
    {
        private ServerConnection connection;
        private List<Partner> allPartners = new List<Partner>();
        public PartnersControl(ServerConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
            Loaded += PartnerControl_Loaded;
        }

        private async void PartnerControl_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadPartnersAsync();
        }
        public async Task LoadPartnersAsync()
        {
            PartnersDataGrid.ItemsSource = null;
            allPartners = await connection.GetPartners();
            PartnersDataGrid.ItemsSource = allPartners;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                PartnersDataGrid.ItemsSource = allPartners;
            }
            else
            {
                // Szűrés név, email vagy kapcsolattartó alapján
                var filtered = allPartners.Where(u =>
                    (u.name != null && u.name.ToLower().Contains(searchText)) ||
                    (u.email != null && u.email.ToLower().Contains(searchText)) ||
                    (u.contactPerson != null && u.contactPerson.ToLower().Contains(searchText)) ||
                    (u.taxNumber.ToString().Contains(searchText))
                ).ToList();

                PartnersDataGrid.ItemsSource = filtered;
            }
        }
        private async void AddPartner_Click(object sender, RoutedEventArgs e) 
        {
            string name = NameBox.Text.Trim();
            string taxNumber = TaxNumberBox.Text.Trim();
            string email = EmailBox.Text.Trim();
            string address = AddressBox.Text.Trim();
            string contactPerson = ContactPersonBox.Text.Trim();
            string phone = PhoneBox.Text.Trim();
            bool isActive = IsActiveBox.IsChecked == true;

            if (string.IsNullOrWhiteSpace(taxNumber))
            {
                MessageBox.Show("A adószám csak egész szám lehet!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("Cím megadása kötelező!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(contactPerson))
            {
                MessageBox.Show("Kapcsolattartó megadása kötelező!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var newPartner = new Partner
            {
                name = name,
                taxNumber = taxNumber,
                address = address,
                contactPerson = contactPerson,
                email = email,
                phone = phone,
                isActive = isActive,
            };

            bool succsess = await connection.PostPartner(newPartner);
            if (succsess) {
                await LoadPartnersAsync();
                NameBox.Text = "";
                TaxNumberBox.Text = "";
                AddressBox.Text = "";
                ContactPersonBox.Text = "";
                EmailBox.Text = "";
                PhoneBox.Text = "";
                IsActiveBox.IsChecked = true;
            }


        }
    }
}
