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
            var gameBoard = new GameBoard(level);

            var buildNodes = BuildNodes(gameBoard, level.Nodes, level.StartNode, level.FinalNode);
            var buildArcs = BuildArcs(gameBoard, level.Arcs);

            // Fail fast if something went wrong
            if (!buildNodes || !buildArcs) return null;

            return gameBoard;
        }

        private static bool BuildNodes(
            GameBoard gameBoard, 
            IEnumerable<Point> nodePositions,
            Point startNode,
            Point finalNode
        ) {
            if (startNode.Equals(finalNode)) {
                return false;
            }
            
            // Generate a list of nodes
            var nodes = nodePositions
                .Select(pos => new Node(pos))
                .ToList();

            // Set start and final nodes
            gameBoard.StartNode = nodes.FirstOrDefault(node => node.Position.Equals(startNode)) ?? nodes.First();
            var final = nodes.FirstOrDefault(node => node.Position.Equals(finalNode)) ?? nodes.Last();
            final.Final = true;
            
            // Place all nodes on the board, and return success if all placements were valid
            var success = nodes
                .Select(gameBoard.PlaceNode)
                .All(valid => valid);
            
            return success;
        }

        private static bool BuildArcs(GameBoard gameBoard, IEnumerable<PointDir> arcStates)
        {
            // Place all arcs on the board, and return sucess if all placements were valid
            var success = arcStates
                .Select(state => gameBoard.CreateArc(state.Point, state.Direction))
                .All(valid => valid);

            return success;
        }
    }


}
