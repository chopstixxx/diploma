using MySqlConnector;
using Org.BouncyCastle.Asn1.Mozilla;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using testmobile.Classes;
using Xamarin.Essentials;
using ZstdSharp.Unsafe;

namespace testmobile
{

    public class Product
    {
        public int Id { get; set; }
        public int Guide_Id { get; set; }

        public string Name { get; set; }
        public List<string> ImageUrls { get; set; }

        public string ShortDescription { get; set; }
        public int Price { get; set; }
        public string Location { get; set; }

        public bool IsWheelChair { get; set; }
        public bool IsWithCane { get; set; }
        public bool IsBlind { get; set; }
        public bool IsDeaf {  get; set; }

    }



    public class CatalogViewModel
    {
        public ObservableCollection<Product> Products { get; set; }

       

        public ObservableCollection<Product> GetPlacesByGuideId(int guideId)
        {
            ObservableCollection<Product> guidePlaces = new ObservableCollection<Product>();

            foreach (var product in Products)
            {
                if (product.Guide_Id == guideId) // Замените Client_id на фактическое свойство, которое хранит идентификатор клиента в вашем классе Order
                {
                    guidePlaces.Add(product);
                }
            }

            return guidePlaces;
        }
        public async Task LoadDataAsync()
        {
            try
            {
                Products = new ObservableCollection<Product>();


                using (DB dB = new DB())
                {
                    dB.openConn();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM `Place`", dB.getConn()))
                    {
                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int placeId = reader.GetInt32(reader.GetOrdinal("id"));

                                Product product = new Product
                                {
                                    Id = placeId,
                                    Guide_Id = reader.GetInt32(reader.GetOrdinal("guide_id")),
                                Name = reader.GetString(reader.GetOrdinal("name")),
                                    ShortDescription = reader.GetString(reader.GetOrdinal("description")),
                                    Location = reader.GetString(reader.GetOrdinal("location")),
                                    Price = reader.GetInt32(reader.GetOrdinal("price")),
                                    ImageUrls = await GetImageUrlsForProductAsync(placeId),


                                };
                                await GetDisabilityInfo(product, placeId);
                                Products.Add(product);
                            }
                        }
                    }

                   
                } 
                
                
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await App.Current.MainPage.DisplayAlert("Error", $"Exception in LoadDataAsync: {ex.Message}", "OK");
                });
            }
        }

        public async Task<List<string>> GetImageUrlsForProductAsync(int placeId)
        {
            List<string> imageUrls = new List<string>();

            try
            {
                using (DB dB = new DB())
                {
                    dB.openConn();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM `Place_pics` WHERE `place_id` = @placeId", dB.getConn()))
                    {
                        cmd.Parameters.Add("@placeId", MySqlDbType.Int32).Value = placeId;

                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                imageUrls.Add(reader.GetString(reader.GetOrdinal("url")));
                            }
                        }
                    }
                }
                   
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await App.Current.MainPage.DisplayAlert("Error", $"Exception in GetImageUrlsForProductAsync: {ex.Message}", "OK");
                });
            }

            return imageUrls;
        }
        public async Task GetDisabilityInfo(Product product, int placeId)
        {
            try
            {
                using (DB dB = new DB())
                {
                    dB.openConn();
                    using (MySqlCommand cmd = new MySqlCommand("SELECT disability_id, Accordance FROM `Disability_place` WHERE `place_id` = @placeId", dB.getConn()))
                    {
                        cmd.Parameters.Add("@placeId", MySqlDbType.Int32).Value = placeId;

                        using (MySqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            // Инициализируем переменные
                            bool isWheelChair = false;
                            bool isWithCane = false;
                            bool isBlind = false;
                            bool isDeaf = false;

                            while (await reader.ReadAsync())
                            {
                                int disabilityId = reader.GetInt32(reader.GetOrdinal("disability_id"));
                                bool accordance = reader.GetBoolean(reader.GetOrdinal("Accordance"));

                                // В зависимости от значения disabilityId и Accordance устанавливаем соответствующую переменную true
                                switch (disabilityId)
                                {
                                    case 1:
                                        isWheelChair = accordance;
                                        break;
                                    case 2:
                                        isWithCane = accordance;
                                        break;
                                    case 3:
                                        isBlind = accordance;
                                        break;
                                    case 4:
                                        isDeaf = accordance;
                                        break;
                                        // Добавьте другие случаи, если необходимо
                                }
                            }

                            // После завершения цикла присваиваем значения переменным в объекте Product
                            product.IsWheelChair = isWheelChair;
                            product.IsWithCane = isWithCane;
                            product.IsBlind = isBlind;
                            product.IsDeaf = isDeaf;
                        }
                    }
                }
               
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await App.Current.MainPage.DisplayAlert("Error", $"Exception in GetDisabilityInfo: {ex.Message}", "OK");
                });
            }
        }

    }







}

