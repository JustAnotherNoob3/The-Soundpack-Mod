using Utils;
using SML;
using System;
using UnityEngine;
using System.IO;
using static SML.Mod;

namespace Main;

[SalemMod]
public class SoundPacks
{
    public static string ModPath => Path.Combine(Directory.GetCurrentDirectory(), "SalemModLoader", "ModFolders", "Soundpacks");

    public static void Start()
    {
        Debug.Log(ModPath);

        if (!Directory.Exists(ModPath))
            Directory.CreateDirectory(ModPath);

        var fullDirectories = Directory.GetDirectories(ModPath);

        foreach (var dir in fullDirectories)
        {
            Debug.Log(Path.GetFileName(dir));
            SoundpackUtils.Directories.Add(Path.GetFileName(dir));
        }

        Console.WriteLine("Working?");
    }
    
}

[SalemMenuItem]
public class MenuItem
{
   public static Mod.SalemMenuButton menuButtonName = new()
   {
      Label = "Soundpacks",
      Icon = FromResources.LoadSprite("TheSoundpackMod.Resources.MusicButton.png"),
      OnClick = OpenSoundpackDirectory
   };

    public static void OpenSoundpackDirectory()
    {
        if (Environment.OSVersion.Platform is PlatformID.MacOSX or PlatformID.Unix)
            System.Diagnostics.Process.Start("open", $"\"{SoundPacks.ModPath}\""); //code stolen from tuba
        else
            Application.OpenURL($"file://{SoundPacks.ModPath}");

    }
}