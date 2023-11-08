using Uno.Extensions.Reactive.Sources;

namespace TubePlayer.Presentation;

public partial record MainModel(IYouTubeService YouTubeService)
{
    public IState<string> SearchTerm => State<string>.Value(this, () => "Uno Platform");

    public IListFeed<YouTubeVideo> VideoSearchResults => SearchTerm
        .Where(searchTerm => searchTerm is { Length: > 0 })
        .SelectPaginatedByCursorAsync(
            firstPage: string.Empty,
            getPage: async (searchTerm, nextPageToken, desiredPageSize, cancellationToken) =>
            {
                var videoSet = await YouTubeService.SearchVideos(searchTerm, nextPageToken, desiredPageSize ?? 10, cancellationToken);

                return new PageResult<string, YouTubeVideo>(videoSet.Videos, videoSet.NextPageToken);
            });
}
