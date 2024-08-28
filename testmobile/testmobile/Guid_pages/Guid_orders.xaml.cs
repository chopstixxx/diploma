using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using testmobile.Classes;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace testmobile.Guid_pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Guid_orders : ContentPage
    {
        OrdersViewModel ordersViewModel;
        int guide_id;
        ObservableCollection<Order> filteredOrders;
        ObservableCollection<Order> allOrders;
        public Guid_orders()
        {
            InitializeComponent();
            Get_guide_id();
            InitializeAsync();
        }

       

        private async void OnRefreshing(object sender, EventArgs e)
        {
            // Вызываем метод RefreshData() для обновления данных
            await RefreshData();
        }

        private async Task RefreshData()
        {
            // загрузка данных
            await InitializeAsync();

            // Завершаем процесс обновления
            refreshView.IsRefreshing = false;
        }
        private async Task InitializeAsync()
        {





            ordersViewModel = new OrdersViewModel();
            await ordersViewModel.LoadOrdersData();

            allOrders = ordersViewModel.GetOrdersByGuideId(guide_id);
           
            
                filteredOrders = new ObservableCollection<Order>(allOrders);
                this.BindingContext = allOrders;
                

            






        }
        
       

        private async Task Get_guide_id()
        {
            string login = await SecureStorage.GetAsync("login");
            using (DB dB = new DB())
            {
                int user_id = dB.get_user_id(login); // получаю user_id
                dB.openConn();
                MySqlCommand cmd = new MySqlCommand("SELECT id FROM `Guide` WHERE `user_id` = @userId", dB.getConn());
                cmd.Parameters.Add("@userId", MySqlDbType.Int32).Value = user_id;
                using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                {

                    while (await reader.ReadAsync())
                    {
                        guide_id = reader.GetInt32(reader.GetOrdinal("id"));

                    }
                }
            }
        }

        private void Order_click(object sender, EventArgs e)
        {
            var selected_order = (sender as Button)?.BindingContext as Order;
            if (selected_order != null)
            {
               
                Navigation.PushAsync(new Order_info_page(selected_order));
            }
        }


        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = e.NewTextValue;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                filteredOrders = new ObservableCollection<Order>(allOrders);

            }
            else
            {
                filteredOrders = new ObservableCollection<Order>(
                    allOrders.Where(o => o.Id.ToString().Contains(searchText)));
            }

            this.BindingContext = filteredOrders;
        }
        private void OnSortPickerSelectedIndexChanged(object sender, EventArgs e)
        {


            if (sortPicker.SelectedIndex == 0) // Если выбрана опция "Более новые"
            {
                filteredOrders = new ObservableCollection<Order>(filteredOrders.OrderByDescending(o => o.Order_date));
            }
            else if (sortPicker.SelectedIndex == 1) // Если выбрана опция "Более старые"
            {
                filteredOrders = new ObservableCollection<Order>(filteredOrders.OrderBy(o => o.Order_date));
            }

            this.BindingContext = filteredOrders;
           
        }
        private void FilterSelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedFilter = filterPicker.SelectedIndex;


            switch (selectedFilter)
            {
                case 0:
                    filteredOrders = new ObservableCollection<Order>(allOrders);
                    break;
                case 1:
                    filteredOrders = new ObservableCollection<Order>(allOrders
                                                        .Where(o => o.Status == "Создан"));
                    break;
                case 2:
                    filteredOrders = new ObservableCollection<Order>(allOrders
                                                        .Where(o => o.Status == "В работе"));
                    break;
                case 3:
                    filteredOrders = new ObservableCollection<Order>(allOrders
                                                        .Where(o => o.Status == "Выполнен"));
                    break;
                case 4:
                    filteredOrders = new ObservableCollection<Order>(allOrders
                                                        .Where(o => o.Status == "Отменён"));
                    break;
            }

            this.BindingContext = filteredOrders;
           

        }
    }
}