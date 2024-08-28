using Firebase.Storage;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using testmobile.Classes;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace testmobile.Guid_pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Guide_add_place : ContentPage
    {
        public ObservableCollection<ImageSource> Images { get; set; }

       

        string name;
        string location;
        string decription;
        int price;
        int place_id;
        int guide_id;

        public Guide_add_place()
        {
            InitializeComponent();
            Images = new ObservableCollection<ImageSource>();
            BindingContext = this;
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
        private async void AddPhotoButton_Clicked(object sender, EventArgs e)
        {
            var pickResult = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Выберите изображение"
            });

            if (pickResult != null)
            {
                Images.Add(ImageSource.FromStream(() => pickResult.OpenReadAsync().Result));
            }
        }
        private void DeletePhotoButton_Clicked(object sender, EventArgs e)
        {
            if (Images.Count > 0)
            {
                // Получаем текущий индекс изображения в CarouselView
                int currentIndex = carouselView.Position;

                // Удаляем изображение из коллекции по текущему индексу
                Images.RemoveAt(currentIndex);
            }
        }
        private async void PlaceAddBtn_Clicked(object sender, EventArgs e)
        {
            name = Name_entry.Text;
            location = Location_entry.Text;
            decription = Description_entry.Text;
            price = Convert.ToInt32(Price_entry.Text);
            if (Images.Count < 1)
            {
                await DisplayAlert("Ошибка", "Добавьте хотя бы 1 изображение!", "Ок");
                return;
            }
            if (string.IsNullOrEmpty(name))
            {
                await DisplayAlert("Ошибка", "Поле с названием пустое!", "Ок");
                return;
            }
            if (string.IsNullOrEmpty(location))
            {
                await DisplayAlert("Ошибка", "Поле с местоположением пустое!", "Ок");
                return;
            }
            if (string.IsNullOrEmpty(decription))
            {
                await DisplayAlert("Ошибка", "Поле с описанием пустое!", "Ок");
                return;
            }
            if (string.IsNullOrEmpty(Price_entry.Text))
            {
                await DisplayAlert("Ошибка", "Поле с ценой пустое!", "Ок");
                return;
            }
            if (!(Disability_1.IsChecked || Disability_2.IsChecked || Disability_3.IsChecked || Disability_4.IsChecked))
            {
                await DisplayAlert("Ошибка", "Выберите хотя бы 1 вариант адаптации места для людей.", "Ок");
                return;
            }
            await Get_guide_id();

            Place_add();
            Place_pics_add();
            Disabilities_add();

        }
        private async void Place_add()
        {
            using (DB dB = new DB())
            {
                dB.openConn();
                using (MySqlCommand cmd = new MySqlCommand("INSERT INTO `Place` (`guide_id`,`name`,`description`,`location`,`price`) VALUES (@guide_id,@name,@description,@location,@price); SELECT LAST_INSERT_ID();", dB.getConn()))
                {
                    cmd.Parameters.Add("@guide_id", MySqlDbType.Int32).Value = guide_id;
                    cmd.Parameters.Add("@name", MySqlDbType.VarChar).Value = name;
                    cmd.Parameters.Add("@description", MySqlDbType.VarChar).Value = decription;
                    cmd.Parameters.Add("@location", MySqlDbType.VarChar).Value = location;
                    cmd.Parameters.Add("@price", MySqlDbType.Int32).Value = price;

                    place_id = Convert.ToInt32(cmd.ExecuteScalar());



                }
            }
        }
        private async void Place_pics_add()
        {
            int i = 1;
            try
            {
                foreach (var imageSource in Images)
                {
                    // Конвертируем ImageSource в MemoryStream
                    Stream imageStream = await ((StreamImageSource)imageSource).Stream(CancellationToken.None);
                    if (imageStream != null)
                    {
                        var fileName = $"{place_id}_pic_{i}.jpg"; // Генерируем уникальное имя файла
                        var task = new FirebaseStorage("diploma-dd308.appspot.com", new FirebaseStorageOptions
                        {
                            ThrowOnCancel = true,
                        })
                        .Child("Place_pics")
                        .Child(fileName)
                        .PutAsync(imageStream);

                        string url = await task;

                        using (DB dB = new DB())
                        {
                            dB.openConn();
                            using (MySqlCommand cmd = new MySqlCommand("INSERT INTO Place_pics (place_id, url) VALUES (@place_id, @url)", dB.getConn()))
                            {
                                cmd.Parameters.Add("@place_id", MySqlDbType.Int32).Value = place_id;
                                cmd.Parameters.Add("@url", MySqlDbType.VarChar).Value = url;

                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Сообщение об ошибке", ex.Message, "OK");
            }
        }
        private async void Disabilities_add()
        {
            using (DB dB = new DB())
            {
                try
                {
                    dB.openConn(); // Открываем соединение с базой данных

                    // Проверяем каждый чекбокс и устанавливаем значение параметра Accordance соответственно
                    for (int i = 1; i <= 4; i++)
                    {
                        CheckBox checkbox = this.FindByName<CheckBox>($"Disability_{i}");

                        bool accordance = checkbox.IsChecked;
                        int disabilityId = i;

                        // Вставляем данные в базу данных для каждого чекбокса
                        using (MySqlCommand cmd = new MySqlCommand("INSERT INTO Disability_place (place_id, disability_id, Accordance) VALUES (@place_id, @disability_id, @accordance)", dB.getConn()))
                        {
                            cmd.Parameters.AddWithValue("@place_id", place_id);
                            cmd.Parameters.AddWithValue("@disability_id", disabilityId);
                            cmd.Parameters.AddWithValue("@accordance", accordance);

                            cmd.ExecuteNonQuery(); // Выполняем запрос
                        }
                    }

                    await DisplayAlert("Уведомление", "Место успешно добавлено!", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Ошибка", ex.Message, "OK");
                }
            }
        }
    }
}
