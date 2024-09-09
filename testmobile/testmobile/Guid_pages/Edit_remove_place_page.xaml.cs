using Firebase.Storage;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace testmobile.Guid_pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Edit_remove_place_page : ContentPage
	{
        private readonly Product _selectedPlace;
        public ObservableCollection<ImageSource> New_ImageUrls { get; set; }

        string name;
        string location;
        string description;
        int price;
        
        public Edit_remove_place_page (Product selectedPlace)
		{
			InitializeComponent ();
			_selectedPlace = selectedPlace;
			BindingContext = _selectedPlace;
            New_ImageUrls = new ObservableCollection<ImageSource>();


            Convert_to_observable();
            carouselView.ItemsSource = New_ImageUrls;
            Disbilitity_chebox();
		}
       
        private void Convert_to_observable()
        {
           
            foreach (string imageUrl in _selectedPlace.ImageUrls)
            {
                
                New_ImageUrls.Add(imageUrl);
            }
        }
      
        private void Disbilitity_chebox()
		{
            Disability_1.IsChecked = _selectedPlace.IsWheelChair;
            Disability_2.IsChecked = _selectedPlace.IsWithCane;
            Disability_3.IsChecked = _selectedPlace.IsBlind;
            Disability_4.IsChecked = _selectedPlace.IsDeaf;

        }
        private void DeletePhotoButton_Clicked(object sender, EventArgs e)
        {
            if (New_ImageUrls.Count > 0)
            {
                int currentIndex = carouselView.Position;

                New_ImageUrls.RemoveAt(currentIndex);
                carouselView.ItemsSource = null;
                carouselView.ItemsSource = New_ImageUrls;
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
                New_ImageUrls.Add(ImageSource.FromStream(() => pickResult.OpenReadAsync().Result));
            }
        }
        private async void EditPlace_Clicked(object sender, EventArgs e)
        {
            name = Name_entry.Text;
            location = Location_entry.Text;
            description = Description_entry.Text;
            price = Convert.ToInt32(Price_entry.Text);
            if (New_ImageUrls.Count < 1)
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
            if (string.IsNullOrEmpty(description))
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

            
           await Place_upd();
           await Place_pics_add();
           await Disabilities_update();
           await DisplayAlert("Уведомление", "Место успешно обновлено!", "OK");

        }
        private async Task Place_upd()
        {
            using (DB dB = new DB())
            {
                dB.openConn();
                using (MySqlCommand cmd = new MySqlCommand("UPDATE Place SET name = @name, description = @description, location = @location, price = @price WHERE id = @id ", dB.getConn()))
                {
                    cmd.Parameters.Add("@id", MySqlDbType.Int32).Value = _selectedPlace.Id;
                    cmd.Parameters.Add("@name", MySqlDbType.VarChar).Value = name;
                    cmd.Parameters.Add("@description", MySqlDbType.VarChar).Value = description;
                    cmd.Parameters.Add("@location", MySqlDbType.VarChar).Value = location;
                    cmd.Parameters.Add("@price", MySqlDbType.Int32).Value = price;

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Ошибка", ""+ex, "Ок");
                    }


                }
            }

        }
        private async Task Place_pics_add()
        {

            try
            {
                // Удаление всех существующих изображений
                await RemoveAllImages();

                // Добавление новых изображений
                await AddNewImages();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Сообщение об ошибке", ex.Message, "OK");
            }
        }

        private async Task RemoveAllImages()
        {
            try
            {
                // Удаляем все изображения из базы данных
                using (DB dB = new DB())
                {
                    dB.openConn();
                    using (MySqlCommand cmd = new MySqlCommand("DELETE FROM Place_pics WHERE place_id = @place_id", dB.getConn()))
                    {
                        cmd.Parameters.AddWithValue("@place_id", _selectedPlace.Id);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Сообщение об ошибке в удалении", ex.Message, "OK");
            }
        }

        private async Task AddNewImages()
        {
            try
            {
                int i = 1;

                foreach (var imageSource in New_ImageUrls)
                {
                    Stream imageStream = null;

                    // Проверяем тип ImageSource
                    if (imageSource is StreamImageSource streamImageSource)
                    {
                        // Если это StreamImageSource, получаем поток изображения
                        imageStream = await streamImageSource.Stream(CancellationToken.None);
                    }
                    else if (imageSource is UriImageSource uriImageSource)
                    {
                        // Если это UriImageSource, загружаем изображение по URL и получаем поток
                        using (var webClient = new WebClient())
                        {
                            var uri = uriImageSource.Uri;
                            if (uri != null)
                            {
                                var imageBytes = await webClient.DownloadDataTaskAsync(uri);
                                imageStream = new MemoryStream(imageBytes);
                            }
                        }
                    }
                    else if (imageSource is FileImageSource fileImageSource)
                    {
                        // Если это FileImageSource, открываем файл и получаем поток
                        var filePath = fileImageSource.File;
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            imageStream = File.OpenRead(filePath);
                        }
                    }

                    // Если удалось получить поток изображения
                    if (imageStream != null)
                    {
                        var fileName = $"{_selectedPlace.Id}_pic_{i}.jpg"; // Генерируем уникальное имя файла

                        // Загружаем изображение в Firebase Storage
                        var task = new FirebaseStorage("diploma-dd308.appspot.com", new FirebaseStorageOptions
                        {
                            ThrowOnCancel = true,
                        })
                        .Child("Place_pics")
                        .Child(fileName)
                        .PutAsync(imageStream);

                        // Получаем URL загруженного изображения
                        string url = await task;

                        // Добавляем новую фотографию в базу данных
                        using (DB dB = new DB())
                        {
                            dB.openConn();
                            using (MySqlCommand cmd = new MySqlCommand("INSERT INTO Place_pics (place_id, url) VALUES (@place_id, @url)", dB.getConn()))
                            {
                                cmd.Parameters.Add("@place_id", MySqlDbType.Int32).Value = _selectedPlace.Id;
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
                await DisplayAlert("Сообщение об ошибке в добавлении", ex.Message, "OK");
            }
        }
        private async Task Disabilities_update()
        {
            using (DB dB = new DB())
            {
                try
                {
                    dB.openConn(); 

                    
                    for (int i = 1; i <= 4; i++)
                    {
                        CheckBox checkbox = this.FindByName<CheckBox>($"Disability_{i}");

                        bool accordance = checkbox.IsChecked;
                        int disabilityId = i;

                       
                        using (MySqlCommand cmd = new MySqlCommand("UPDATE Disability_place SET Accordance = @accordance WHERE place_id = @place_id AND disability_id = @disability_id", dB.getConn()))
                        {
                            cmd.Parameters.AddWithValue("@place_id", _selectedPlace.Id);
                            cmd.Parameters.AddWithValue("@disability_id", disabilityId);
                            cmd.Parameters.AddWithValue("@accordance", accordance);

                            cmd.ExecuteNonQuery(); 
                        }
                    }

                }
                catch (Exception ex)
                {
                    await DisplayAlert("Ошибка", ex.Message, "OK");
                }
            }
        }

       
    }
}