namespace GameLibrary
{
    public delegate bool GameFilterDelegate(Game game);

    public interface IGameManager
    {
        void AddGame(Game game);
        List<Game> FilterGames(GameFilterDelegate filter);
        List<Game> GetAllGames();
    }
}
