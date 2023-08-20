using HarmonyLib;
using Utils;
using SML;
using System;
using UnityEngine;
using System.Reflection;

namespace Main
{
    [Mod.SalemMod]
    public class Main
    {
        public static void Start()
            {
            Debug.Log("Working?");
            SoundpackUtils.ForSoundpackMod();     
            }
        
    }
[Mod.SalemMenuItem]
public class MenuItem
{
   public static Mod.SalemMenuButton menuButtonName = new()
   {
      Label = "Soundpacks",
      Icon = FromResources.LoadSprite("TheSoundpackMod.resources.images.MusicButton.png"),
      OnClick = () =>
      {
         SoundpackUtils.OpenSoundpackDirectory();
      }
   };
}

}
