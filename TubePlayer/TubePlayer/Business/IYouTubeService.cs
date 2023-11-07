namespace TubePlayer.Business;

public interface IYouTubeService
{
    Task<YouTubeVideoSet> SearchVideos(string searchQuery, string nextPageToken, uint maxResult, CancellationToken cancellationToken);
}
