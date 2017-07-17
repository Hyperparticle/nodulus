using Core.Data;
using Core.Game;
using NUnit.Framework;

namespace Test
{
    public class PuzzleTest
    {
        [Test]
        public void Level0Test()
        {
            var gameBoard = Levels.BuildLevel(0);
            var puzzle = new Puzzle(gameBoard);

            //Debug.Log(gameBoard.GetBoard(puzzle.PullFields, puzzle.PushFields));

            // Play the entire level
            Pull(1, 1, Direction.Right, puzzle, gameBoard);
            Push(1, 1, Direction.Down,  puzzle, gameBoard);
            Pull(1, 0, Direction.Down, puzzle, gameBoard);
            Push(1, 0, Direction.Right, puzzle, gameBoard);
            Pull(2, 0, Direction.Right, puzzle, gameBoard);
            Push(4, 0, Direction.Right, puzzle, gameBoard);
            Pull(4, 0, Direction.Right, puzzle, gameBoard);
            Push(4, 0, Direction.Up, puzzle, gameBoard);
            Pull(4, 0, Direction.Left, puzzle, gameBoard);
            Push(1, 2, Direction.Down, puzzle, gameBoard);
            Pull(4, 2, Direction.Up, puzzle, gameBoard);
            Push(1, 1, Direction.Right, puzzle, gameBoard);
            Pull(1, 2, Direction.Left, puzzle, gameBoard);
            Push(3, 1, Direction.Right, puzzle, gameBoard);

            Assert.IsTrue(puzzle.Win);
        }

        private static void Pull(int x, int y, Direction dir, Puzzle puzzle, GameBoard gameBoard)
        {
            var status = puzzle.PullArc(new Point(x, y), dir);
            //Debug.Log(gameBoard.GetBoard(puzzle.PullFields, puzzle.PushFields));
            Assert.IsTrue(status);
            Assert.IsTrue(puzzle.IsPulled);
            Assert.IsTrue(!puzzle.Win);
        }

        private static void Push(int x, int y, Direction dir, Puzzle puzzle, GameBoard gameBoard)
        {
            var status = puzzle.PushArc(new Point(x, y), dir);
            //Debug.Log(gameBoard.GetBoard(puzzle.PullFields, puzzle.PushFields));
            Assert.IsTrue(status);
            Assert.IsTrue(!puzzle.IsPulled);
        }
    }
}
