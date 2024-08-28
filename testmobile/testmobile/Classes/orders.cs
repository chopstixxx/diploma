using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace testmobile.Classes
{
    public class Order
    {
        public int Id { get; set; }

        public int Client_id { get; set; }
        public int Guide_id { get; set; }

        public string Client_name { get; set; }

        public string Client_phone_num { get; set; }
        public string Place_name { get; set; }
        public bool Escort_need { get; set; }
        public DateTime Order_date { get; set; }
        public int Number_of_people { get; set; }
        public int Final_price { get; set; }
        public DateTime Order_start { get; set; }

        public string Status { get; set; }


    }
    public class OrdersViewModel 
    {
        public ObservableCollection<Order> Orders { get; set; }

       

        public ObservableCollection<Order> GetOrdersByClientId(int clientId)
        {
            ObservableCollection<Order> clientOrders = new ObservableCollection<Order>();

            foreach (var order in Orders)
            {
                if (order.Client_id == clientId) // Замените Client_id на фактическое свойство, которое хранит идентификатор клиента в вашем классе Order
                {
                    clientOrders.Add(order);
                }
            }

            return clientOrders;
        }

        public ObservableCollection<Order> GetOrdersByGuideId(int guide_id)
        {
            ObservableCollection<Order> guide_orders = new ObservableCollection<Order>();

            foreach (var order in Orders)
            {
                if (order.Guide_id == guide_id) // Замените Client_id на фактическое свойство, которое хранит идентификатор клиента в вашем классе Order
                {
                    guide_orders.Add(order);
                }
            }

            return guide_orders;
        }


        public async Task LoadOrdersData()
        {
            try
            {
                Orders = new ObservableCollection<Order>();

                using (DB dB = new DB())
                {
                    dB.openConn();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM `Order_`", dB.getConn()))
                    {
                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {

                            while (await reader.ReadAsync())
                            {
                                int status_id = reader.GetInt32(reader.GetOrdinal("status_id"));
                                int client_id = reader.GetInt32(reader.GetOrdinal("client_id"));
                                int place_id = reader.GetInt32(reader.GetOrdinal("place_id"));
                                

                                Order order = new Order
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                                    Client_id = client_id,
                                    Escort_need = reader.GetBoolean(reader.GetOrdinal("escort_need")),
                                    Order_date = reader.GetDateTime(reader.GetOrdinal("order_date")),
                                    Number_of_people = reader.GetInt32(reader.GetOrdinal("number_of_people")),
                                    Final_price = reader.GetInt32(reader.GetOrdinal("final_price")),
                                    Order_start = reader.GetDateTime(reader.GetOrdinal("order_start")),



                                };
                                GetStatusInfo(order, status_id);
                                await GetPlaceInfo(order, place_id);
                                await GetClientInfo(order, client_id);
                                Orders.Add(order);
                            }
                        }
                    }

                }



            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await App.Current.MainPage.DisplayAlert("Error", $"Exception in LoadOrdersData: {ex.Message}", "OK");
                });
            }




            async Task GetClientInfo(Order order, int client_id)
            {

                try
                {
                    using (DB dB = new DB())
                    {
                        dB.openConn();
                        using (MySqlCommand cmd = new MySqlCommand("SELECT name, phone_number FROM `Client` WHERE `id` = @clientId", dB.getConn()))
                        {
                            cmd.Parameters.Add("@clientId", MySqlDbType.Int32).Value = client_id;
                            using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    string name = reader.GetString(reader.GetOrdinal("name"));
                                    string phone_number = reader.GetString(reader.GetOrdinal("phone_number"));

                                    order.Client_name = name;
                                    order.Client_phone_num = phone_number;

                                }

                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await App.Current.MainPage.DisplayAlert("Error", $"Exception in GetClientInfo: {ex.Message}", "OK");
                    });
                }









            }

            async Task GetPlaceInfo(Order order, int place_id)
            {
                try
                {
                    using (DB dB = new DB())
                    {
                        dB.openConn();
                        using (MySqlCommand cmd = new MySqlCommand("SELECT name, guide_id FROM `Place` WHERE `id` = @placeId", dB.getConn()))
                        {
                            cmd.Parameters.Add("@placeId", MySqlDbType.Int32).Value = place_id;
                            using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    string name = reader.GetString(reader.GetOrdinal("name"));
                                    int guide_id = reader.GetInt32(reader.GetOrdinal("guide_id"));

                                    order.Guide_id = guide_id;
                                    order.Place_name = name;

                                }

                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await App.Current.MainPage.DisplayAlert("Error", $"Exception in GetClientInfo: {ex.Message}", "OK");
                    });

                }
            }
            void GetStatusInfo(Order order, int status_id)
            {
                string status_name = "";

                switch(status_id)
                {
                    case 1:
                        status_name = "Создан";
                        break;
                        case 2:
                        status_name = "Принят";
                        break;
                        case 3:
                        status_name = "Выполнен";
                        break;
                        case 4:
                        status_name = "Отменён";
                        break;

                }
                order.Status = status_name;




            }
        }
    }
}
