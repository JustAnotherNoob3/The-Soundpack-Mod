using Utils;
using HarmonyLib;
using SML;
using Cinematics.Players;
using UnityEngine;
using Server.Shared.State;
using Server.Shared.Cinematics.Data;
using Server.Shared.Extensions;
using Services;
using Server.Shared.Info;
using Game.Services;

namespace OtherStuff{
    [HarmonyPatch(typeof(ProsecutionCinematicPlayer), "Init")]
    class ProsecutionCinematicPlayer_Init_Patch{
        public static void Prefix(){
            Service.Home.AudioService.StopMusic("Audio/Music/Judgement");
                
        }
    }
    [HarmonyPatch(typeof(ProsecutionCinematicPlayer), "Cleanup")]
    class ProsecutionCinematicPlayer_Cleanup_Patch{
        public static void Prefix(){
            Service.Home.AudioService.PlayMusic("Audio/Music/Judgement");
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
    [HarmonyPatch(typeof(TribunalCinematicPlayer), "Init")]
    class IsTribunal {
        public static void Prefix(){
            SoundpackUtils.isTribunal = true;
            SoundpackUtils.isRapid = false;
            SoundpackUtils.loop = false;
        }
    }
    [HarmonyPatch(typeof(TribunalCinematicPlayer), "Cleanup")]
    class TribunalPlayMusic {
        public static void Prefix(){
            Service.Home.AudioService.PlayMusic("Audio/Music/VotingMusic");
        }
    }
    [HarmonyPatch(typeof(AttackedCinematicPlayer), "Init")]
    class CheckAttack {
        public static void Prefix(ICinematicData cinematic){
            AttackedCinematicData attackCinematicData = cinematic as AttackedCinematicData;
            SoundpackUtils.isNB = Service.Game.Cast.GetCharacterByPosition(attackCinematicData.attackedPosition).characterModel.gender == Gender.Other;
        }
    }
    [HarmonyPatch(typeof(WrapupCinematicPlayer), "Init")]
    class WrapupCinematicPlayer_Init_Patch {
        public static void Prefix(ICinematicData a_cinematicData){
            SoundpackUtils.win = ((WrapUpCinematicData)a_cinematicData).didWin.GetElement(Pepper.GetMyPosition());
            SoundpackUtils.isRapid = false;
            SoundpackUtils.loop = false;
                Service.Home.AudioService.StopMusic();
                Service.Home.AudioService.PlayMusic("Audio/Music/LoginMusicLoop_old");
        }
    }
    [HarmonyPatch(typeof(RankedWrapUpCinematicPlayer), "Init")]
    class RankedWrapupCinematicPlayer_Init_Patch {
        public static void Prefix(ICinematicData cinematicData){
            SoundpackUtils.win = ((RankedWrapUpCinematicData)cinematicData).didWin.GetElement(Pepper.GetMyPosition());
            SoundpackUtils.isRapid = false;
            SoundpackUtils.loop = false;
            Service.Home.AudioService.StopMusic();
            Service.Home.AudioService.PlayMusic("Audio/Music/LoginMusicLoop_old");
        }
    }
    [HarmonyPatch(typeof(MayorRevealCinematicPlayer), "Init")]
    class MayorRevealPatch{
        public static void Prefix(){
            SoundpackPatchs.PlayMusicPatch.moddedMusic = "";
        }
    }
}