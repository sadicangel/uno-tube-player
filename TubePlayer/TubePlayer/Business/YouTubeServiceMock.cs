using TubePlayer.Services.Models;

namespace TubePlayer.Business;

internal sealed class YouTubeServiceMock : IYouTubeService
{
    private readonly VideoDetailsResultData _details;
    private readonly IDictionary<string, ChannelData> _channels;

    public YouTubeServiceMock(ISerializer serializer)
    {
        _details = serializer.FromString<VideoDetailsResultData>(YouTubeServiceMockData.DetailsData)!;

        var channelsData = serializer.FromString<ChannelSearchResultData>(YouTubeServiceMockData.ChannelData)!;
        _channels = channelsData.Items!.ToDictionary(channel => channel.Id!, StringComparer.OrdinalIgnoreCase);
    }

    public Task<YouTubeVideoSet> SearchVideos(string searchQuery, string nextPageToken, uint maxResult, CancellationToken ct)
    {
        var filtered = _details
            .Items!
            .Where(detail =>
                detail.Snippet!.Title!.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));

        var videos = filtered
                    .Select(detail => new YouTubeVideo(_channels[detail.Snippet!.ChannelId!], detail))
                    .ToImmutableList();

        var result = new YouTubeVideoSet(videos, NextPageToken: string.Empty);

        return Task.FromResult(result);
    }
}
