using System;
using System.Collections.Generic;
using System.Linq;
using Highlands.Server.BusinessServer;
using MessagePack;
using TMPro;
using UnityEngine;

namespace Highlands.Server
{
    public class MessageHandler
    {
        #region 채팅

        public static byte[] PackChatMessage(TMP_InputField inputField, int channelIndex, string nickname)
        {
            var message = inputField.text;

            if (message.Equals(""))
            {
                return null;
            }

            var pack = new Message
            {
                command = Command.CHAT,
                channelIndex = channelIndex,
                userName = nickname,
                message = message
            };

            var msgpack = MessagePackSerializer.Serialize(pack);

            inputField.text = "";
            inputField.Select();
            inputField.ActivateInputField();

            return msgpack;
        }

        #endregion

        #region 비즈니스

        // 패킹
        
        // 1. 최초 접속시 전송할 메시지
        public static byte[] PackInitialMessage()
        {
            var pack = new InitialSendMessage
            {
                Command = Command.CLIENT,
                UserName = GameManager.Instance.loginUserInfo.NickName,
                EscapeString = "\n"
            };

            var msgpack = MessagePackSerializer.Serialize(pack);

            return msgpack;
        }
        
        // 2. 방 생성시 전송할 메시지
        public static byte[] PackCreateMessage(string channelName, string mapName, string gameMode)
        {
            var message = new CreateMessage
            {
                Command = Command.CREATE,
                UserName = GameManager.Instance.loginUserInfo.NickName, // 방 만드는 유저 닉네임
                ChannelName = channelName,         // 방 제목
                MapName = mapName,          // 맵 이름
                GameMode = gameMode,
                EscapeString = "\n"
            };

            byte[] bytes = MessagePackSerializer.Serialize(message);

            return bytes;
        }
        
        // 3. 방 입장시 전송할 메시지
        
        // 4. 방 나갈때 전송할 메시지
        
        // 5. 방에서 준비할 때 전송할 메시지
        
        // 6. 게임 시작할 때 전송할 메시지
        
        // 7. 게임이 끝났을 때 전송할 메시지
        
        // 8. 맵 바꿀 떼 전송할 메시지
        
        // 10. 팀 바꿀 때 전송할 메시지 
                    
        // 10. 캐릭터 변공할 때 전송할 메시지
        
        // 언패킹 
        public static void UnPackBusinessMessage(byte[] buffer, int bytesRead)
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

            LobbyController lobbyController =
                GameObject.Find("LobbyController").GetComponent<LobbyController>();

            switch (type)
            {
                case "userList":
                    trimmedString = receivedMessage.List.TrimStart('[').TrimEnd(']');
                    userList = new List<string>(trimmedString.Split(','));

                    Debug.Log(receivedMessage.List);
                    lobbyController.SetUserList(userList);
                    // LobbyManagerScript.getAllUsers(userList);
                    break;
                case "channelList":
                    trimmedString = receivedMessage.List.TrimStart('[').TrimEnd(']');
                    channelList =
                        new List<string>(trimmedString.Split(new string[] { "}, " }, StringSplitOptions.None));
                    
                    lobbyController.SetChannelList(channelList);
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
        
        #endregion

        #region 라이브

        // 1. 게임시작 할 때 전송할 메시지
        
        // 2. 게임이 끝났을 때 전송할 메시지

        #endregion
    }
}