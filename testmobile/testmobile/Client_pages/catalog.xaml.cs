using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace testmobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class catalog : ContentPage
    {
        CatalogViewModel viewModel;
        ObservableCollection<Product> AllProducts;
        ObservableCollection<Product> FiltredProducts;
        public catalog()
        {

            InitializeComponent();
            viewModel = new CatalogViewModel();
            // Создаем экземпляр ViewModel
           

            LoadDataAsync(viewModel);



        }

       

        private async void LoadDataAsync(CatalogViewModel viewModel)
        {
            // Асинхронно загружаем данные
            await viewModel.LoadDataAsync();

            // Устанавливаем ViewModel в качестве контекста привязки
            this.BindingContext = viewModel.Products;
            AllProducts = viewModel.Products;
        }

        private void OnDetailsClicked(object sender, EventArgs e)
        {
            var selectedProduct = (sender as Button)?.BindingContext as Product;
            if (selectedProduct != null)
            {
                // Используйте selectedProduct для отображения подробной информации о товаре
                // Например, установите тексты Label и изображение Image на основе свойств товара.

                // Навигация на новую страницу
                Navigation.PushAsync(new ProductDetailsPage(selectedProduct));
            }
        }
        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = e.NewTextValue;

            if (string.IsNullOrWhiteSpace(searchText))
            {
                FiltredProducts = new ObservableCollection<Product>(AllProducts);

            }
            else
            {
                FiltredProducts = new ObservableCollection<Product>(
                    AllProducts.Where(o => o.Name.Contains(searchText)));
            }

            this.BindingContext = FiltredProducts;
            
        }
        private void OnSortPickerSelectedIndexChanged(object sender, EventArgs e)
        {


            if (sortPicker.SelectedIndex == 0) 
            {
                FiltredProducts = new ObservableCollection<Product>(FiltredProducts.OrderByDescending(o => o.Price));
            }
            else if (sortPicker.SelectedIndex == 1) 
            {
                FiltredProducts = new ObservableCollection<Product>(FiltredProducts.OrderBy(o => o.Price));
            }

            this.BindingContext = FiltredProducts;

        }
        private void FilterSelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedFilter = filterPicker.SelectedIndex;


            switch (selectedFilter)
            {
                case 0:
                    FiltredProducts = new ObservableCollection<Product>(AllProducts);
                    break;
                case 1:
                    FiltredProducts = new ObservableCollection<Product>(AllProducts
                                                        .Where(o => o.IsWheelChair == true));
                    break;
                case 2:
                    FiltredProducts = new ObservableCollection<Product>(AllProducts
                                                         .Where(o => o.IsWithCane == true));
                    break;
                case 3:
                    FiltredProducts = new ObservableCollection<Product>(AllProducts
                                                          .Where(o => o.IsBlind == true));
                    break;
                case 4:
                    FiltredProducts = new ObservableCollection<Product>(AllProducts
                                                          .Where(o => o.IsDeaf == true));
                    break;
            }

            this.BindingContext = FiltredProducts;
            

        }
    }
}

