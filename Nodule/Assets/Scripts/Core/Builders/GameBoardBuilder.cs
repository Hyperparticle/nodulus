using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Game;
using Assets.Scripts.Core.Items;

namespace Assets.Scripts.Core.Builders
{
    public class GameBoardBuilder 
    {
        public static GameBoard BuildBoard(IEnumerable<Point> nodePositions, IEnumerable<ArcArg> arcArgs)
        {
            var gameBoard = new GameBoard();

            var buildNodes = BuildNodes(gameBoard, nodePositions);
            var buildArcs = BuildArcs(gameBoard, arcArgs);

            // Fail fast if something went wrong
            if (!buildNodes || !buildArcs) return null;

            return gameBoard;
        }

        private static bool BuildNodes(GameBoard gameBoard, IEnumerable<Point> nodePositions)
        {
            // Generate a list of nodes
            var nodes = nodePositions
                .Select(pos => new Node(pos))
                .ToList();

            // Set start and final nodes
            gameBoard.StartNode = nodes.First();
            nodes.Last().Final = true;

            // Place all nodes on the board, and return success if all placements were valid
            var success = nodes
                .Select(node => gameBoard.PlaceNode(node))
                .All(valid => valid);

            return success;
        }

        private static bool BuildArcs(GameBoard gameBoard, IEnumerable<ArcArg> arcArgs)
        {
            // Place all arcs on the board, and return sucess if all placements were valid
            var success = arcArgs
                .Select(arcArg => gameBoard.CreateArc(arcArg.Position, arcArg.Direction))
                .All(valid => valid);

            return success;
        }
    }

    public struct ArcArg
    {
        public Direction Direction { get; private set; }
        public Point Position { get; private set; }

        public ArcArg(Point pos, Direction dir) : this()
        {
            Position = pos;
            Direction = dir;
        }
    }
}
