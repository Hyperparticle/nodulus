using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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
        
        public static readonly string SavedLevels = Application.persistentDataPath + "/SavedLevels.yaml";

        private static readonly LevelPack OriginalLevels = LevelParser.DeserializeLevelPackDef(BeginnerLevels);
        private static readonly LevelPack CurrentLevels = LevelParser.DeserializeLevelPack(SavedLevels, BeginnerLevels);
        
        public static int LevelCount => CurrentLevels.Levels.Count;

        public static int CurrentLevelNum => CurrentLevels.CurrentLevelNum;

        public static GameBoard BuildLevel(int levelNum, bool restart = false)
        {
            if (levelNum < 0 || levelNum >= LevelCount) {
                return null;
            }

            var level = restart ? OriginalLevels.Levels[levelNum] : CurrentLevels.Levels[levelNum];
            return GameBoardBuilder.BuildBoard(level);
        }

        public static void SaveLevel(Level level, bool win = false)
        {
            if (win) {
                var originalLevel = OriginalLevels.Levels[level.Number];
                
                var winCount = level.WinCount + 1;
                level = new Level(originalLevel) {
                    WinCount = winCount,
                    MovesBestScore = level.MovesBestScore
                };
            }
            
            CurrentLevels.CurrentLevelNum = level.Number;
            CurrentLevels.Levels[level.Number] = level;
            
            LevelParser.SerializeLevelPack(CurrentLevels, SavedLevels);
        }
    }

    public class LevelParser
    {
        private static readonly string TempFilePath = Application.persistentDataPath + "/TempLevels.yaml";
        private static StreamWriter _tempFileWriter;

        private static Thread _writerThread;

        public static LevelPack DeserializeLevelPack(string filePath, string fallbackFilePath)
        {
            var fallbackFile = Resources.Load<TextAsset>(fallbackFilePath);
            var fallbackLevelPack = DeserializeLevelPack(fallbackFile.text);

            // If the save file doesn't exist, copy from the fallback
            if (!File.Exists(filePath)) {
                File.WriteAllText(filePath, fallbackFile.text);
                return fallbackLevelPack;
            }

            var file = File.ReadAllText(filePath);
            var levelPack = DeserializeLevelPack(file);
            var update = !levelPack.PackInfo.Version.Equals(fallbackLevelPack.PackInfo.Version);

            if (!update) {
                return levelPack;
            }
            
            // If there is an update, use the fallback
            File.WriteAllText(filePath, fallbackFile.text);
            return fallbackLevelPack;
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

        public static void SerializeLevelPack(LevelPack levelPack, string filePath)
        {
            #if !UNITY_WEBGL
            
            var levelPackSer = LevelPackSer.Create(levelPack);
            
            var serializer = new SerializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();

            if (_tempFileWriter == null) {
                _tempFileWriter = new StreamWriter(TempFilePath);
            }

            if (_writerThread?.IsAlive ?? false) {
                return;
            }
            
            _writerThread = new Thread(() => {
                // ATOMIC OPERATION
                // Write the level to a temporary file
                serializer.Serialize(_tempFileWriter, levelPackSer);
                _tempFileWriter.Flush();
                    
                // Replace the original file with the temporary file
                File.Copy(TempFilePath, filePath, true);
            });
                
            _writerThread.Start();
            
            #endif
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
            public long MovesBestScore { get; set; }
            public double TimeElapsed { get; set; }
            public long WinCount { get; set; }
            
            public List<int[]> Nodes { get; set; }
            public List<PointDirSer> Arcs { get; set; }
            
            public int[] StartNode { get; set; }
            public int[] FinalNode { get; set; }
            public Direction StartPull { get; set; } = Direction.None;
            
            public List<PointDirSer> Tutorial { get; set; }

            public static LevelSer Create(Level level)
            {
                return new LevelSer {
                    Name = level.Name,
                    Description = level.Description,
                    Moves = level.Moves,
                    MovesBestScore = level.MovesBestScore,
                    TimeElapsed = level.TimeElapsed,
                    Nodes = level.Nodes.Select(node => new[] {node.x, node.y}).ToList(),
                    Arcs = level.Arcs
                        .Select(arc => new PointDirSer {
                            Parent = new[] {arc.Point.x, arc.Point.y},
                            Direction = arc.Direction
                        })
                        .ToList(),
                    StartNode = new[] {level.StartNode.x, level.StartNode.y},
                    FinalNode = new[] {level.FinalNode.x, level.FinalNode.y},
                    StartPull = level.StartPull,
                    WinCount = level.WinCount,
                    Tutorial =  level.Tutorial
                        ?.Select(arc => new PointDirSer {
                            Parent = new[] {arc.Point.x, arc.Point.y},
                            Direction = arc.Direction
                        })
                        ?.ToList()
                };
            }
        }

        public class PointDirSer
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
        public long MovesBestScore { get; set; }
        public double TimeElapsed { get; }
        public long WinCount { get; set; }
        
        public int Number { get; }
        
        public List<Point> Nodes { get; }
        public List<PointDir> Arcs { get; }

        public Point StartNode { get; }
        public Point FinalNode { get; }
        
        public Direction StartPull { get; }
        
        public List<PointDir> Tutorial { get; }
        
        public Level(LevelParser.LevelSer levelSer, int number)
        {
            Name = levelSer.Name;
            Description = levelSer.Description;
            Moves = levelSer.Moves;
            MovesBestScore = levelSer.MovesBestScore;
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

            Tutorial = levelSer.Tutorial
                ?.Select(arc => {
                    var point = new Point(arc.Parent[0], arc.Parent[1]);
                    return new PointDir(point, arc.Direction);
                })
                ?.ToList();
        }

        public Level(string name, string description, int number,
            IEnumerable<Point> nodes, IEnumerable<PointDir> arcs,
            Point startNode, Point finalNode, Direction startPull = Direction.None,
            long moves = 0, long movesBestScore = 0, double timeElapsed = 0, long winCount = 0,
            IEnumerable<PointDir> tutorial = null)
        {
            Name = name;
            Description = description;
            Moves = moves;
            MovesBestScore = movesBestScore;
            TimeElapsed = timeElapsed;
            WinCount = winCount;

            Number = number;

            Nodes = nodes.ToList();
            Arcs = arcs.ToList();

            StartNode = startNode;
            FinalNode = finalNode;

            StartPull = startPull;

            Tutorial = tutorial?.ToList();
        }

        public Level(Level level)
        {
            Name = level.Name;
            Description = level.Description;
            Moves = level.Moves;
            MovesBestScore = level.MovesBestScore;
            TimeElapsed = level.TimeElapsed;
            WinCount = level.WinCount;

            Number = level.Number;

            Nodes = level.Nodes;
            Arcs = level.Arcs;

            StartNode = level.StartNode;
            FinalNode = level.FinalNode;

            StartPull = level.StartPull;

            Tutorial = level.Tutorial;
        }
    }
}
