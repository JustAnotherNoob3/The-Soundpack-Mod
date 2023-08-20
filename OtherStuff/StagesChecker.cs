using Utils;
using HarmonyLib;
using SML;
using Cinematics.Players;
using Server.Shared.Info;
using UnityEngine;
using Server.Shared.Messages;
using Server.Shared.State.Chat;
using Server.Shared.State;

namespace OtherStuff{
    [HarmonyPatch(typeof(ProsecutionCinematicPlayer), "Init")]
    class ProsecutionCinematicPlayer_Init_Patch{
        public static void Prefix(){
            SoundpackUtils.prosecutor = true;
        }
    }
    [HarmonyPatch(typeof(ChatAvailableStagesObservation), "StartTrialAsDefendant")]
    class Trial{
        public static void Prefix(){
            SoundpackUtils.playerOnStand = true;
        }
    }
    [HarmonyPatch(typeof(Game.Interface.HudEndGame), "HandleGameResults")]
    class EndGame {
        public static void Prefix(GameResults results){
            SoundpackUtils.draw = results.winType == WinType.DRAW;
            SoundpackUtils.loop = false;
        }
    }
}