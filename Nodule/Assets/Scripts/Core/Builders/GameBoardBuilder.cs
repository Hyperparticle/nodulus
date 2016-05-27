using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Core.Data;
using Assets.Scripts.Core.Game;
using Assets.Scripts.Core.Items;

namespace Assets.Scripts.Core.Builders
{
    public class GameBoardBuilder 
    {
        public static GameBoard BuildBoard(IEnumerable<Point> nodePositions, IEnumerable<EdgeArg> edgeArgs)
        {
            var gameBoard = new GameBoard();

            var buildNodes = BuildNodes(gameBoard, nodePositions);
            var buildEdges = BuildEdges(gameBoard, edgeArgs);

            // Fail fast if something went wrong
            if (!buildNodes || !buildEdges) return null;

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

        private static bool BuildEdges(GameBoard gameBoard, IEnumerable<EdgeArg> edgeArgs)
        {
            // Place all edges on the board, and return sucess if all placements were valid
            var success = edgeArgs
                .Select(edgeArg => gameBoard.CreateArc(edgeArg.Position, edgeArg.Direction))
                .All(valid => valid);

            return success;
        }
    }

    public struct EdgeArg
    {
        public Direction Direction { get; private set; }
        public Point Position { get; private set; }

        public EdgeArg(Point position, Direction direction) : this()
        {
            Position = position;
            Direction = direction;
        }
    }
}
