using Utils;
using HarmonyLib;
using SML;
using SRandom = System.Random;
using UObject = UnityEngine.Object;

namespace OtherStuff;

[HarmonyPatch(typeof(Game.Interface.PickNamesPanel), "Start")]
public static class InitGame
{
    public static void Prefix()
    {
        //! IF SOMEONE REPORTS THE RAPID STUFF FOLLOWING OTHER GAME BE SURE TO MAKE LOOP FALSE
        SoundpackUtils.Horsemen = new();
        SoundpackUtils.DayOne = true;
        SoundpackUtils.TargetOnStand = false;
        SoundpackUtils.PlayerOnStand = false; 
        SoundpackUtils.Prosecutor = false;
        SoundpackUtils.Draw = false;
        SoundpackUtils.IsTT = false;

        if (ModSettings.GetBool("Randomize Soundpacks") && SoundpackUtils.Directories.Count > 0)
        {
            var r = new SRandom();
            ModSettings.SetString("Selected Soundpack", SoundpackUtils.Directories[r.Next(0, SoundpackUtils.Directories.Count)]);
            var a = UObject.FindObjectOfType<AudioController>();
            a.StopMusic();
            a.PlayMusic("Audio/Music/SelectionMusic");
        }
    }
}