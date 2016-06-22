using Assets.Scripts.Core.Game;
using NUnit.Framework;

public class PuzzleTest
{
    [Test]
    public void Level0Test()
    {
        var gameboard = Level.BuildLevel(0);
        var puzzle = new Puzzle(gameboard);
    }
}
