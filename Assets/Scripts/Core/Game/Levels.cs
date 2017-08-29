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
        private static readonly string SavedLevels = Application.persistentDataPath + "/SavedLevels.yaml";

        private static readonly LevelPack OriginalLevels = LevelParser.DeserializeLevelPackDef(BeginnerLevels);
        private static readonly LevelPack CurrentLevels = LevelParser.DeserializeLevelPack(SavedLevels, BeginnerLevels);
        
        public static int LevelCount => CurrentLevels.Levels.Count;

        public static int CurrentLevelNum => CurrentLevels.CurrentLevelNum;

        public static GameBoard BuildLevel(int levelNum)
        {
            if (levelNum < 0 || levelNum >= LevelCount) {
                return null;
            }
            
            var level = CurrentLevels.Levels[levelNum];
            return GameBoardBuilder.BuildBoard(level);
        }

        public static void SaveLevel(Level level, bool win = false)
        {
            if (win) {
                var originalLevel = OriginalLevels.Levels[level.Number];
                
                var winCount = level.WinCount + 1;
                level = new Level(originalLevel) { WinCount = winCount};
            }
            
            CurrentLevels.CurrentLevelNum = level.Number;
            CurrentLevels.Levels[level.Number] = level;
            LevelParser.SerializeLevelPack(SavedLevels, CurrentLevels);
        }
    }

    public class LevelParser
    {
        public static LevelPack DeserializeLevelPack(string filePath, string fallbackFilePath)
        {
            if (File.Exists(filePath)) {
                var file = File.ReadAllText(filePath);
                return DeserializeLevelPack(file);
            }
            
            var fallbackFile = Resources.Load<TextAsset>(fallbackFilePath);
            File.WriteAllText(filePath, fallbackFile.text);
            
            return DeserializeLevelPack(fallbackFile.text);
        }

        public static LevelPack DeserializeLevelPackDef(string filePath)
        {
            var file = Resources.Load<TextAsset>(filePath);
            return DeserializeLevelPack(file.text);
        }
        
        public static LevelPack DeserializeLevelPack(string fileText)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .IgnoreUnmatchedProperties()
                .Build();
            
            using (var reader = new StringReader(fileText)) {
                var levelPackSer = deserializer.Deserialize<LevelPackSer>(reader);
                return new LevelPack(levelPackSer);
            }
        }

        public static void SerializeLevelPack(string filePath, LevelPack levelPack)
        {
            var levelPackSer = LevelPackSer.Create(levelPack);
            
            var serializer = new SerializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();

            var copyFilePath = filePath + "_Next";
            var replaceFilePath = filePath + "_Prev";

            // ATOMIC OPERATION
            // Write the level to a copy file
            using (var writer = new StreamWriter(copyFilePath)) {
                serializer.Serialize(writer, levelPackSer);
            }
            
            // Replace the original file with the new copy,
            // creating a backup of the original file in the process
            File.Replace(copyFilePath, filePath, replaceFilePath);
        }

        public class LevelPackSer
        {
            public LevelPackInfo Info { get; set; }
            public int CurrentLevel { get; set; }
            public List<LevelSer> Levels { get; set; }

            public static LevelPackSer Create(LevelPack levelPack)
            {
                return new LevelPackSer {
                    Info = levelPack.PackInfo,
                    Levels = levelPack.Levels.Select(LevelSer.Create).ToList(),
                    CurrentLevel = levelPack.CurrentLevelNum
                };
            }
        }

        public class LevelSer
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public long Moves { get; set; }
            public double TimeElapsed { get; set; }
            public long WinCount { get; set; }
            
            public List<int[]> Nodes { get; set; }
            public List<ArcSer> Arcs { get; set; }
            
            public int[] StartNode { get; set; }
            public int[] FinalNode { get; set; }
            public Direction StartPull { get; set; } = Direction.None;

            public static LevelSer Create(Level level)
            {
                return new LevelSer {
                    Name = level.Name,
                    Description = level.Description,
                    Moves = level.Moves,
                    TimeElapsed = level.TimeElapsed,
                    Nodes = level.Nodes.Select(node => new[] {node.x, node.y}).ToList(),
                    Arcs = level.Arcs
                        .Select(arc => new ArcSer() {
                            Parent = new[] {arc.Point.x, arc.Point.y},
                            Direction = arc.Direction
                        })
                        .ToList(),
                    StartNode = new[] {level.StartNode.x, level.StartNode.y},
                    FinalNode = new[] {level.FinalNode.x, level.FinalNode.y},
                    StartPull = level.StartPull,
                    WinCount = level.WinCount
                };
            }
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
        public int CurrentLevelNum { get; set; }
        public List<Level> Levels { get; }

        public LevelPack(LevelParser.LevelPackSer levelPackSer)
        {
            PackInfo = levelPackSer.Info;
            Levels = levelPackSer.Levels
                .Select((levelSer, i) => new Level(levelSer, i))
                .ToList();
            CurrentLevelNum = levelPackSer.CurrentLevel;
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
        public long WinCount { get; set; }
        
        public int Number { get; }
        
        public List<Point> Nodes { get; }
        public List<PointDir> Arcs { get; }

        public Point StartNode { get; }
        public Point FinalNode { get; }
        
        public Direction StartPull { get; }
        
        public Level(LevelParser.LevelSer levelSer, int number)
        {
            Name = levelSer.Name;
            Description = levelSer.Description;
            Moves = levelSer.Moves;
            TimeElapsed = levelSer.TimeElapsed;
            WinCount = levelSer.WinCount;

            Number = number;

            Nodes = levelSer.Nodes
                .Select(node => new Point(node[0], node[1]))
                .ToList();

            Arcs = levelSer.Arcs
                .Select(arc => {
                    var point = new Point(arc.Parent[0], arc.Parent[1]);
                    return new PointDir(point, arc.Direction);
                })
                .ToList();

            var startNode = levelSer.StartNode ?? levelSer.Nodes[0];
            var finalNode = levelSer.FinalNode ?? levelSer.Nodes[levelSer.Nodes.Count - 1];
            
            StartNode = new Point(startNode[0], startNode[1]);
            FinalNode = new Point(finalNode[0], finalNode[1]);

            StartPull = levelSer.StartPull;
        }

        public Level(string name, string description, int number,
            IEnumerable<Point> nodes, IEnumerable<PointDir> arcs,
            Point startNode, Point finalNode, Direction startPull = Direction.None,
            long moves = 0, double timeElapsed = 0, long winCount = 0)
        {
            Name = name;
            Description = description;
            Moves = moves;
            TimeElapsed = timeElapsed;
            WinCount = winCount;

            Number = number;

            Nodes = nodes.ToList();
            Arcs = arcs.ToList();

            StartNode = startNode;
            FinalNode = finalNode;

            StartPull = startPull;
        }

        public Level(Level level)
        {
            Name = level.Name;
            Description = level.Description;
            Moves = level.Moves;
            TimeElapsed = level.TimeElapsed;
            WinCount = level.WinCount;

            Number = level.Number;

            Nodes = level.Nodes;
            Arcs = level.Arcs;

            StartNode = level.StartNode;
            FinalNode = level.FinalNode;

            StartPull = level.StartPull;
        }
    }
}
