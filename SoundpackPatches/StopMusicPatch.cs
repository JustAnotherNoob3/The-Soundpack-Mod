using HarmonyLib;
using Utils;

namespace SoundpackPatches;

[HarmonyPatch(typeof(AudioController), "StopMusic")]
public class StopMusicPatch
{        
    public static bool Prefix()
    {
        if (SoundpackUtils.Loop)
            return false;

        PlayMusicPatch.ModdedMusic = "";
        return true;
    }
}