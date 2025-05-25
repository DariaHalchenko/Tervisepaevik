using Syncfusion.Maui.Charts;
using Tervisepaevik.Database;
using Tervisepaevik.Models;

namespace Tervisepaevik.View;

public partial class VeejalgimineGrafikPage : ContentPage
{
    public VeejalgimineGrafikPage(List<Models.VeejalgimineClass> andmed)
    {
        Title = "Vee tarbimise graafik";

        var groupedData = andmed
            .GroupBy(v => v.Kuupaev.ToString("MMMM yyyy"))
            .OrderBy(g => g.First().Kuupaev)
            .Select(g => new ChartGroup
            {
                MonthYear = g.Key,
                Data = g.OrderBy(v => v.Kuupaev)
                        .Select(v => new ChartPoint
                        {
                            Kuupaev = v.Kuupaev.ToString("dd.MM"),
                            Kogus = v.Kogus
                        }).ToList()
            }).ToList();

        var carousel = new CarouselView
        {
            ItemsSource = groupedData,
            ItemTemplate = new DataTemplate(() =>
            {
                var label = new Label
                {
                    FontSize = 20,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center
                };
                label.SetBinding(Label.TextProperty, nameof(ChartGroup.MonthYear));

                var chart = new SfCartesianChart();
                chart.XAxes.Add(new CategoryAxis
                {
                    Title = new ChartAxisTitle { Text = "Kuupäev" }
                });
                chart.YAxes.Add(new NumericalAxis
                {
                    Title = new ChartAxisTitle { Text = "Kogus (ml)" }
                });

                var columnSeries = new ColumnSeries
                {
                    XBindingPath = nameof(ChartPoint.Kuupaev),
                    YBindingPath = nameof(ChartPoint.Kogus),
                    EnableTooltip = true
                };
                columnSeries.SetBinding(ColumnSeries.ItemsSourceProperty, nameof(ChartGroup.Data));
                chart.Series.Add(columnSeries);

                return new StackLayout
                {
                    Padding = 20,
                    Children =
                {
                    label,
                    chart
                }
                };
            })
        };

        Content = carousel;
    }
}

public class ChartGroup
{
    public string MonthYear { get; set; }
    public List<ChartPoint> Data { get; set; }
}

public class ChartPoint
{
    public string Kuupaev { get; set; }
    public int Kogus { get; set; }
}

