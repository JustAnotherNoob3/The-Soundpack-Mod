using System.Net;
using System.IO;
using UnityEngine;
using Server.Shared.State;
using Services;
using SML;
using System.Collections.Generic;
using Server.Shared.Extensions;
using System;
using System.Linq;
using Home.Shared;
using Game.Decorations;
using System.Collections.ObjectModel;
namespace Utils;

public static partial class SoundpackUtils
{
    public static List<CustomTrigger> customTriggers = new();
    public static List<CustomTrigger> customGameplayTriggers = new();
    public static List<CustomTrigger> customVelocityTriggers = new();
    public static List<CustomTrigger> customFolderTriggers = new();
    public static List<CustomTrigger> flattenedList = new();
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
    public static RoleData roleData;
    public static string alignment;
    public static Pair<List<string>, string> cachedSound = new(new(), "");
    public static bool isJailed = false;
    public static bool isDueled = false;
    public static string GetCustomSound(string ogSoundPath)
    {

        List<string> ogSoundPathNames = [.. ogSoundPath.Split('/')];
        if (!Directory.Exists(Path.Combine(directoryPath, soundpack, ogSoundPathNames[1])))
        {
            return ogSoundPath;
        }
        if (!ModSettings.GetBool("Deactivate Custom Triggers", "JAN.soundpacks") && (ModSettings.GetBool("Allow Custom Triggers SFX", "JAN.soundpacks") || (ogSoundPathNames[1] != "Steps" && ogSoundPathNames[1] != "UI")))
        {
            if (Leo.IsGameScene() && Pepper.IsGamePhasePlay())
            {
                flattenedList = customFolderTriggers.Flat();
                roleData = RoleExtensions.GetRoleData(Pepper.GetMyRole());
                alignment = roleData.roleAlignment.ToString().ToTitleCase();
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
                        List<string> pathToRapidLooping = ["Audio", "Music", "DayOne"];
                        foreach (CustomTrigger x in flattenedList)
                        {
                            if (FindCustomSound(x.GetPath(pathToRapidLooping))) return cachedSound.Value;
                        }
                        if (FindCustomSound(pathToRapidLooping)) return cachedSound.Value;
                    }
                    if (isRapid)
                    {
                        if (ModSettings.GetBool("Looping Rapid Mode"))
                        {
                            List<string> pathToRapidLooping = ["Audio", "Music", "RapidModeLooping"];
                            foreach (CustomTrigger x in flattenedList)
                            {
                                if (FindCustomSound(x.GetPath(pathToRapidLooping)))
                                {
                                    loop = true;
                                    loopString = cachedSound.Value;
                                    return cachedSound.Value;
                                }
                            }
                            if (FindCustomSound(pathToRapidLooping))
                            {
                                loop = true;
                                loopString = cachedSound.Value;
                                return cachedSound.Value;
                            }
                        }
                        List<string> rapidList = ogSoundPathNames.ShallowCopy();
                        if (customVelocityTriggers.Resolve(rapidList, true, "RapidMode")) return cachedSound.Value;
                        rapidList[2] = "RapidMode" + rapidList[2];
                        foreach (CustomTrigger x in flattenedList)
                        {
                            if (FindCustomSound(x.GetPath(rapidList))) return cachedSound.Value;
                        }
                        if (FindCustomSound(rapidList)) return cachedSound.Value;
                    }
                    else if (!string.IsNullOrEmpty(gameVelocity))
                    {
                        List<string> velocityList = ogSoundPathNames.ShallowCopy();
                        if (customVelocityTriggers.Resolve(velocityList, true, gameVelocity)) return cachedSound.Value;
                        foreach (CustomTrigger x in flattenedList)
                        {
                            if (FindCustomSound(x.GetPath(velocityList))) return cachedSound.Value;
                        }
                        if (FindCustomSound(velocityList)) return cachedSound.Value;
                    }
                }
                List<string> gameplayList = ogSoundPathNames.ShallowCopy();
                if (customGameplayTriggers.Resolve(gameplayList, true, null)) return cachedSound.Value;
                foreach (CustomTrigger x in flattenedList)
                {
                    if (FindCustomSound(x.GetPath(gameplayList))) return cachedSound.Value;
                }
            }
            else
            {
                List<string> nonGameplayList = ogSoundPathNames.ShallowCopy();
                if (customTriggers.Resolve(nonGameplayList, false, null)) return cachedSound.Value;
            }
        }
        if (FindCustomSound(ogSoundPathNames)) return cachedSound.Value; else return null;
    }
    public static bool Resolve(this List<CustomTrigger> triggers, List<string> path, bool useList, string velocity)
    {
        List<CustomTrigger> t = triggers.ShallowCopy();
        for (int i = 0; i < t.Count; i++)
        {
            CustomTrigger trigger = t[i];
            if (!trigger.IsActivated(path)) continue;
            List<string> resolve = trigger.GetPath(path, useList, velocity);
            if (resolve == null) continue;
            if (cachedSound.Key == resolve) return true;
            if (trigger.worksWithOthers && i != t.Count - 1)
            {
                //* for future me: im using a binary number to go through the list and in each iteration subtracting 1 from the num: ex. 1111 -> check full list, 1101 -> check list except the one before last.
                //? i spent three whole days on this. there aren't even enough triggers to use this LOL.
                var array = t.GetRange(i + 1, t.Count - 1 - i).Where(x => x.IsActivated(path) && x.worksWithOthers).ToArray();
                int n = array.Length;
                int max = 1 << n;
                for (int j = max - 1; j > 0; j--)
                {
                    List<string> resolveWorking = resolve.ShallowCopy();
                    List<CustomTrigger.Type> usedTypes = new() { trigger.type };
                    int reversedI = MiscUtils.ReverseBits(j, n);
                    for (int k = 0; k < n; k++)
                    {
                        if (usedTypes.Count == 3) break;
                        if ((reversedI & (1 << k)) == 0) continue;
                        CustomTrigger t2 = array[k];
                        if (usedTypes.Contains(t2.type)) continue;
                        usedTypes.Add(t2.type);
                        resolveWorking = t2.GetPath(resolveWorking, useList, velocity);
                    }
                    if (!string.IsNullOrEmpty(velocity)) resolveWorking[2] = velocity + resolveWorking[2];
                    if (useList)
                        foreach (CustomTrigger x in flattenedList)
                        {
                            if (!FindCustomSound(x.GetPath(resolveWorking, useList, velocity))) continue;
                            return true;
                        }
                    if (!FindCustomSound(resolveWorking)) continue;
                    return true;
                }
            }
            if (!string.IsNullOrEmpty(velocity)) resolve[2] = velocity + resolve[2];
            if (useList)
                foreach (CustomTrigger x in flattenedList)
                {
                    if (!FindCustomSound(x.GetPath(resolve))) continue;
                    return true;
                }
            if (!FindCustomSound(resolve)) continue;
            return true;
        }
        return false;
    }
    public static List<CustomTrigger> Flat(this List<CustomTrigger> triggers)
    {
        List<CustomTrigger> flattenedList = new();
        triggers.Where(x => x.IsActivated(null)).ForEach(x =>
        {
            if (x is GroupTriggers)
                flattenedList.AddRange(x.FullTriggers());
            else flattenedList.Add(x);
        });
        return flattenedList;
    }
    static void PrepareNonGameplayTriggers()
    {
        customTriggers.AddRange([
            new CustomTrigger(() => "DrawMusic", CustomTrigger.Type.Name, (s) => draw && s[2] == "CovenVictory"),
                new CustomTrigger(() => {if(win) return "VictoryMusic"; return "DefeatMusic";}, CustomTrigger.Type.Name, (s) => s[2] == "LoginMusicLoop_old")
        ]);
    }
    static void PrepareVelocityTriggers()
    {
        customVelocityTriggers.AddRange([
            new GroupTriggers([
                new CustomTrigger(()=>"Prosecutor", CustomTrigger.Type.Suffix, (s) => prosecutor),
                new CustomTrigger(()=>"Player", CustomTrigger.Type.Prefix, (s) => playerOnStand),
                new CustomTrigger(()=>"Target", CustomTrigger.Type.Prefix, (s) => roleData.role == Role.EXECUTIONER && targetOnStand && Pepper.AmIAlive())
            ], (s)=>s[2] == "Judgement"),
            new GroupTriggers([
                new CustomTrigger(() => "PartyMusic", CustomTrigger.Type.Name, (s) => isParty),
                new CustomTrigger(() => "JailedMusic", CustomTrigger.Type.Name, (s) => isJailed,callback: () => {isJailed = false;}),
                new CustomTrigger(() => "DueledMusic", CustomTrigger.Type.Name, (s) => isDueled,callback: () => {isDueled = false;})
            ], (s) => s[2] == "NightMusic", callback: () => {isTribunal = false;}),
            new CustomTrigger(()=>"TribunalMusic", CustomTrigger.Type.Name, (s) => s[2] == "VotingMusic" && isTribunal)
        ]);
    }
    static void PrepareGameplayTriggers()
    {
        customGameplayTriggers.AddRange([
            ..customVelocityTriggers,
                new CustomTrigger(()=>"NonBinary", CustomTrigger.Type.Name, (s) => isNB && s.Any(x => x.Contains("Male")), replacing: "Male")
        ]);
    }
    static void PrepareFolderTriggers()
    {
        customFolderTriggers.AddRange([
            new CustomTrigger(() => "Town Traitor", CustomTrigger.Type.Folder, (s) => isTT),
            new GroupTriggers([
                new GroupTriggers([
                    new CustomTrigger(() => {
                            switch (roleData.role)
                            {
                                case Role.SOULCOLLECTOR:
                                return "Death";
                                case Role.BERSERKER:
                                return "War";
                                case Role.PLAGUEBEARER:
                                return "Pestilence";
                                case Role.BAKER:
                                return "Famine";
                            }
                            return roleData.roleName;
                        }, CustomTrigger.Type.Folder, (s) => true),
                    new CustomTrigger(() => "TransformedHorseman", CustomTrigger.Type.Folder, (s) => true)
                ], (s) => Service.Game.Sim.info.roleCardObservation.Data.defense == 3 && roleData.factionType == FactionType.APOCALYPSE),
                new CustomTrigger(() => "Horsemen", CustomTrigger.Type.Folder, (s) => true)
            ],(s) => pest || war || death || fam),
            new CustomTrigger(() => "Dead", CustomTrigger.Type.Folder, (s) => !Pepper.AmIAlive()),
            new CustomTrigger(() => alignment, CustomTrigger.Type.Folder, (s) => true),
            new CustomTrigger(() => $"{alignment} {roleData.subAlignment.ToString().ToTitleCase()}", CustomTrigger.Type.Folder, (s) => true),
            new CustomTrigger(() => roleData.roleName, CustomTrigger.Type.Folder, (s) => true)
        ]);
    }
    static void PrepareTriggers()
    {
        PrepareFolderTriggers();
        PrepareNonGameplayTriggers();
        PrepareVelocityTriggers();
        PrepareGameplayTriggers();
    }
    public static bool FindCustomSound(List<string> soundPathArray)
    {
        if (soundPathArray == null) return false;
        if (soundPathArray == cachedSound.Key) return true;
        List<string> list = soundPathArray.ShallowCopy();
        if (list[0] != "Audio") list.Insert(0, soundpack);
        else list[0] = soundpack;
        list.Insert(0, directoryPath);
        string soundPath = Path.Combine(list.ToArray());

        string[] files;
        string dir = Path.GetDirectoryName(soundPath);
        if (!Directory.Exists(dir)) goto CheckExtension;
        files = Directory.GetFiles(dir, Path.GetFileName(soundPath) + ".*");
        if (files.Length < 1) goto CheckExtension;
        if(files.Length > 1 && ModSettings.GetBool("Allow Randomized Tracks","JAN.soundpacks")){
            System.Random r = new();
            
            cachedSound.SetValues(soundPathArray, files[r.Next(files.Length)]);
        }
        else cachedSound.SetValues(soundPathArray, files[0]);
        return true;

    CheckExtension:
        if (string.IsNullOrEmpty(curExtension)) return false;
        string extensionPath = soundPath.Replace(soundpack, curExtension);
        dir = Path.GetDirectoryName(extensionPath);
        if (!Directory.Exists(dir)) return false;
        files = Directory.GetFiles(dir, Path.GetFileName(extensionPath) + ".*");
        if (files.Length < 1)
        {
            return false;
        }
        cachedSound.SetValues(soundPathArray, files[0]);
        return true;
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
        Debug.LogWarning(soundpacks.TryGetValue(selection, out string llll));

        Debug.Log(llll);
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
        string curMusic = a.currentMusicSound;
        a.StopMusic();
        a.PlayMusic(curMusic);
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
        PrepareTriggers();
        directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "SalemModLoader", "ModFolders", "Soundpacks");
        Debug.Log(directoryPath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        Debug.Log("Working?");
        SoundpacksDebugger.GetInstance();
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
public class GroupTriggers : CustomTrigger
{
    public GroupTriggers(List<CustomTrigger> triggers, Func<List<string>, bool> GroupCondition, bool checkForFolders = false, Action callback = null)
    : base(null, Type.Group, GroupCondition, callback, false)
    {
        this.triggers = triggers;
        this.checkForFolders = checkForFolders;
    }
    public bool checkForFolders;
    public List<CustomTrigger> triggers;
    override public List<string> GetPath(List<string> path, bool useList = true, string velocity = null)
    {
        if (callback != null) callback();
        bool smth = triggers.Resolve(path, useList, velocity);
        if (smth) return SoundpackUtils.cachedSound.Key;
        else return null;
    }
    public override List<CustomTrigger> FullTriggers()
    {
        return triggers.Flat();
    }
}
public class CustomTrigger
{
    public Func<string> info;
    public enum Type
    {
        Prefix,
        Suffix,
        Name,
        Folder,
        Group
    };
    public Type type;
    public bool worksWithOthers;
    public Func<List<string>, bool> IsActivated;
    public Action callback;
    public string replacing;
    public Action callbackIfFound;
    public CustomTrigger(Func<string> info, Type type, Func<List<string>, bool> activated, Action callback = null, bool worksWithOthers = false, string replacing = null, Action callbackIfFound = null)
    {
        this.info = info;
        this.type = type;
        IsActivated = activated;
        this.callback = callback;
        this.worksWithOthers = worksWithOthers;
        this.replacing = replacing;
        this.callbackIfFound = callbackIfFound;
    }
    virtual public List<string> GetPath(List<string> path, bool useList = true, string velocity = null)
    {
        List<string> r = path.ShallowCopy();
        if (callback != null) callback();

        switch (type)
        {
            case Type.Name:
                if (replacing != null)
                {
                    r[path.Count - 1] = r[r.Count - 1].Replace(replacing, info());
                    return r;
                }
                r[r.Count - 1] = info();
                return r;
            case Type.Suffix:
                r[r.Count - 1] = path.Last() + info();
                return r;
            case Type.Prefix:
                r[r.Count - 1] = info() + path.Last();
                return r;
            case Type.Folder:
                r[0] = info();
                return r;
        }
        return null;
    }
    virtual public List<CustomTrigger> FullTriggers()
    {
        return null;
    }
}