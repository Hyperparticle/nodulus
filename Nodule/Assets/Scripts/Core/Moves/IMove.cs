namespace Assets.Scripts.Core.Moves
{
    public interface IMove
    {
        bool IsValid { get; }
        bool Play();
        bool Undo();
    }
}
