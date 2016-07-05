using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Core.Builders;
using Assets.Scripts.Core.Data;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Assets.Scripts.Core.Game
{
    public class Levels
    {
        private static LevelPack _levelPack;
        private static LevelPack LevelPack
        {
            get { return _levelPack ?? (_levelPack = LevelParser.DeserializeLevel()); }
        }

        public static GameBoard BuildLevel(int levelNum)
        {
            if (levelNum < 0 || levelNum >= LevelCount) {
                return null;
            }

            var level = LevelPack.Levels[levelNum];
            return GameBoardBuilder.BuildBoard(level);
        }

        public static int LevelCount { get { return LevelPack.Levels.Count(); } }
    }

    public class LevelParser
    {
        private const string BeginnerLevels = "./Assets/Scripts/Core/Game/Levels/BeginnerLevels.yaml";
        
        public static LevelPack DeserializeLevel()
        {
            var deserializer = new Deserializer(namingConvention: new CamelCaseNamingConvention());

            using (var reader = File.OpenText(BeginnerLevels))
            {
                var levelPackSer = deserializer.Deserialize<LevelPackSer>(reader);
                return new LevelPack(levelPackSer);
            }
        }

        public class LevelPackSer
        {
            public LevelInfo Info { get; set; }
            public List<LevelSer> Levels { get; set; }
        }

        public class LevelSer
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public List<int[]> Nodes { get; set; }
            public List<ArcSer> Arcs { get; set; }
        }

        public class ArcSer
        {
            public int[] Parent { get; set; }
            public Direction Direction { get; set; }
        }
    }

    public class LevelPack
    {
        public readonly LevelInfo Info;
        public readonly List<Level> Levels;

        public LevelPack(LevelParser.LevelPackSer levelPackSer)
        {
            Info = levelPackSer.Info;
            Levels = levelPackSer.Levels
                .Select(levelSer => new Level(levelSer))
                .ToList();
        }
    }

    public class LevelInfo
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
    }

    public class Level
    {
        public readonly string Name;
        public readonly string Description;
        public readonly IEnumerable<Point> Nodes;
        public readonly IEnumerable<PointDir> Arcs;

        public Level(LevelParser.LevelSer levelSer)
        {
            Name = levelSer.Name;
            Description = levelSer.Description;

            Nodes = levelSer.Nodes
                .Select(node => new Point(node[0], node[1]));

            Arcs = levelSer.Arcs
                .Select(arc => {
                    var point = new Point(arc.Parent[0], arc.Parent[1]);
                    return new PointDir(point, arc.Direction);
                });
        }
    }
}

   

//private static readonly Func<GameBoard>[] Levels = new Func<GameBoard>[]
//{
//    delegate    // Level 0
//    {
//        var nodePositions = new[]
//        {
//            new Point(0, 1),
//            new Point(1, 1),
//            new Point(2, 1)
//        };

//        var arcArgs = new[]
//        {
//            new ArcArg(nodePositions[0], Direction.Right),
//        };

//        return GameBoardBuilder.BuildBoard(nodePositions, arcArgs);
//    },

//    delegate    // Level 1
//    {
//        var nodePositions = new[]
//        {
//            new Point(0, 1),
//            new Point(1, 2),
//            new Point(1, 1),
//            new Point(1, 0),
//            new Point(2, 0),
//            new Point(3, 1),
//            new Point(4, 2),
//            new Point(4, 0),
//            new Point(5, 0),
//            new Point(6, 1)
//        };

//        var arcArgs = new[]
//        {
//            new ArcArg(nodePositions[0], Direction.Right),
//            new ArcArg(nodePositions[1], Direction.Right),
//            new ArcArg(nodePositions[4], Direction.Right)
//        };

//        return GameBoardBuilder.BuildBoard(nodePositions, arcArgs);
//    },

