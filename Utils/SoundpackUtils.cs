using System.IO;
using UnityEngine;
using Server.Shared.State;
using Services;
using SML;
using System.Collections.Generic;
using Server.Shared.Extensions;

namespace Utils
{
    public class SoundpackUtils
    {
        public static bool draw = false;
        public static bool loop = false;
        public static bool isRapid = false;
        public static string gameVelocity = "";
        public static string loopString = "";
        public static bool targetOnStand = false; // when you are exe.
        public static bool playerOnStand = false; //when you have been voted.
        public static bool prosecutor = false; //if a prosecutor is prosecuting someone.
        public static int horsemen = 0; //amount of horsemen transformed atm.
        public static bool dayOne = false; // if it is day one.
        public static string directoryPath;
        public static List<string> directories = new();
        public static string GetCustomSound(string ogSoundPath)
        {
            string[] ogSoundPathNames = ogSoundPath.Split('/');
            if (ogSoundPathNames[1] != "Music" && ogSoundPathNames[2] != "cinematicsfx" && ModSettings.GetBool("Only Allow Custom Music"))
                return ogSoundPath;
            if (!ModSettings.GetBool("Deactivate Custom Triggers"))
            {
                if (Pepper.IsGamePhasePlay()) 
                {
                    if(loop){ //needs test
                        return loopString;
                    }
                    RoleData roleData = RoleExtensions.GetRoleData(Pepper.GetMyRole());
                    string roleFolderName = roleData.roleName;
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
                        string pathToFirstDay = Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), roleFolderName, "Music", "DayOne");
                        string customSoundPath = FindCustomSound(pathToFirstDay);
                        if (!string.IsNullOrEmpty(customSoundPath))
                            return customSoundPath;
                        pathToFirstDay = Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), "Music", "DayOne");
                        customSoundPath = FindCustomSound(pathToFirstDay);
                        if (!string.IsNullOrEmpty(customSoundPath))
                            return customSoundPath;
                    }
                    if (ogSoundPathNames[2] == "Judgement")
                    {
                        if (prosecutor) //test
                        {
                            if (Pepper.GetMyRole() == Role.EXECUTIONER && targetOnStand){
                                string pathToTargetPros = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), "Executioner")).Replace(ogSoundPathNames[2], "Target" + ogSoundPathNames[2] + "Prosecutor");
                                string customSoundPath = FindCustomSound(pathToTargetPros);
                                if (!string.IsNullOrEmpty(customSoundPath))
                                    return customSoundPath;
                            } else if (playerOnStand){
                                if (horsemen > 0)
                                {
                                string pathToHorsemen = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), "Horsemen")).Replace(ogSoundPathNames[2], "Player" + ogSoundPathNames[2] + "Prosecutor");
                                string customHorsemenSoundPath = FindCustomSound(pathToHorsemen);
                                if (!string.IsNullOrEmpty(customHorsemenSoundPath))
                                    return customHorsemenSoundPath;
                                }
                                string pathToTargetPros = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), roleFolderName)).Replace(ogSoundPathNames[2], "Player" + ogSoundPathNames[2] + "Prosecutor");
                                string customSoundPath = FindCustomSound(pathToTargetPros);
                                if (!string.IsNullOrEmpty(customSoundPath))
                                    return customSoundPath;
                                pathToTargetPros = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"))).Replace(ogSoundPathNames[2], "Player" + ogSoundPathNames[2] + "Prosecutor");
                                customSoundPath = FindCustomSound(pathToTargetPros);
                                if (!string.IsNullOrEmpty(customSoundPath))
                                    return customSoundPath;
                            }
                            if (horsemen > 0)
                            {
                                string pathToHorsemen = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), "Horsemen")).Replace(ogSoundPathNames[2], ogSoundPathNames[2] + "Prosecutor");
                                string customSoundPath = FindCustomSound(pathToHorsemen);
                                if (!string.IsNullOrEmpty(customSoundPath))
                                    return customSoundPath;
                            }
                            string pathToPros = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), roleFolderName)).Replace(ogSoundPathNames[2], ogSoundPathNames[2] + "Prosecutor");
                            string customProsSoundPath = FindCustomSound(pathToPros);
                            if (!string.IsNullOrEmpty(customProsSoundPath))
                                return customProsSoundPath;
                            pathToPros = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"))).Replace(ogSoundPathNames[2], ogSoundPathNames[2] + "Prosecutor");
                            customProsSoundPath = FindCustomSound(pathToPros);
                            if (!string.IsNullOrEmpty(customProsSoundPath))
                                return customProsSoundPath;
                        }
                        if (Pepper.GetMyRole() == Role.EXECUTIONER && targetOnStand)
                        {
                            //test
                            string pathToTarget = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), "Executioner")).Replace(ogSoundPathNames[2], "Target" + ogSoundPathNames[2]);
                            string customTargetSoundPath = FindCustomSound(pathToTarget);
                            if (!string.IsNullOrEmpty(customTargetSoundPath))
                                return customTargetSoundPath;

                        }
                        else if (playerOnStand)
                        {
                            
                            //test
                            if (horsemen > 0)
                            {
                                string pathToHorsemen = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), "Horsemen")).Replace(ogSoundPathNames[2], "Player"+ogSoundPathNames[2]);
                                string customSoundPath = FindCustomSound(pathToHorsemen);
                                if (!string.IsNullOrEmpty(customSoundPath))
                                    return customSoundPath;
                            }
                            string pathToTarget = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), roleFolderName)).Replace(ogSoundPathNames[2], "Player" + ogSoundPathNames[2]);
                            string customTargetSoundPath = FindCustomSound(pathToTarget);
                            if (!string.IsNullOrEmpty(customTargetSoundPath))
                                return customTargetSoundPath;
                            pathToTarget = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"))).Replace(ogSoundPathNames[2], "Player" + ogSoundPathNames[2]);
                            customTargetSoundPath = FindCustomSound(pathToTarget);
                            if (!string.IsNullOrEmpty(customTargetSoundPath))
                                return customTargetSoundPath;
                        }
                        
                        if (horsemen > 0)
                        {
                            string pathToHorsemen = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), "Horsemen"));
                            string customSoundPath = FindCustomSound(pathToHorsemen);
                            if (!string.IsNullOrEmpty(customSoundPath))
                                return customSoundPath;
                        }
                        string pathToSound = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), roleFolderName));
                        string SoundPath = FindCustomSound(pathToSound);
                        if (!string.IsNullOrEmpty(SoundPath))
                            return SoundPath;
                        pathToSound = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack")));
                        SoundPath = FindCustomSound(pathToSound);
                        if (!string.IsNullOrEmpty(SoundPath))
                            return SoundPath;
                    }
                    if (isRapid)
                    {
                        if (ModSettings.GetBool("Looping Rapid Mode") && ogSoundPathNames[1] == "Music") //needs test
                        {
                            if (horsemen > 0)
                            {
                                string pathToHorsemenVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), "Horsemen")).Replace(ogSoundPathNames[2], "RapidModeLooping");
                                string customHorsemenVelocitySoundsPath = FindCustomSound(pathToHorsemenVelocitySounds);
                                if (!string.IsNullOrEmpty(customHorsemenVelocitySoundsPath))
                                {
                                    loop = true;
                                    loopString = customHorsemenVelocitySoundsPath;
                                    return customHorsemenVelocitySoundsPath;
                                    
                                }
                            }
                            string pathToCustomRapidSounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), roleFolderName)).Replace(ogSoundPathNames[2], "RapidModeLooping");
                            string customRapidSoundsPath = FindCustomSound(pathToCustomRapidSounds);
                            if (!string.IsNullOrEmpty(customRapidSoundsPath))
                            {
                                loop = true;
                                loopString = customRapidSoundsPath;
                                return customRapidSoundsPath;
                            }
                            pathToCustomRapidSounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"))).Replace(ogSoundPathNames[2], "RapidModeLooping");
                            customRapidSoundsPath = FindCustomSound(pathToCustomRapidSounds);
                            if (!string.IsNullOrEmpty(customRapidSoundsPath))
                            {
                                loop = true;
                                loopString = customRapidSoundsPath;
                                return customRapidSoundsPath;
                            }
                        }

                        if (horsemen > 0)
                        {
                            string pathToHorsemenVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), "Horsemen")).Replace(ogSoundPathNames[2], "RapidMode" + ogSoundPathNames[2]);
                            string customHorsemenVelocitySoundsPath = FindCustomSound(pathToHorsemenVelocitySounds);
                            if (!string.IsNullOrEmpty(customHorsemenVelocitySoundsPath))
                            {
                                return customHorsemenVelocitySoundsPath;
                            }
                        }
                        string pathToCustomVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), roleFolderName)).Replace(ogSoundPathNames[2], "RapidMode" + ogSoundPathNames[2]);
                        string customVelocitySoundsPath = FindCustomSound(pathToCustomVelocitySounds);
                        if (!string.IsNullOrEmpty(customVelocitySoundsPath)) return customVelocitySoundsPath;
                        pathToCustomVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"))).Replace(ogSoundPathNames[2], "RapidMode" + ogSoundPathNames[2]);
                        customVelocitySoundsPath = FindCustomSound(pathToCustomVelocitySounds);
                        if (!string.IsNullOrEmpty(customVelocitySoundsPath)) return customVelocitySoundsPath;
                    }
                    else if (!string.IsNullOrEmpty(gameVelocity)) { 
                        if (horsemen > 0)
                        {
                            string pathToHorsemenVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), "Horsemen")).Replace(ogSoundPathNames[2], gameVelocity + ogSoundPathNames[2]);
                            string customHorsemenVelocitySoundsPath = FindCustomSound(pathToHorsemenVelocitySounds);
                            if (!string.IsNullOrEmpty(customHorsemenVelocitySoundsPath))
                            {
                                return customHorsemenVelocitySoundsPath;
                            }
                        }
                        string pathToCustomVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), roleFolderName)).Replace(ogSoundPathNames[2], gameVelocity + ogSoundPathNames[2]);
                        string customVelocitySoundsPath = FindCustomSound(pathToCustomVelocitySounds);
                        if (!string.IsNullOrEmpty(customVelocitySoundsPath)) return customVelocitySoundsPath;
                        pathToCustomVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"))).Replace(ogSoundPathNames[2], gameVelocity + ogSoundPathNames[2]);
                        customVelocitySoundsPath = FindCustomSound(pathToCustomVelocitySounds);
                        if (!string.IsNullOrEmpty(customVelocitySoundsPath)) return customVelocitySoundsPath;
                    }
                    if (horsemen > 0)
                    {

                        string pathToHorsemen = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), "Horsemen"));
                        string customSoundPath = FindCustomSound(pathToHorsemen);
                        if (!string.IsNullOrEmpty(customSoundPath))
                            return customSoundPath;
                    }
                    string pathToRoleSounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), roleFolderName));
                    string customRoleSoundPath = FindCustomSound(pathToRoleSounds);
                    if (!string.IsNullOrEmpty(customRoleSoundPath)) return customRoleSoundPath;
                }
                if (draw && ogSoundPathNames[2] == "CovenVictory") //test
                {
                    draw = false;
                    string pathToCustomDrawMusic = Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), "Music", "DrawMusic");
                    string customDrawMusicPath = FindCustomSound(pathToCustomDrawMusic);
                    if (!string.IsNullOrEmpty(customDrawMusicPath)) return customDrawMusicPath;
                }
            }

            string pathToCustomSounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack")));
            string customPath = FindCustomSound(pathToCustomSounds);
            if (!string.IsNullOrEmpty(customPath)) return customPath; else return ogSoundPath;
        }
        static string FindCustomSound(string soundPath) //are you happy now curtis?
        {
            if (string.IsNullOrEmpty(soundPath)) return null;
            string[] extensions = { ".ogg", ".wav", ".mp3" };
            foreach (string extension in extensions)
            {
                string fullPath = soundPath + extension;
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }

            }
            return null;
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
            if(Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix){
                System.Diagnostics.Process.Start("open", "\"" + text + "\""); //code stolen from tuba
            }else {
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
                default:
                    return AudioType.UNKNOWN;
            }
        }

    }

}