using HarmonyLib;
using Home.Shared;
using SML;
using System;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using Utils;

namespace SoundpackPatchs{
    [HarmonyPatch(typeof(AudioController), "PlaySoundEffect")]
    public class PlaySoundEffectPatch{        
        
        public static bool Prefix(AudioController __instance, string sound, bool randomizePitch, float minPitch, float maxPitch){
            if(ModSettings.GetBool("Deactivate Custom SFX")){
                return true;
            }
            string modSound = SoundpackUtils.GetCustomSound(sound);
            if(sound == modSound)
            {
                return true;
            }
            if (ApplicationController.ApplicationContext.disableSound)
		    {
			    return false;
		    }
            __instance.StartCoroutine(LoadSoundEffectAudioFile(modSound, randomizePitch, minPitch, maxPitch, __instance));
            return false;
        }
		public static IEnumerator LoadSoundEffectAudioFile(string path, bool randomizePitch, float minPitch, float maxPitch,  AudioController instance)
    {
        string extension = path.Substring(path.LastIndexOf('.'));
    using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://"+path, SoundpackUtils.GetAudioType(extension)))
    {
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
    {
        AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
        OnSoundEffectAudioClipLoaded(audioClip, randomizePitch, minPitch, maxPitch,  instance);
    }
    else
    {
        Console.WriteLine("Error al cargar archivo de audio: " + www.error);
        OnSoundEffectAudioClipLoaded(null, randomizePitch, minPitch, maxPitch,  instance);
    }
    }
}
public static void OnSoundEffectAudioClipLoaded(AudioClip audioClip, bool randomizePitch, float minPitch, float maxPitch,  AudioController instance){
	if(audioClip == null) return;
	if (randomizePitch)
	{
		instance.EffectsSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
	}
	else
	{
		instance.EffectsSource.pitch = 1f;
	}
	instance.EffectsSource.PlayOneShot(audioClip);

}
			
}
}