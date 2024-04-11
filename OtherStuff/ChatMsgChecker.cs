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
                switch(entry.messageId){
                    case GameFeedbackMessage.RAPID_MODE_STARTING:
                    SoundpackUtils.isRapid = true;
                    break;
                    case GameFeedbackMessage.DAYS_LEFT_TO_FIND_TRAITOR:
                    SoundpackUtils.isTT = true;
                    break;
                    case GameFeedbackMessage.SOCIALITE_PARTY:
                    SoundpackUtils.isParty = true;
                    break;
                    case GameFeedbackMessage.BEING_JAILED:
                    SoundpackUtils.isJailed = true;
                    break;
                    case GameFeedbackMessage.BEING_DUELED:
                    SoundpackUtils.isDueled = true;
                    break;
                    case GameFeedbackMessage.LYNCHED_JESTER:
                    SoundpackUtils.isHaunt = true;
                    break;
                }
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
                if (msg.Contains("HAS_EMERGED"))
                {
                    PlayMusicPatch.moddedMusic = "";
                    switch ((Role)Enum.Parse(typeof(Role), msg.Split('_')[0]))
                    {
                        case Role.PESTILENCE:
                            SoundpackUtils.pest = true;
                            break;
                        case Role.DEATH:
                            SoundpackUtils.death = true;
                            break;
                        case Role.FAMINE:
                            SoundpackUtils.fam = true;
                            break;
                        case Role.WAR:
                            SoundpackUtils.war = true;
                            break;
                    }
                    SoundpackUtils.loop = false;
                }

            }
        }
    }
    [HarmonyPatch(typeof(HudGraveyardPanel), "CreateItem")]
    class HorsemenDiedCheck
    {
        static public void Prefix(KillRecord killRecord)
        {
            SoundpackUtils.prosecutor = false;
            switch (killRecord.playerRole)
            {
                case Role.PESTILENCE:
                    SoundpackUtils.pest = false;
                    break;
                case Role.DEATH:
                    SoundpackUtils.death = false;
                    break;
                case Role.FAMINE:
                    SoundpackUtils.fam = false;
                    break;
                case Role.WAR:
                    SoundpackUtils.war = false;
                    break;
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
                SoundpackUtils.isNB = Service.Game.Cast.GetCharacterByPosition(message.trialData.defendantPosition).characterModel.gender == Gender.Other;
                SoundpackUtils.targetOnStand = message.trialData.defendantPosition == Service.Game.Sim.info.executionerTargetObservation.Data.targetPosition;

            }
        }
    }

}