using Windows.Media.Core;

namespace TubePlayer.Presentation;

public partial record VideoDetailsModel(YouTubeVideo Video, IYouTubePlayerEndpoint PlayerEndpoint)
{
    public IFeed<MediaSource> VideoSource => Feed.Async(GetVideoSourceAsync);

    private async ValueTask<MediaSource> GetVideoSourceAsync(CancellationToken cancellationToken)
    {
        var streamVideo =
        $$"""
        {
            "videoId": "{{Video.Id}}",
            "context": {
                "client": {
                    "clientName": "ANDROID_TESTSUITE",
                    "clientVersion": "1.9",
                    "androidSdkVersion": 30,
                    "hl": "en",
                    "gl": "US",
                    "utcOffsetMinutes": 0
                }
            }
        }
        """;

        // Get the available stream data
        var streamData = await PlayerEndpoint.GetStreamData(streamVideo, cancellationToken);

        // Get the video stream with the highest video quality
        var streamWithHighestVideoQuality = streamData.Content?.StreamingData?.Formats?.OrderByDescending(s => s.QualityLabel).FirstOrDefault()
            ?? throw new InvalidOperationException("Input stream collection is empty.");

        // Get the stream URL
        var streamUrl = streamWithHighestVideoQuality.Url;

        // Return the MediaSource using the stream URL
        return MediaSource.CreateFromUri(new Uri(streamUrl!));
    }
}
