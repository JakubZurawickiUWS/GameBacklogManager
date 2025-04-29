namespace GameLibrary
{
    public interface IGame
    {
        string Title { get; set; }
        string Genre { get; set; }
        string Platform { get; set; }
        int PlaytimeMinutes { get; set; }
        int EstimatedPlaytimeMinutes { get; set; }
        GameStatus Status { get; set; }
        int Rating { get; set; }
    }
}
