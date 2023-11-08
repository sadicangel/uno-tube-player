namespace TubePlayer.Business;

internal sealed class YouTubeService(IYouTubeEndpoint endpoint) : IYouTubeService
{
    public async Task<YouTubeVideoSet> SearchVideos(string searchQuery, string nextPageToken, uint maxResult, CancellationToken cancellationToken)
    {
        var resultData = await endpoint.SearchVideos(searchQuery, nextPageToken, maxResult, cancellationToken);

        var results = resultData?.Items?
            .Where(result => !string.IsNullOrWhiteSpace(result.Snippet?.ChannelId) && !string.IsNullOrWhiteSpace(result.Id?.VideoId))
            .ToArray();

        if (results is not { Length: > 0 })
            return YouTubeVideoSet.CreateEmpty();

        var channelIds = results
            .Select(v => v.Snippet!.ChannelId!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var videoIds = results
            .Select(v => v.Id!.VideoId!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var asyncDetails = endpoint.GetVideoDetails(videoIds, cancellationToken);
        var asyncChannels = endpoint.GetChannels(channelIds, cancellationToken);

        await Task.WhenAll(asyncDetails, asyncChannels);

        var detailItems = (await asyncDetails)?.Items;
        var channelItems = (await asyncChannels)?.Items;

        if (detailItems is null || channelItems is null)
            return YouTubeVideoSet.CreateEmpty();

        var detailsResult = detailItems!
            .Where(detail => !string.IsNullOrWhiteSpace(detail.Id))
            .DistinctBy(detail => detail.Id)
            .ToDictionary(detail => detail.Id!, StringComparer.OrdinalIgnoreCase);

        var channelsResult = channelItems!
            .Where(channel => !string.IsNullOrWhiteSpace(channel.Id))
            .DistinctBy(channel => channel.Id)
            .ToDictionary(channel => channel.Id!, StringComparer.OrdinalIgnoreCase);

        var videoSet = new List<YouTubeVideo>();

        foreach (var result in results)
        {
            if (channelsResult.TryGetValue(result.Snippet!.ChannelId!, out var channel) && detailsResult.TryGetValue(result.Id!.VideoId!, out var details))
                videoSet.Add(new YouTubeVideo(channel, details));
        }

        return new YouTubeVideoSet(videoSet.ToImmutableList(), resultData?.NextPageToken ?? string.Empty);
    }
}
