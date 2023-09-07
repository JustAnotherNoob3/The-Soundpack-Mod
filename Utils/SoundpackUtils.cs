using System.Net;
using System.IO;
using UnityEngine;
using Server.Shared.State;
using Services;
using SML;
using System.Collections.Generic;
using Server.Shared.Extensions;
using System;

namespace Utils
{
    public class SoundpackUtils
    {
        public static bool draw = false;
        public static bool win = false;
        public static bool loop = false;
        public static bool isRapid = false;
        public static string gameVelocity = "";
        public static string loopString = "";
        public static bool targetOnStand = false; // when you are exe.
        public static bool playerOnStand = false; //when you have been voted.
        public static bool prosecutor = false; //if a prosecutor is prosecuting someone.
        public static List<Role> horsemen = new(); //amount of horsemen transformed atm. for some reason it is multiplied by 2 :/
        public static bool dayOne = false; // if it is day one.
        public static string directoryPath;
        public static List<string> directories = new();
        public static string GetCustomSound(string ogSoundPath)
        {
            string[] ogSoundPathNames = ogSoundPath.Split('/');
            string soundpack = ModSettings.GetString("Selected Soundpack", "JAN.soundpacks");
            if (!Directory.Exists(Path.Combine(directoryPath, soundpack, ogSoundPathNames[1])))
            {
                return ogSoundPath;
            }
            if (!ModSettings.GetBool("Deactivate Custom Triggers", "JAN.soundpacks") && (ModSettings.GetBool("Allow Custom Triggers SFX", "JAN.soundpacks") || (ogSoundPathNames[1] != "Steps" && ogSoundPathNames[1] != "UI")))
            {
                if (Pepper.IsGamePhasePlay())
                {
                    RoleData roleData = RoleExtensions.GetRoleData(Pepper.GetMyRole());
                    string roleFolderName = roleData.roleName;
                    if (ogSoundPathNames[1] == "Music")
                    {
                        if (loop)
                        {
                            return loopString;
                        }

                        if (dayOne && ogSoundPathNames[2] == "DiscussionMusic")
                        {
                            List<Role> modifiers = Service.Game.Sim.simulation.roleDeckBuilder.Data.modifierCards;
                            if (modifiers.Contains(Role.FAST_MODE))
                            {
                                
                                gameVelocity = "FastMode";
                            }
                            else if (modifiers.Contains(Role.SLOW_MODE))
                            {
                                
                                gameVelocity = "SlowMode";
                            }
                            else
                            {
                                
                                gameVelocity = "";
                            }
                            
                            dayOne = false;
                            string pathToFirstDay = Path.Combine(directoryPath, soundpack, roleFolderName, "Music", "DayOne");
                            string customSoundPath = FindCustomSound(pathToFirstDay);
                            if (!string.IsNullOrEmpty(customSoundPath))
                                return customSoundPath;
                            pathToFirstDay = Path.Combine(directoryPath, soundpack, "Music", "DayOne");
                            customSoundPath = FindCustomSound(pathToFirstDay);
                            if (!string.IsNullOrEmpty(customSoundPath))
                                return customSoundPath;
                            
                        }

                        if (ogSoundPathNames[2] == "Judgement")
                        {
                            if (prosecutor) //test
                            {
                                if (Pepper.GetMyRole() == Role.EXECUTIONER && targetOnStand)
                                {
                                    string pathToTargetPros = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "Executioner")).Replace(ogSoundPathNames[2], "Target" + ogSoundPathNames[2] + "Prosecutor");
                                    string customSoundPath = FindCustomSound(pathToTargetPros);
                                    if (!string.IsNullOrEmpty(customSoundPath))
                                        return customSoundPath;
                                }
                                else if (playerOnStand)
                                {
                                    if (horsemen.Count > 0)
                                    {
                                        if (Service.Game.Sim.info.roleCardObservation.Data.defense == 3 && roleData.factionType == FactionType.APOCALYPSE)
                                        {
                                            string pathToMeHorsemenVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, roleFolderName)).Replace(ogSoundPathNames[2], "Player" + ogSoundPathNames[2] + "Prosecutor");
                                            string customMeHorsemenVelocitySoundsPath = FindCustomSound(pathToMeHorsemenVelocitySounds);
                                            if (!string.IsNullOrEmpty(customMeHorsemenVelocitySoundsPath))
                                            {
                                                return customMeHorsemenVelocitySoundsPath;
                                            }
                                        }
                                        string pathToHorsemen = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "Horsemen")).Replace(ogSoundPathNames[2], "Player" + ogSoundPathNames[2] + "Prosecutor");
                                        string customHorsemenSoundPath = FindCustomSound(pathToHorsemen);
                                        if (!string.IsNullOrEmpty(customHorsemenSoundPath))
                                            return customHorsemenSoundPath;
                                    }
                                    string pathToTargetPros = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, roleFolderName)).Replace(ogSoundPathNames[2], "Player" + ogSoundPathNames[2] + "Prosecutor");
                                    string customSoundPath = FindCustomSound(pathToTargetPros);
                                    if (!string.IsNullOrEmpty(customSoundPath))
                                        return customSoundPath;
                                    pathToTargetPros = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack)).Replace(ogSoundPathNames[2], "Player" + ogSoundPathNames[2] + "Prosecutor");
                                    customSoundPath = FindCustomSound(pathToTargetPros);
                                    if (!string.IsNullOrEmpty(customSoundPath))
                                        return customSoundPath;
                                }
                                if (horsemen.Count > 0)
                                {
                                    if (Service.Game.Sim.info.roleCardObservation.Data.defense == 3 && roleData.factionType == FactionType.APOCALYPSE)
                                    {
                                        string pathToMeHorsemenVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, roleFolderName)).Replace(ogSoundPathNames[2], ogSoundPathNames[2] + "Prosecutor");
                                        string customMeHorsemenVelocitySoundsPath = FindCustomSound(pathToMeHorsemenVelocitySounds);
                                        if (!string.IsNullOrEmpty(customMeHorsemenVelocitySoundsPath))
                                        {
                                            return customMeHorsemenVelocitySoundsPath;
                                        }
                                    }
                                    string pathToHorsemen = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "Horsemen")).Replace(ogSoundPathNames[2], ogSoundPathNames[2] + "Prosecutor");
                                    string customSoundPath = FindCustomSound(pathToHorsemen);
                                    if (!string.IsNullOrEmpty(customSoundPath))
                                        return customSoundPath;
                                }
                                string pathToPros = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, roleFolderName)).Replace(ogSoundPathNames[2], ogSoundPathNames[2] + "Prosecutor");
                                string customProsSoundPath = FindCustomSound(pathToPros);
                                if (!string.IsNullOrEmpty(customProsSoundPath))
                                    return customProsSoundPath;
                                pathToPros = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack)).Replace(ogSoundPathNames[2], ogSoundPathNames[2] + "Prosecutor");
                                customProsSoundPath = FindCustomSound(pathToPros);
                                if (!string.IsNullOrEmpty(customProsSoundPath))
                                    return customProsSoundPath;
                            }
                            if (Pepper.GetMyRole() == Role.EXECUTIONER && targetOnStand)
                            {
                                //test
                                string pathToTarget = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "Executioner")).Replace(ogSoundPathNames[2], "Target" + ogSoundPathNames[2]);
                                string customTargetSoundPath = FindCustomSound(pathToTarget);
                                if (!string.IsNullOrEmpty(customTargetSoundPath))
                                    return customTargetSoundPath;

                            }
                            else if (playerOnStand)
                            {
                                if (horsemen.Count > 0)
                                {
                                    if (Service.Game.Sim.info.roleCardObservation.Data.defense == 3 && roleData.factionType == FactionType.APOCALYPSE)
                                    {
                                        string pathToMeHorsemenVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, roleFolderName)).Replace(ogSoundPathNames[2], "Player" + ogSoundPathNames[2]);
                                        string customMeHorsemenVelocitySoundsPath = FindCustomSound(pathToMeHorsemenVelocitySounds);
                                        if (!string.IsNullOrEmpty(customMeHorsemenVelocitySoundsPath))
                                        {
                                            return customMeHorsemenVelocitySoundsPath;
                                        }
                                    }
                                    string pathToHorsemen = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "Horsemen")).Replace(ogSoundPathNames[2], "Player" + ogSoundPathNames[2]);
                                    string customSoundPath = FindCustomSound(pathToHorsemen);
                                    if (!string.IsNullOrEmpty(customSoundPath))
                                        return customSoundPath;
                                }
                                string pathToTarget = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, roleFolderName)).Replace(ogSoundPathNames[2], "Player" + ogSoundPathNames[2]);
                                string customTargetSoundPath = FindCustomSound(pathToTarget);
                                if (!string.IsNullOrEmpty(customTargetSoundPath))
                                    return customTargetSoundPath;
                                pathToTarget = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack)).Replace(ogSoundPathNames[2], "Player" + ogSoundPathNames[2]);
                                customTargetSoundPath = FindCustomSound(pathToTarget);
                                if (!string.IsNullOrEmpty(customTargetSoundPath))
                                    return customTargetSoundPath;
                            }

                            if (horsemen.Count > 0)
                            {
                                if (Service.Game.Sim.info.roleCardObservation.Data.defense == 3 && roleData.factionType == FactionType.APOCALYPSE)
                                {
                                    string pathToMeHorsemenVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, roleFolderName));
                                    string customMeHorsemenVelocitySoundsPath = FindCustomSound(pathToMeHorsemenVelocitySounds);
                                    if (!string.IsNullOrEmpty(customMeHorsemenVelocitySoundsPath))
                                    {
                                        return customMeHorsemenVelocitySoundsPath;
                                    }
                                }
                                string pathToHorsemen = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "Horsemen"));
                                string customSoundPath = FindCustomSound(pathToHorsemen);
                                if (!string.IsNullOrEmpty(customSoundPath))
                                    return customSoundPath;
                            }
                            string pathToSound = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, roleFolderName));
                            string SoundPath = FindCustomSound(pathToSound);
                            if (!string.IsNullOrEmpty(SoundPath))
                                return SoundPath;
                            pathToSound = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack));
                            SoundPath = FindCustomSound(pathToSound);
                            if (!string.IsNullOrEmpty(SoundPath))
                                return SoundPath;
                        }
                        if (isRapid)
                        {
                            if (ModSettings.GetBool("Looping Rapid Mode")) //needs test
                            {
                                if (horsemen.Count > 0)
                                {
                                    if (Service.Game.Sim.info.roleCardObservation.Data.defense == 3 && roleData.factionType == FactionType.APOCALYPSE)
                                    {
                                        string pathToMeHorsemenVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, roleFolderName)).Replace(ogSoundPathNames[2], "RapidModeLooping");
                                        string customMeHorsemenVelocitySoundsPath = FindCustomSound(pathToMeHorsemenVelocitySounds);
                                        if (!string.IsNullOrEmpty(customMeHorsemenVelocitySoundsPath))
                                        {
                                            loop = true;
                                            loopString = customMeHorsemenVelocitySoundsPath;
                                            return customMeHorsemenVelocitySoundsPath;

                                        }
                                    }
                                    string pathToHorsemenVelocitySounds = Path.Combine(directoryPath, soundpack, "Horsemen", "Music", "RapidModeLooping");
                                    string customHorsemenVelocitySoundsPath = FindCustomSound(pathToHorsemenVelocitySounds);
                                    if (!string.IsNullOrEmpty(customHorsemenVelocitySoundsPath))
                                    {
                                        loop = true;
                                        loopString = customHorsemenVelocitySoundsPath;
                                        return customHorsemenVelocitySoundsPath;

                                    }
                                }
                                string pathToCustomRapidSounds = Path.Combine(directoryPath, soundpack, roleFolderName, "Music", "RapidModeLooping");
                                string customRapidSoundsPath = FindCustomSound(pathToCustomRapidSounds);
                                if (!string.IsNullOrEmpty(customRapidSoundsPath))
                                {
                                    loop = true;
                                    loopString = customRapidSoundsPath;
                                    return customRapidSoundsPath;
                                }
                                pathToCustomRapidSounds = Path.Combine(directoryPath, soundpack, "Music", "RapidModeLooping");
                                customRapidSoundsPath = FindCustomSound(pathToCustomRapidSounds);
                                if (!string.IsNullOrEmpty(customRapidSoundsPath))
                                {
                                    loop = true;
                                    loopString = customRapidSoundsPath;
                                    return customRapidSoundsPath;
                                }
                            }

                            if (horsemen.Count > 0)
                            {
                                if (Service.Game.Sim.info.roleCardObservation.Data.defense == 3 && roleData.factionType == FactionType.APOCALYPSE)
                                {
                                    string pathToMeHorsemenVelocitySounds = Path.Combine(directoryPath, soundpack, roleFolderName, "Music", "RapidMode" + ogSoundPathNames[2]);
                                    string customMeHorsemenVelocitySoundsPath = FindCustomSound(pathToMeHorsemenVelocitySounds);
                                    if (!string.IsNullOrEmpty(customMeHorsemenVelocitySoundsPath))
                                    {
                                        return customMeHorsemenVelocitySoundsPath;
                                    }
                                }
                                string pathToHorsemenVelocitySounds = Path.Combine(directoryPath, soundpack, "Horsemen", "Music", "RapidMode" + ogSoundPathNames[2]);
                                string customHorsemenVelocitySoundsPath = FindCustomSound(pathToHorsemenVelocitySounds);
                                if (!string.IsNullOrEmpty(customHorsemenVelocitySoundsPath))
                                {
                                    return customHorsemenVelocitySoundsPath;
                                }
                            }
                            string pathToCustomVelocitySounds = Path.Combine(directoryPath, soundpack, roleFolderName, "Music", "RapidMode" + ogSoundPathNames[2]);
                            string customVelocitySoundsPath = FindCustomSound(pathToCustomVelocitySounds);
                            if (!string.IsNullOrEmpty(customVelocitySoundsPath)) return customVelocitySoundsPath;
                            pathToCustomVelocitySounds = Path.Combine(directoryPath, soundpack, "Music", "RapidMode" + ogSoundPathNames[2]);
                            customVelocitySoundsPath = FindCustomSound(pathToCustomVelocitySounds);
                            if (!string.IsNullOrEmpty(customVelocitySoundsPath)) return customVelocitySoundsPath;
                        }
                        else if (!string.IsNullOrEmpty(gameVelocity))
                        {
                            if (horsemen.Count > 0)
                            {
                                if (Service.Game.Sim.info.roleCardObservation.Data.defense == 3 && roleData.factionType == FactionType.APOCALYPSE)
                                {
                                    string pathToMeHorsemenVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, roleFolderName)).Replace(ogSoundPathNames[2], gameVelocity + ogSoundPathNames[2]);
                                    string customMeHorsemenVelocitySoundsPath = FindCustomSound(pathToMeHorsemenVelocitySounds);
                                    if (!string.IsNullOrEmpty(customMeHorsemenVelocitySoundsPath))
                                    {
                                        return customMeHorsemenVelocitySoundsPath;
                                    }
                                }
                                string pathToHorsemenVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "Horsemen")).Replace(ogSoundPathNames[2], gameVelocity + ogSoundPathNames[2]);
                                string customHorsemenVelocitySoundsPath = FindCustomSound(pathToHorsemenVelocitySounds);
                                if (!string.IsNullOrEmpty(customHorsemenVelocitySoundsPath))
                                {
                                    return customHorsemenVelocitySoundsPath;
                                }
                            }
                            string pathToCustomVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, roleFolderName)).Replace(ogSoundPathNames[2], gameVelocity + ogSoundPathNames[2]);
                            string customVelocitySoundsPath = FindCustomSound(pathToCustomVelocitySounds);
                            if (!string.IsNullOrEmpty(customVelocitySoundsPath)) return customVelocitySoundsPath;
                            pathToCustomVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack)).Replace(ogSoundPathNames[2], gameVelocity + ogSoundPathNames[2]);
                            customVelocitySoundsPath = FindCustomSound(pathToCustomVelocitySounds);
                            if (!string.IsNullOrEmpty(customVelocitySoundsPath)) return customVelocitySoundsPath;
                        }
                    }
                    if (horsemen.Count > 0)
                    {
                        if (Service.Game.Sim.info.roleCardObservation.Data.defense == 3 && roleData.factionType == FactionType.APOCALYPSE)
                        {
                            string pathToMeHorsemenVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, roleFolderName));
                            string customMeHorsemenVelocitySoundsPath = FindCustomSound(pathToMeHorsemenVelocitySounds);
                            if (!string.IsNullOrEmpty(customMeHorsemenVelocitySoundsPath))
                            {
                                return customMeHorsemenVelocitySoundsPath;
                            }
                        }
                        string pathToHorsemen = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "Horsemen"));
                        string customSoundPath = FindCustomSound(pathToHorsemen);
                        if (!string.IsNullOrEmpty(customSoundPath))
                            return customSoundPath;
                    }
                    string pathToRoleSounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, roleFolderName));
                    string customRoleSoundPath = FindCustomSound(pathToRoleSounds);
                    if (!string.IsNullOrEmpty(customRoleSoundPath)) return customRoleSoundPath;
                }
                if (draw && ogSoundPathNames[2] == "CovenVictory") //test
                {
                    draw = false;
                    string pathToCustomDrawMusic = Path.Combine(directoryPath, soundpack, "Music", "DrawMusic");
                    string customDrawMusicPath = FindCustomSound(pathToCustomDrawMusic);
                    if (!string.IsNullOrEmpty(customDrawMusicPath)) return customDrawMusicPath;
                }
                if (ogSoundPathNames[2] == "LoginMusicLoop_old")
                {
                    if (win)
                    {
                        string pathToCustomWin = Path.Combine(directoryPath, soundpack, "Music", "VictoryMusic");
                        string customWinMusicPath = FindCustomSound(pathToCustomWin);
                        if (!string.IsNullOrEmpty(customWinMusicPath)) return customWinMusicPath;
                    }
                    else
                    {
                        string pathToCustomWin = Path.Combine(directoryPath, soundpack, "Music", "DefeatMusic");
                        string customWinMusicPath = FindCustomSound(pathToCustomWin);
                        if (!string.IsNullOrEmpty(customWinMusicPath)) return customWinMusicPath;
                    }
                }
            }

            string pathToCustomSounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack));
            string customPath = FindCustomSound(pathToCustomSounds);
            if (!string.IsNullOrEmpty(customPath)) return customPath; else return ogSoundPath;
        }
        static string FindCustomSound(string soundPath) //are you happy now curtis?
        {
            if (string.IsNullOrEmpty(soundPath)) return null;
            string dir = Path.GetDirectoryName(soundPath);
            if (!Directory.Exists(dir)) return null;
            string[] files = Directory.GetFiles(Path.GetDirectoryName(soundPath), Path.GetFileName(soundPath) + ".*");
            if (files.Length < 1)
            {
                
                return null;
            }
            return files[0];
        }

        public static void ForSoundpackMod()
        {

            directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "SalemModLoader", "ModFolders", "Soundpacks");
            
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string[] fullDirectories = Directory.GetDirectories(directoryPath);
            foreach (string dir in fullDirectories)
            {
                directories.Add(Path.GetFileName(dir));
            }
        }
        public static void OpenSoundpackDirectory()
        {
            string text = Path.Combine(Directory.GetCurrentDirectory(), "SalemModLoader", "ModFolders", "Soundpacks");
            if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
            {
                System.Diagnostics.Process.Start("open", "\"" + text + "\""); //code stolen from tuba
            }
            else
            {
                Application.OpenURL("file://" + text);
            }

        }


        //this stuff i got from a 2d beat saber game i did like a year ago.

        public static AudioType GetAudioType(string extension)
        {
            extension = extension.ToLower();

            switch (extension)
            {
                case ".wav":
                    return AudioType.WAV;
                case ".mp3":
                    return AudioType.MPEG;
                case ".ogg":
                    return AudioType.OGGVORBIS;
                case ".aiff":
                return AudioType.AIFF;
                 case ".mod":
                return AudioType.MOD;
                default:
                    return AudioType.UNKNOWN;
            }
        }

    }

}