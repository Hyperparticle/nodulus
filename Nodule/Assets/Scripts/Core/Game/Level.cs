using System;
using Assets.Scripts.Core.Builders;
using Assets.Scripts.Core.Data;

namespace Assets.Scripts.Core.Game
{
    public class Level
    {
        public static GameBoard BuildLevel(int level)
        {
            if (level < 0 || level >= Levels.Length) return null;
            return Levels[level]();
        }

        public static int LevelCount { get { return Levels.Length; } }

        private static readonly Func<GameBoard>[] Levels = new Func<GameBoard>[]
        {
            delegate    // Level 1
            {
                var nodePositions = new[]
                {
                    new Point(0, 1),
                    new Point(1, 2),
                    new Point(1, 1),
                    new Point(1, 0),
                    new Point(2, 0),
                    new Point(3, 1),
                    new Point(4, 2),
                    new Point(4, 0),
                    new Point(5, 0),
                    new Point(6, 1)
                };

                var arcArgs = new[]
                {
                    new ArcArg(nodePositions[0], Direction.Right),
                    new ArcArg(nodePositions[1], Direction.Right),
                    new ArcArg(nodePositions[4], Direction.Right)
                };

                return GameBoardBuilder.BuildBoard(nodePositions, arcArgs);
            },

            delegate    // Level 2
            {
                var nodePositions = new[]
                {
                    new Point(0, 1),
                    new Point(1, 0),
                    new Point(1, 2),
                    new Point(2, 1),
                    new Point(3, 0),
                    new Point(4, 1),
                    new Point(4, 2),
                    new Point(5, 2),
                    new Point(6, 0),
                    new Point(6, 1),
                    new Point(8, 0),
                    new Point(8, 2),
                    new Point(9, 1)
                };

                var arcArgs = new[]
                {
                    new ArcArg(nodePositions[0], Direction.Right),
                    new ArcArg(nodePositions[7], Direction.Right),
                    new ArcArg(nodePositions[9], Direction.Down)
                };

                return GameBoardBuilder.BuildBoard(nodePositions, arcArgs);;
            },

            delegate    // Level 3
            {
                var nodePositions = new[]
                {
                    new Point(0, 1),
                    new Point(1, 2),
                    new Point(2, 0),
                    new Point(2, 1),
                    new Point(4, 1),
                    new Point(4, 2),
                    new Point(5, 0),
                    new Point(5, 2),
                    new Point(6, 0),
                    new Point(6, 1),
                    new Point(8, 0),
                    new Point(8, 2),
                    new Point(9, 1)
                };

                var arcArgs = new[]
                {
                    new ArcArg(nodePositions[0], Direction.Right),
                    new ArcArg(nodePositions[2], Direction.Right),
                    new ArcArg(nodePositions[5], Direction.Down)
                };

                return GameBoardBuilder.BuildBoard(nodePositions, arcArgs);
            },

            delegate    // Level 4
            {
                var nodePositions = new[]
                {
                    new Point(0, 1),
                    new Point(1, 0),
                    new Point(1, 2),
                    new Point(2, 1),
                    new Point(2, 2),
                    new Point(4, 0),
                    new Point(4, 2),
                    new Point(5, 0),
                    new Point(5, 1),
                    new Point(6, 2),
                    new Point(7, 1),
                    new Point(7, 2),
                    new Point(8, 0),
                    new Point(9, 1)
                };

                var arcArgs = new[]
                {
                    new ArcArg(nodePositions[0], Direction.Right),
                    new ArcArg(nodePositions[1], Direction.Right),
                    new ArcArg(nodePositions[4], Direction.Down)
                };

                return GameBoardBuilder.BuildBoard(nodePositions, arcArgs);
            },

            delegate    // Level 5
            {
                var nodePositions = new[]
                {
                    new Point(0, 1),
                    new Point(1, 2),
                    new Point(2, 0),
                    new Point(3, 1),
                    new Point(3, 2),
                    new Point(4, 0),
                    new Point(4, 1),
                    new Point(6, 0),
                    new Point(6, 2),
                    new Point(7, 0),
                    new Point(7, 1),
                    new Point(8, 2),
                    new Point(9, 1)
                };

                var arcArgs = new[]
                {
                    new ArcArg(nodePositions[0], Direction.Right),
                    new ArcArg(nodePositions[1], Direction.Right),
                    new ArcArg(nodePositions[4], Direction.Down)
                };

                return GameBoardBuilder.BuildBoard(nodePositions, arcArgs);
            },

            delegate    // Level 6
            {
                var nodePositions = new[]
                {
                    new Point(0, 3),
                    new Point(1, 0),
                    new Point(1, 1),
                    new Point(1, 4),
                    new Point(2, 0),
                    new Point(2, 2),
                    new Point(2, 3),
                    new Point(2, 4),
                    new Point(3, 1),
                    new Point(3, 2),
                    new Point(4, 0),
                    new Point(4, 3),
                    new Point(5, 0),
                    new Point(5, 2),
                    new Point(5, 3),
                    new Point(5, 4),
                    new Point(6, 1)
                };

                var arcArgs = new[]
                {
                    new ArcArg(nodePositions[0], Direction.Right),
                    new ArcArg(nodePositions[1], Direction.Right),
                    new ArcArg(nodePositions[11], Direction.Down)
                };

                return GameBoardBuilder.BuildBoard(nodePositions, arcArgs);
            }
        };
    }
}
