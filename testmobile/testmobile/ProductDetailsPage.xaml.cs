using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace testmobile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProductDetailsPage : ContentPage
	{
        private readonly Product _selectedProduct;

        public ProductDetailsPage (Product selectedProduct)
		{
			InitializeComponent ();

            _selectedProduct = selectedProduct;
            BindingContext = _selectedProduct;

            

        }

     

        private async void Order_page(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new OrderPage(_selectedProduct));
        }
    }
}