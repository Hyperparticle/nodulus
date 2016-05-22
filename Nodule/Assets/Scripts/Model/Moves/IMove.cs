namespace Assets.Scripts.Model.Moves
{
    public interface IMove
    {
        bool IsValid { get; }
        bool Play();
        bool Undo();
    }
}
