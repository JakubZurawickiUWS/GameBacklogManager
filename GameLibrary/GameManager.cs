namespace GameLibrary
{
    public class GameManager : IGameManager
    {
        private List<Game> games = new List<Game>();

        public void AddGame(Game game)
        {
            if (string.IsNullOrWhiteSpace(game.Title))
                throw new InvalidGameTitleException(game.Title);

            if (games.Any(g => g.Title == game.Title && g.Platform == game.Platform))
                throw new InvalidGameTitleException($"Gra '{game.Title}' na platformę '{game.Platform}' już istnieje!");

            games.Add(game);
        }

        public void UpdateGame(Game original, Game updated)
        {
            var game = games.FirstOrDefault(g => g == original);
            if (game != null)
            {
                game.Title = updated.Title;
                game.Genre = updated.Genre;
                game.Platform = updated.Platform;
                game.EstimatedPlaytimeMinutes = updated.EstimatedPlaytimeMinutes;
                game.Rating = updated.Rating;
                game.Status = updated.Status;
            }
        }

        public List<Game> FilterGames(GameFilterDelegate filter)
        {
            return games.Where(g => filter(g)).ToList();
        }

        public List<Game> GetAllGames() => games;

        public bool RemoveGame(Game game) => games.Remove(game);
    }
}
