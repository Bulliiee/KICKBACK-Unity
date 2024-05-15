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
        #region ??? ????

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
            
            switch (currentPlayerLocation) //TODO : GameManager.Instance.CurrentPlayerLocation?? ????
            {
                case CurrentPlayerLocation.Lobby:
                    UpdateCurrentChattingPlace = ?¥ê? ???? UI ??????? ???? (currentChat);
                    break;
                case CurrentPlayerLocation.WaitingRoom:
                    UpdateCurrentChattingPlace = ???? ???? UI ??????? ???? (currentChat);
                    break;
                case CurrentPlayerLocation.InGame:
                    UpdateCurrentChattingPlace = ????? ???? UI ??????? ???? (currentChat);
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
            // ????
            _chattingServerController.Deliver(msgpack);
            inputField.Select();
            inputField.ActivateInputField();

            Debug.Log("send complete");
        }
        
        #endregion

        #region ?????? ????

        

        #endregion

        #region ????? ????

        

        #endregion
    }
}
