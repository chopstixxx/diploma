using MySqlConnector;
using MySqlConnector.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace testmobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrderPage : ContentPage
    {
        public Product _selectedProduct;
        int price;
        int people_count;
        int final_price;
       
       
        public OrderPage(Product selectedProduct)
        {
            InitializeComponent();
            _selectedProduct = selectedProduct;
            BindingContext = _selectedProduct;
            Price_upd();
            people_count = 1;
        }

        

        public void Price_upd()
        {
            final_price = _selectedProduct.Price;
            price = _selectedProduct.Price;
            Final_price.Text = string.Format("{0}₽", price);

        }
        private void OnStepperValueChanged(object sender, ValueChangedEventArgs e)
        {
            
            if (header != null)
            {               
                header.Text = String.Format("Количество человек: " + e.NewValue);
                people_count = (int)e.NewValue;
                final_price = (int)e.NewValue * price;
                Final_price.Text = string.Format("{0}₽", final_price);
            }
               
        }

        private async void Order_Clicked(object sender, EventArgs e)
        {
           

            string login = await SecureStorage.GetAsync("login");
            int client_id = GetClientId(login);
            string current_dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            using (DB dB = new DB())
            {
                dB.openConn();
                using (MySqlCommand cmd = new MySqlCommand("INSERT INTO `Order_` (`status_id`, `client_id`, `place_id`, `escort_need`, `order_date`, `number_of_people`, `final_price`, `order_start`) VALUES (@status_id, @cl_id, @pl_id, @esc_need, @ordr_date, @num_of_people, @final_price, @ordr_strt);", dB.getConn()))
                {
                    cmd.Parameters.Add("@status_id", MySqlDbType.Int32).Value = 1;
                    cmd.Parameters.Add("@cl_id", MySqlDbType.Int32).Value = client_id;
                    cmd.Parameters.Add("@pl_id", MySqlDbType.Int32).Value = _selectedProduct.Id;
                    cmd.Parameters.Add("@esc_need", MySqlDbType.Bool).Value = escort_need.IsChecked;
                    cmd.Parameters.Add("@ordr_date", MySqlDbType.DateTime).Value = current_dt;
                    cmd.Parameters.Add("@num_of_people", MySqlDbType.Int32).Value = people_count;
                    cmd.Parameters.Add("@final_price", MySqlDbType.Int32).Value = final_price;
                    cmd.Parameters.Add("@ordr_strt", MySqlDbType.Date).Value = order_date.Date;

                    if (cmd.ExecuteNonQuery() == 1)
                    {
                        await DisplayAlert("Оформление заказа", "Заказ успешно создан!", "ОК");

                    }
                    //else
                    //{
                    //    await DisplayAlert("Оформление заказа", "что-то пошло не так :(", "ОК");

                    //}
                }
            }
            
           

            

        }
        public int GetClientId(string login)
        {
            int client_id;          
            DB dB = new DB();
            int id = dB.get_user_id(login);
            dB.openConn();

            using (MySqlCommand cmd = new MySqlCommand("SELECT id FROM `Client` WHERE `user_id` = @uId", dB.getConn()))
            {
                cmd.Parameters.Add("@uId", MySqlDbType.Int32).Value = id;

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        client_id = reader.GetInt32(reader.GetOrdinal("id"));
                        return client_id;
                    }
                    return -1;
                }

            }
        }
    }
}