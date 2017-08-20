using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Builders;
using Core.Data;
using Core.Items;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Core.Game
{
    public class Levels
    {
        private const string BeginnerLevels = "Levels/BeginnerLevels";
//        private const string BeginnerLevels = "Levels/SavedLevels";
        private const string SavedLevels = "Levels/SavedLevels";
        
        private static readonly LevelPack LevelPack = LevelParser.DeserializeLevelPack(BeginnerLevels);
        private static readonly LevelPack SavedPack = LevelParser.DeserializeLevelPack(SavedLevels);
        
        public static int LevelCount => LevelPack.Levels.Count;

        public static GameBoard BuildLevel(int levelNum)
        {
            if (levelNum < 0 || levelNum >= LevelCount) {
                return null;
            }

            var level = LevelPack.Levels[levelNum];
            return GameBoardBuilder.BuildBoard(level);
        }
    }

    public class LevelParser
    {
        public static LevelPack DeserializeLevelPack(string filePath)
        {
            var builder = new DeserializerBuilder();
            builder.WithNamingConvention(new CamelCaseNamingConvention());
            builder.IgnoreUnmatchedProperties();
            
            var deserializer = builder.Build();
            
            var file = Resources.Load<TextAsset>(filePath);

            using (var reader = new StringReader(file.text))
            {
                var levelPackSer = deserializer.Deserialize<LevelPackSer>(reader);
                return new LevelPack(levelPackSer);
            }
        }

        public class LevelPackSer
        {
            public LevelPackInfo Info { get; set; }
            public List<LevelSer> Levels { get; set; }
        }

        public class LevelSer
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public long Moves { get; set; }
            public double TimeElapsed { get; set; }
            
            public List<int[]> Nodes { get; set; }
            public List<ArcSer> Arcs { get; set; }
            
            public int[] StartNode { get; set; }
            public int[] FinalNode { get; set; }
            public Direction StartPull { get; set; } = Direction.None;
        }

        public class ArcSer
        {
            public int[] Parent { get; set; }
            public Direction Direction { get; set; }
        }
    }

    public class LevelPack
    {
        public LevelPackInfo PackInfo { get; }
        public List<Level> Levels { get; }

        public LevelPack(LevelParser.LevelPackSer levelPackSer)
        {
            PackInfo = levelPackSer.Info;
            Levels = levelPackSer.Levels
                .Select(levelSer => new Level(levelSer))
                .ToList();
        }
    }

    public class LevelPackInfo
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
    }

    public class Level
    {
        public string Name { get; }
        public string Description { get; }
        public long Moves { get; }
        public double TimeElapsed { get; }
        
        public IEnumerable<Point> Nodes { get; }
        public IEnumerable<PointDir> Arcs { get; }

        public Point StartNode { get; }
        public Point FinalNode { get; }
        public Direction StartPull { get; }
        
        public Level(LevelParser.LevelSer levelSer)
        {
            Name = levelSer.Name;
            Description = levelSer.Description;
            Moves = levelSer.Moves;
            TimeElapsed = levelSer.TimeElapsed;

            Nodes = levelSer.Nodes
                .Select(node => new Point(node[0], node[1]));

            Arcs = levelSer.Arcs
                .Select(arc => {
                    var point = new Point(arc.Parent[0], arc.Parent[1]);
                    return new PointDir(point, arc.Direction);
                });

            var startNode = levelSer.StartNode ?? levelSer.Nodes[0];
            var finalNode = levelSer.FinalNode ?? levelSer.Nodes[levelSer.Nodes.Count - 1];
            
            StartNode = new Point(startNode[0], startNode[1]);
            FinalNode = new Point(finalNode[0], finalNode[1]);

            StartPull = levelSer.StartPull;
        }
    }
}
