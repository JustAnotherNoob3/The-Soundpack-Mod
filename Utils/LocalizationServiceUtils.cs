using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Home.Services;
using System.Collections.Generic;


namespace Utils{
[HarmonyPatch(typeof(HomeLocalizationService), "LoadStringTable")]
public class StringTableModifier{
    [HarmonyPostfix]
    public static void AddToStringTable(HomeLocalizationService __instance){
    Dictionary<string, string> dictionary = (Dictionary<string, string>)__instance.GetType().GetField("stringTable_", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);
    
}
}
}
        
