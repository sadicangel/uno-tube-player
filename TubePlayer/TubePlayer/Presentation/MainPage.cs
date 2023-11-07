namespace TubePlayer.Presentation;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.DataContext<BindableMainModel>((page, vm) => page
            .NavigationCacheMode(NavigationCacheMode.Required)
            .Background(Theme.Brushes.Background.Default)
            .Content(
                new Grid()
                    .SafeArea(SafeArea.InsetMask.All)
                    .RowDefinitions("Auto, *")
                    .Children(
                        new TextBox()
                            .Text(x => x
                                .Bind(() => vm.SearchTerm)
                                .Mode(BindingMode.TwoWay)
                                .UpdateSourceTrigger(UpdateSourceTrigger.PropertyChanged))
                            .PlaceholderText("Search term"),
                        new ListView()
                            .Grid(row: 1)
                            .ItemsSource(() => vm.VideoSearchResults)
                            .ItemTemplate<YouTubeVideo>(video =>
                                new StackPanel()
                                    .Children(
                                        new TextBlock()
                                            .FontWeight(FontWeights.Bold)
                                            .Text(() => video.Details.Snippet?.ChannelTitle),
                                        new TextBlock()
                                            .Text(() => video.Details.Snippet?.Title)
                            )
                    )
                )
            )
        );
    }
}
