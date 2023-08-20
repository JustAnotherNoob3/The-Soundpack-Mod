using Utils;
using HarmonyLib;
using SML;
using UnityEngine;
using System.Collections.Generic;
using Services;
using Server.Shared.State;

namespace OtherStuff{
    [HarmonyPatch(typeof(Game.Interface.PickNamesPanel), "Start")]
    class InitGame{
        static public void Prefix(){
            SoundpackUtils.horsemen = 0;
            SoundpackUtils.dayOne = true;
            SoundpackUtils.targetOnStand = false;
            SoundpackUtils.playerOnStand = false; 
            SoundpackUtils.prosecutor = false;
            SoundpackUtils.draw = false;
            if(ModSettings.GetBool("Randomize Soundpacks") && SoundpackUtils.directories.Count > 0){
                System.Random r = new();
                ModSettings.SetString("Selected Soundpack", SoundpackUtils.directories[r.Next(SoundpackUtils.directories.Count)]);
                AudioController a = Object.FindObjectOfType<AudioController>();
                a.StopMusic();
                a.PlayMusic("Audio/Music/SelectionMusic");
            }
            if(ModSettings.GetBool("Deactivate Custom Triggers")) return;
            List<Role> modifiers = Service.Game.Sim.simulation.roleDeckBuilder.Data.modifierCards;
            if(modifiers.Contains(Role.FAST_MODE)){
                SoundpackUtils.gameVelocity = "FastMode";
            } else if(modifiers.Contains(Role.SLOW_MODE)){
                SoundpackUtils.gameVelocity = "SlowMode";
            } else {
                SoundpackUtils.gameVelocity = "";
            }
        }
    }
}