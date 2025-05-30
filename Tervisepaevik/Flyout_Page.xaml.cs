using Tervisepaevik.View;

namespace Tervisepaevik;

public partial class Flyout_Page : FlyoutPage
{
    public Flyout_Page()
    {
        InitializeComponent();
    }

    private void btnNewPage_Clicked(object sender, EventArgs e)
    {
        Detail = new NavigationPage(new NewPage1());
        if (!((IFlyoutPageController)this).ShouldShowSplitMode)
           
            IsPresented = false;
    }

    private void btnStartPage_Clicked(object sender, EventArgs e)
    {
        Detail = new NavigationPage(new StartPage());
        if (!((IFlyoutPageController)this).ShouldShowSplitMode)
            IsPresented = false;
    }

    private void btnHommikusookFotoPage_Clicked(object sender, EventArgs e)
    {
        Detail = new NavigationPage(new HommikusookFotoPage());
        if (!((IFlyoutPageController)this).ShouldShowSplitMode)
            IsPresented = false;
    }
    private void btnLounasookFotoPage_Clicked(object sender, EventArgs e)
    {
        Detail = new NavigationPage(new LounasookFotoPage());
        if (!((IFlyoutPageController)this).ShouldShowSplitMode)
            IsPresented = false;
    }
    private void btnOhtusookFotoPage_Clicked(object sender, EventArgs e)
    {
        Detail = new NavigationPage(new OhtusookFotoPage());
        if (!((IFlyoutPageController)this).ShouldShowSplitMode)
            IsPresented = false;
    }
    private void btnVahepalaFotoPage_Clicked(object sender, EventArgs e)
    {
        Detail = new NavigationPage(new VahepalaFotoPage());
        if (!((IFlyoutPageController)this).ShouldShowSplitMode)
            IsPresented = false;
    }
    private void btnTreeningudFotoPage_Clicked(object sender, EventArgs e)
    {
        Detail = new NavigationPage(new TreeningudFotoPage());
        if (!((IFlyoutPageController)this).ShouldShowSplitMode)
            IsPresented = false;
    }

    private void btnVeejalgiminePage_Clicked(object sender, EventArgs e)
    {
        Detail = new NavigationPage(new VeejalgiminePage());
        if (!((IFlyoutPageController)this).ShouldShowSplitMode)
            IsPresented = false;
    }
    private void btnEnesetunnePage_Clicked(object sender, EventArgs e)
    {
        Detail = new NavigationPage(new EnesetunnePage());
        if (!((IFlyoutPageController)this).ShouldShowSplitMode)
            IsPresented = false;
    }
}
