// Project:         RandomStartingDungeon mod for Daggerfall Unity (http://www.dfworkshop.net)
// Copyright:       Copyright (C) 2022 Kirk.O
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Author:          Kirk.O
// Created On: 	    8/12/2020, 5:05 PM
// Last Edit:		7/1/2022, 11:00 PM
// Version:			1.10
// Special Thanks:  Jehuty, TheLacus, Hazelnut
// Modifier:

using DaggerfallWorkshop.Game;
using System;
using System.IO;
using DaggerfallWorkshop;
using UnityEngine;
using Wenzil.Console;
using DaggerfallConnect.Arena2;

namespace RandomStartingDungeon
{
    public static class RandomStartingDungeonConsoleCommands
    {
        const string noInstanceMessage = "Random Starting Dungeon instance not found.";

        public static void RegisterCommands()
        {
            try
            {
                ConsoleCommandsDatabase.RegisterCommand(ManualRandomTeleport.name, ManualRandomTeleport.description, ManualRandomTeleport.usage, ManualRandomTeleport.Execute);
                ConsoleCommandsDatabase.RegisterCommand(TransformDungPos.name, TransformDungPos.description, TransformDungPos.usage, TransformDungPos.Execute);
                ConsoleCommandsDatabase.RegisterCommand(CurrentBlockInfo.name, CurrentBlockInfo.description, CurrentBlockInfo.usage, CurrentBlockInfo.Execute);
                ConsoleCommandsDatabase.RegisterCommand(CreateLocationDatabase.name, CreateLocationDatabase.description, CreateLocationDatabase.usage, CreateLocationDatabase.Execute);
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Error Registering RandomStartingDungeon Console commands: {0}", e.Message));
            }
        }

        private static class ManualRandomTeleport
        {
            public static readonly string name = "manual_random_teleport";
            public static readonly string description = "Randomly Teleport To Dungeon Based On Current Sessions Options";
            public static readonly string usage = "Randomly Teleport To Dungeon";

            public static string Execute(params string[] args)
            {
                var randomStartingDungeon = RandomStartingDungeon.Instance;
                if (randomStartingDungeon == null)
                    return noInstanceMessage;

                RandomStartingDungeon.PickRandomDungeonTeleport();

                return "Teleporting To Random Dungeon Now...";
            }
        }

        private static class CreateLocationDatabase
        {
            public static readonly string name = "create_location_database";
            public static readonly string description = "Builds location database";
            public static readonly string usage = "Takes no params";

            public static string Execute(params string[] args)
            {
                var filePath = Application.persistentDataPath + @"/AllTheTowns.txt";
                try
                {
                    // Check if the file exists and delete it if it does
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        writer.WriteLine($"Region Name\tRegion ID\tLocation Index\tLocation Name\tLocation ID\tLocation Key\tLocation Type\tDungeon Type\tLatitude\tLongitude\tMapPixel X\tMapPixel Y");

                        for (int n = 0; n < 62; n++)
                        {
                            var regionData = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetRegion(n);
                            for (int i = 0; i < regionData.LocationCount; i++)
                            {
                                var location = regionData.MapTable[i];
                                var dfloc = DaggerfallUnity.Instance.ContentReader.MapFileReader.GetLocation(n, i);
                                var mapPixel = MapsFile.LongitudeLatitudeToMapPixel(location.Longitude, location.Latitude);
                                var str = $"{regionData.Name}\t{n}\t{i}\t{dfloc.Name}\t{location.MapId}";
                                str += $"\t{dfloc.MapTableData.Key}\t{location.LocationType.ToString()}\t{location.DungeonType.ToString()}";
                                str += $"\t{location.Latitude}\t{location.Longitude}\t{mapPixel.X}\t{mapPixel.Y}";
                                writer.WriteLine($"{str}");
                            }
                        }
                    }

                    return $"Location database successfully written to {filePath}.";
                }
                catch (Exception e)
                {
                    return $"Error writing location database: {e.Message}";
                }
            }
        }
        private static class TransformDungPos
        {
            public static readonly string name = "transform_dung_pos";
            public static readonly string description = "Randomly Transform Position Of Player Inside Dungeon";
            public static readonly string usage = "Randomly Transform Player Position In Dungeon";

            public static string Execute(params string[] args)
            {
                var randomStartingDungeon = RandomStartingDungeon.Instance;
                if (randomStartingDungeon == null)
                    return noInstanceMessage;

                bool successCheck = RandomStartingDungeon.TransformPlayerPosition();

                if (successCheck)
                    return "Transforming Player Dungeon Position...";
                else
                    return "Transformation Failed, Could Not Find Valid Dungeon Position.";
            }
        }

        private static class CurrentBlockInfo
        {
            public static readonly string name = "current_block_info";
            public static readonly string description = "Display The Block Info Of Block Player Is Currently Standing In";
            public static readonly string usage = "Display The Current Block Info";

            public static string Execute(params string[] args)
            {
                var randomStartingDungeon = RandomStartingDungeon.Instance;
                if (randomStartingDungeon == null)
                    return noInstanceMessage;

                RandomStartingDungeon.FindCurrentBlockInfo();

                return "Transforming Player Dungeon Position...";
            }
        }
    }
}
