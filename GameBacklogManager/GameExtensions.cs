using GameLibrary;

namespace GameBacklogManager
{
    public static class GameExtensions
    {
        public static int ToProgressPercent(this Game game)
        {
            if (game.EstimatedPlaytimeMinutes == 0)
                return 0;
            return (int)((double)game.PlaytimeMinutes / game.EstimatedPlaytimeMinutes * 100);
        }
    }
}
