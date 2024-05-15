using System;
using MessagePack;
using Modules;
using TMPro;
using UnityEngine;

namespace Highlands.Server
{
    public enum CurrentPlayerLocation
    {
        Lobby,
        WaitingRoom,
        InGame
    }
    
    public class NetworkManager : Singleton<NetworkManager>
    {
        private void Update()
        {
            UpdateChatLog();
            UpdateBusinessLog();
        }
        
        private delegate void UpdateCurrentChattingPlace();
        
        //Todo : GameManager Stanby
        public CurrentPlayerLocation currentPlayerLocation = CurrentPlayerLocation.Lobby;

        #region 채팅 서버
        
        private TCPConnectionController _chattingServer;
        
        private void UpdateChatLog()
        {
            var (data, bytesRead) = _chattingServer.ChatIncoming();
            ChatMessage message = MessagePackSerializer.Deserialize<ChatMessage>(data.AsSpan().Slice(0, bytesRead).ToArray());

            
            switch (currentPlayerLocation) //TODO : GameManager.Instance.CurrentPlayerLocation로 변경
            {
                case CurrentPlayerLocation.Lobby:
                    // UpdateCurrentChattingPlace = 로비 채팅창 UI 업데이트 로직 (currentChat);
                    break;
                case CurrentPlayerLocation.WaitingRoom:
                    // UpdateCurrentChattingPlace = 대기룸 채팅창 UI 업데이트 로직 (currentChat);
                    break;
                case CurrentPlayerLocation.InGame:
                    // UpdateCurrentChattingPlace = 인게임 채팅창 UI 업데이트 로직 (currentChat);
                    break;
            }
        }

        public void SendMessage(TMP_InputField inputField, int channelIndex, string nickname)
        {
            var message = inputField.text;

            if (message.Equals(""))
            {
                return;
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
            // 전송
            _chattingServer.Deliver(msgpack);
            inputField.Select();
            inputField.ActivateInputField();

            Debug.Log("send complete");
        }
        
        #endregion

        #region 비즈니스 서버

        private TCPConnectionController _businessServer;

        private void UpdateBusinessLog()
        {
            var (data, bytesRead) = _businessServer.BusinessIncoming();
            MessageHandler.unPackMEssage(data, bytesRead);
        }

        #endregion

        #region 라이브 서버



        #endregion
    }
}