using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.PlatformConfiguration;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;



namespace testmobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabbedPage1 : TabbedPage
    {
        
        public TabbedPage1()
        {

            InitializeComponent();         
        }
        protected override bool OnBackButtonPressed()
        {
           
            return true; // Запретить свайп назад
        }
        
    }
}