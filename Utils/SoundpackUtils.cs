using System.IO;
using UnityEngine;
using Server.Shared.State;
using Services;
using SML;
using System.Collections.Generic;
using Server.Shared.Extensions;
using Main;

namespace Utils;

public class SoundpackUtils
{
    public static bool Draw = false;
    public static bool Win = false;
    public static bool Loop = false;
    public static bool IsRapid = false;
    public static bool IsTT = false;
    public static string GameVelocity = "";
    private static string loopString = "";
    public static bool TargetOnStand = false; // when you are exe.
    public static bool PlayerOnStand = false; //when you have been voted.
    public static bool Prosecutor = false; //if a Prosecutor is prosecuting someone.
    public static List<Role> Horsemen = new();
    public static bool DayOne = false; // if it is day one.
    public static List<string> Directories = new();

    public static string GetCustomSound(string ogSoundPath)
    {
        var ogSoundPathNames = ogSoundPath.Split('/');

        if (!Directory.Exists(Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, ogSoundPathNames[1])))
            return ogSoundPath;

        if (!ModSettings.GetBool("Deactivate Custom Triggers", "JAN.soundpacks") && (ModSettings.GetBool("Allow Custom Triggers SFX", "JAN.soundpacks") || (ogSoundPathNames[1] != "Steps" &&
            ogSoundPathNames[1] != "UI")))
        {
            if (Pepper.IsGamePhasePlay())
            {
                var roleData = RoleExtensions.GetRoleData(Pepper.GetMyRole());
                var roleFolderName = roleData.roleName;

                if (ogSoundPathNames[1] == "Music")
                {
                    if (Loop)
                        return loopString;

                    if (DayOne && ogSoundPathNames[2] == "DiscussionMusic")
                    {
                        var modifiers = Service.Game.Sim.simulation.roleDeckBuilder.Data.modifierCards;

                        if (modifiers.Contains(Role.FAST_MODE))
                        {
                            Debug.LogError("it is fast mode");
                            GameVelocity += "FastMode";
                        }
                        else if (modifiers.Contains(Role.SLOW_MODE))
                        {
                            Debug.LogError("it is slow mode");
                            GameVelocity += "SlowMode";
                        }
                        else
                        {
                            Debug.LogError("no modifier");
                            GameVelocity = "";
                        }

                        Debug.LogWarning("It is day one");
                        DayOne = false;
                        var pathToFirstDay = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, roleFolderName, "Music", "DayOne");
                        var customSoundPath = FindCustomSound(pathToFirstDay);

                        if (!string.IsNullOrEmpty(customSoundPath))
                            return customSoundPath;

                        pathToFirstDay = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Music", "DayOne");
                        customSoundPath = FindCustomSound(pathToFirstDay);

                        if (!string.IsNullOrEmpty(customSoundPath))
                            return customSoundPath;

                        Debug.Log(pathToFirstDay + " was not found.");
                    }

                    if (ogSoundPathNames[2] == "Judgement")
                    {
                        if (Prosecutor) //test
                        {
                            if (Pepper.GetMyRole() == Role.EXECUTIONER && TargetOnStand)
                            {
                                var pathToTargetPros = ogSoundPath.Replace("Audio", Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Executioner")).Replace(ogSoundPathNames[2],
                                    $"Target{ogSoundPathNames[2]}Prosecutor");
                                var customSoundPath = FindCustomSound(pathToTargetPros);

                                if (!string.IsNullOrEmpty(customSoundPath))
                                    return customSoundPath;
                            }
                            else if (PlayerOnStand)
                            {
                                if (IsTT)
                                {
                                    var pathToTT = ogSoundPath.Replace("Audio", Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Town Traitor")).Replace(ogSoundPathNames[2],
                                        $"Player{ogSoundPathNames[2]}Prosecutor");
                                    var customTTSoundPath = FindCustomSound(pathToTT);

                                    if (!string.IsNullOrEmpty(customTTSoundPath))
                                        return customTTSoundPath;
                                }

                                if (Horsemen.Count > 0)
                                {
                                    if (Service.Game.Sim.info.roleCardObservation.Data.defense == 3 && roleData.factionType == FactionType.APOCALYPSE)
                                    {
                                        // could this switch have been done with a method instead? yeah but was more easy like this.
                                        switch (roleData.role)
                                        {
                                            case Role.SOULCOLLECTOR:
                                                var pathToMeDeathVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Death", "Music", "PlayerJudgementProsecutor");
                                                var customMeDeathVelocitySoundsPath = FindCustomSound(pathToMeDeathVelocitySounds);

                                                if (!string.IsNullOrEmpty(customMeDeathVelocitySoundsPath))
                                                    return customMeDeathVelocitySoundsPath;

                                                break;

                                            case Role.BERSERKER:
                                                var pathToMeBersVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "War", "Music", "PlayerJudgementProsecutor");
                                                var customMeBersVelocitySoundsPath = FindCustomSound(pathToMeBersVelocitySounds);

                                                if (!string.IsNullOrEmpty(customMeBersVelocitySoundsPath))
                                                    return customMeBersVelocitySoundsPath;

                                                break;

                                            case Role.PLAGUEBEARER:
                                                var pathToMePestVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Pestilence", "Music", "PlayerJudgementProsecutor");
                                                var customMePestVelocitySoundsPath = FindCustomSound(pathToMePestVelocitySounds);

                                                if (!string.IsNullOrEmpty(customMePestVelocitySoundsPath))
                                                    return customMePestVelocitySoundsPath;

                                                break;

                                            case Role.BAKER:
                                                var pathToMeFamVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Famine", "Music", "PlayerJudgementProsecutor");
                                                var customMeFamVelocitySoundsPath = FindCustomSound(pathToMeFamVelocitySounds);

                                                if (!string.IsNullOrEmpty(customMeFamVelocitySoundsPath))
                                                    return customMeFamVelocitySoundsPath;

                                                break;
                                        }
                                    }

                                    var pathToHorsemen = ogSoundPath.Replace("Audio", Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Horsemen")).Replace(ogSoundPathNames[2],
                                        $"Player{ogSoundPathNames[2]}Prosecutor");
                                    var customHorsemenSoundPath = FindCustomSound(pathToHorsemen);

                                    if (!string.IsNullOrEmpty(customHorsemenSoundPath))
                                        return customHorsemenSoundPath;
                                }

                                var pathToTargetPros = ogSoundPath.Replace("Audio", Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, roleFolderName)).Replace(ogSoundPathNames[2],
                                    $"Player{ogSoundPathNames[2]}Prosecutor");
                                var customSoundPath = FindCustomSound(pathToTargetPros);

                                if (!string.IsNullOrEmpty(customSoundPath))
                                    return customSoundPath;

                                pathToTargetPros = ogSoundPath.Replace("Audio", Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack)).Replace(ogSoundPathNames[2],
                                    $"Player{ogSoundPathNames[2]}Prosecutor");
                                customSoundPath = FindCustomSound(pathToTargetPros);

                                if (!string.IsNullOrEmpty(customSoundPath))
                                    return customSoundPath;
                            }

                            if (IsTT)
                            {
                                var pathToTT = ogSoundPath.Replace("Audio", Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Town Traitor")).Replace(ogSoundPathNames[2],
                                    $"{ogSoundPathNames[2]}Prosecutor");
                                var customTTSoundPath = FindCustomSound(pathToTT);

                                if (!string.IsNullOrEmpty(customTTSoundPath))
                                    return customTTSoundPath;
                            }

                            if (Horsemen.Count > 0)
                            {
                                if (Service.Game.Sim.info.roleCardObservation.Data.defense == 3 && roleData.factionType == FactionType.APOCALYPSE)
                                {
                                    switch (roleData.role)
                                    {
                                        case Role.SOULCOLLECTOR:
                                            var pathToMeDeathVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Death", "Music", "JudgementProsecutor");
                                            var customMeDeathVelocitySoundsPath = FindCustomSound(pathToMeDeathVelocitySounds);

                                            if (!string.IsNullOrEmpty(customMeDeathVelocitySoundsPath))
                                                return customMeDeathVelocitySoundsPath;

                                            break;

                                        case Role.BERSERKER:
                                            var pathToMeBersVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "War", "Music", "JudgementProsecutor");
                                            var customMeBersVelocitySoundsPath = FindCustomSound(pathToMeBersVelocitySounds);

                                            if (!string.IsNullOrEmpty(customMeBersVelocitySoundsPath))
                                                return customMeBersVelocitySoundsPath;

                                            break;

                                        case Role.PLAGUEBEARER:
                                            var pathToMePestVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Pestilence", "Music", "JudgementProsecutor");
                                            var customMePestVelocitySoundsPath = FindCustomSound(pathToMePestVelocitySounds);

                                            if (!string.IsNullOrEmpty(customMePestVelocitySoundsPath))
                                                return customMePestVelocitySoundsPath;

                                            break;

                                        case Role.BAKER:
                                            var pathToMeFamVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Famine", "Music", "JudgementProsecutor");
                                            var customMeFamVelocitySoundsPath = FindCustomSound(pathToMeFamVelocitySounds);

                                            if (!string.IsNullOrEmpty(customMeFamVelocitySoundsPath))
                                                return customMeFamVelocitySoundsPath;

                                            break;
                                    }
                                }

                                var pathToHorsemen = ogSoundPath.Replace("Audio", Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Horsemen")).Replace(ogSoundPathNames[2], ogSoundPathNames[2] + "Prosecutor");
                                var customSoundPath = FindCustomSound(pathToHorsemen);

                                if (!string.IsNullOrEmpty(customSoundPath))
                                    return customSoundPath;
                            }

                            var pathToPros = ogSoundPath.Replace("Audio", Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, roleFolderName)).Replace(ogSoundPathNames[2], ogSoundPathNames[2] + "Prosecutor");
                            var customProsSoundPath = FindCustomSound(pathToPros);

