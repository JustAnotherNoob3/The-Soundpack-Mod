using Utils;
using HarmonyLib;
using SML;
using UnityEngine;
using System.Collections.Generic;
using Services;
using UnityEngine.UI;
using Server.Shared.State;
using System.Linq;
using Game.Interface;
using Home.Common;
using System;
using TMPro;
using System.Collections;

namespace OtherStuff
{
    [HarmonyPatch(typeof(Game.Interface.PickNamesPanel), "Start")]
    class InitGame
    {
        static public void Postfix(PickNamesPanel __instance)
        {
            SoundpackUtils.horsemen = new();
            SoundpackUtils.dayOne = true;
            SoundpackUtils.targetOnStand = false;
            SoundpackUtils.playerOnStand = false;
            SoundpackUtils.prosecutor = false;
            SoundpackUtils.draw = false;
            SoundpackUtils.isTT = false;
            SoundpackUtils.loop = false;
            if (ModSettings.GetBool("Randomize Soundpacks") && SoundpackUtils.soundpacks.Count > 0)
            {
                System.Random r = new();
                string randomized = "";
                if (ModSettings.GetString("Subfolder to randomize", "JAN.soundpacks") == "No Subfolder")
                {
                    randomized = SoundpackUtils.soundpacks.ToList()[r.Next(0, SoundpackUtils.soundpacks.Count)].Key;
                }
                else
                {
                    List<string> soundpacksInSubfolder = SoundpackUtils.soundpacks.Where(x => x.Value == ModSettings.GetString("Subfolder to randomize", "JAN.soundpacks")).Select(x => x.Key).ToList();
                    randomized = soundpacksInSubfolder[r.Next(0, soundpacksInSubfolder.Count)];
                }
                GameObject nowHearingText = UnityEngine.Object.Instantiate(__instance.transform.GetChild(0).GetChild(0).GetChild(0).gameObject, __instance.transform.parent);
                nowHearingText.transform.localPosition = Vector3.zero;
                nowHearingText.transform.localScale = new Vector3(2.25f, 2.25f);
                ((RectTransform)nowHearingText.transform).anchorMin = new(0.83f, 0.135f);
                ((RectTransform)nowHearingText.transform).anchorMax = new(0.83f, 0.135f);
                ((RectTransform)nowHearingText.transform).anchoredPosition = new(725f, 0f);
                UnityEngine.Object.Destroy(nowHearingText.transform.GetChild(0).GetComponent<TMProLocalizedTextController>());
                nowHearingText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"Now Playing...\n";
                __instance.StartCoroutine(TextCoroutine(nowHearingText, randomized));
                ModSettings.SetString("Selected Soundpack", randomized);
                Console.WriteLine($"Randomized to: {ModSettings.GetString("Selected Soundpack")}");
                SoundpackUtils.LoadSoundpack(randomized);

            }
        }
        public static IEnumerator TextCoroutine(GameObject text, string stringToAdd)
        {
            yield return new WaitForSeconds(2.25f);
            for (int i = 0; i < 50; i++)
            {
                yield return new WaitForSeconds(0.02f);
                ((RectTransform)text.transform).anchoredPosition -= new Vector2(14.5f, 0f);
            }
            TextMeshProUGUI t = text.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            yield return new WaitForSeconds(1.5f);
            for (int j = 0; j < stringToAdd.Length; j++)
            {
                yield return new WaitForSeconds(0.03f);
                t.text += stringToAdd[j];
            }
        }
    }
}