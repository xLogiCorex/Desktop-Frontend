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
    /// Interaction logic for UsersControl.xaml
    /// </summary>
    public partial class UsersControl : UserControl
    {
        private ServerConnection connection;
        private List<User> allUsers = new List<User>();

        public UsersControl(ServerConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
            Loaded += UsersControl_Loaded;
        }
        private async void UsersControl_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadUsersAsync();
        }

        public async Task LoadUsersAsync()
        {
            UsersDataGrid.ItemsSource = null;
            allUsers = await connection.GetUsers();
            UsersDataGrid.ItemsSource = allUsers;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                UsersDataGrid.ItemsSource = allUsers;
            }
            else
            {
                // Szűrés név, email vagy szerepkör alapján
                var filtered = allUsers.Where(u =>
                    (u.name != null && u.name.ToLower().Contains(searchText)) ||
                    (u.email != null && u.email.ToLower().Contains(searchText)) ||
                    (u.role != null && u.role.ToLower().Contains(searchText))
                ).ToList();

                UsersDataGrid.ItemsSource = filtered;
            }
        }

        private async void AddUser_Click(object sender, RoutedEventArgs e)
        {
            // Adatok begyűjtése
            string name = NameBox.Text.Trim();
            string email = EmailBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            // Szerepkör validáció
            if (RoleBox.SelectedItem == null)
            {
                MessageBox.Show("Válassz szerepkört!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string role = RoleBox.SelectedItem as string;

            bool isActive = IsActiveBox.IsChecked == true;

            // Egyszerű validáció
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Minden mező kitöltése kötelező!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }



            // Új felhasználó létrehozása
            var newUser = new User
            {
                name = name,
                email = email,
                password = password,
                role = role,
                isActive = isActive
            };

            // Küldés a szerverre
            bool success = await connection.PostUser(newUser);
            foreach (var item in RoleBox.Items)
                MessageBox.Show("Item: " + item?.ToString());
            MessageBox.Show("SelectedItem: " + (RoleBox.SelectedItem?.ToString() ?? "null"));
            if (success)
            {
                // Lista frissítése
                await LoadUsersAsync();

                // Mezők ürítése
                NameBox.Text = "";
                EmailBox.Text = "";
                PasswordBox.Password = "";
                RoleBox.SelectedIndex = -1;
                IsActiveBox.IsChecked = true;
            }
        }
        private async void UsersDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column is DataGridCheckBoxColumn)
            {
                var user = e.Row.Item as User;
                if (user != null)
                {
                    bool success = await connection.UpdateUserStatus(user.id, user.isActive);
                    if (!success)
                    {
                        MessageBox.Show("Nem sikerült a státusz módosítása!");
                        user.isActive = !user.isActive;
                        UsersDataGrid.Items.Refresh();
                    }
                    else
                    {
                        UsersDataGrid.Items.Refresh();
                        MessageBox.Show("Státusz módosítva!");
                    }
                }
            }
        }


    }
}
