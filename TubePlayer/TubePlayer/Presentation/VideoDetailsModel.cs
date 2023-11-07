using Windows.Media.Core;

namespace TubePlayer.Presentation;

public partial record VideoDetailsModel(YouTubeVideo Video)
{
    public IFeed<MediaSource> VideoSource => Feed.Async((ct) => new ValueTask<MediaSource>());
}
