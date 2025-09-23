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
    /// Interaction logic for LogsControls.xaml
    /// </summary>
    public partial class LogsControls : UserControl
    {
        private ServerConnection connection;
        private List<Logs> allLogs = new List<Logs>();
        private List<User> users = new List<User>();

        public LogsControls(ServerConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
            Loaded += LogsControls_Loaded;
        }
        private async void LogsControls_Loaded(object s, RoutedEventArgs e)
        {
            await LoadUsersAsync();
            await LoadLogsAsync();
        }
        public async Task LoadLogsAsync()
        {
            LogsDataGrid.ItemsSource = null;
            allLogs = await connection.GetLogs();
            foreach (var log in allLogs)
            {
                var user = users.FirstOrDefault(u => u.id == log.userId);
                log.username = user != null ? user.name : "Ismeretlen felhasználó";
            }

            LogsDataGrid.ItemsSource = allLogs;
        }
        private void SearchBox_TextChanged(object s, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText)) LogsDataGrid.ItemsSource = allLogs;
            else
            {
                var filtered = allLogs.Where(u =>
                    (u.username != null && u.username.ToLower().Contains(searchText)) ||
                    (u.targetType != null && u.targetType.ToLower().Contains(searchText)) ||
                    (u.action != null && u.action.ToLower().Contains(searchText))
                    ).ToList();
                LogsDataGrid.ItemsSource = filtered;
            }
        }
        private async Task LoadUsersAsync() => users = await connection.GetUsers();
        
    }
    
}
