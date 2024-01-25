using HarmonyLib;
using Home.Shared;
using SML;
using System;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using Utils;
using System.Collections.Generic;

namespace SoundpackPatchs
{
    [HarmonyPatch(typeof(AudioController), "PlaySoundEffect")]
    public class PlaySoundEffectPatch
    {
        public static Dictionary<string, AudioClip> memory = new();

        public static bool Prefix(AudioController __instance, string sound, bool randomizePitch, float minPitch, float maxPitch)
        {
            string notSound = sound.Contains(".") ? sound.Remove(sound.IndexOf('.')) : sound;
            notSound = notSound.Contains("/") ? notSound : "Audio/BetterTOS2/" + notSound;
            if (ModSettings.GetBool("Deactivate Custom SFX", "JAN.soundpacks"))
            {
                return true;
            }
            string modSound = SoundpackUtils.GetCustomSound(notSound);
            if (notSound == modSound)
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
        public static IEnumerator LoadSoundEffectAudioFile(string path, bool randomizePitch, float minPitch, float maxPitch, AudioController instance)
        {
            string extension = path.Substring(path.LastIndexOf('.'));
            if (memory.ContainsKey(path))
            {
                OnSoundEffectAudioClipLoaded(memory[path], randomizePitch, minPitch, maxPitch, instance);
            }
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + path, SoundpackUtils.GetAudioType(extension)))
            {
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.Success)
                {
                    AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                    memory.Add(path, audioClip);
                    OnSoundEffectAudioClipLoaded(audioClip, randomizePitch, minPitch, maxPitch, instance);
                }
                else
                {
                    Console.WriteLine("Error al cargar archivo de audio: " + www.error);
                    OnSoundEffectAudioClipLoaded(null, randomizePitch, minPitch, maxPitch, instance);
                }
            }
        }
        public static void OnSoundEffectAudioClipLoaded(AudioClip audioClip, bool randomizePitch, float minPitch, float maxPitch, AudioController instance)
        {
            if (audioClip == null) return;
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