using System.Collections.Generic;
using System.Linq;
using Core.Data;
using Core.Game;
using Core.Items;

namespace Core.Builders
{
    public class GameBoardBuilder 
    {
        public static GameBoard BuildBoard(Level level)
        {
            var gameBoard = new GameBoard();

            var buildNodes = BuildNodes(gameBoard, level.Nodes);
            var buildArcs = BuildArcs(gameBoard, level.Arcs);

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
                .Select(gameBoard.PlaceNode)
                .All(valid => valid);

            return success;
        }

        private static bool BuildArcs(GameBoard gameBoard, IEnumerable<PointDir> arcPositions)
        {
            // Place all arcs on the board, and return sucess if all placements were valid
            var success = arcPositions
                .Select(pointDir => gameBoard.CreateArc(pointDir.Point, pointDir.Direction))
                .All(valid => valid);

            return success;
        }
    }


}
