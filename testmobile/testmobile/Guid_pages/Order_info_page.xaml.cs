using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using testmobile.Classes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace testmobile.Guid_pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Order_info_page : ContentPage
    {
        private readonly Order _selectedOrder;

        public Order_info_page(Order selectedOrder)
        {
            InitializeComponent();
            _selectedOrder = selectedOrder;
            BindingContext = _selectedOrder;
            Escort_need_lbl_change();
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
        public void Btn_appearance()
        {
            switch (_selectedOrder.Status)
            {
                case "Создан":
                    cancel_btn.IsVisible = true;
                    accept_btn.IsVisible = true;
                    break;

                case "Принят":
                    cancel_btn.IsVisible = true;
                    finish_btn.IsVisible = true;
                    break;

                case "Выполнен":

                    break;

                case "Отменён":

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
                            finish_btn.IsVisible = false;
                            accept_btn.IsVisible = false;
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
        private void Accept_clicked(object sender, EventArgs e)
        {
            using (DB dB = new DB())
            {
                dB.openConn();
                using (MySqlCommand cmd = new MySqlCommand("UPDATE Order_ SET status_id = @status_id WHERE id = @id", dB.getConn()))
                {
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = _selectedOrder.Id;
                    cmd.Parameters.Add("@status_id", MySqlDbType.Int32).Value = 2;

                    try
                    {
                        cmd.ExecuteNonQuery();
                        DisplayAlert("Уведомление", "Вы успешно приняли заказ!", "Ок");

                        cancel_btn.IsVisible = true;
                        finish_btn.IsVisible = true;
                        accept_btn.IsVisible = false;
                        status_lbl.Text = "Принят";

                    }
                    catch (Exception ex)
                    {
                        DisplayAlert("Ошибка", "" + ex, "ОК");
                    }

                }

            }
        }
        private void Finish_clicked(object sender, EventArgs e)
        {
            using (DB dB = new DB())
            {
                dB.openConn();
                using (MySqlCommand cmd = new MySqlCommand("UPDATE Order_ SET status_id = @status_id WHERE id = @id", dB.getConn()))
                {
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = _selectedOrder.Id;
                    cmd.Parameters.Add("@status_id", MySqlDbType.Int32).Value = 3;

                    try
                    {
                        cmd.ExecuteNonQuery();
                        DisplayAlert("Уведомление", "Вы успешно выполнили заказ!", "Ок");

                        cancel_btn.IsVisible = false;
                        finish_btn.IsVisible = false;
                        accept_btn.IsVisible = false;
                        status_lbl.Text = "Выполнен";
                    }
                    catch (Exception ex)
                    {
                        DisplayAlert("Ошибка", "" + ex, "ОК");
                    }

                }

            }
        }
       
    }
}