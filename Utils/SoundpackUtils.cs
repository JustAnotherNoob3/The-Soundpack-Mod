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
            if (ogSoundPathNames[1] != "Music" && ogSoundPathNames[1] != "Sfx" && ModSettings.GetBool("Only Allow Custom Music"))
                return ogSoundPath;
                Debug.Log(directoryPath);
                Debug.Log(ogSoundPath);
                Debug.Log(ModSettings.GetString("Selected Soundpack"));
            if (Pepper.IsGamePhasePlay() && !ModSettings.GetBool("Deactivate Custom Triggers"))
            {
                if (Pepper.GetMyRole() == Role.EXECUTIONER) Debug.LogError("I am executioner. My target is: " + Service.Game.Sim.info.executionerTargetObservation.Data.targetPosition);
                Debug.Log(directoryPath);
                Debug.Log(ogSoundPath);
                Debug.Log(ModSettings.GetString("Selected Soundpack"));
                string roleFolderName = RoleExtensions.ToDisplayString(Pepper.GetMyRole());
                Debug.Log(roleFolderName);
                if (dayOne && ogSoundPathNames[2] == "DiscussionMusic")
                {
                    Debug.LogWarning("It is day one");
                    dayOne = false;
                    string pathToFirstDay = Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), roleFolderName, "Music", "DayOne");
                    string customSoundPath = FindCustomSound(pathToFirstDay);
                    if (!string.IsNullOrEmpty(customSoundPath))
                        return customSoundPath;
                    pathToFirstDay = Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), "Music", "DayOne");
                    customSoundPath = FindCustomSound(pathToFirstDay);
                    if (!string.IsNullOrEmpty(customSoundPath))
                        return customSoundPath;
                    Debug.Log(pathToFirstDay + " was not found.");
                }
                else
                {
                    Debug.LogWarning("not day 1");
                }
                if (ogSoundPathNames[2] == "Judgement")
                {
                    if (Pepper.GetMyRole() == Role.EXECUTIONER && targetOnStand)
                    {
                        if (prosecutor)
                        {
                            string pathToTargetPros = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), "Executioner")).Replace(ogSoundPathNames[2], "Target" + ogSoundPathNames[2] + "Prosecutor");
                            string customSoundPath = FindCustomSound(pathToTargetPros);
                            if (!string.IsNullOrEmpty(customSoundPath))
                                return customSoundPath;
                        }
                        string pathToTarget = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), "Executioner")).Replace(ogSoundPathNames[2], "Target" + ogSoundPathNames[2]);
                        string customTargetSoundPath = FindCustomSound(pathToTarget);
                        if (!string.IsNullOrEmpty(customTargetSoundPath))
                            return customTargetSoundPath;

                    }
                    else if (playerOnStand)
                    {
                        if (prosecutor)
                        {
                            string pathToTargetPros = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), roleFolderName)).Replace(ogSoundPathNames[2], "Player" + ogSoundPathNames[2] + "Prosecutor");
                            string customSoundPath = FindCustomSound(pathToTargetPros);
                            if (!string.IsNullOrEmpty(customSoundPath))
                                return customSoundPath;
                            pathToTargetPros = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"))).Replace(ogSoundPathNames[2], "Player" + ogSoundPathNames[2] + "Prosecutor");
                            customSoundPath = FindCustomSound(pathToTargetPros);
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
                    if (prosecutor)
                    {
                        if (horsemen > 0)
                        {
                            string pathToHorsemen = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), "Horsemen")).Replace(ogSoundPathNames[2], ogSoundPathNames[2]+"Prosecutor"); 
                            string customSoundPath = FindCustomSound(pathToHorsemen);
                            if (!string.IsNullOrEmpty(customSoundPath))
                                return customSoundPath;
                        }
                        string pathToPros = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), roleFolderName)).Replace(ogSoundPathNames[2], ogSoundPathNames[2]+"Prosecutor"); 
                        string customProsSoundPath = FindCustomSound(pathToPros);
                        if (!string.IsNullOrEmpty(customProsSoundPath))
                            return customProsSoundPath;
                        pathToPros = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"))).Replace(ogSoundPathNames[2], ogSoundPathNames[2]+"Prosecutor"); 
                        customProsSoundPath = FindCustomSound(pathToPros);
                        if (!string.IsNullOrEmpty(customProsSoundPath))
                            return customProsSoundPath;
                    }
                    if (horsemen > 0)
                    {
                        string pathToHorsemen = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), "Horsemen")); 
                        string customSoundPath = FindCustomSound(pathToHorsemen);
                        if (!string.IsNullOrEmpty(customSoundPath))
                            return customSoundPath;
                    }
                    string pathToSound = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"),roleFolderName)); 
                    string SoundPath = FindCustomSound(pathToSound);
                    if (!string.IsNullOrEmpty(SoundPath))
                        return SoundPath;
                    pathToSound = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"))); 
                    SoundPath = FindCustomSound(pathToSound);
                    if (!string.IsNullOrEmpty(SoundPath))
                        return SoundPath;
                }
                if (horsemen > 0)
                {
                    string pathToHorsemen = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), "Horsemen")); 
                    string customSoundPath = FindCustomSound(pathToHorsemen);
                    if (!string.IsNullOrEmpty(customSoundPath))
                        return customSoundPath;
                }
                if (isRapid)
                {
                    if (ModSettings.GetBool("Looping Rapid Mode"))
                    {
                        loop = true;
                    }
                    else
                    {
                        string pathToCustomVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), roleFolderName)).Replace(ogSoundPathNames[2], "RapidMode" + ogSoundPathNames[2]);
                        string customVelocitySoundsPath = FindCustomSound(pathToCustomVelocitySounds);
                        if (!string.IsNullOrEmpty(customVelocitySoundsPath)) return customVelocitySoundsPath;
                         pathToCustomVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"))).Replace(ogSoundPathNames[2], "RapidMode" + ogSoundPathNames[2]);
                         customVelocitySoundsPath = FindCustomSound(pathToCustomVelocitySounds);
                        if (!string.IsNullOrEmpty(customVelocitySoundsPath)) return customVelocitySoundsPath;
                    }
                }
                else if (!string.IsNullOrEmpty(gameVelocity)) { }
                else
                {
                    string pathToCustomVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), roleFolderName)).Replace(ogSoundPathNames[2], gameVelocity + ogSoundPathNames[2]);
                    string customVelocitySoundsPath = FindCustomSound(pathToCustomVelocitySounds);
                    if (!string.IsNullOrEmpty(customVelocitySoundsPath)) return customVelocitySoundsPath;
                     pathToCustomVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"))).Replace(ogSoundPathNames[2], gameVelocity + ogSoundPathNames[2]);
                     customVelocitySoundsPath = FindCustomSound(pathToCustomVelocitySounds);
                    if (!string.IsNullOrEmpty(customVelocitySoundsPath)) return customVelocitySoundsPath;
                }
                string pathToRoleSounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), roleFolderName));
                string customRoleSoundPath = FindCustomSound(pathToRoleSounds);
                if (!string.IsNullOrEmpty(customRoleSoundPath)) return customRoleSoundPath;
            }
            else if (draw)
            {
                draw = false;
                string pathToCustomDrawMusic = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack"), "Music", "DrawCinematic"));
                string customDrawMusicPath = FindCustomSound(pathToCustomDrawMusic);
                if (!string.IsNullOrEmpty(customDrawMusicPath)) return customDrawMusicPath;
            }
            else
            {

                Debug.LogWarning("Not in-game");
            }

            string pathToCustomSounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, ModSettings.GetString("Selected Soundpack")));
            string customPath = FindCustomSound(pathToCustomSounds);
            if (!string.IsNullOrEmpty(customPath)) return customPath; else return ogSoundPath;
        }
        static string FindCustomSound(string soundPath) //are you happy now curtis?
        {
            if (!string.IsNullOrEmpty(soundPath)) return null;
            string[] extensions = { ".wav", ".mp3", ".ogg", ".aiff", ".mod" };
            foreach (string extension in extensions)
            {
                string fullPath = soundPath + extension;
                if (File.Exists(fullPath))
                    return fullPath;
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
                string[] dirs = dir.Split('/');
                directories.Add(dirs[dirs.Length]);
            }
        }
        public static void OpenSoundpackDirectory()
        {
            string text = Path.Combine(Directory.GetCurrentDirectory(), "SalemModLoader", "ModFolders", "Soundpacks");
            Application.OpenURL("file://" + text);
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
                    Debug.LogWarning("AudioUtility: La extensión de archivo " + extension + " no es compatible.");
                    return AudioType.UNKNOWN;
            }
        }

    }

}