using HarmonyLib;
using Home.Shared;

using System;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using Utils;
namespace SoundpackPatchs{
    [HarmonyPatch(typeof(AudioController), "PlaySoundEffect")]
    public class StopMusicPatch{        
        
        public static bool Prefix(){
            if(SoundpackUtils.loop) return false; else return true;
        }
		
}
}