using HarmonyLib;
using Home.Shared;

using System;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using Utils;
namespace SoundpackPatchs{
    [HarmonyPatch(typeof(AudioController), "StopMusic")]
    public class StopMusicPatch{        
        
        public static bool Prefix(){
            
            if(SoundpackUtils.loop) return false;
            PlayMusicPatch.moddedMusic = "";
            return true;
        }
		
}
}