using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Builders;
using Core.Data;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Core.Game
{
    public class Levels
    {
        private const string BeginnerLevels = "Levels/BeginnerLevels";
        
        private static readonly LevelPack LevelPack = LevelParser.DeserializeLevel(BeginnerLevels);

        public static GameBoard BuildLevel(int levelNum)
        {
            if (levelNum < 0 || levelNum >= LevelCount) {
                return null;
            }

            var level = LevelPack.Levels[levelNum];
            return GameBoardBuilder.BuildBoard(level);
        }

        public static int LevelCount => LevelPack.Levels.Count;
    }

    public class LevelParser
    {
        
        public static LevelPack DeserializeLevel(string filePath)
        {
            var builder = new DeserializerBuilder();
            builder.WithNamingConvention(new CamelCaseNamingConvention());
            
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
