using Assets.Scripts.Core.Data;

namespace Assets.Scripts.Core.Items
{
    /// <summary>
    /// Represents a main game item that is on the game board
    /// </summary>
    public interface IBoardItem
    {
        /// <summary>
        /// The position of this item on the game grid
        /// </summary>
        Point Position { get; }

        /// <summary>
        /// The length of this item (0 if point behavior)
        /// </summary>
        int Length { get; }

        /// <summary>
        /// The direction the item faces
        /// </summary>
        Direction Direction { get; }

        /// <summary>
        /// True if the item is enabled
        /// </summary>
        bool IsEnabled { get; }
    }
}
