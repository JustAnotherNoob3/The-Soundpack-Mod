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
            OnClick = SoundpackUtils.OpenSoundpackDirectory
        };
    }
    [DynamicSettings]
    public class Settings
    {
        public ModSettings.DropdownSetting SelectedSoundpack
        {
            get
            {
                ModSettings.DropdownSetting SelectedSoundpack = new()
                {
                    Name = "Selected Soundpack",
                    Description = "The soundpack you are going to listen to. Restart the game for this option to update correctly if you made any changes to the organization of folders.",
                    Options = SoundpackUtils.GetSoundpacks(),
                    AvailableInGame = true,
                    Available = true,
                    OnChanged = (s) => SoundpackUtils.LoadSoundpack(s)
                };
                return SelectedSoundpack;
            }
        }
        public ModSettings.DropdownSetting SelectedSubFolder
        {
            get
            {
                ModSettings.DropdownSetting SelectedSubFolder = new()
                {
                    Name = "Subfolder to randomize",
                    Description = "The mod will only randomize soundpacks inside this folder. Restart the game for this option to update correctly if you made any changes to the organization of folders.",
                    Options = SoundpackUtils.GetSubfolders(),
                    AvailableInGame = false,
                    Available = true
                };
                return SelectedSubFolder;
            }
        }
    }
}


