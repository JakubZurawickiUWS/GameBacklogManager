namespace GameLibrary
{
    public class Game : IGame
    {
        public string Title { get; set; }
        public string Genre { get; set; }
        public string Platform { get; set; }
        public int PlaytimeMinutes { get; set; }
        public int EstimatedPlaytimeMinutes { get; set; }
        public GameStatus Status { get; set; }
        public int Rating { get; set; } = 0;

        public string ProgressPercent => $"{Math.Min((int)((double)PlaytimeMinutes / EstimatedPlaytimeMinutes * 100), 100)}%";


        public override string ToString() => $"{Title} ({Platform})";

        public override bool Equals(object obj)
        {
            return obj is Game other &&
                   Title == other.Title &&
                   Platform == other.Platform;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Title, Platform);
        }

    }
}
