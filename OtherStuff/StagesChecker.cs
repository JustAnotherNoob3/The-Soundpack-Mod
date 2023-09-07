using Utils;
using HarmonyLib;
using SML;
using Cinematics.Players;
using UnityEngine;
using Server.Shared.State;
using Server.Shared.Cinematics.Data;
using Server.Shared.Extensions;

namespace OtherStuff{
    [HarmonyPatch(typeof(ProsecutionCinematicPlayer), "Init")]
    class ProsecutionCinematicPlayer_Init_Patch{
        public static void Prefix(){
            AudioController a = Object.FindObjectOfType<AudioController>();
                a.StopMusic("Audio/Music/Judgement");
                
        }
    }
    [HarmonyPatch(typeof(ProsecutionCinematicPlayer), "Cleanup")]
    class ProsecutionCinematicPlayer_Cleanup_Patch{
        public static void Prefix(){
            AudioController a = Object.FindObjectOfType<AudioController>();
            a.PlayMusic("Audio/Music/Judgement");
        }
    }
    
    [HarmonyPatch(typeof(FactionWinsCinematicPlayer), "Init")]
    class EndGame {
        public static void Prefix(ICinematicData cinematicData){
            SoundpackUtils.isRapid = false;
            SoundpackUtils.loop = false;
            if(((FactionWinsCinematicData)cinematicData).winningFaction == FactionType.NONE){
                SoundpackUtils.draw = true;
            }
            
        }
    }
    [HarmonyPatch(typeof(WrapupCinematicPlayer), "Init")]
    class WrapupCinematicPlayer_Init_Patch {
        public static void Prefix(ICinematicData a_cinematicData){
            SoundpackUtils.win = ((WrapUpCinematicData)a_cinematicData).didWin.GetElement(Pepper.GetMyPosition());
            AudioController a = Object.FindObjectOfType<AudioController>();
                a.StopMusic();
                a.PlayMusic("Audio/Music/LoginMusicLoop_old");
        }
    }
}