using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;

[assembly: UserSecretsId("d2334460-3161-4580-a0bc-98624b687b80")]

namespace TubePlayer;

public class App : Application
{
    protected Window? MainWindow { get; private set; }
    protected IHost? Host { get; private set; }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var builder = this.CreateBuilder(args)
            // Add navigation support for toolkit controls such as TabBar and NavigationView
            .UseToolkitNavigation()
            .Configure(host => host
#if DEBUG
                // Switch to Development environment when running in DEBUG
                .UseEnvironment(Environments.Development)
#endif
                .UseConfiguration(
                    configureHostConfiguration: builder =>
                        builder
                            .AddUserSecrets<App>(optional: false),
                    configure: configBuilder =>
                        configBuilder
                            .EmbeddedSource<App>()
                            .Section<AppConfig>()
                )
                .UseSerialization(services =>
                {
                    services.AddSingleton(new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                })
                .ConfigureServices((context, services) =>
                {
#if USE_MOCKS
                    services.AddSingleton<IYouTubeService, YouTubeServiceMock>();
#else
                    services.AddSingleton<IYouTubeService, YouTubeService>();
#endif
                })
                .UseNavigation(ReactiveViewModelMappings.ViewModelMappings, RegisterRoutes)
                .UseHttp((context, services) =>
                {
                    services.AddRefitClientWithEndpoint<IYouTubeEndpoint, YouTubeEndpointOptions>(context, configure: (builder, options) => builder
                        .ConfigureHttpClient(httpClient =>
                        {
                            httpClient.BaseAddress = new Uri(options!.Url!);
                            httpClient.DefaultRequestHeaders.Add("X-Goog-Api-Key", options.ApiKey);
                        }));
                })
            );
        MainWindow = builder.Window;

#if DEBUG
        MainWindow.EnableHotReload();
#endif

        Host = await builder.NavigateAsync<Shell>();
    }

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            new ViewMap(ViewModel: typeof(ShellModel)),
            new ViewMap<MainPage, MainModel>(),
            new DataViewMap<VideoDetailsPage, VideoDetailsModel, YouTubeVideo>()
        );

        routes.Register(
            new RouteMap("", View: views.FindByViewModel<ShellModel>(),
                Nested:
                [
                    new RouteMap("Main", View: views.FindByViewModel<MainModel>()),
                    new RouteMap("VideoDetails", View: views.FindByViewModel<VideoDetailsModel>()),
                ]
            )
        );
    }
}
