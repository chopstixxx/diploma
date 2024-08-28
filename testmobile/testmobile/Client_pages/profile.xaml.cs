using Firebase.Storage;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using testmobile.Classes;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace testmobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class profile : ContentPage
    {
        User user;
        public profile()
        {
            
            InitializeComponent();
            profile_method();

        }

       

        private async void profile_method()
        {

             user = new User();
            await user.LoadUserDataFromDatabase(true);
            this.BindingContext = user;

            try
            {
                var storage = new FirebaseStorage("diploma-dd308.appspot.com");
                var storageRef = storage.Child("Profile_pics/" + user.Login + "_profile_photo.jpg");

                // Получаем метаданные файла

                // Если файл существует, получаем его URL и устанавливаем его в качестве источника изображения
                string imageUrl = await storageRef.GetDownloadUrlAsync();
                profile_pic.Source = imageUrl;
            }
            catch
            {

            }

        }
        private async void get_photo_click(object sender, EventArgs e)
        {

            try
            {
                var photo = await MediaPicker.PickPhotoAsync();
                if (photo != null)
                {
                    var task = new FirebaseStorage("diploma-dd308.appspot.com", new FirebaseStorageOptions
                    {
                        ThrowOnCancel = true,

                    })
                         .Child("Profile_pics")
                         .Child(user.Login + "_profile_photo.jpg")
                         .PutAsync(await photo.OpenReadAsync());

                  
                    string url = await task;
                    profile_pic.Source = url;


                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Сообщение об ошибке", ex.Message, "OK");
            }

           
           

        }
        private async void exit_click(object sender, EventArgs e)
        {
            SecureStorage.Remove("login");
          
            await Navigation.PopAsync();
        }
       
        
    }
}