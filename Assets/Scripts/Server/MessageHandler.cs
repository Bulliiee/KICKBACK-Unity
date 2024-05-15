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
        public static void unPackMEssage(byte[] buffer, int bytesRead)
        {
            List<string> userList = new List<string>();
            List<string> roomList = new List<string>();
            RoomInfo roomInfo = new RoomInfo();
            
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
                case "roomList":
                    trimmedString = receivedMessage.List.TrimStart('[').TrimEnd(']');
                    roomList = new List<string>(trimmedString.Split(new string[] { "}, " }, StringSplitOptions.None));
                    // LobbyManagerScript.getRoomList(roomList);
                    break;
                case "roomInfo":
                    string roomUserList = receivedMessage.List.TrimStart('[').TrimEnd(']');
                    string isReadyList = receivedMessage.IsReady.TrimStart('[').TrimEnd(']');
                    string teamColorList = receivedMessage.TeamColor.TrimStart('[').TrimEnd(']');

                    roomInfo.RoomIndex = receivedMessage.RoomIndex;
                    // dataManager.channelIndex = roomInfo.RoomIndex;
                    roomInfo.RoomName = receivedMessage.RoomName;
                    // dataManager.channelName = roomInfo.RoomName;
                    roomInfo.RoomUserList = new List<string>(roomUserList.Split(','));
                    // dataManager.roomUserList = roomInfo.RoomUserList;
                    // dataManager.cnt = roomInfo.RoomUserList.Count;
                    roomInfo.RoomManager = receivedMessage.RoomManager;
                    roomInfo.MapName = receivedMessage.MapName;
                    roomInfo.IsReady = isReadyList.Split(',').Select(s => bool.Parse(s)).ToList();
                    roomInfo.TeamColor = teamColorList.Split(',').Select(s => int.Parse(s)).ToList();
                    break;
                default:
                    Debug.Log("언패킹 에러(메시지 핸들러)");
                    break;
            }
        }
    }
}