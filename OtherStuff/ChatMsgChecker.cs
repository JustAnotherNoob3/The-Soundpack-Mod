using Utils;
using HarmonyLib;
using SML;
using UnityEngine;
using Server.Shared.Messages;
using Server.Shared.State.Chat;

namespace OtherStuff{
    [HarmonyPatch(typeof(Game.Chat.PooledChatController), "AddMessage")]
    class ChatMsgChecker{
        static public void Prefix(ChatLogMessage message){
            if(message.chatLogEntry.type == ChatType.GAME_MESSAGE){
                ChatLogGameMessageEntry entry = (ChatLogGameMessageEntry)message.chatLogEntry;
                if(entry.messageId.ToString().Contains("HAS_EMERGED")) SoundpackUtils.horsemen++;
                if(entry.messageId.ToString() == "RAPID_MODE_STARTING") SoundpackUtils.isRapid = true;
            }
        }
    }
}