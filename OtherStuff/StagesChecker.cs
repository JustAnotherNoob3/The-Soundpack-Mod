using Utils;
using HarmonyLib;
using Cinematics.Players;
using UnityEngine;
using Server.Shared.State;
using Server.Shared.Cinematics.Data;
using Server.Shared.Extensions;

namespace OtherStuff;

[HarmonyPatch(typeof(ProsecutionCinematicPlayer), "Init")]
public static class ProsecutionCinematicPlayer_Init_Patch
{
    public static void Prefix() => Object.FindObjectOfType<AudioController>()?.StopMusic("Audio/Music/Judgement");
}

[HarmonyPatch(typeof(ProsecutionCinematicPlayer), "Cleanup")]
public static class ProsecutionCinematicPlayer_Cleanup_Patch
{
    public static void Prefix() => Object.FindObjectOfType<AudioController>()?.PlayMusic("Audio/Music/Judgement");
}

[HarmonyPatch(typeof(FactionWinsCinematicPlayer), "Init")]
public static class EndGame
{
    public static void Prefix(ICinematicData cinematicData)
    {
        SoundpackUtils.IsRapid = false;
        SoundpackUtils.Loop = false;

        if (((FactionWinsCinematicData)cinematicData).winningFaction == FactionType.NONE)
            SoundpackUtils.Draw = true;
    }
}

[HarmonyPatch(typeof(WrapupCinematicPlayer), "Init")]
public static class WrapupCinematicPlayer_Init_Patch
{
    public static void Prefix(ICinematicData a_cinematicData)
    {
        SoundpackUtils.Win = ((WrapUpCinematicData)a_cinematicData).didWin.GetElement(Pepper.GetMyPosition());
        var a = Object.FindObjectOfType<AudioController>();
        a.StopMusic();
        a.PlayMusic("Audio/Music/LoginMusicLoop_old");
    }
}