using System;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using MessagePack;
using Highlands.Server;

public class TCPConnectManager : MonoBehaviour
{
    public static TCPConnectManager Instance = null;

    [Header("Chat")]
    [SerializeField] private TMP_Text MessageElement; // 채팅 메세지
    [SerializeField] private GameObject LobbyChattingList; // 로비 채팅 리스트
    [SerializeField] private TMP_InputField LobbyChat; // 로비 입력 메세지
    [SerializeField] private Button LobbyChatSendBtn; // 채널 메세지 전송 버튼

    [Header("Connect")]
    private TcpClient _tcpClient;
    private NetworkStream _networkStream;
    private StreamWriter writer;
    private User loginUserInfo;

    // 호스트
    private string hostname = "k10c209.p.ssafy.io"; // 로컬 호스트
    private int port = 1371;
    // private string hostname = "k10c209.p.ssafy.io"; // 로컬 호스트
    // private int port = 1370;

    private void Awake()
    {
        // 싱글톤
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            // 기존 데이터 instance로 복사
            Instance.MessageElement = MessageElement;
            Instance.LobbyChattingList = LobbyChattingList;
            Instance.LobbyChat = LobbyChat;
            Instance.LobbyChatSendBtn = LobbyChatSendBtn;
        }


        // 리스너 붙이기
        Instance.LobbyChatSendBtn.onClick.RemoveAllListeners();
        Instance.LobbyChatSendBtn.onClick.AddListener(() => Instance.MessageSendBtnClicked(LobbyChat));
    }

    void Start()
    {
        LobbyChat.characterLimit = 20;
    }

    void Update()
    {
        // 데이터가 들어온 경우
        while (_networkStream != null && _networkStream.DataAvailable)
        {
            Debug.Log("incoming");
            ReadMessageFromServer();
        }
        
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            MessageSendBtnClicked(LobbyChat);
        }
    }
    
    //#region 요청 분배하기
    //private void DispatchResponse(string response)
    //{
    //    string type = getType(response);

    //    if (type == "0101")
    //    {
    // showMessage(response, LobbyChattingList);
    //    }
    //}

    //private string getType(string response)
    //{
    //    string[] words = response.Split('\"');
    //    return words[3];
    //}
    // #endregion

    #region 서버 연결
    public void ConnectToServer()
    {
        string greeting = "Hello Server";


        try
        {
            // TCP 서버에 연결
            _tcpClient = new TcpClient(hostname, port);
            _networkStream = _tcpClient.GetStream();
            writer = new StreamWriter(_networkStream);
            loginUserInfo = DataManager.Instance.loginUserInfo;
            
            Message message = new Message
            {
                command = Command.CHAT,
                channelIndex = 0,
                userName = loginUserInfo.dataBody.nickname,
                message = greeting
            };

            var bytes = MessagePackSerializer.Serialize(message);
             
            SendMessageToServer(bytes);

            Debug.Log("ConnectToServer");
        }
        catch (Exception e)
        {
            // 연결 중 오류 발생 시
            Debug.Log($"Failed to connect to the server: {e.Message}");
        }
    }

    // 서버로 메세지 보내기
    private void SendMessageToServer(byte[] message)
    {
        if (_tcpClient == null) return;

        _networkStream.Write(message, 0, message.Length);
        _networkStream.Flush();
    }

    // 서버로부터 수신한 메세지 읽기
    private void ReadMessageFromServer()
    {
        if (_tcpClient == null || !_tcpClient.Connected) return;
        try
        {
            if (_networkStream == null)
            {
                _networkStream = _tcpClient.GetStream();
            }

            StringBuilder message = new StringBuilder();
        
            // 네트워크 스트림에 데이터가 있을 때까지 반복
            while (_networkStream.DataAvailable)
            {
                byte[] buffer = new byte[_tcpClient.ReceiveBufferSize];
                int bytesRead = _networkStream.Read(buffer, 0, buffer.Length); // 실제 데이터를 읽음

                if (bytesRead > 0)
                {
                    // MessagePackSerializer를 사용하여 메시지 역직렬화
                    Message receivedMessage = MessagePackSerializer.Deserialize<Message>(buffer.AsSpan().Slice(0, bytesRead).ToArray());

                    // 수신된 메시지를 StringBuilder에 추가
                    // message.Append(receivedMessage.userName + ": " + receivedMessage.message + "\n");
                    
                    showMessage(receivedMessage.userName, receivedMessage.message, LobbyChattingList);

                }
            }
        }
        catch(Exception e)
        {
            Debug.Log("응답 읽기 실패 : " + e.Message);
        }
    }
    
    #endregion



    #region 채팅 관련
    // 메세지 전송 버튼 클릭 시
    public void MessageSendBtnClicked(TMP_InputField inputField)
    {
        Debug.Log("send message");

        string message = inputField.text;

        if (message == "")
        {
            return;
        }

        Message pack = new Message
        {
            command = Command.CHAT,
            channelIndex = 0,
            userName = loginUserInfo.dataBody.nickname,
            message = message
        };

        var msgpack = MessagePackSerializer.Serialize(pack);
        
        inputField.text = "";
        SendMessageToServer(msgpack);
        inputField.Select();
        inputField.ActivateInputField();
    }

    // 메세지 들어왔을 때
    public void showMessage(string userName, string message, GameObject ChatScrollView)
    {
        StringBuilder sb = new StringBuilder();
        
        // 본인 확인 및 메시지 조합
        if (userName == loginUserInfo.dataBody.nickname)
        {
            sb.Append(userName);
            sb.Append("(나): ");
            sb.Append(message);
        }
        else
        {
            sb.Append(userName);
            sb.Append(": ");
            sb.Append(message);
        }

        
        // 붙일 부모 오브젝트
        Transform content = ChatScrollView.transform.Find("Viewport/Content");
        TMP_Text temp1 = Instantiate(MessageElement);

        temp1.text = sb.ToString();
        temp1.transform.SetParent(content, false);


        // 20개 넘어가면 채팅 위에서부터 지우기
        if (content.childCount >= 20)
        {
            Destroy(content.GetChild(1).gameObject);
        }

        StartCoroutine(ScrollToBottom(ChatScrollView));
    }

    // 스크롤 맨 아래로 내리기
    IEnumerator ScrollToBottom(GameObject ChatScrollView)
    {
        // 다음 프레임 기다림
        yield return null;

        Transform content = ChatScrollView.transform.Find("Viewport/Content");

        // LayOut Gropu을 강제로 즉시 업데이트
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)content);

        // 스크롤 맨 아래로 내림
        ChatScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
    }

    #endregion

    #region 종료 관련

    // 종료 시
    public void OnApplicationQuit()
    {
        clearChat();

        DataManager.Instance.gameDataClear();

        // tcp 연결 관련
        if (_tcpClient != null)
        {
            DisconnectFromServer();
        }
        writer = null;
        loginUserInfo = null;
    }

    public void DisconnectFromServer()
    {
        // 연결 종료
        _networkStream.Close();
        _tcpClient.Close();
        _networkStream = null;
        _tcpClient = null;
    }

    public void clearChat()
    {
        Transform content;

        if (LobbyChattingList != null)
        {
            content = LobbyChattingList.transform.Find("Viewport/Content");
            foreach (Transform child in content)
            {
                Destroy(child.gameObject);
            }
        }
    }

    #endregion
}