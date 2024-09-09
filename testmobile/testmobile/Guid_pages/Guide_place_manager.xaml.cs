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
    public partial class Guide_place_manager : ContentPage
    {
        int guide_id;
        CatalogViewModel viewModel;
        public Guide_place_manager()
        {
            InitializeComponent();
            Get_guide_id();
            InitializeAsync();
        }

       

        private async void OnRefreshing(object sender, EventArgs e)
        {
           
            await RefreshData();
        }

        private async Task RefreshData()
        {
            // загрузка данных
            await InitializeAsync();

            // Завершаем процесс обновления
            refreshView.IsRefreshing = false;
        }
        private async void Add_place_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Guide_add_place());
        }
        private async void Delete_place_Clicked(object sender, EventArgs e)
        {
            var selected_place = (sender as Button)?.BindingContext as Product;
            if (selected_place != null)
            {
                bool result = await DisplayAlert("Подтвердить действие", "Вы хотите удалить место?", "Да", "Нет");
                if (result == true)
                {
                    using (DB dB = new DB())
                    {
                        dB.openConn();
                        using (MySqlCommand cmd = new MySqlCommand("DELETE FROM Place WHERE id=@id", dB.getConn()))
                        {
                            cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = selected_place.Id;

                            if(await cmd.ExecuteNonQueryAsync() == 1) 
                            {
                                await DisplayAlert("Уведомление", "Место успешно удалено!", "OK");

                            }
                        }
                    }
                }

            }
           
        }
        private void Place_click(object sender, EventArgs e)
        {
            var selected_place = (sender as Button)?.BindingContext as Product;
            if (selected_place != null)
            {

                Navigation.PushAsync(new Edit_remove_place_page(selected_place));
            }
        }
        private async Task InitializeAsync()
        {
            viewModel = new CatalogViewModel();
            await viewModel.LoadDataAsync();

              ObservableCollection <Product> places = viewModel.GetPlacesByGuideId(guide_id);


            this.BindingContext = places;

        }
        private async Task Get_guide_id()
        {
            string login = await SecureStorage.GetAsync("login");
            using (DB dB = new DB())
            {
                int user_id = dB.get_user_id(login); 
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
    }
}