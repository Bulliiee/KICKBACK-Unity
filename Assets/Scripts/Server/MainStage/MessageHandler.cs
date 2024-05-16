using System;
using System.Collections.Generic;
using System.Linq;
using Highlands.Server.BusinessServer;
using MessagePack;
using UnityEngine;

namespace Highlands.Server
{
    public class MessageHandler
    {
        public static void UnPackMessage(byte[] buffer, int bytesRead)
        {
            List<string> userList = new List<string>();
            List<string> channelList = new List<string>();
            ChannelInfo channelInfo = new ChannelInfo();
            
            // MessagePackSerializer를 사용하여 메시지 역직렬화
            RecieveLoginMessage receivedMessage =
                MessagePackSerializer.Deserialize<RecieveLoginMessage>(buffer.AsSpan().Slice(0, bytesRead)
                    .ToArray());

            string type = receivedMessage.Type;
            string trimmedString;
            
            switch (type)
            {
                case "userList":
                    trimmedString = receivedMessage.List.TrimStart('[').TrimEnd(']');
                    userList = new List<string>(trimmedString.Split(','));
                    // LobbyManagerScript.getAllUsers(userList);
                    break;
                case "channelList":
                    trimmedString = receivedMessage.List.TrimStart('[').TrimEnd(']');
                    channelList = new List<string>(trimmedString.Split(new string[] { "}, " }, StringSplitOptions.None));
                    // LobbyManagerScript.getRoomList(roomList);
                    break;
                case "channelInfo":
                    string channelUserList = receivedMessage.List.TrimStart('[').TrimEnd(']');
                    string isReadyList = receivedMessage.IsReady.TrimStart('[').TrimEnd(']');
                    string teamColorList = receivedMessage.TeamColor.TrimStart('[').TrimEnd(']');
                    string userCharacterList = receivedMessage.UserCharacter.TrimStart('[').TrimEnd(']');
                    
                    channelInfo.ChannelIndex = receivedMessage.ChannelIndex;
                    // dataManager.channelIndex = channelInfo.ChannelIndex;
                    channelInfo.ChannelName = receivedMessage.ChannelName;
                    // dataManager.channelName = channelInfo.ChannelName;
                    channelInfo.ChannelUserList = new List<string>(channelUserList.Split(','));
                    // dataManager.roomUserList = channelInfo.ChannelUserList;
                    // dataManager.cnt = channelInfo.ChannelUserList.Count;
                    channelInfo.ChannelManager = receivedMessage.ChannelManager;
                    channelInfo.MapName = receivedMessage.MapName;
                    channelInfo.IsReady = isReadyList.Split(',').Select(s => bool.Parse(s)).ToList();
                    channelInfo.TeamColor = teamColorList.Split(',').Select(s => int.Parse(s)).ToList();
                    channelInfo.UserCharacter = userCharacterList.Split(',').Select(s => int.Parse(s)).ToList();
                    break;
                default:
                    Debug.Log("언패킹 에러(메시지 핸들러)");
                    break;
            }
        }
    }
}