//    delegate    // Level 2
//    {
//        var nodePositions = new[]
//        {
//            new Point(0, 1),
//            new Point(1, 0),
//            new Point(1, 2),
//            new Point(2, 1),
//            new Point(3, 0),
//            new Point(4, 1),
//            new Point(4, 2),
//            new Point(5, 2),
//            new Point(6, 0),
//            new Point(6, 1),
//            new Point(8, 0),
//            new Point(8, 2),
//            new Point(9, 1)
//        };

//        var arcArgs = new[]
//        {
//            new ArcArg(nodePositions[0], Direction.Right),
//            new ArcArg(nodePositions[7], Direction.Right),
//            new ArcArg(nodePositions[9], Direction.Down)
//        };

//        return GameBoardBuilder.BuildBoard(nodePositions, arcArgs);;
//    },

//    delegate    // Level 3
//    {
//        var nodePositions = new[]
//        {
//            new Point(0, 1),
//            new Point(1, 2),
//            new Point(2, 0),
//            new Point(2, 1),
//            new Point(4, 1),
//            new Point(4, 2),
//            new Point(5, 0),
//            new Point(5, 2),
//            new Point(6, 0),
//            new Point(6, 1),
//            new Point(8, 0),
//            new Point(8, 2),
//            new Point(9, 1)
//        };

//        var arcArgs = new[]
//        {
//            new ArcArg(nodePositions[0], Direction.Right),
//            new ArcArg(nodePositions[2], Direction.Right),
//            new ArcArg(nodePositions[5], Direction.Down)
//        };

//        return GameBoardBuilder.BuildBoard(nodePositions, arcArgs);
//    },

//    delegate    // Level 4
//    {
//        var nodePositions = new[]
//        {
//            new Point(0, 1),
//            new Point(1, 0),
//            new Point(1, 2),
//            new Point(2, 1),
//            new Point(2, 2),
//            new Point(4, 0),
//            new Point(4, 2),
//            new Point(5, 0),
//            new Point(5, 1),
//            new Point(6, 2),
//            new Point(7, 1),
//            new Point(7, 2),
//            new Point(8, 0),
//            new Point(9, 1)
//        };

//        var arcArgs = new[]
//        {
//            new ArcArg(nodePositions[0], Direction.Right),
//            new ArcArg(nodePositions[1], Direction.Right),
//            new ArcArg(nodePositions[4], Direction.Down)
//        };

//        return GameBoardBuilder.BuildBoard(nodePositions, arcArgs);
//    },

//    delegate    // Level 5
//    {
//        var nodePositions = new[]
//        {
//            new Point(0, 1),
//            new Point(1, 2),
//            new Point(2, 0),
//            new Point(3, 1),
//            new Point(3, 2),
//            new Point(4, 0),
//            new Point(4, 1),
//            new Point(6, 0),
//            new Point(6, 2),
//            new Point(7, 0),
//            new Point(7, 1),
//            new Point(8, 2),
//            new Point(9, 1)
//        };

//        var arcArgs = new[]
//        {
//            new ArcArg(nodePositions[0], Direction.Right),
//            new ArcArg(nodePositions[1], Direction.Right),
//            new ArcArg(nodePositions[4], Direction.Down)
//        };

//        return GameBoardBuilder.BuildBoard(nodePositions, arcArgs);
//    },

//    delegate    // Level 6
//    {
//        var nodePositions = new[]
//        {
//            new Point(0, 3),
//            new Point(1, 0),
//            new Point(1, 1),
//            new Point(1, 4),
//            new Point(2, 0),
//            new Point(2, 2),
//            new Point(2, 3),
//            new Point(2, 4),
//            new Point(3, 1),
//            new Point(3, 2),
//            new Point(4, 0),
//            new Point(4, 3),
//            new Point(5, 0),
//            new Point(5, 2),
//            new Point(5, 3),
//            new Point(5, 4),
//            new Point(6, 1)
//        };

//        var arcArgs = new[]
//        {
//            new ArcArg(nodePositions[0], Direction.Right),
//            new ArcArg(nodePositions[1], Direction.Right),
//            new ArcArg(nodePositions[11], Direction.Down)
//        };

//        return GameBoardBuilder.BuildBoard(nodePositions, arcArgs);
//    }
//};
