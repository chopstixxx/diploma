using MySqlConnector;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using testmobile.Classes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace testmobile.Client_pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class Order_info_page_client : ContentPage
    {
        
        private readonly Order _selectedOrder;
        public Order_info_page_client(Order selectedOrder)
        {
            InitializeComponent();
            _selectedOrder = selectedOrder;
            BindingContext = _selectedOrder;
            Escort_need_lbl_change();
            Guide_info();
            Btn_appearance();
        }

      

        public void Escort_need_lbl_change()
        {
            if (_selectedOrder.Escort_need == true)
            {
                ecort_need_lbl.Text = "Нужда в сопровождающем: Да";
            }
            else
            {
                ecort_need_lbl.Text = "Нужда в сопровождающем: Нет";

            }
        }
        public async Task Guide_info()
        {
            using (DB dB = new DB())
            {
                dB.openConn();
                using (MySqlCommand cmd = new MySqlCommand("SELECT name, phone_number FROM `Guide` WHERE `id` = @guideId", dB.getConn()))
                {
                    cmd.Parameters.Add("@guideId", MySqlDbType.Int32).Value = _selectedOrder.Guide_id ;
                    using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string name = reader.GetString(reader.GetOrdinal("name"));
                            string phone_number = reader.GetString(reader.GetOrdinal("phone_number"));

                          guide_name.Text = string.Format("Имя: {0}", name); ;
                            guide_phone_number.Text = string.Format("Номер телефона: {0}", phone_number);
                        }

                    }
                }
            }
        }
        public void Btn_appearance()
        {
            switch (_selectedOrder.Status)
            {
                case "Создан":
                    cancel_btn.IsVisible = true;
                    break;


            }
        }
        private async void Cancel_clicked(object sender, EventArgs e)
        {
            bool result = await DisplayAlert("Подтвердить действие", "Вы хотите отменить заказ?", "Да", "Нет");

            if (result == true)
            {
                using (DB dB = new DB())
                {
                    dB.openConn();
                    using (MySqlCommand cmd = new MySqlCommand("UPDATE Order_ SET status_id = @status_id WHERE id = @id", dB.getConn()))
                    {
                        cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = _selectedOrder.Id;
                        cmd.Parameters.Add("@status_id", MySqlDbType.Int32).Value = 4;

                        try
                        {
                            cmd.ExecuteNonQuery();
                            await DisplayAlert("Уведомление", "Вы успешно отменили заказ!", "Ок");

                            cancel_btn.IsVisible = false;

                            status_lbl.Text = "Отменён";



                        }
                        catch (Exception ex)
                        {
                           await DisplayAlert("Ошибка", "" + ex, "ОК");
                        }

                    }

                }
            }
           

           
        }
    }
}