using Utils;
using HarmonyLib;
using Services;
using Server.Shared.Messages;
using Server.Shared.State.Chat;
using Game.Interface;
using Server.Shared.State;
using Game.Simulation;
using SoundpackPatches;
using System;
using Game.Chat;

namespace OtherStuff;

[HarmonyPatch(typeof(PooledChatController), "AddMessage")]
public static class ChatMsgChecker
{
    public static void Prefix(ChatLogMessage message)
    {
        if (message.chatLogEntry.type == ChatType.GAME_MESSAGE)
        {
            var entry = (ChatLogGameMessageEntry)message.chatLogEntry;
            var msg = entry.messageId.ToString();

            if (msg.Contains("HAS_EMERGED"))
            {
                PlayMusicPatch.ModdedMusic = "";
                SoundpackUtils.Horsemen.Add((Role)Enum.Parse(typeof(Role), msg.Split('_')[0]));
                SoundpackUtils.Loop = false;
            }

            if (entry.messageId == GameFeedbackMessage.RAPID_MODE_STARTING)
                SoundpackUtils.IsRapid = true;

            if (entry.messageId == GameFeedbackMessage.DAYS_LEFT_TO_FIND_TRAITOR)
                SoundpackUtils.IsTT = true;
        }
    }
}

[HarmonyPatch(typeof(HudGraveyardPanel), "CreateItem")]
public static class HorsemenDiedCheck
{
    public static void Prefix(KillRecord killRecord)
    {
        SoundpackUtils.Prosecutor = false;

        if (killRecord.playerRole is Role.FAMINE or Role.WAR or Role.PESTILENCE or Role.DEATH)
            SoundpackUtils.Horsemen.DeleteAll(killRecord.playerRole);
    }
}

[HarmonyPatch(typeof(WhoDiedAndHowPanel), "HandleSubphaseWhoDied")]
public static class LoopCheck
{
    public static void Prefix()
    {
        SoundpackUtils.IsRapid = false;

        if (SoundpackUtils.Loop)
        {
            SoundpackUtils.Loop = false;
            var a = UnityEngine.Object.FindObjectOfType<AudioController>();
            a.StopMusic();
            a.PlayMusic("Audio/Music/WDAH");
        }
    }
}

[HarmonyPatch(typeof(GameSimulation), "HandleTrialState")]
public static class Trial
{
    public static void Prefix(TrialStateMessage message)
    {
        if (message.trialData.trialPhase == TrialPhase.STARTING)
        {
            SoundpackUtils.Prosecutor = message.trialData.IsProsecution();
            SoundpackUtils.PlayerOnStand = message.trialData.defendantPosition == Pepper.GetMyPosition();
            SoundpackUtils.TargetOnStand = message.trialData.defendantPosition == Service.Game.Sim.info.executionerTargetObservation.Data.targetPosition;
        }
    }
}