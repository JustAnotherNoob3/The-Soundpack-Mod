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
using System.Runtime.CompilerServices;
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
        public static bool pest = false;
        public static bool death = false;
        public static bool war = false;
        public static bool fam = false;
        public static string directoryPath;
        public static string soundpack;
        public static string curExtension = null;
        public static bool isTribunal = false;
        public static bool isParty = false;
        public static bool isNB = false;
        public static string GetCustomSoundRework(string ogSoundPath)
        {
            return ogSoundPath;
        }
        public static string GetCustomSound(string ogSoundPath)
        {

            string[] ogSoundPathNames = ogSoundPath.Split('/');
            if (!Directory.Exists(Path.Combine(directoryPath, soundpack, ogSoundPathNames[1])))
            {
                return ogSoundPath;
            }
            if (!ModSettings.GetBool("Deactivate Custom Triggers", "JAN.soundpacks") && (ModSettings.GetBool("Allow Custom Triggers SFX", "JAN.soundpacks") || (ogSoundPathNames[1] != "Steps" && ogSoundPathNames[1] != "UI")))
            {
                if (Leo.IsGameScene() && Pepper.IsGamePhasePlay())
                {
                    bool horsemen = pest | war | death | fam;
                    RoleData roleData = RoleExtensions.GetRoleData(Pepper.GetMyRole());
                    if (ogSoundPathNames[1] == "Music")
                    {
                        if (loop)
                        {
                            return loopString;
                        }
                        PlayPhase playPhase = Service.Game.Sim.simulation.playPhaseState.Data.playPhase;
                        if ((playPhase == PlayPhase.FIRST_DISCUSSION || playPhase == PlayPhase.FIRST_DAY) && ogSoundPathNames[2] == "DiscussionMusic")
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
                            string pathToFirstDay = FindNormalPath(ogSoundPath.Replace(ogSoundPathNames[2], "DayOne"), roleData) ;

                            if (!string.IsNullOrEmpty(pathToFirstDay))
                                return pathToFirstDay;
                        }

                        if (ogSoundPathNames[2] == "Judgement")
                        {
                            if (prosecutor)
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
                                    if (horsemen)
                                    {
                                        string pathToMeHorsemenVelocitySounds = GetHorsemenString(Path.Combine(directoryPath, soundpack, "XROLE", "Music", "PlayerJudgementProsecutor"), roleData);
                                        if (!string.IsNullOrEmpty(pathToMeHorsemenVelocitySounds))
                                            return pathToMeHorsemenVelocitySounds;
                                    }
                                    string pathToTargetPlayer = FindNormalPath(ogSoundPath.Replace(ogSoundPathNames[2], "Player" + ogSoundPathNames[2] + "Prosecutor"), roleData);
                                    if (!string.IsNullOrEmpty(pathToTargetPlayer))
                                        return pathToTargetPlayer;
                                }
                                if (isTT)
                                {
                                    string pathToTT = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "Town Traitor")).Replace(ogSoundPathNames[2], ogSoundPathNames[2] + "Prosecutor");
                                    string customTTSoundPath = FindCustomSound(pathToTT);
                                    if (!string.IsNullOrEmpty(customTTSoundPath))
                                        return customTTSoundPath;
                                }

                                if (horsemen)
                                {
                                    string pathToMeHorsemenVelocitySounds = GetHorsemenString(Path.Combine(directoryPath, soundpack, "XROLE", "Music", "JudgementProsecutor"), roleData);
                                    if (!string.IsNullOrEmpty(pathToMeHorsemenVelocitySounds))
                                        return pathToMeHorsemenVelocitySounds;
                                }
                                string pathToTarget = FindNormalPath(ogSoundPath.Replace(ogSoundPathNames[2], ogSoundPathNames[2] + "Prosecutor"), roleData);
                                if (!string.IsNullOrEmpty(pathToTarget))
                                    return pathToTarget;
                            }
                            if (Pepper.GetMyRole() == Role.EXECUTIONER && targetOnStand)
                            {
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
                                if (horsemen)
                                {
                                    string pathToMeHorsemenVelocitySounds = GetHorsemenString(Path.Combine(directoryPath, soundpack, "XROLE", "Music", "PlayerJudgement"), roleData);
                                    if (!string.IsNullOrEmpty(pathToMeHorsemenVelocitySounds))
                                        return pathToMeHorsemenVelocitySounds;
                                }
                                string pathToTarget = FindNormalPath(ogSoundPath.Replace(ogSoundPathNames[2], "Player" + ogSoundPathNames[2]), roleData);
                                if (!string.IsNullOrEmpty(pathToTarget))
                                    return pathToTarget;
                            }
                            if (isTT)
                            {
                                string pathToTT = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "Town Traitor"));
                                string customTTSoundPath = FindCustomSound(pathToTT);
                                if (!string.IsNullOrEmpty(customTTSoundPath))
                                    return customTTSoundPath;
                            }
                            if (horsemen)
                            {
                                string pathToMeHorsemenVelocitySounds = GetHorsemenString(Path.Combine(directoryPath, soundpack, "XROLE", "Music", "Judgement"), roleData);
                                if (!string.IsNullOrEmpty(pathToMeHorsemenVelocitySounds))
                                    return pathToMeHorsemenVelocitySounds;
                            }
                            string pathToSound = FindNormalPath(ogSoundPath, roleData);
                            if (!string.IsNullOrEmpty(pathToSound))
                                return pathToSound;
                        }
                        if (ogSoundPathNames[2] == "NightMusic")
                        {
                            isTribunal = false;
                            if(isParty){
                            isParty = false;
                            if (isRapid)
                            {
                                if (isTT)
                                {
                                    string pathToTT = Path.Combine(directoryPath, soundpack, "Town Traitor", "Music", "RapidModePartyMusic");
                                    string customTTSoundPath = FindCustomSound(pathToTT);
                                    if (!string.IsNullOrEmpty(customTTSoundPath))
                                        return customTTSoundPath;
                                }
                                if (horsemen)
                                {
                                    string pathToMeHorsemenVelocitySounds = GetHorsemenString(Path.Combine(directoryPath, soundpack, "XROLE", "Music", "RapidModePartyMusic"), roleData);
                                    if (!string.IsNullOrEmpty(pathToMeHorsemenVelocitySounds)) return pathToMeHorsemenVelocitySounds;
                                }
                                string pathToASound = FindNormalPath(ogSoundPath.Replace(ogSoundPathNames[2], "RapidModePartyMusic"), roleData);
                                if (!string.IsNullOrEmpty(pathToASound))
                                    return pathToASound;
                            }
                            else if (!string.IsNullOrEmpty(gameVelocity))
                            {
                                if (isTT)
                                {
                                    string pathToTT = Path.Combine(directoryPath, soundpack, "Town Traitor", "Music", gameVelocity + "PartyMusic");
                                    string customTTSoundPath = FindCustomSound(pathToTT);
                                    if (!string.IsNullOrEmpty(customTTSoundPath))
                                        return customTTSoundPath;
                                }
                                if (horsemen)
                                {
                                    string pathToMeHorsemenVelocitySounds = GetHorsemenString(Path.Combine(directoryPath, soundpack, "XROLE", "Music", gameVelocity + "PartyMusic"), roleData);
                                    if (!string.IsNullOrEmpty(pathToMeHorsemenVelocitySounds)) return pathToMeHorsemenVelocitySounds;
                                }
                                string pathToASound = FindNormalPath(ogSoundPath.Replace(ogSoundPathNames[2], gameVelocity + "PartyMusic"), roleData);
                                if (!string.IsNullOrEmpty(pathToASound))
                                    return pathToASound;
                            }
                            if (isTT)
                            {
                                string pathToTT = Path.Combine(directoryPath, soundpack, "Town Traitor", "Music", "PartyMusic");
                                string customTTSoundPath = FindCustomSound(pathToTT);
                                if (!string.IsNullOrEmpty(customTTSoundPath))
                                    return customTTSoundPath;
                            }
                            if (horsemen)
                            {
                                string pathToMeHorsemenVelocitySounds = GetHorsemenString(Path.Combine(directoryPath, soundpack, "XROLE", "Music", "PartyMusic"), roleData);
                                if (!string.IsNullOrEmpty(pathToMeHorsemenVelocitySounds)) return pathToMeHorsemenVelocitySounds;
                            }
                            string pathToSound = FindNormalPath(ogSoundPath.Replace(ogSoundPathNames[2], "PartyMusic"), roleData);
                            if (!string.IsNullOrEmpty(pathToSound))
                                return pathToSound;
                        }
                        
                        }
                        if (ogSoundPathNames[2] == "VotingMusic" && isTribunal) //bro making this one was fast af.
                        {
                            isTribunal = false;
                            if (isRapid)
                            {
                                if (isTT)
                                {
                                    string pathToTT = Path.Combine(directoryPath, soundpack, "Town Traitor", "Music", "RapidModeTribunalMusic");
                                    string customTTSoundPath = FindCustomSound(pathToTT);
                                    if (!string.IsNullOrEmpty(customTTSoundPath))
                                        return customTTSoundPath;
                                }
                                if (horsemen)
                                {
                                    string pathToMeHorsemenVelocitySounds = GetHorsemenString(Path.Combine(directoryPath, soundpack, "XROLE", "Music", "RapidModeTribunalMusic"), roleData);
                                    if (!string.IsNullOrEmpty(pathToMeHorsemenVelocitySounds)) return pathToMeHorsemenVelocitySounds;
                                }
                                string pathToASound = FindNormalPath(ogSoundPath.Replace(ogSoundPathNames[2], "RapidModeTribunalMusic"), roleData);
                                if (!string.IsNullOrEmpty(pathToASound))
                                    return pathToASound;
                            }
                            else if (!string.IsNullOrEmpty(gameVelocity))
                            {
                                if (isTT)
                                {
                                    string pathToTT = Path.Combine(directoryPath, soundpack, "Town Traitor", "Music", gameVelocity + "TribunalMusic");
                                    string customTTSoundPath = FindCustomSound(pathToTT);
                                    if (!string.IsNullOrEmpty(customTTSoundPath))
                                        return customTTSoundPath;
                                }
                                if (horsemen)
                                {
                                    string pathToMeHorsemenVelocitySounds = GetHorsemenString(Path.Combine(directoryPath, soundpack, "XROLE", "Music", gameVelocity + "TribunalMusic"), roleData);
                                    if (!string.IsNullOrEmpty(pathToMeHorsemenVelocitySounds)) return pathToMeHorsemenVelocitySounds;
                                }
                                string pathToASound = FindNormalPath(ogSoundPath.Replace(ogSoundPathNames[2], gameVelocity + "TribunalMusic"), roleData);
                                if (!string.IsNullOrEmpty(pathToASound))
                                    return pathToASound;
                            }
                            if (isTT)
                            {
                                string pathToTT = Path.Combine(directoryPath, soundpack, "Town Traitor", "Music", "TribunalMusic");
                                string customTTSoundPath = FindCustomSound(pathToTT);
                                if (!string.IsNullOrEmpty(customTTSoundPath))
                                    return customTTSoundPath;
                            }
                            if (horsemen)
                            {
                                string pathToMeHorsemenVelocitySounds = GetHorsemenString(Path.Combine(directoryPath, soundpack, "XROLE", "Music", "TribunalMusic"), roleData);
                                if (!string.IsNullOrEmpty(pathToMeHorsemenVelocitySounds)) return pathToMeHorsemenVelocitySounds;
                            }
                            string pathToSound = FindNormalPath(ogSoundPath.Replace(ogSoundPathNames[2], "TribunalMusic"), roleData);
                            if (!string.IsNullOrEmpty(pathToSound))
                                return pathToSound;
                        }
                        if (isRapid)
                        {
                            if (ModSettings.GetBool("Looping Rapid Mode"))
                            {
                                if (isTT)
                                {
                                    string pathToTT = Path.Combine(directoryPath, soundpack, "Town Traitor", "Music", "RapidModeLooping");
                                    string customTTSoundPath = FindCustomSound(pathToTT);
                                    if (!string.IsNullOrEmpty(customTTSoundPath))
                                    { loop = true; loopString = customTTSoundPath; return customTTSoundPath; }
                                }
                                if (horsemen)
                                {
                                    string pathToMeHorsemenVelocitySounds = GetHorsemenString(Path.Combine(directoryPath, soundpack, "XROLE", "Music", "RapidModeLooping"), roleData);
                                    if (!string.IsNullOrEmpty(pathToMeHorsemenVelocitySounds))
                                    {
                                        loop = true;
                                        loopString = pathToMeHorsemenVelocitySounds;
                                        return pathToMeHorsemenVelocitySounds;
                                    }
                                }
                                string pathToCustomRapidSounds = FindNormalPath(ogSoundPath.Replace(ogSoundPathNames[2], "RapidModeLooping"), roleData);
                                if (!string.IsNullOrEmpty(pathToCustomRapidSounds))
                                {
                                    loop = true;
                                    loopString = pathToCustomRapidSounds;
                                    return pathToCustomRapidSounds;
                                }
                            }
                            if (isTT)
                            {
                                string pathToTT = Path.Combine(directoryPath, soundpack, "Town Traitor", "Music", "RapidMode" + ogSoundPathNames[2]);
                                string customTTSoundPath = FindCustomSound(pathToTT);
                                if (!string.IsNullOrEmpty(customTTSoundPath))
                                    return customTTSoundPath;
                            }
                            if (horsemen)
                            {
                                string pathToMeHorsemenVelocitySounds = GetHorsemenString(ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "XROLE")).Replace(ogSoundPathNames[2], "RapidMode" + ogSoundPathNames[2]), roleData);
                                if (!string.IsNullOrEmpty(pathToMeHorsemenVelocitySounds)) return pathToMeHorsemenVelocitySounds;
                            }
                            string pathToCustomVelocitySounds = FindNormalPath(ogSoundPath.Replace(ogSoundPathNames[2], "RapidMode" + ogSoundPathNames[2]), roleData);
                            if (!string.IsNullOrEmpty(pathToCustomVelocitySounds)) return pathToCustomVelocitySounds;
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
                            if (horsemen)
                            {
                                string pathToMeHorsemenVelocitySounds = GetHorsemenString(ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "XROLE")).Replace(ogSoundPathNames[2], gameVelocity + ogSoundPathNames[2]), roleData);
                                if (!string.IsNullOrEmpty(pathToMeHorsemenVelocitySounds)) return pathToMeHorsemenVelocitySounds;
                            }
                            string pathToCustomVelocitySounds = FindNormalPath(ogSoundPath.Replace(ogSoundPathNames[2], gameVelocity + ogSoundPathNames[2]), roleData);
                            if (!string.IsNullOrEmpty(pathToCustomVelocitySounds)) return pathToCustomVelocitySounds;
                        }

                    }
                    if (isNB && ogSoundPath.Contains("Male"))
                    {
                        isNB = false;
                        if (isTT)
                        {
                            string pathToTT = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "Town Traitor")).Replace("Male","NonBinary");
                            string customTTSoundPath = FindCustomSound(pathToTT);
                            if (!string.IsNullOrEmpty(customTTSoundPath))
                                return customTTSoundPath;
                        }
                        if (horsemen)
                        {
                            string pathToMeHorsemenVelocitySounds = GetHorsemenString(ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "XROLE")).Replace("Male","NonBinary"), roleData);
                            if (!string.IsNullOrEmpty(pathToMeHorsemenVelocitySounds))
                            {
                                return pathToMeHorsemenVelocitySounds;
                            }
                        }
                        string pathToSound = FindNormalPath(ogSoundPath.Replace("Male","NonBinary"), roleData);
                        if (!string.IsNullOrEmpty(pathToSound))
                        return pathToSound;
                    }
                    if (isTT)
                    {
                        string pathToTT = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "Town Traitor"));
                        string customTTSoundPath = FindCustomSound(pathToTT);
                        if (!string.IsNullOrEmpty(customTTSoundPath))
                            return customTTSoundPath;
                    }
                    if (horsemen)
                    {
                        string pathToMeHorsemenVelocitySounds = GetHorsemenString(ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, "XROLE")), roleData);
                        if (!string.IsNullOrEmpty(pathToMeHorsemenVelocitySounds))
                        {
                            return pathToMeHorsemenVelocitySounds;
                        }
                    }
                    string pathToRoleSounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, roleData.roleName));
                    string customRoleSoundPath = FindCustomSound(pathToRoleSounds);
                    if (!string.IsNullOrEmpty(customRoleSoundPath)) return customRoleSoundPath;
                    string alignment = roleData.roleAlignment.ToString().ToTitleCase();
                    pathToRoleSounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, $"{alignment} {roleData.subAlignment.ToString().ToTitleCase()}"));
                    customRoleSoundPath = FindCustomSound(pathToRoleSounds);
                    if (!string.IsNullOrEmpty(customRoleSoundPath)) return customRoleSoundPath;
                    pathToRoleSounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, alignment));
                    customRoleSoundPath = FindCustomSound(pathToRoleSounds);
                    if (!string.IsNullOrEmpty(customRoleSoundPath)) return customRoleSoundPath;
                }
                if (draw && ogSoundPathNames[2] == "CovenVictory")
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
        static string FindNormalPath(string ogSoundPath, RoleData roleData)
        {
            string alignment = roleData.roleAlignment.ToString().ToTitleCase();
            string subalignment = $"{alignment} {roleData.subAlignment.ToString().ToTitleCase()}";
            string roleFolderName = roleData.roleName;
            string pathToRoleSounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, roleFolderName));
            string customRoleSoundPath = FindCustomSound(pathToRoleSounds);
            if (!string.IsNullOrEmpty(customRoleSoundPath)) return customRoleSoundPath;
            pathToRoleSounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, subalignment));
            customRoleSoundPath = FindCustomSound(pathToRoleSounds);
            if (!string.IsNullOrEmpty(customRoleSoundPath)) return customRoleSoundPath;
            pathToRoleSounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack, alignment));
            customRoleSoundPath = FindCustomSound(pathToRoleSounds);
            if (!string.IsNullOrEmpty(customRoleSoundPath)) return customRoleSoundPath;
            pathToRoleSounds = ogSoundPath.Replace("Audio", Path.Combine(directoryPath, soundpack));
            customRoleSoundPath = FindCustomSound(pathToRoleSounds);
            if (!string.IsNullOrEmpty(customRoleSoundPath)) return customRoleSoundPath;
            return null;
        }
        static string GetHorsemenString(string path, RoleData roleData)
        {
            if (Service.Game.Sim.info.roleCardObservation.Data.defense == 3 && roleData.factionType == FactionType.APOCALYPSE)
            {
                string horsemenString = path;
                switch (roleData.role)
                {
                    case Role.SOULCOLLECTOR:
                        horsemenString = path.Replace("XROLE", "Death");
                        break;
                    case Role.BERSERKER:
                        horsemenString = path.Replace("XROLE", "War");
                        break;
                    case Role.PLAGUEBEARER:
                        horsemenString = path.Replace("XROLE", "Pestilence");
                        break;
                    case Role.BAKER:
                        horsemenString = path.Replace("XROLE", "Famine");
                        break;
                }
                string customMeHorsemenVelocitySoundsPath = FindCustomSound(horsemenString);
                if (!string.IsNullOrEmpty(customMeHorsemenVelocitySoundsPath))
                {
                    return customMeHorsemenVelocitySoundsPath;
                }
                horsemenString = path.Replace("XROLE", "TransformedHorsemen");
                customMeHorsemenVelocitySoundsPath = FindCustomSound(horsemenString);
                if (!string.IsNullOrEmpty(customMeHorsemenVelocitySoundsPath))
                {
                    return customMeHorsemenVelocitySoundsPath;
                }

            }
            string pathToHorsemen = path.Replace("XROLE", "Horsemen");
            string customSoundPath = FindCustomSound(pathToHorsemen);
            if (!string.IsNullOrEmpty(customSoundPath))
                return customSoundPath;
            return null;
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
            if (!File.Exists(MeWhen)) return null;
            string file = File.ReadAllText(MeWhen);
            if (!soundpacks.ContainsKey(file)) return null;
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
    class CustomTrigger
    {
        public string info;
        public enum Type
        {
            Prefix,
            Suffix,
            Name,
            Folder
        };
        public bool worksWithOthers;
        public Func<bool> activated;
    }

}