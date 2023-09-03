using Utils;
using HarmonyLib;
using SML;
using Cinematics.Players;
using Server.Shared.Info;
using UnityEngine;
using Server.Shared.Messages;
using Server.Shared.State.Chat;
using Server.Shared.State;
using Server.Shared.Cinematics.Data;
using Services;

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
}