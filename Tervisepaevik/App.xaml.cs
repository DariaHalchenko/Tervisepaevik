namespace Tervisepaevik
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new Flyout_Page();
        }
    }
}
