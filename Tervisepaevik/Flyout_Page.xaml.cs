using Tervisepaevik.Resources.Localization;
using Tervisepaevik.View;

namespace Tervisepaevik;

public partial class Flyout_Page : FlyoutPage
{
    public Flyout_Page()
    {
        InitializeComponent();
        UpdateTexts();
    }

    // ОБНОВЛЕНИЕ ТЕКСТОВ
    public void UpdateTexts()
    {
        btnStartPage.Text = AppResources.StartPage;
        btnNewPage.Text = AppResources.NewPage;

        btnHommikusookFotoPage.Text = AppResources.Breakfast;
        btnLounasookFotoPage.Text = AppResources.Lunch;
        btnOhtusookFotoPage.Text = AppResources.Dinner;
        btnVahepalaFotoPage.Text = AppResources.Snack;

        btnTreeningudFotoPage.Text = AppResources.Training;
        btnVeejalgiminePage.Text = AppResources.Water;
        btnEnesetunnePage.Text = AppResources.Feeling;
        btnInfoPage.Text = AppResources.Info;
    }

    // НАВИГАЦИЯ
    void Navigate(Page page)
    {
        Detail = new NavigationPage(page);

        if (!((IFlyoutPageController)this).ShouldShowSplitMode)
            IsPresented = false;
    }

    private void btnStartPage_Clicked(object sender, EventArgs e)
    {
        Navigate(new StartPage());
    }

    private void btnNewPage_Clicked(object sender, EventArgs e)
    {
        Navigate(new NewPage1());
    }

    private void btnHommikusookFotoPage_Clicked(object sender, EventArgs e)
    {
        Navigate(new HommikusookFotoPage());
    }

    private void btnLounasookFotoPage_Clicked(object sender, EventArgs e)
    {
        Navigate(new LounasookFotoPage());
    }

    private void btnOhtusookFotoPage_Clicked(object sender, EventArgs e)
    {
        Navigate(new OhtusookFotoPage());
    }

    private void btnVahepalaFotoPage_Clicked(object sender, EventArgs e)
    {
        Navigate(new VahepalaFotoPage());
    }

    private void btnTreeningudFotoPage_Clicked(object sender, EventArgs e)
    {
        Navigate(new TreeningudFotoPage());
    }

    private void btnVeejalgiminePage_Clicked(object sender, EventArgs e)
    {
        Navigate(new VeejalgiminePage());
    }

    private void btnEnesetunnePage_Clicked(object sender, EventArgs e)
    {
        Navigate(new EnesetunnePage());
    }

    private void btnInfoPage_Clicked(object sender, EventArgs e)
    {
        Navigate(new InfoPage());
    }
}