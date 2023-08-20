using HarmonyLib;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Audio;
using System.Reflection;
using Utils;
using Home.Shared;
using UnityEngine;
using Services;

namespace SoundpackPatchs{
    
        [HarmonyPatch(typeof(AudioController), "PlayMusic")]
        public static class PlayMusicPatch{
            public static bool Prefix(AudioController __instance, string sound, bool loop, AudioController.AudioChannel channel, bool stopAllMusic){
                string modSound = SoundpackUtils.GetCustomSound(sound);
                if (modSound == sound) return true;
                loop = SoundpackUtils.loop;
                if (ApplicationController.ApplicationContext.disableSound)
		        {
			        return false;
		        }
                //TODO: check if sound is already playing via singleton instead of IsMusicPlaying() (Only if this doesn't work (Probably won't)).
                //* nevermind it does, dont check shit.
                if (__instance.IsMusicPlaying(modSound))
		        {
			        return false;
		        }
                __instance.StopAllCoroutines();
                if(stopAllMusic){
                    __instance.StopMusic();
                }
		        AudioController.AudioTrack audioTrack = new AudioController.AudioTrack();
                __instance.currentMusicSound = modSound;
	            audioTrack.source = __instance.MusicSource;
	            audioTrack.channel = channel;
                Dictionary<string, AudioController.AudioTrack> dictionary = (Dictionary<string, AudioController.AudioTrack>)__instance.GetType().GetField("MusicTracks", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);
                Dictionary<string, AudioClip> audioClipCache = (Dictionary<string, AudioClip>)__instance.GetType().GetField("audioClipCache", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);
                if (dictionary.ContainsKey(modSound))
	            {
		            dictionary[modSound].source.Stop();
		            dictionary[modSound].source.clip = null;
		            dictionary.Remove(modSound);
	            }
                dictionary.Add(modSound, audioTrack);
		        audioTrack.source.loop = loop;
		        audioTrack.source.volume = ((channel == AudioController.AudioChannel.Music) ? Service.Home.UserService.Settings.MusicVolume : Service.Home.UserService.Settings.SoundEffectsVolume);
		        audioTrack.source.mute = ((channel == AudioController.AudioChannel.Music) ? Service.Home.UserService.Settings.MusicMuted : Service.Home.UserService.Settings.SoundEffectsMuted);
                foreach (KeyValuePair<string, AudioClip> item in audioClipCache)
		        {
			        Debug.Log("Key: "+item.Key+", Value: "+item.Value);
		        }
                if(audioClipCache.ContainsKey(modSound)){
                    audioTrack.source.clip = audioClipCache[modSound];
                        audioTrack.source.Play();
                        return false;
                } else {
                    __instance.StartCoroutine(LoadMusicAudioFile(modSound, __instance, audioClipCache, audioTrack, sound));
                    return false;
                }
    
}
public static IEnumerator LoadMusicAudioFile(string path,  AudioController instance, Dictionary<string, AudioClip> audioClipCache, AudioController.AudioTrack audioTrack, string ogSound)
    {
        string extension = path.Substring(path.LastIndexOf('.'));
    using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, SoundpackUtils.GetAudioType(extension)))
    {
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
    {
        AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
        audioClipCache.Add(path, audioClip);
        OnMusicAudioClipLoaded(audioClip,  instance, audioTrack, ogSound);
    }
    else
    {
        Debug.LogError("Error al cargar archivo de audio: " + www.error);
        OnMusicAudioClipLoaded(null,   instance, audioTrack, ogSound);
    }
    }
}
public static void OnMusicAudioClipLoaded(AudioClip audioClip,  AudioController instance, AudioController.AudioTrack audioTrack, string ogSound)
    {
        if(audioClip == null) return;
        if(audioTrack != null && instance.currentMusicSound == ogSound){
            if(audioTrack.source){
				audioTrack.source.clip = audioClip;
			    audioTrack.source.Play();
				return;  
            }
        }
    }
}
}
