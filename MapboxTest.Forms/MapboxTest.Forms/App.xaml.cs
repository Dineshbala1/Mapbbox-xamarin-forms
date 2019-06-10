using MapboxTest.Forms.Pages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MapboxTest.Forms
{
    public partial class App : Application
    {
        public const string MessageAlertSender = "PopupMessageSender";
        public const string MessageAlertMessageName = "PopupMessage";

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
