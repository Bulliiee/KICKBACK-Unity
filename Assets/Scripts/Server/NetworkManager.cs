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
        #region 채팅 서버

        private delegate void UpdateCurrentChattingPlace();
        
        private ChattingServerController _chattingServerController;
        
        //Todo : GameManager Stanby
        public CurrentPlayerLocation currentPlayerLocation = CurrentPlayerLocation.Lobby;

        private void Update()
        {
            UpdateChatLog();
        }

        private void UpdateChatLog()
        {
            var currentChat = _chattingServerController.Incoming();
            
            switch (currentPlayerLocation) //TODO : GameManager.Instance.CurrentPlayerLocation로 변경
            {
                case CurrentPlayerLocation.Lobby:
                    UpdateCurrentChattingPlace = 로비 채팅창 UI 업데이트 로직 (currentChat);
                    break;
                case CurrentPlayerLocation.WaitingRoom:
                    UpdateCurrentChattingPlace = 대기룸 채팅창 UI 업데이트 로직 (currentChat);
                    break;
                case CurrentPlayerLocation.InGame:
                    UpdateCurrentChattingPlace = 인게임 채팅창 UI 업데이트 로직 (currentChat);
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
            _chattingServerController.Deliver(msgpack);
            inputField.Select();
            inputField.ActivateInputField();

            Debug.Log("send complete");
        }
        
        #endregion

        #region 비즈니스 서버

        

        #endregion

        #region 라이브 서버

        

        #endregion
    }
}
