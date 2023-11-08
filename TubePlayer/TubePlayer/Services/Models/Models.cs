namespace TubePlayer.Services.Models;

public sealed record ThumbnailData(string? Url);

public sealed record ThumbnailsData(ThumbnailData? Medium, ThumbnailData? High);

public sealed record SnippetData(string? ChannelId, string? Title, string? Description, ThumbnailsData? Thumbnails, string? ChannelTitle, DateTime? PublishedAt);

public sealed record StatisticsData(string? ViewCount, string? LikeCount, string? CommentCount, string? SubscriberCount);

public sealed partial record ChannelData(
    string? Id,
    SnippetData? Snippet,
    StatisticsData? Statistics = default);

public sealed record ContentDetailsData(string? Duration);

public sealed partial record YouTubeVideoDetailsData(
    string? Id,
    SnippetData? Snippet,
    StatisticsData? Statistics = default,
    ContentDetailsData? ContentDetails = default);

public sealed record VideoDetailsResultData(ImmutableList<YouTubeVideoDetailsData>? Items);

public sealed record ChannelSearchResultData(ImmutableList<ChannelData>? Items);

public partial record IdData(string? VideoId);

public partial record YouTubeVideoData(IdData? Id, SnippetData? Snippet);

public record PageInfoData(int? TotalResults, int? ResultsPerPage);

public record VideoSearchResultData(IImmutableList<YouTubeVideoData>? Items, string? NextPageToken, PageInfoData? PageInfo);
