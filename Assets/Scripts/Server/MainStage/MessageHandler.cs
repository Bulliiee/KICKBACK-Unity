using System;
using System.Collections.Generic;
using System.Linq;
using MessagePack;
using TMPro;
using UnityEngine;

namespace Highlands.Server
{
    public class MessageHandler
    {
        #region 채팅

        public static byte[] PackChatMessage(string message, int channelIndex)
        {
            if (message.Equals(""))
            {
                return null;
            }

            var pack = new ChatMessage
            {
                Command = Command.CHAT,
                ChannelIndex = channelIndex,
                UserName = GameManager.Instance.loginUserInfo.NickName,
                Message = message
            };

            return MessagePackSerializer.Serialize(pack);
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

            return MessagePackSerializer.Serialize(pack);
        }

        // 2. 방 생성시 전송할 메시지
        public static byte[] PackCreateMessage(string channelName, string mapName, string gameMode)
        {
            var message = new CreateMessage
            {
                Command = Command.CREATE,
                UserName = GameManager.Instance.loginUserInfo.NickName, // 방 만드는 유저 닉네임
                ChannelName = channelName, // 방 제목
                MapName = mapName, // 맵 이름
                GameMode = gameMode,
                EscapeString = "\n"
            };

            return MessagePackSerializer.Serialize(message);
        }
        
        // 3, 4, 5. 방 입장, 나가기, 레디시 전송할 메시지
        public static byte[] PackJLRMessage(int channelIndex, Command command)
        {
            var message = new JLRMessage()
            {
                Command = command,
                UserName = GameManager.Instance.loginUserInfo.NickName,
                ChannelIndex = channelIndex,
                EscapeString = "\n"
            };

            return MessagePackSerializer.Serialize(message);
        }
        
        // 6, 7. 게임 시작, 종료할 때 전송할 메시지
        public byte[] PackStartOrEndMessage(Command command, int channelIndex)
        {
            var message = new StartOrEndMessage
            {
                Command = command,
                ChannelIndex = channelIndex,
                EscapeString = "\n"
            };

            return MessagePackSerializer.Serialize(message);
        }
        
        // 7. 맵 바꿀 떼 전송할 메시지
        public byte[] PackChangeMapMessage(string mapName, int channelIndex)
        {
            var message = new ChangeMapMessage
            {
                Command = Command.MAP,
                MapName = mapName,
                ChannelIndex = channelIndex,
                EscapeString = "\n"
            };

            return MessagePackSerializer.Serialize(message);
        }
        
        // 8. 팀 바꿀 때 전송할 메시지 
        public byte[] PackTeamChangeMessage(int channelIndex)
        {
            var message = new TeamChangeMessage
            {
                Command = Command.TEAMCHANGE,
                ChannelIndex = channelIndex,
                UserName = GameManager.Instance.loginUserInfo.NickName,
                EscapeString = "\n"
            };

            return MessagePackSerializer.Serialize(message);
        }
                    
        // 9. 캐릭터 변공할 때 전송할 메시지
        public byte[] PackCharacterChangeMessage(int channelIndex, int characterIndex)
        {
            var message = new CharacterChangeMessage
            {
                Command = Command.CHARCHANGE,
                ChannelIndex = channelIndex,
                UserName = GameManager.Instance.loginUserInfo.NickName,
                CharacterIndex = characterIndex,
                EscapeString = "\n"
            };

            return MessagePackSerializer.Serialize(message);
        }
        

        // 언패킹 
        public static void UnPackBusinessMessage(byte[] buffer, int bytesRead)
        {
            List<string> userList = new List<string>();
            List<string> channelList = new List<string>();

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
                    // userList = new List<string>(trimmedString.Split(','));
                    userList = new List<string>(trimmedString.Split(new string[] { ", " }, StringSplitOptions.None));

                    lobbyController.SetUserList(userList);
                    // LobbyManagerScript.getAllUsers(userList);
                    break;
                case "channelList":
                    trimmedString = receivedMessage.List.TrimStart('[').TrimEnd(']');
                    channelList =
                        new List<string>(trimmedString.Split(new string[] { "}, " }, StringSplitOptions.None));

                    // 파싱 시 아무것도 없을 때 처리
                    if (channelList[0] == "")
                    {
                        channelList.Clear();
                    }

                    lobbyController.SetChannelList(channelList);
                    // LobbyManagerScript.getRoomList(roomList);
                    break;
                case "channelInfo":
                    // 기본 데이터 저장
                    ChannelInfo channelInfo = new ChannelInfo();
                    channelInfo.channelIndex = receivedMessage.ChannelIndex;
                    channelInfo.channelName = receivedMessage.ChannelName;
                    channelInfo.channelManager = receivedMessage.ChannelManager;
                    channelInfo.mapName = receivedMessage.MapName;
                    
                    // 리스트 형태 파싱
                    string channelUserListString = receivedMessage.List.TrimStart('[').TrimEnd(']');
                    string isReadyListString = receivedMessage.IsReady.TrimStart('[').TrimEnd(']');
                    string teamColorListString = receivedMessage.TeamColor.TrimStart('[').TrimEnd(']');
                    string userCharacterListString = receivedMessage.UserCharacter.TrimStart('[').TrimEnd(']');

                    // 스피드전인 경우
                    if (teamColorListString == "")
                    {
                        channelInfo.gameMode = "speed";
                    }
                    
                    // 리스트 형태 저장
                    channelInfo.channelUserList = new List<string>(channelUserListString.Split(new string[] { ", " }, StringSplitOptions.None));
                    channelInfo.isReady = isReadyListString.Split(',').Select(s => bool.Parse(s)).ToList();
                    if (channelInfo.gameMode == "soccer")
                    {
                        channelInfo.teamColor = teamColorListString.Split(',').Select(s => int.Parse(s)).ToList();
                    }
                    channelInfo.userCharacter = userCharacterListString.Split(',').Select(s => int.Parse(s)).ToList();

                    // 본인 인덱스 찾기
                    channelInfo.SetMyIndex();
                    
                    lobbyController.EnterChannel(channelInfo);
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