using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using testmobile.Guid_pages;

namespace testmobile
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();

            check_con();
            check_auth();
        }

        async public void check_auth()
        {
            string login = await SecureStorage.GetAsync("login");
            if (string.IsNullOrEmpty(login) == false)
            {
                switch (role_check(login))
                {
                    case "User":
                        await Navigation.PushAsync(new TabbedPage1());
                        break;
                    case "Guide":
                        await Navigation.PushAsync(new Tabbed_guide());
                        break;
                    case "Admin":
                        await DisplayAlert("урааа", "Вы админ!!", "ОК");
                        break;
                }










            }
        }

        private async void login_click(object sender, EventArgs e)
        {
            string login = login_field.Text;
            string password = pass_field.Text;
            if (string.IsNullOrEmpty(login) == true)
            {
                await DisplayAlert("Авторизация", "Поле с логином пустое!", "ОК");
                return;
            }
            if (string.IsNullOrEmpty(password) == true)
            {
                await DisplayAlert("Авторизация", "Поле с паролем пустое!", "ОК");
                return;
            }

            using (DB dB = new DB())
            {
                hashing hash = new hashing();
                var password_hashed = hash.Hash_password(pass_field.Text);

                DataTable dt = new DataTable();
                MySqlDataAdapter adapter = new MySqlDataAdapter();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM `Users` WHERE `login` = @uL AND `password`= @uP", dB.getConn());
                cmd.Parameters.Add("@uL", MySqlDbType.VarChar).Value = login;
                cmd.Parameters.Add("@uP", MySqlDbType.VarChar).Value = password_hashed;

                adapter.SelectCommand = cmd;
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    //await DisplayAlert("Логин", "Пользователь есть", "ОК");
                    await Xamarin.Essentials.SecureStorage.SetAsync("login", login);
                    switch (role_check(login))
                    {
                        case "User":
                            await Navigation.PushAsync(new TabbedPage1());
                            break;
                        case "Guide":
                            await Navigation.PushAsync(new Tabbed_guide());
                            break;
                        case "Admin":
                            await DisplayAlert("урааа", "Вы админ!!", "ОК");
                            break;
                    }
                }
                else
                {
                    await DisplayAlert("Авторизация", "Пользователь не существует!", "ОК");
                }

            }



        }
        private async void regpage(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new regpage());
        }

        async public void check_con()
        {
            try
            {
                var conn = new MySqlConnection("Server = db4free.net; Port = 3306; Database = mobile_test; Uid = mobile; Pwd = mobile12;")
                ;
                conn.Open();
                conn.Close();
            }
            catch
            {

                await DisplayAlert("Ошибка", "Соединения с БД нет", "ОК");

            }

        }

        public string role_check(string login)
        {
            using (DB dB = new DB())
            {
                int id = dB.get_user_id(login);
                dB.openConn();

                using (MySqlCommand cmd = new MySqlCommand("SELECT role_id FROM `Users` WHERE `id` = @uId", dB.getConn()))
                {
                    cmd.Parameters.Add("@uId", MySqlDbType.Int32).Value = id;

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int roleId = reader.GetInt32(reader.GetOrdinal("role_id"));

                            switch (roleId)
                            {
                                case 1:
                                    return "User";
                                case 2:
                                    return "Guide";
                                case 3:
                                    return "Admin";
                            }
                        }
                    }
                    return "Unknown";
                }

            }

        }

    }
}
