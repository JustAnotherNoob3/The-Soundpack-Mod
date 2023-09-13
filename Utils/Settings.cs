using SML;

namespace Utils;

public static class Settings
{
    public static string CurrentSoundPack => ModSettings.GetString("Selected Soundpack", "JAN.soundpacks");
}