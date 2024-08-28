using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace testmobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            //check_auth();
            MainPage = new NavigationPage(new MainPage());


        }

        protected override void OnStart()
        {
          
            
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
        async public void check_auth()
        {
            string check_login = await SecureStorage.GetAsync("login");
            if (string.IsNullOrEmpty(check_login) == false)
            {
                MainPage = new NavigationPage(new TabbedPage1());
            }
            else
            {
                MainPage = new NavigationPage(new MainPage());
            }
        }
    }
}
