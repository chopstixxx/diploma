using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace testmobile.Classes
{
    public class User
    {
        public string Name { get; set; }
        public string Login { get; set; }
        public string Date_of_birth { get; set; }
        public string Phone_number { get; set; }

        

        public async Task LoadUserDataFromDatabase(bool IsClient)
        {
            string cmdtext = "";
            string login = await SecureStorage.GetAsync("login"); // получаем логин из внутреннего файла
            Login = login;
            using (DB dB = new DB())
            {

                int id = dB.get_user_id(login); 
                dB.openConn();
                switch (IsClient)
                {
                    case true:
                        cmdtext = "SELECT name, birth_date, phone_number FROM `Client` WHERE `user_id` = @uID";
                        break;

                    case false:
                        cmdtext = "SELECT name, birth_date, phone_number FROM `Guide` WHERE `user_id` = @uID";
                        break;
                }


                using (MySqlCommand cmd = new MySqlCommand(cmdtext, dB.getConn()))
                {
                    cmd.Parameters.Add("@uID", MySqlDbType.Int32).Value = id;
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Name = reader.GetString(reader.GetOrdinal("name"));
                            DateTime birthDate = reader.GetDateTime(reader.GetOrdinal("birth_date"));
                            Date_of_birth = birthDate.ToString("dd.MM.yyyy"); // Преобразование в строку
                            Phone_number = reader.GetString(reader.GetOrdinal("phone_number"));
                        }

                    }

                }
            }

        }
    }
}

