namespace Core.Moves
{
    /// <summary>
    /// A playable player move.
    /// </summary>
    public interface IMove
    {
        bool IsValid { get; }
        bool Play();
        bool Undo();
    }
}
