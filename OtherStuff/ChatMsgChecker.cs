using Utils;
using HarmonyLib;
using Services;
using Server.Shared.Messages;
using Server.Shared.State.Chat;
using Game.Interface;
using Server.Shared.State;
using Game.Simulation;
using UnityEngine;
using SoundpackPatchs;
using Server.Shared.Extensions;
using System;
using Game.Chat.Decoders;

namespace OtherStuff
{
    [HarmonyPatch(typeof(Game.Chat.PooledChatController), "AddMessage")]
    class ChatMsgChecker
    {
        static public void Prefix(ChatLogMessage message)
        {
            if (message.chatLogEntry.type == ChatType.GAME_MESSAGE)
            {
                ChatLogGameMessageEntry entry = (ChatLogGameMessageEntry)message.chatLogEntry;
                if (entry.messageId == GameFeedbackMessage.RAPID_MODE_STARTING) SoundpackUtils.isRapid = true;
                if (entry.messageId == GameFeedbackMessage.DAYS_LEFT_TO_FIND_TRAITOR) SoundpackUtils.isTT = true;
            }
        }
    }
    [HarmonyPatch(typeof(TransformGameMessageDecoder), "Encode")]
    class CheckForHorsemen
    {
        static public void Prefix(ChatLogMessage chatLogMessage)
        {
            if (chatLogMessage.chatLogEntry.type == ChatType.GAME_MESSAGE)
            {
                ChatLogGameMessageEntry entry = (ChatLogGameMessageEntry)chatLogMessage.chatLogEntry;
                string msg = entry.messageId.ToString();
                if (msg.Contains("HAS_EMERGED")) { PlayMusicPatch.moddedMusic = ""; SoundpackUtils.horsemen.Add((Role)Enum.Parse(typeof(Role), msg.Split('_')[0])); SoundpackUtils.loop = false; }
            }
        }
    }
    [HarmonyPatch(typeof(HudGraveyardPanel), "CreateItem")]
    class HorsemenDiedCheck
    {
        static public void Prefix(KillRecord killRecord)
        {
            SoundpackUtils.prosecutor = false;
            if (killRecord.playerRole == Role.FAMINE || killRecord.playerRole == Role.WAR || killRecord.playerRole == Role.PESTILENCE || killRecord.playerRole == Role.DEATH)
            {
                SoundpackUtils.horsemen.DeleteAll(killRecord.playerRole);
            }
        }
    }
    [HarmonyPatch(typeof(WhoDiedAndHowPanel), "HandleSubphaseWhoDied")]
    class LoopCheck
    {
        static public void Prefix()
        {
            SoundpackUtils.isRapid = false;
            if (SoundpackUtils.loop)
            {
                SoundpackUtils.loop = false;
                AudioController a = UnityEngine.Object.FindObjectOfType<AudioController>();
                a.StopMusic();
                a.PlayMusic("Audio/Music/WDAH");
            }

        }
    }

    [HarmonyPatch(typeof(GameSimulation), "HandleTrialState")]
    class Trial
    {
        static public void Prefix(TrialStateMessage message)
        {
            if (message.trialData.trialPhase == TrialPhase.STARTING)
            {
                SoundpackUtils.prosecutor = message.trialData.IsProsecution();
                SoundpackUtils.playerOnStand = message.trialData.defendantPosition == Pepper.GetMyPosition();
                SoundpackUtils.targetOnStand = message.trialData.defendantPosition == Service.Game.Sim.info.executionerTargetObservation.Data.targetPosition;
            }
        }
    }

}