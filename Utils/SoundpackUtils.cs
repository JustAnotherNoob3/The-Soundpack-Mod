using System.Net;
using System.IO;
using UnityEngine;
using Server.Shared.State;
using Services;
using SML;
using System.Collections.Generic;
using Server.Shared.Extensions;
using System;
using System.Net.Sockets;
using System.Linq;
namespace Utils
{
    public class SoundpackUtils
    {
        static private string[] toCheck { get; } = {
            "UI",
            "Steps",
            "Sfx",
            "Music",
            "Cauldron"
        };
        public static List<string> subfolders = new();
        public static Dictionary<string, string> soundpacks = new();
        public static bool draw = false;
        public static bool win = false;
        public static bool loop = false;
        public static bool isRapid = false;
        public static bool isTT = false;
        public static string gameVelocity = "";
        public static string loopString = "";
        public static bool targetOnStand = false; // when you are exe.
        public static bool playerOnStand = false; //when you have been voted.
        public static bool prosecutor = false; //if a prosecutor is prosecuting someone.
        public static List<Role> horsemen = new();
        public static bool dayOne = false; // if it is day one.
        public static string directoryPath;
        public static string soundpack;
        public static string curExtension = null;
        public static string GetCustomSound(string ogSoundPath)
        {
            string[] ogSoundPathNames = ogSoundPath.Split('/');
            if (!Directory.Exists(Path.Combine(directoryPath, soundpack, ogSoundPathNames[1])))
            {
                return ogSoundPath;
            }
            if (!ModSettings.GetBool("Deactivate Custom Triggers", "JAN.soundpacks") && (ModSettings.GetBool("Allow Custom Triggers SFX", "JAN.soundpacks") || (ogSoundPathNames[1] != "Steps" && ogSoundPathNames[1] != "UI")))
            {
                if (Pepper.IsGamePhasePlay())
                {
                    RoleData roleData = RoleExtensions.GetRoleData(Pepper.GetMyRole());
                    string alignment = roleData.roleAlignment.ToString().ToTitleCase();
                    string subalignment = $"{alignment} {roleData.subAlignment.ToString().ToTitleCase()}";
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
                                gameVelocity += "FastMode";
                            }
                            else if (modifiers.Contains(Role.SLOW_MODE))
                            {
                                gameVelocity += "SlowMode";
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
                            pathToFirstDay = Path.Combine(directoryPath, soundpack, subalignment, "Music", "DayOne");
                            customSoundPath = FindCustomSound(pathToFirstDay);
                            if (!string.IsNullOrEmpty(customSoundPath))
                                return customSoundPath;
                            pathToFirstDay = Path.Combine(directoryPath, soundpack, alignment, "Music", "DayOne");
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
                                    if (isTT)
                                    {
                                        string pathToTT = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "Town Traitor")).Replace(ogSoundPathNames[2], "Player" + ogSoundPathNames[2] + "Prosecutor");
                                        string customTTSoundPath = FindCustomSound(pathToTT);
                                        if (!string.IsNullOrEmpty(customTTSoundPath))
                                            return customTTSoundPath;
                                    }
                                    if (horsemen.Count > 0)
                                    {
                                        if (Service.Game.Sim.info.roleCardObservation.Data.defense == 3 && roleData.factionType == FactionType.APOCALYPSE)
                                        {
                                            // could this switch have been done with a method instead? yeah but was more easy like this.
                                            switch (roleData.role)
                                            {
                                                case Role.SOULCOLLECTOR:
                                                    string pathToMeDeathVelocitySounds = Path.Combine(directoryPath, soundpack, "Death", "Music", "PlayerJudgementProsecutor");
                                                    string customMeDeathVelocitySoundsPath = FindCustomSound(pathToMeDeathVelocitySounds);
                                                    if (!string.IsNullOrEmpty(customMeDeathVelocitySoundsPath))
                                                    {
                                                        return customMeDeathVelocitySoundsPath;
                                                    }
                                                    break;
                                                case Role.BERSERKER:
                                                    string pathToMeBersVelocitySounds = Path.Combine(directoryPath, soundpack, "War", "Music", "PlayerJudgementProsecutor");
                                                    string customMeBersVelocitySoundsPath = FindCustomSound(pathToMeBersVelocitySounds);
                                                    if (!string.IsNullOrEmpty(customMeBersVelocitySoundsPath))
                                                    {
                                                        return customMeBersVelocitySoundsPath;
                                                    }
                                                    break;
                                                case Role.PLAGUEBEARER:
                                                    string pathToMePestVelocitySounds = Path.Combine(directoryPath, soundpack, "Pestilence", "Music", "PlayerJudgementProsecutor");
                                                    string customMePestVelocitySoundsPath = FindCustomSound(pathToMePestVelocitySounds);
                                                    if (!string.IsNullOrEmpty(customMePestVelocitySoundsPath))
                                                    {
                                                        return customMePestVelocitySoundsPath;
                                                    }
                                                    break;
                                                case Role.BAKER:
                                                    string pathToMeFamVelocitySounds = Path.Combine(directoryPath, soundpack, "Famine", "Music", "PlayerJudgementProsecutor");
                                                    string customMeFamVelocitySoundsPath = FindCustomSound(pathToMeFamVelocitySounds);
                                                    if (!string.IsNullOrEmpty(customMeFamVelocitySoundsPath))
                                                    {
                                                        return customMeFamVelocitySoundsPath;
                                                    }
                                                    break;
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
                                    pathToTargetPros = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, subalignment)).Replace(ogSoundPathNames[2], "Player" + ogSoundPathNames[2] + "Prosecutor");
                                    customSoundPath = FindCustomSound(pathToTargetPros);
                                    if (!string.IsNullOrEmpty(customSoundPath))
                                        return customSoundPath;
                                    pathToTargetPros = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, alignment)).Replace(ogSoundPathNames[2], "Player" + ogSoundPathNames[2] + "Prosecutor");
                                    customSoundPath = FindCustomSound(pathToTargetPros);
                                    if (!string.IsNullOrEmpty(customSoundPath))
                                        return customSoundPath;
                                    pathToTargetPros = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack)).Replace(ogSoundPathNames[2], "Player" + ogSoundPathNames[2] + "Prosecutor");
                                    customSoundPath = FindCustomSound(pathToTargetPros);
                                    if (!string.IsNullOrEmpty(customSoundPath))
                                        return customSoundPath;
                                }
                                if (isTT)
                                {
                                    string pathToTT = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "Town Traitor")).Replace(ogSoundPathNames[2], ogSoundPathNames[2] + "Prosecutor");
                                    string customTTSoundPath = FindCustomSound(pathToTT);
                                    if (!string.IsNullOrEmpty(customTTSoundPath))
                                        return customTTSoundPath;
                                }
                                if (horsemen.Count > 0)
                                {
                                    if (Service.Game.Sim.info.roleCardObservation.Data.defense == 3 && roleData.factionType == FactionType.APOCALYPSE)
                                    {
                                        switch (roleData.role)
                                        {
                                            case Role.SOULCOLLECTOR:
                                                string pathToMeDeathVelocitySounds = Path.Combine(directoryPath, soundpack, "Death", "Music", "JudgementProsecutor");
                                                string customMeDeathVelocitySoundsPath = FindCustomSound(pathToMeDeathVelocitySounds);
                                                if (!string.IsNullOrEmpty(customMeDeathVelocitySoundsPath))
                                                {
                                                    return customMeDeathVelocitySoundsPath;
                                                }
                                                break;
                                            case Role.BERSERKER:
                                                string pathToMeBersVelocitySounds = Path.Combine(directoryPath, soundpack, "War", "Music", "JudgementProsecutor");
                                                string customMeBersVelocitySoundsPath = FindCustomSound(pathToMeBersVelocitySounds);
                                                if (!string.IsNullOrEmpty(customMeBersVelocitySoundsPath))
                                                {
                                                    return customMeBersVelocitySoundsPath;
                                                }
                                                break;
                                            case Role.PLAGUEBEARER:
                                                string pathToMePestVelocitySounds = Path.Combine(directoryPath, soundpack, "Pestilence", "Music", "JudgementProsecutor");
                                                string customMePestVelocitySoundsPath = FindCustomSound(pathToMePestVelocitySounds);
                                                if (!string.IsNullOrEmpty(customMePestVelocitySoundsPath))
                                                {
                                                    return customMePestVelocitySoundsPath;
                                                }
                                                break;
                                            case Role.BAKER:
                                                string pathToMeFamVelocitySounds = Path.Combine(directoryPath, soundpack, "Famine", "Music", "JudgementProsecutor");
                                                string customMeFamVelocitySoundsPath = FindCustomSound(pathToMeFamVelocitySounds);
                                                if (!string.IsNullOrEmpty(customMeFamVelocitySoundsPath))
                                                {
                                                    return customMeFamVelocitySoundsPath;
                                                }
                                                break;
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
                                pathToPros = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, subalignment)).Replace(ogSoundPathNames[2], ogSoundPathNames[2] + "Prosecutor");
                                customProsSoundPath = FindCustomSound(pathToPros);
                                if (!string.IsNullOrEmpty(customProsSoundPath))
                                    return customProsSoundPath;
                                pathToPros = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, alignment)).Replace(ogSoundPathNames[2], ogSoundPathNames[2] + "Prosecutor");
                                customProsSoundPath = FindCustomSound(pathToPros);
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
                                if (isTT)
                                {
                                    string pathToTT = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "Town Traitor")).Replace(ogSoundPathNames[2], "Player" + ogSoundPathNames[2]);
                                    string customTTSoundPath = FindCustomSound(pathToTT);
                                    if (!string.IsNullOrEmpty(customTTSoundPath))
                                        return customTTSoundPath;
                                }
                                if (horsemen.Count > 0)
                                {
                                    if (Service.Game.Sim.info.roleCardObservation.Data.defense == 3 && roleData.factionType == FactionType.APOCALYPSE)
                                    {
                                        switch (roleData.role)
                                        {
                                            case Role.SOULCOLLECTOR:
                                                string pathToMeDeathVelocitySounds = Path.Combine(directoryPath, soundpack, "Death", "Music", "PlayerJudgement");
                                                string customMeDeathVelocitySoundsPath = FindCustomSound(pathToMeDeathVelocitySounds);
                                                if (!string.IsNullOrEmpty(customMeDeathVelocitySoundsPath))
                                                {
                                                    return customMeDeathVelocitySoundsPath;
                                                }
                                                break;
                                            case Role.BERSERKER:
                                                string pathToMeBersVelocitySounds = Path.Combine(directoryPath, soundpack, "War", "Music", "PlayerJudgement");
                                                string customMeBersVelocitySoundsPath = FindCustomSound(pathToMeBersVelocitySounds);
                                                if (!string.IsNullOrEmpty(customMeBersVelocitySoundsPath))
                                                {
                                                    return customMeBersVelocitySoundsPath;
                                                }
                                                break;
                                            case Role.PLAGUEBEARER:
                                                string pathToMePestVelocitySounds = Path.Combine(directoryPath, soundpack, "Pestilence", "Music", "PlayerJudgement");
                                                string customMePestVelocitySoundsPath = FindCustomSound(pathToMePestVelocitySounds);
                                                if (!string.IsNullOrEmpty(customMePestVelocitySoundsPath))
                                                {
                                                    return customMePestVelocitySoundsPath;
                                                }
                                                break;
                                            case Role.BAKER:
                                                string pathToMeFamVelocitySounds = Path.Combine(directoryPath, soundpack, "Famine", "Music", "PlayerJudgement");
                                                string customMeFamVelocitySoundsPath = FindCustomSound(pathToMeFamVelocitySounds);
                                                if (!string.IsNullOrEmpty(customMeFamVelocitySoundsPath))
                                                {
                                                    return customMeFamVelocitySoundsPath;
                                                }
                                                break;
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
                                pathToTarget = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, subalignment)).Replace(ogSoundPathNames[2], "Player" + ogSoundPathNames[2]);
                                customTargetSoundPath = FindCustomSound(pathToTarget);
                                if (!string.IsNullOrEmpty(customTargetSoundPath))
                                    return customTargetSoundPath;
                                pathToTarget = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, alignment)).Replace(ogSoundPathNames[2], "Player" + ogSoundPathNames[2]);
                                customTargetSoundPath = FindCustomSound(pathToTarget);
                                if (!string.IsNullOrEmpty(customTargetSoundPath))
                                    return customTargetSoundPath;
                                pathToTarget = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack)).Replace(ogSoundPathNames[2], "Player" + ogSoundPathNames[2]);
                                customTargetSoundPath = FindCustomSound(pathToTarget);
                                if (!string.IsNullOrEmpty(customTargetSoundPath))
                                    return customTargetSoundPath;
                            }
                            if (isTT)
                            {
                                string pathToTT = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "Town Traitor"));
                                string customTTSoundPath = FindCustomSound(pathToTT);
                                if (!string.IsNullOrEmpty(customTTSoundPath))
                                    return customTTSoundPath;
                            }
                            if (horsemen.Count > 0)
                            {
                                if (Service.Game.Sim.info.roleCardObservation.Data.defense == 3 && roleData.factionType == FactionType.APOCALYPSE)
                                {
                                    switch (roleData.role)
                                    {
                                        case Role.SOULCOLLECTOR:
                                            string pathToMeDeathVelocitySounds = Path.Combine(directoryPath, soundpack, "Death", "Music", "Judgement");
                                            string customMeDeathVelocitySoundsPath = FindCustomSound(pathToMeDeathVelocitySounds);
                                            if (!string.IsNullOrEmpty(customMeDeathVelocitySoundsPath))
                                            {
                                                return customMeDeathVelocitySoundsPath;
                                            }
                                            break;
                                        case Role.BERSERKER:
                                            string pathToMeBersVelocitySounds = Path.Combine(directoryPath, soundpack, "War", "Music", "Judgement");
                                            string customMeBersVelocitySoundsPath = FindCustomSound(pathToMeBersVelocitySounds);
                                            if (!string.IsNullOrEmpty(customMeBersVelocitySoundsPath))
                                            {
                                                return customMeBersVelocitySoundsPath;
                                            }
                                            break;
                                        case Role.PLAGUEBEARER:
                                            string pathToMePestVelocitySounds = Path.Combine(directoryPath, soundpack, "Pestilence", "Music", "Judgement");
                                            string customMePestVelocitySoundsPath = FindCustomSound(pathToMePestVelocitySounds);
                                            if (!string.IsNullOrEmpty(customMePestVelocitySoundsPath))
                                            {
                                                return customMePestVelocitySoundsPath;
                                            }
                                            break;
                                        case Role.BAKER:
                                            string pathToMeFamVelocitySounds = Path.Combine(directoryPath, soundpack, "Famine", "Music", "Judgement");
                                            string customMeFamVelocitySoundsPath = FindCustomSound(pathToMeFamVelocitySounds);
                                            if (!string.IsNullOrEmpty(customMeFamVelocitySoundsPath))
                                            {
                                                return customMeFamVelocitySoundsPath;
                                            }
                                            break;
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
                            pathToSound = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, subalignment));
                            SoundPath = FindCustomSound(pathToSound);
                            if (!string.IsNullOrEmpty(SoundPath))
                                return SoundPath;
                            pathToSound = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, alignment));
                            SoundPath = FindCustomSound(pathToSound);
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
                                if (isTT)
                                {
                                    string pathToTT = Path.Combine(directoryPath, soundpack, "Town Traitor", "Music", "RapidModeLooping");
                                    string customTTSoundPath = FindCustomSound(pathToTT);
                                    if (!string.IsNullOrEmpty(customTTSoundPath))
                                        return customTTSoundPath;
                                }
                                if (horsemen.Count > 0)
                                {
                                    if (Service.Game.Sim.info.roleCardObservation.Data.defense == 3 && roleData.factionType == FactionType.APOCALYPSE)
                                    {
                                        switch (roleData.role)
                                        {
                                            case Role.SOULCOLLECTOR:
                                                string pathToMeDeathVelocitySounds = Path.Combine(directoryPath, soundpack, "Death", "Music", "RapidModeLooping");
                                                string customMeDeathVelocitySoundsPath = FindCustomSound(pathToMeDeathVelocitySounds);
                                                if (!string.IsNullOrEmpty(customMeDeathVelocitySoundsPath))
                                                {
                                                    return customMeDeathVelocitySoundsPath;
                                                }
                                                break;
                                            case Role.BERSERKER:
                                                string pathToMeBersVelocitySounds = Path.Combine(directoryPath, soundpack, "War", "Music", "RapidModeLooping");
                                                string customMeBersVelocitySoundsPath = FindCustomSound(pathToMeBersVelocitySounds);
                                                if (!string.IsNullOrEmpty(customMeBersVelocitySoundsPath))
                                                {
                                                    return customMeBersVelocitySoundsPath;
                                                }
                                                break;
                                            case Role.PLAGUEBEARER:
                                                string pathToMePestVelocitySounds = Path.Combine(directoryPath, soundpack, "Pestilence", "Music", "RapidModeLooping");
                                                string customMePestVelocitySoundsPath = FindCustomSound(pathToMePestVelocitySounds);
                                                if (!string.IsNullOrEmpty(customMePestVelocitySoundsPath))
                                                {
                                                    return customMePestVelocitySoundsPath;
                                                }
                                                break;
                                            case Role.BAKER:
                                                string pathToMeFamVelocitySounds = Path.Combine(directoryPath, soundpack, "Famine", "Music", "RapidModeLooping");
                                                string customMeFamVelocitySoundsPath = FindCustomSound(pathToMeFamVelocitySounds);
                                                if (!string.IsNullOrEmpty(customMeFamVelocitySoundsPath))
                                                {
                                                    return customMeFamVelocitySoundsPath;
                                                }
                                                break;
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
                                pathToCustomRapidSounds = Path.Combine(directoryPath, soundpack, subalignment, "Music", "RapidModeLooping");
                                customRapidSoundsPath = FindCustomSound(pathToCustomRapidSounds);
                                if (!string.IsNullOrEmpty(customRapidSoundsPath))
                                {
                                    loop = true;
                                    loopString = customRapidSoundsPath;
                                    return customRapidSoundsPath;
                                }
                                pathToCustomRapidSounds = Path.Combine(directoryPath, soundpack, alignment, "Music", "RapidModeLooping");
                                customRapidSoundsPath = FindCustomSound(pathToCustomRapidSounds);
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
                            if (isTT)
                            {
                                string pathToTT = Path.Combine(directoryPath, soundpack, "Town Traitor", "Music", "RapidMode" + ogSoundPathNames[2]);
                                string customTTSoundPath = FindCustomSound(pathToTT);
                                if (!string.IsNullOrEmpty(customTTSoundPath))
                                    return customTTSoundPath;
                            }
                            if (horsemen.Count > 0)
                            {
                                if (Service.Game.Sim.info.roleCardObservation.Data.defense == 3 && roleData.factionType == FactionType.APOCALYPSE)
                                {
                                    switch (roleData.role)
                                    {
                                        case Role.SOULCOLLECTOR:
                                            string pathToMeDeathVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "Death")).Replace(ogSoundPathNames[2], "RapidMode" + ogSoundPathNames[2]);
                                            string customMeDeathVelocitySoundsPath = FindCustomSound(pathToMeDeathVelocitySounds);
                                            if (!string.IsNullOrEmpty(customMeDeathVelocitySoundsPath))
                                            {
                                                return customMeDeathVelocitySoundsPath;
                                            }
                                            break;
                                        case Role.BERSERKER:
                                            string pathToMeBersVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "War")).Replace(ogSoundPathNames[2], "RapidMode" + ogSoundPathNames[2]);
                                            string customMeBersVelocitySoundsPath = FindCustomSound(pathToMeBersVelocitySounds);
                                            if (!string.IsNullOrEmpty(customMeBersVelocitySoundsPath))
                                            {
                                                return customMeBersVelocitySoundsPath;
                                            }
                                            break;
                                        case Role.PLAGUEBEARER:
                                            string pathToMePestVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "Pestilence")).Replace(ogSoundPathNames[2], "RapidMode" + ogSoundPathNames[2]);
                                            string customMePestVelocitySoundsPath = FindCustomSound(pathToMePestVelocitySounds);
                                            if (!string.IsNullOrEmpty(customMePestVelocitySoundsPath))
                                            {
                                                return customMePestVelocitySoundsPath;
                                            }
                                            break;
                                        case Role.BAKER:
                                            string pathToMeFamVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "Famine")).Replace(ogSoundPathNames[2], "RapidMode" + ogSoundPathNames[2]);
                                            string customMeFamVelocitySoundsPath = FindCustomSound(pathToMeFamVelocitySounds);
                                            if (!string.IsNullOrEmpty(customMeFamVelocitySoundsPath))
                                            {
                                                return customMeFamVelocitySoundsPath;
                                            }
                                            break;
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
                            pathToCustomVelocitySounds = Path.Combine(directoryPath, soundpack, subalignment, "Music", "RapidMode" + ogSoundPathNames[2]);
                            customVelocitySoundsPath = FindCustomSound(pathToCustomVelocitySounds);
                            if (!string.IsNullOrEmpty(customVelocitySoundsPath)) return customVelocitySoundsPath;
                            pathToCustomVelocitySounds = Path.Combine(directoryPath, soundpack, alignment, "Music", "RapidMode" + ogSoundPathNames[2]);
                            customVelocitySoundsPath = FindCustomSound(pathToCustomVelocitySounds);
                            if (!string.IsNullOrEmpty(customVelocitySoundsPath)) return customVelocitySoundsPath;
                            pathToCustomVelocitySounds = Path.Combine(directoryPath, soundpack, "Music", "RapidMode" + ogSoundPathNames[2]);
                            customVelocitySoundsPath = FindCustomSound(pathToCustomVelocitySounds);
                            if (!string.IsNullOrEmpty(customVelocitySoundsPath)) return customVelocitySoundsPath;
                        }
                        else if (!string.IsNullOrEmpty(gameVelocity))
                        {
                            if (isTT)
                            {
                                string pathToTT = Path.Combine(directoryPath, soundpack, "Town Traitor", "Music", gameVelocity + ogSoundPathNames[2]);
                                string customTTSoundPath = FindCustomSound(pathToTT);
                                if (!string.IsNullOrEmpty(customTTSoundPath))
                                    return customTTSoundPath;
                            }
                            if (horsemen.Count > 0)
                            {
                                if (Service.Game.Sim.info.roleCardObservation.Data.defense == 3 && roleData.factionType == FactionType.APOCALYPSE)
                                {
                                    switch (roleData.role)
                                    {
                                        case Role.SOULCOLLECTOR:
                                            string pathToMeDeathVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "Death")).Replace(ogSoundPathNames[2], gameVelocity + ogSoundPathNames[2]);
                                            string customMeDeathVelocitySoundsPath = FindCustomSound(pathToMeDeathVelocitySounds);
                                            if (!string.IsNullOrEmpty(customMeDeathVelocitySoundsPath))
                                            {
                                                return customMeDeathVelocitySoundsPath;
                                            }
                                            break;
                                        case Role.BERSERKER:
                                            string pathToMeBersVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "War")).Replace(ogSoundPathNames[2], gameVelocity + ogSoundPathNames[2]);
                                            string customMeBersVelocitySoundsPath = FindCustomSound(pathToMeBersVelocitySounds);
                                            if (!string.IsNullOrEmpty(customMeBersVelocitySoundsPath))
                                            {
                                                return customMeBersVelocitySoundsPath;
                                            }
                                            break;
                                        case Role.PLAGUEBEARER:
                                            string pathToMePestVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "Pestilence")).Replace(ogSoundPathNames[2], gameVelocity + ogSoundPathNames[2]);
                                            string customMePestVelocitySoundsPath = FindCustomSound(pathToMePestVelocitySounds);
                                            if (!string.IsNullOrEmpty(customMePestVelocitySoundsPath))
                                            {
                                                return customMePestVelocitySoundsPath;
                                            }
                                            break;
                                        case Role.BAKER:
                                            string pathToMeFamVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "Famine")).Replace(ogSoundPathNames[2], gameVelocity + ogSoundPathNames[2]);
                                            string customMeFamVelocitySoundsPath = FindCustomSound(pathToMeFamVelocitySounds);
                                            if (!string.IsNullOrEmpty(customMeFamVelocitySoundsPath))
                                            {
                                                return customMeFamVelocitySoundsPath;
                                            }
                                            break;
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
                            pathToCustomVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, subalignment)).Replace(ogSoundPathNames[2], gameVelocity + ogSoundPathNames[2]);
                            customVelocitySoundsPath = FindCustomSound(pathToCustomVelocitySounds);
                            if (!string.IsNullOrEmpty(customVelocitySoundsPath)) return customVelocitySoundsPath;
                            pathToCustomVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, alignment)).Replace(ogSoundPathNames[2], gameVelocity + ogSoundPathNames[2]);
                            customVelocitySoundsPath = FindCustomSound(pathToCustomVelocitySounds);
                            if (!string.IsNullOrEmpty(customVelocitySoundsPath)) return customVelocitySoundsPath;
                            pathToCustomVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack)).Replace(ogSoundPathNames[2], gameVelocity + ogSoundPathNames[2]);
                            customVelocitySoundsPath = FindCustomSound(pathToCustomVelocitySounds);
                            if (!string.IsNullOrEmpty(customVelocitySoundsPath)) return customVelocitySoundsPath;
                        }
                    }
                    if (isTT)
                    {
                        string pathToTT = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "Town Traitor"));
                        string customTTSoundPath = FindCustomSound(pathToTT);
                        if (!string.IsNullOrEmpty(customTTSoundPath))
                            return customTTSoundPath;
                    }
                    if (horsemen.Count > 0)
                    {
                        if (Service.Game.Sim.info.roleCardObservation.Data.defense == 3 && roleData.factionType == FactionType.APOCALYPSE)
                        {
                            switch (roleData.role)
                            {
                                case Role.SOULCOLLECTOR:
                                    string pathToMeDeathVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "Death"));
                                    string customMeDeathVelocitySoundsPath = FindCustomSound(pathToMeDeathVelocitySounds);
                                    if (!string.IsNullOrEmpty(customMeDeathVelocitySoundsPath))
                                    {
                                        return customMeDeathVelocitySoundsPath;
                                    }
                                    break;
                                case Role.BERSERKER:
                                    string pathToMeBersVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "War"));
                                    string customMeBersVelocitySoundsPath = FindCustomSound(pathToMeBersVelocitySounds);
                                    if (!string.IsNullOrEmpty(customMeBersVelocitySoundsPath))
                                    {
                                        return customMeBersVelocitySoundsPath;
                                    }
                                    break;
                                case Role.PLAGUEBEARER:
                                    string pathToMePestVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "Pestilence"));
                                    string customMePestVelocitySoundsPath = FindCustomSound(pathToMePestVelocitySounds);
                                    if (!string.IsNullOrEmpty(customMePestVelocitySoundsPath))
                                    {
                                        return customMePestVelocitySoundsPath;
                                    }
                                    break;
                                case Role.BAKER:
                                    string pathToMeFamVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "Famine"));
                                    string customMeFamVelocitySoundsPath = FindCustomSound(pathToMeFamVelocitySounds);
                                    if (!string.IsNullOrEmpty(customMeFamVelocitySoundsPath))
                                    {
                                        return customMeFamVelocitySoundsPath;
                                    }
                                    break;
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
                    pathToRoleSounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, subalignment));
                    customRoleSoundPath = FindCustomSound(pathToRoleSounds);
                    if (!string.IsNullOrEmpty(customRoleSoundPath)) return customRoleSoundPath;
                    pathToRoleSounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, alignment));
                    customRoleSoundPath = FindCustomSound(pathToRoleSounds);
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
            string[] files;
            string dir = Path.GetDirectoryName(soundPath);
            if (!Directory.Exists(dir)) goto CheckExtension;
            files = Directory.GetFiles(Path.GetDirectoryName(soundPath), Path.GetFileName(soundPath) + ".*");
            if (files.Length < 1)
            {
                goto CheckExtension;
            }
            return files[0];

        CheckExtension:
            if (string.IsNullOrEmpty(curExtension)) return null;
            string extensionPath = soundPath.Replace(soundpack, curExtension);
            dir = Path.GetDirectoryName(extensionPath);
            if (!Directory.Exists(dir)) return null;
            files = Directory.GetFiles(Path.GetDirectoryName(extensionPath), Path.GetFileName(extensionPath) + ".*");
            if (files.Length < 1)
            {
                return null;
            }
            return files[0];
        }
        public static void LoadSoundpack(string selection)
        {
            if (selection == "No Soundpack")
            {
                soundpack = "No Soundpack"; AudioController p = UnityEngine.Object.FindObjectOfType<AudioController>();
                string cuMusic = p.currentMusicSound.Split('/')[2];
                p.StopMusic();
                p.PlayMusic($"Audio/Music/{cuMusic}"); return;
            }
            string llll = soundpacks[selection];
            if (!string.IsNullOrEmpty(llll))
            {
                soundpack = Path.Combine(llll, selection);
            }
            else
            {
                soundpack = selection;
            }
            curExtension = FindExtension(Path.Combine(directoryPath, soundpack, "extension.txt"));
            AudioController a = UnityEngine.Object.FindObjectOfType<AudioController>();
            string curMusic = a.currentMusicSound.Split('/')[2];
            a.StopMusic();
            a.PlayMusic($"Audio/Music/{curMusic}");
        }
        public static List<string> GetSubfolders()
        {
            List<string> options = new(){
                "No Subfolder"
            };
            foreach (string dir in subfolders)
            {
                options.Add(dir);
            }
            return options;
        }
        public static void ForSoundpackMod()
        {

            directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "SalemModLoader", "ModFolders", "Soundpacks");
            Debug.Log(directoryPath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            Debug.Log("Working?");
            string[] fullDirectories = Directory.GetDirectories(directoryPath);
            foreach (string dir in fullDirectories)
            {
                string[] insides = Directory.GetDirectories(dir);
                if (insides.Any(n => toCheck.Contains(Path.GetFileName(n))))
                {
                    soundpacks.Add(Path.GetFileName(dir), null);
                }
                else
                {
                    string dirName = Path.GetFileName(dir);
                    subfolders.Add(dirName);
                    foreach (string L in insides)
                    {
                        soundpacks.Add(Path.GetFileName(L), dirName);
                    }
                }
            }
            string v = ModSettings.GetString("Selected Soundpack");
            if (v == "No Soundpack" || !soundpacks.ContainsKey(v)) { soundpack = "No Soundpack"; return; }
            string b = soundpacks[v];
            soundpack = string.IsNullOrEmpty(b) ? v : Path.Combine(b, v);
            curExtension = FindExtension(Path.Combine(directoryPath, soundpack, "extension.txt"));
        }
        public static string FindExtension(string MeWhen)
        {
            if(!File.Exists(MeWhen)) return null;
            string file = File.ReadAllText(MeWhen);
            if(!soundpacks.ContainsKey(file)) return null;
            string b = soundpacks[file];
            return string.IsNullOrEmpty(b) ? file : Path.Combine(b, file);
        }
        public static List<string> GetSoundpacks()
        {
            List<string> options = new(){
                "No Soundpack"
            };
            foreach (KeyValuePair<string, string> pair in soundpacks)
            {
                options.Add(pair.Key);
            }
            return options;
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
                    Debug.LogWarning("AudioUtility: La extensión de archivo " + extension + " no es compatible.");
                    return AudioType.UNKNOWN;
            }
        }

    }

}