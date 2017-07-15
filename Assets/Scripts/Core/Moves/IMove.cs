namespace Core.Moves
{
    public interface IMove
    {
        bool IsValid { get; }
        bool Play();
        bool Undo();
    }
}
