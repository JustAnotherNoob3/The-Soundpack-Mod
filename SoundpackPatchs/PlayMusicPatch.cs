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
using System;

namespace SoundpackPatchs
{

    [HarmonyPatch(typeof(AudioController), "PlayMusic")]
    public static class PlayMusicPatch
    {
        public static string moddedMusic = "";
        public static bool Prefix(AudioController __instance, string sound, bool loop, AudioController.AudioChannel channel, bool stopAllMusic)
        {
            string notSound = sound.Contains(".") ? sound.Remove(sound.IndexOf('.')) : sound;
            string modSound = SoundpackUtils.GetCustomSound(notSound);
            if (notSound == modSound) return true;

            if (ApplicationController.ApplicationContext.disableSound)
            {
                return false;
            }
            //TODO: check if sound is already playing via singleton instead of IsMusicPlaying() (Only if this doesn't work (Probably won't)).
            //* nevermind it does, dont check shit.
            if (__instance.IsMusicPlaying(sound) || (moddedMusic == modSound && Pepper.IsGamePhasePlay()))
            {
                Debug.Log($"{modSound} is already playing.");
                return false;
            }
            __instance.StopAllCoroutines();
            if (stopAllMusic)
            {
                __instance.StopMusic();
            }
            AudioController.AudioTrack audioTrack = new AudioController.AudioTrack();
            __instance.currentMusicSound = sound;
            if(sound != "")moddedMusic = modSound;
            audioTrack.source = __instance.MusicSource;
            audioTrack.channel = channel;
            audioTrack.source.loop = loop;
            audioTrack.source.volume = ((channel == AudioController.AudioChannel.Music) ? Service.Home.UserService.Settings.MusicVolume : Service.Home.UserService.Settings.SoundEffectsVolume);
            audioTrack.source.mute = ((channel == AudioController.AudioChannel.Music) ? Service.Home.UserService.Settings.MusicMuted : Service.Home.UserService.Settings.SoundEffectsMuted);
            __instance.StartCoroutine(LoadMusicAudioFile(modSound, __instance, audioTrack, sound));
            return false;
        }
        public static IEnumerator LoadMusicAudioFile(string path, AudioController instance, AudioController.AudioTrack audioTrack, string sound)
        {
            string extension = path.Substring(path.LastIndexOf('.'));
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, SoundpackUtils.GetAudioType(extension)))
            {
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.Success)
                {
                    AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                    OnMusicAudioClipLoaded(audioClip, instance, audioTrack, sound);
                }
                else
                {
                    Console.WriteLine("Error al cargar archivo de audio: " + www.error);
                    OnMusicAudioClipLoaded(null, instance, audioTrack, sound);
                }
            }
        }
        public static void OnMusicAudioClipLoaded(AudioClip audioClip, AudioController instance, AudioController.AudioTrack audioTrack, string sound)
        {
            if (audioClip == null) return;
            if (audioTrack != null && instance.currentMusicSound == sound)
            {
                if (audioTrack.source)
                {
                    audioTrack.source.clip = audioClip;
                    audioTrack.source.Play();
                    return;
                }
            }
        }
    }
}
