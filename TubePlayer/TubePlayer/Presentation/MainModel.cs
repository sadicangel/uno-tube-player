namespace TubePlayer.Presentation;

public partial record MainModel(IYouTubeService YouTubeService)
{
    public IState<string> SearchTerm => State<string>.Value(this, () => "Uno Platform");

    public IListFeed<YouTubeVideo> VideoSearchResults => SearchTerm
        .Where(searchTerm => searchTerm is { Length: > 0 })
        .SelectAsync(async (searchTerm, cancellationToken) => await YouTubeService.SearchVideos(searchTerm, nextPageToken: "", maxResult: 30, cancellationToken))
        .Select(result => result.Videos)
        .AsListFeed();
}
