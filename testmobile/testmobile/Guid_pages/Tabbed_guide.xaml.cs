using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace testmobile.Guid_pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Tabbed_guide : TabbedPage
    {
        
        public Tabbed_guide()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {

            return true; // Запретить свайп назад
        }
    }
}