using System;
using MessagePack;
using Modules;
using TMPro;
using UnityEngine;

namespace Highlands.Server
{
    public enum CurrentPlayerLocation
    {
        Login,
        Lobby,
        WaitingRoom,
        InGame
    }
    
    public class NetworkManager : Singleton<NetworkManager>
    {
        private void Update()
        {
            // 로그인이 아닌 경우 TCP 연결 체킹
            if (currentPlayerLocation != CurrentPlayerLocation.Login)
            {
                UpdateChatLog();
                UpdateBusinessLog();
            }

            if (currentPlayerLocation == CurrentPlayerLocation.InGame)
            {
                // 라이브 서버 로직
                UpdateLiveLog();
            }
        }
        
        private delegate void UpdateCurrentChattingPlace();
        
        //Todo : GameManager Stanby
        public CurrentPlayerLocation currentPlayerLocation = CurrentPlayerLocation.Login;

        #region 인증

        private HTTPController _httpController = new HTTPController();
        
        // Get요청 보내기
        public void GetRequest<T>(string requestData, string requestUrl, Action<T> resultCallback)
        {
            StartCoroutine(_httpController.SendGetRequest(requestData, requestUrl, (result) =>
            {
                try
                {
                    // 문자열 형태의 JSON을 받은 경우 객체로 변환 후 결과 넘김
                    T t = JsonUtility.FromJson<T>(result);
                    resultCallback?.Invoke(t);
                }
                catch (Exception e)
                {
                    // 파싱 실패, 문자열 형태 숫자인 경우(에러코드)
                    if (typeof(T) == typeof(string))
                    {
                        resultCallback?.Invoke((T)(object)result);
                    }
                    else
                    {
                        Debug.Log("HTTPController RequestData execute fail");
                    }
                }
            }));
        }
    
        // Post 요청 보내기
        public void PostRequest<T>(T t, string requestUrl, Action<long> resultCallback)
        {
            StartCoroutine(_httpController.SendPostRequest(t, requestUrl, (result) =>
            {
                resultCallback?.Invoke(result);
            }));
        }

        #endregion

        #region 채팅 서버
        
        private TCPConnectionController _chattingServer;
        
        private void UpdateChatLog()
        {
            var (data, bytesRead) = _chattingServer.ChatIncoming();
            var message = MessagePackSerializer.Deserialize<ChatMessage>(data.AsSpan().Slice(0, bytesRead).ToArray());
            
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

        public void SendChatMessage(byte[] buffer)
        {
            // 전송
            _chattingServer.Deliver(buffer);

            Debug.Log("Chatting send complete");
        }
        
        #endregion

        #region 비즈니스 서버

        private TCPConnectionController _businessServer;

        private void UpdateBusinessLog()
        {
            var (data, bytesRead) = _businessServer.BusinessIncoming();
            MessageHandler.UnPackBusinessMessage(data, bytesRead);
        }

        public void SendBusinessMessage(byte[] buffer)
        {
            _businessServer.Deliver(buffer);
            
            Debug.Log("Business send complete");
        }

        #endregion

        #region 라이브 서버

        private UDPConnectionController _liveServer;

        private void UpdateLiveLog()
        {
            var (data, bytesRead) = _liveServer.LiveReceiver();
            // 핸들링
        }

        public void SendLiveMessage(byte[] buffer)
        {
            _liveServer.Deliver(buffer);
            Debug.Log("Live send complete");
        }
        
        #endregion
    }
}