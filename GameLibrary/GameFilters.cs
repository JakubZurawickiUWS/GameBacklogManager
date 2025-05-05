namespace GameLibrary
{
    public static class GameFilters
    {
        public static GameFilterDelegate HighRatedAndInProgress => g =>
            g.Rating >= 8 && g.Status == GameStatus.InProgress;

        public static GameFilterDelegate ShortGames => g =>
            g.EstimatedPlaytimeMinutes <= 120;

        public static GameFilterDelegate ByPlatform(string platform) => g =>
            g.Platform == platform;
    }
}
