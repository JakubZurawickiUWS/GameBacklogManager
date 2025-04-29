namespace GameLibrary
{
    public class InvalidGameTitleException : Exception
    {
        public InvalidGameTitleException(string title)
            : base($"Tytuł gry \"{title}\" jest niepoprawny.") { }
    }
}