                            if (!string.IsNullOrEmpty(customProsSoundPath))
                                return customProsSoundPath;

                            pathToPros = ogSoundPath.Replace("Audio", Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack)).Replace(ogSoundPathNames[2], ogSoundPathNames[2] + "Prosecutor");
                            customProsSoundPath = FindCustomSound(pathToPros);

                            if (!string.IsNullOrEmpty(customProsSoundPath))
                                return customProsSoundPath;
                        }

                        if (Pepper.GetMyRole() == Role.EXECUTIONER && TargetOnStand)
                        {
                            //test
                            var pathToTarget = ogSoundPath.Replace("Audio", Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Executioner")).Replace(ogSoundPathNames[2], "Target" + ogSoundPathNames[2]);
                            var customTargetSoundPath = FindCustomSound(pathToTarget);

                            if (!string.IsNullOrEmpty(customTargetSoundPath))
                                return customTargetSoundPath;
                        }
                        else if (PlayerOnStand)
                        {
                            if (IsTT)
                            {
                                var pathToTT = ogSoundPath.Replace("Audio", Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Town Traitor")).Replace(ogSoundPathNames[2], "Player" + ogSoundPathNames[2]);
                                var customTTSoundPath = FindCustomSound(pathToTT);

                                if (!string.IsNullOrEmpty(customTTSoundPath))
                                    return customTTSoundPath;
                            }

                            if (Horsemen.Count > 0)
                            {
                                if (Service.Game.Sim.info.roleCardObservation.Data.defense == 3 && roleData.factionType == FactionType.APOCALYPSE)
                                {
                                    switch (roleData.role)
                                    {
                                        case Role.SOULCOLLECTOR:
                                            var pathToMeDeathVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Death", "Music", "JudgementProsecutor");
                                            var customMeDeathVelocitySoundsPath = FindCustomSound(pathToMeDeathVelocitySounds);

                                            if (!string.IsNullOrEmpty(customMeDeathVelocitySoundsPath))
                                                return customMeDeathVelocitySoundsPath;

                                            break;

                                        case Role.BERSERKER:
                                            var pathToMeBersVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "War", "Music", "JudgementProsecutor");
                                            var customMeBersVelocitySoundsPath = FindCustomSound(pathToMeBersVelocitySounds);

                                            if (!string.IsNullOrEmpty(customMeBersVelocitySoundsPath))
                                                return customMeBersVelocitySoundsPath;

                                            break;

                                        case Role.PLAGUEBEARER:
                                            var pathToMePestVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Pestilence", "Music", "JudgementProsecutor");
                                            var customMePestVelocitySoundsPath = FindCustomSound(pathToMePestVelocitySounds);

                                            if (!string.IsNullOrEmpty(customMePestVelocitySoundsPath))
                                                return customMePestVelocitySoundsPath;

                                            break;

                                        case Role.BAKER:
                                            var pathToMeFamVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Famine", "Music", "JudgementProsecutor");
                                            var customMeFamVelocitySoundsPath = FindCustomSound(pathToMeFamVelocitySounds);

                                            if (!string.IsNullOrEmpty(customMeFamVelocitySoundsPath))
                                                return customMeFamVelocitySoundsPath;

                                            break;
                                    }
                                }

                                var pathToHorsemen = ogSoundPath.Replace("Audio", Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Horsemen")).Replace(ogSoundPathNames[2], "Player" + ogSoundPathNames[2]);
                                var customSoundPath = FindCustomSound(pathToHorsemen);

                                if (!string.IsNullOrEmpty(customSoundPath))
                                    return customSoundPath;
                            }

                            var pathToTarget = ogSoundPath.Replace("Audio", Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, roleFolderName)).Replace(ogSoundPathNames[2], "Player" + ogSoundPathNames[2]);
                            var customTargetSoundPath = FindCustomSound(pathToTarget);

                            if (!string.IsNullOrEmpty(customTargetSoundPath))
                                return customTargetSoundPath;

                            pathToTarget = ogSoundPath.Replace("Audio", Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack)).Replace(ogSoundPathNames[2], "Player" + ogSoundPathNames[2]);
                            customTargetSoundPath = FindCustomSound(pathToTarget);

                            if (!string.IsNullOrEmpty(customTargetSoundPath))
                                return customTargetSoundPath;
                        }

                        if (IsTT)
                        {
                            var pathToTT = ogSoundPath.Replace("Audio", Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Town Traitor"));
                            var customTTSoundPath = FindCustomSound(pathToTT);

                            if (!string.IsNullOrEmpty(customTTSoundPath))
                                return customTTSoundPath;
                        }

                        if (Horsemen.Count > 0)
                        {
                            if (Service.Game.Sim.info.roleCardObservation.Data.defense == 3 && roleData.factionType == FactionType.APOCALYPSE)
                            {
                                switch (roleData.role)
                                {
                                    case Role.SOULCOLLECTOR:
                                        var pathToMeDeathVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Death", "Music", "JudgementProsecutor");
                                        var customMeDeathVelocitySoundsPath = FindCustomSound(pathToMeDeathVelocitySounds);

                                        if (!string.IsNullOrEmpty(customMeDeathVelocitySoundsPath))
                                            return customMeDeathVelocitySoundsPath;

                                        break;

                                    case Role.BERSERKER:
                                        var pathToMeBersVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "War", "Music", "JudgementProsecutor");
                                        var customMeBersVelocitySoundsPath = FindCustomSound(pathToMeBersVelocitySounds);

                                        if (!string.IsNullOrEmpty(customMeBersVelocitySoundsPath))
                                            return customMeBersVelocitySoundsPath;

                                        break;

                                    case Role.PLAGUEBEARER:
                                        var pathToMePestVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Pestilence", "Music", "JudgementProsecutor");
                                        var customMePestVelocitySoundsPath = FindCustomSound(pathToMePestVelocitySounds);

                                        if (!string.IsNullOrEmpty(customMePestVelocitySoundsPath))
                                            return customMePestVelocitySoundsPath;

                                        break;

                                    case Role.BAKER:
                                        var pathToMeFamVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Famine", "Music", "JudgementProsecutor");
                                        var customMeFamVelocitySoundsPath = FindCustomSound(pathToMeFamVelocitySounds);

                                        if (!string.IsNullOrEmpty(customMeFamVelocitySoundsPath))
                                            return customMeFamVelocitySoundsPath;

                                        break;
                                }
                            }

                            var pathToHorsemen = ogSoundPath.Replace("Audio", Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Horsemen"));
                            var customSoundPath = FindCustomSound(pathToHorsemen);

                            if (!string.IsNullOrEmpty(customSoundPath))
                                return customSoundPath;
                        }

                        var pathToSound = ogSoundPath.Replace("Audio", Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, roleFolderName));
                        var SoundPath = FindCustomSound(pathToSound);

                        if (!string.IsNullOrEmpty(SoundPath))
                            return SoundPath;

                        pathToSound = ogSoundPath.Replace("Audio", Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack));
                        SoundPath = FindCustomSound(pathToSound);

                        if (!string.IsNullOrEmpty(SoundPath))
                            return SoundPath;
                    }
                    if (IsRapid)
                    {
                        if (ModSettings.GetBool("Looping Rapid Mode")) //needs test
                        {
                            if (IsTT)
                            {
                                var pathToTT = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Town Traitor", "Music", "RapidModeLooping");
                                var customTTSoundPath = FindCustomSound(pathToTT);

                                if (!string.IsNullOrEmpty(customTTSoundPath))
                                    return customTTSoundPath;
                            }

                            if (Horsemen.Count > 0)
                            {
                                if (Service.Game.Sim.info.roleCardObservation.Data.defense == 3 && roleData.factionType == FactionType.APOCALYPSE)
                                {
                                    switch (roleData.role)
                                    {
                                        case Role.SOULCOLLECTOR:
                                            var pathToMeDeathVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Death", "Music", "JudgementProsecutor");
                                            var customMeDeathVelocitySoundsPath = FindCustomSound(pathToMeDeathVelocitySounds);

                                            if (!string.IsNullOrEmpty(customMeDeathVelocitySoundsPath))
                                                return customMeDeathVelocitySoundsPath;

                                            break;

                                        case Role.BERSERKER:
                                            var pathToMeBersVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "War", "Music", "JudgementProsecutor");
                                            var customMeBersVelocitySoundsPath = FindCustomSound(pathToMeBersVelocitySounds);

                                            if (!string.IsNullOrEmpty(customMeBersVelocitySoundsPath))
                                                return customMeBersVelocitySoundsPath;

                                            break;

                                        case Role.PLAGUEBEARER:
                                            var pathToMePestVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Pestilence", "Music", "JudgementProsecutor");
                                            var customMePestVelocitySoundsPath = FindCustomSound(pathToMePestVelocitySounds);

                                            if (!string.IsNullOrEmpty(customMePestVelocitySoundsPath))
                                                return customMePestVelocitySoundsPath;

                                            break;

                                        case Role.BAKER:
                                            var pathToMeFamVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Famine", "Music", "JudgementProsecutor");
                                            var customMeFamVelocitySoundsPath = FindCustomSound(pathToMeFamVelocitySounds);

                                            if (!string.IsNullOrEmpty(customMeFamVelocitySoundsPath))
                                                return customMeFamVelocitySoundsPath;

                                            break;
                                    }
                                }

                                var pathToHorsemenVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Horsemen", "Music", "RapidModeLooping");
                                var customHorsemenVelocitySoundsPath = FindCustomSound(pathToHorsemenVelocitySounds);

                                if (!string.IsNullOrEmpty(customHorsemenVelocitySoundsPath))
                                {
                                    Loop = true;
                                    loopString = customHorsemenVelocitySoundsPath;
                                    return customHorsemenVelocitySoundsPath;
                                }
                            }

                            var pathToCustomRapidSounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, roleFolderName, "Music", "RapidModeLooping");
                            var customRapidSoundsPath = FindCustomSound(pathToCustomRapidSounds);

                            if (!string.IsNullOrEmpty(customRapidSoundsPath))
                            {
                                Loop = true;
                                loopString = customRapidSoundsPath;
                                return customRapidSoundsPath;
                            }

                            pathToCustomRapidSounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Music", "RapidModeLooping");
                            customRapidSoundsPath = FindCustomSound(pathToCustomRapidSounds);

                            if (!string.IsNullOrEmpty(customRapidSoundsPath))
                            {
                                Loop = true;
                                loopString = customRapidSoundsPath;
                                return customRapidSoundsPath;
                            }
                        }

                        if (IsTT)
                        {
                            var pathToTT = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Town Traitor", "Music", "RapidMode" + ogSoundPathNames[2]);
                            var customTTSoundPath = FindCustomSound(pathToTT);

                            if (!string.IsNullOrEmpty(customTTSoundPath))
                                return customTTSoundPath;
                        }

                        if (Horsemen.Count > 0)
                        {
                            if (Service.Game.Sim.info.roleCardObservation.Data.defense == 3 && roleData.factionType == FactionType.APOCALYPSE)
                            {
                                switch (roleData.role)
                                {
                                    case Role.SOULCOLLECTOR:
                                        var pathToMeDeathVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Death", "Music", "JudgementProsecutor");
                                        var customMeDeathVelocitySoundsPath = FindCustomSound(pathToMeDeathVelocitySounds);

                                        if (!string.IsNullOrEmpty(customMeDeathVelocitySoundsPath))
                                            return customMeDeathVelocitySoundsPath;

                                        break;

                                    case Role.BERSERKER:
                                        var pathToMeBersVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "War", "Music", "JudgementProsecutor");
                                        var customMeBersVelocitySoundsPath = FindCustomSound(pathToMeBersVelocitySounds);

                                        if (!string.IsNullOrEmpty(customMeBersVelocitySoundsPath))
                                            return customMeBersVelocitySoundsPath;

                                        break;

                                    case Role.PLAGUEBEARER:
                                        var pathToMePestVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Pestilence", "Music", "JudgementProsecutor");
                                        var customMePestVelocitySoundsPath = FindCustomSound(pathToMePestVelocitySounds);

                                        if (!string.IsNullOrEmpty(customMePestVelocitySoundsPath))
                                            return customMePestVelocitySoundsPath;

                                        break;

                                    case Role.BAKER:
                                        var pathToMeFamVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Famine", "Music", "JudgementProsecutor");
                                        var customMeFamVelocitySoundsPath = FindCustomSound(pathToMeFamVelocitySounds);

                                        if (!string.IsNullOrEmpty(customMeFamVelocitySoundsPath))
                                            return customMeFamVelocitySoundsPath;

                                        break;
                                }
                            }

                            var pathToHorsemenVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Horsemen", "Music", "RapidMode" + ogSoundPathNames[2]);
                            var customHorsemenVelocitySoundsPath = FindCustomSound(pathToHorsemenVelocitySounds);

                            if (!string.IsNullOrEmpty(customHorsemenVelocitySoundsPath))
                                return customHorsemenVelocitySoundsPath;
                        }

                        var pathToCustomVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, roleFolderName, "Music", "RapidMode" + ogSoundPathNames[2]);
                        var customVelocitySoundsPath = FindCustomSound(pathToCustomVelocitySounds);

                        if (!string.IsNullOrEmpty(customVelocitySoundsPath))
                            return customVelocitySoundsPath;

                        pathToCustomVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Music", "RapidMode" + ogSoundPathNames[2]);
                        customVelocitySoundsPath = FindCustomSound(pathToCustomVelocitySounds);

                        if (!string.IsNullOrEmpty(customVelocitySoundsPath))
                            return customVelocitySoundsPath;
                    }
                    else if (!string.IsNullOrEmpty(GameVelocity))
                    {
                        if (IsTT)
                        {
                            var pathToTT = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Town Traitor", "Music", GameVelocity + ogSoundPathNames[2]);
                            var customTTSoundPath = FindCustomSound(pathToTT);

                            if (!string.IsNullOrEmpty(customTTSoundPath))
                                return customTTSoundPath;
                        }

                        if (Horsemen.Count > 0)
                        {
                            if (Service.Game.Sim.info.roleCardObservation.Data.defense == 3 && roleData.factionType == FactionType.APOCALYPSE)
                            {
                                switch (roleData.role)
                                {
                                    case Role.SOULCOLLECTOR:
                                        var pathToMeDeathVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Death", "Music", "JudgementProsecutor");
                                        var customMeDeathVelocitySoundsPath = FindCustomSound(pathToMeDeathVelocitySounds);

                                        if (!string.IsNullOrEmpty(customMeDeathVelocitySoundsPath))
                                            return customMeDeathVelocitySoundsPath;

                                        break;

                                    case Role.BERSERKER:
                                        var pathToMeBersVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "War", "Music", "JudgementProsecutor");
                                        var customMeBersVelocitySoundsPath = FindCustomSound(pathToMeBersVelocitySounds);

                                        if (!string.IsNullOrEmpty(customMeBersVelocitySoundsPath))
                                            return customMeBersVelocitySoundsPath;

                                        break;

                                    case Role.PLAGUEBEARER:
                                        var pathToMePestVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Pestilence", "Music", "JudgementProsecutor");
                                        var customMePestVelocitySoundsPath = FindCustomSound(pathToMePestVelocitySounds);

                                        if (!string.IsNullOrEmpty(customMePestVelocitySoundsPath))
                                            return customMePestVelocitySoundsPath;

                                        break;

                                    case Role.BAKER:
                                        var pathToMeFamVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Famine", "Music", "JudgementProsecutor");
                                        var customMeFamVelocitySoundsPath = FindCustomSound(pathToMeFamVelocitySounds);

                                        if (!string.IsNullOrEmpty(customMeFamVelocitySoundsPath))
                                            return customMeFamVelocitySoundsPath;

                                        break;
                                }
                            }

                            var pathToHorsemenVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Horsemen")).Replace(ogSoundPathNames[2], GameVelocity + ogSoundPathNames[2]);
                            var customHorsemenVelocitySoundsPath = FindCustomSound(pathToHorsemenVelocitySounds);

                            if (!string.IsNullOrEmpty(customHorsemenVelocitySoundsPath))
                                return customHorsemenVelocitySoundsPath;
                        }

                        var pathToCustomVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, roleFolderName)).Replace(ogSoundPathNames[2], GameVelocity + ogSoundPathNames[2]);
                        var customVelocitySoundsPath = FindCustomSound(pathToCustomVelocitySounds);

                        if (!string.IsNullOrEmpty(customVelocitySoundsPath))
                            return customVelocitySoundsPath;

                        pathToCustomVelocitySounds = ogSoundPath.Replace("Audio", Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack)).Replace(ogSoundPathNames[2], GameVelocity + ogSoundPathNames[2]);
                        customVelocitySoundsPath = FindCustomSound(pathToCustomVelocitySounds);

                        if (!string.IsNullOrEmpty(customVelocitySoundsPath))
                            return customVelocitySoundsPath;
                    }
                }

                if (IsTT)
                {
                    var pathToTT = ogSoundPath.Replace("Audio", Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Town Traitor"));
                    var customTTSoundPath = FindCustomSound(pathToTT);

                    if (!string.IsNullOrEmpty(customTTSoundPath))
                        return customTTSoundPath;
                }

                if (Horsemen.Count > 0)
                {
                    if (Service.Game.Sim.info.roleCardObservation.Data.defense == 3 && roleData.factionType == FactionType.APOCALYPSE)
                    {
                        switch (roleData.role)
                        {
                            case Role.SOULCOLLECTOR:
                                var pathToMeDeathVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Death", "Music", "JudgementProsecutor");
                                var customMeDeathVelocitySoundsPath = FindCustomSound(pathToMeDeathVelocitySounds);

                                if (!string.IsNullOrEmpty(customMeDeathVelocitySoundsPath))
                                    return customMeDeathVelocitySoundsPath;

                                break;

                            case Role.BERSERKER:
                                var pathToMeBersVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "War", "Music", "JudgementProsecutor");
                                var customMeBersVelocitySoundsPath = FindCustomSound(pathToMeBersVelocitySounds);

                                if (!string.IsNullOrEmpty(customMeBersVelocitySoundsPath))
                                    return customMeBersVelocitySoundsPath;

                                break;

                            case Role.PLAGUEBEARER:
                                var pathToMePestVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Pestilence", "Music", "JudgementProsecutor");
                                var customMePestVelocitySoundsPath = FindCustomSound(pathToMePestVelocitySounds);

                                if (!string.IsNullOrEmpty(customMePestVelocitySoundsPath))
                                    return customMePestVelocitySoundsPath;

                                break;

                            case Role.BAKER:
                                var pathToMeFamVelocitySounds = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Famine", "Music", "JudgementProsecutor");
                                var customMeFamVelocitySoundsPath = FindCustomSound(pathToMeFamVelocitySounds);

                                if (!string.IsNullOrEmpty(customMeFamVelocitySoundsPath))
                                    return customMeFamVelocitySoundsPath;

                                break;
                        }
                    }

                    var pathToHorsemen = ogSoundPath.Replace("Audio", Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Horsemen"));
                    var customSoundPath = FindCustomSound(pathToHorsemen);

                    if (!string.IsNullOrEmpty(customSoundPath))
                        return customSoundPath;
                }

                var pathToRoleSounds = ogSoundPath.Replace("Audio", Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, roleFolderName));
                var customRoleSoundPath = FindCustomSound(pathToRoleSounds);

                if (!string.IsNullOrEmpty(customRoleSoundPath))
                    return customRoleSoundPath;
            }

            if (Draw && ogSoundPathNames[2] == "CovenVictory") //test
            {
                Draw = false;
                var pathToCustomDrawMusic = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Music", "DrawMusic");
                var customDrawMusicPath = FindCustomSound(pathToCustomDrawMusic);

                if (!string.IsNullOrEmpty(customDrawMusicPath))
                    return customDrawMusicPath;
            }

            if (ogSoundPathNames[2] == "LoginMusicLoop_old")
            {
                var pathToCustomWin = Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack, "Music", Win ? "VictoryMusic" : "DefeatMusic");
                var customWinMusicPath = FindCustomSound(pathToCustomWin);

                if (!string.IsNullOrEmpty(customWinMusicPath))
                    return customWinMusicPath;
            }
        }

        var pathToCustomSounds = ogSoundPath.Replace("Audio", Path.Combine(SoundPacks.ModPath, Settings.CurrentSoundPack));
        var customPath = FindCustomSound(pathToCustomSounds);

        if (!string.IsNullOrEmpty(customPath))
            return customPath;

        return ogSoundPath;
    }

    private static string FindCustomSound(string soundPath) //are you happy now curtis?
    {
        if (string.IsNullOrEmpty(soundPath))
            return null;

        var dir = Path.GetDirectoryName(soundPath);

        if (!Directory.Exists(dir))
            return null;

        var files = Directory.GetFiles(Path.GetDirectoryName(soundPath), Path.GetFileName(soundPath) + ".*");

        if (files.Length < 1)
            return null;

        return files[0];
    }

    //this stuff i got from a 2d beat saber game i did like a year ago.
    public static AudioType GetAudioType(string extension)
    {
        var result = extension.ToLower() switch
        {
            ".wav" => AudioType.WAV,
            ".mp3" => AudioType.MPEG,
            ".ogg" => AudioType.OGGVORBIS,
            ".aiff" => AudioType.AIFF,
            ".mod" => AudioType.MOD,
            _ => AudioType.UNKNOWN
        };

        if (result == AudioType.UNKNOWN)
            Debug.LogWarning($"AudioUtility: La extensión de archivo {extension} no es compatible.");

        return result;
    }
}