using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace testmobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class regpage : ContentPage
    {
        MySqlCommand clientInsertCmd;
        MySqlCommand userInsertCmd;
        public regpage()
        {
            InitializeComponent();
        }

      

        private async void reg_click(object sender, EventArgs e)
        {

            if (is_user_exsit() == true)
            {
                return;
            }
            if (pass_validation(pass_reg.Text) == false)
            {
                return;
            }

            DB db = new DB();


            hashing hash = new hashing();
            var password = hash.Hash_password(pass_reg.Text);



            if (is_guide.IsChecked)
            {

                userInsertCmd = new MySqlCommand("INSERT INTO `Users` (`login`, `password`, `role_id`) VALUES (@login, @password, @role_id); SELECT LAST_INSERT_ID();", db.getConn());
                userInsertCmd.Parameters.Add("@login", MySqlDbType.VarChar).Value = login_reg.Text;
                userInsertCmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = password;
                userInsertCmd.Parameters.Add("@role_id", MySqlDbType.Int32).Value = 2;


            }
            else
            {
                userInsertCmd = new MySqlCommand("INSERT INTO `Users` (`login`, `password`, `role_id`) VALUES (@login, @password, @role_id); SELECT LAST_INSERT_ID();", db.getConn());
                userInsertCmd.Parameters.Add("@login", MySqlDbType.VarChar).Value = login_reg.Text;
                userInsertCmd.Parameters.Add("@password", MySqlDbType.VarChar).Value = password;
                userInsertCmd.Parameters.Add("@role_id", MySqlDbType.Int32).Value = 1;
            }



            
            db.openConn();
            int userId = Convert.ToInt32(userInsertCmd.ExecuteScalar()); 

            

            if (is_guide.IsChecked)
            {
                clientInsertCmd = new MySqlCommand("INSERT INTO `Guide` (`user_id`, `name`, `birth_date`, `phone_number`) VALUES (@user_id, @name, @birth_date, @phone_number);", db.getConn());
                clientInsertCmd.Parameters.Add("@user_id", MySqlDbType.Int32).Value = userId;
                clientInsertCmd.Parameters.Add("@name", MySqlDbType.VarChar).Value = name_reg.Text;
                clientInsertCmd.Parameters.Add("@birth_date", MySqlDbType.Date).Value = birth_date.Date;
                clientInsertCmd.Parameters.Add("@phone_number", MySqlDbType.VarChar).Value = phone_reg.Text;
            }
            else
            {
                clientInsertCmd = new MySqlCommand("INSERT INTO `Client` (`user_id`, `name`, `birth_date`, `phone_number`) VALUES (@user_id, @name, @birth_date, @phone_number);", db.getConn());
                clientInsertCmd.Parameters.Add("@user_id", MySqlDbType.Int32).Value = userId;
                clientInsertCmd.Parameters.Add("@name", MySqlDbType.VarChar).Value = name_reg.Text;
                clientInsertCmd.Parameters.Add("@birth_date", MySqlDbType.Date).Value = birth_date.Date;
                clientInsertCmd.Parameters.Add("@phone_number", MySqlDbType.VarChar).Value = phone_reg.Text;
            }



            if (clientInsertCmd.ExecuteNonQuery() == 1)
            {
                await DisplayAlert("Регистрация", "Вы успешно зарегистрировались!", "ОК");
            }
            else
            {
                await DisplayAlert("Регистрация", "Пользователь не создан", "ОК");
            }

            db.closeConn();

        }
        public Boolean is_user_exsit()
        {

            DB dB = new DB();
            DataTable dt = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();

            MySqlCommand cmd = new MySqlCommand("SELECT id FROM `Users` WHERE `login` = @uL", dB.getConn());
            cmd.Parameters.Add("@uL", MySqlDbType.VarChar).Value = login_reg.Text;


            adapter.SelectCommand = cmd;
            adapter.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                DisplayAlert("Ошибка!", "Данный логин занят", "ОК");
                return true;

            }
            else
            {

                return false;
            }

        }
        public Boolean pass_validation(string password)
        {

            var input = password;

            if (string.IsNullOrEmpty(input))
            {
                DisplayAlert("Ошибка!", "Поле с паролем пустое", "ОК");

                return false;
            }

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMiniMaxChars = new Regex(@".{6}");



            if (!hasUpperChar.IsMatch(input))
            {
                DisplayAlert("Ошибка!", "Пароль должен содержать хотя бы одну заглавную букву", "ОК");
                return false;
            }
            else if (!hasMiniMaxChars.IsMatch(input))
            {
                DisplayAlert("Ошибка!", "Пароль должен быть длиннее 6 символов", "ОК");

                return false;
            }
            else if (!hasNumber.IsMatch(input))
            {
                DisplayAlert("Ошибка!", "Пароль должен содержать хотя бы одно числовое значение", "ОК");

                return false;
            }
            else
            {
                return true;
            }
        }

    }
}