using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Highlands.Server;
using Highlands.Server.BusinessServer;
using MessagePack;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Message = Highlands.Server.BusinessServer.InitialMessage;

public class BusinessManager : MonoBehaviour
{   
    public static BusinessManager Instance = null;
    public LobbyManager LobbyManagerScript;
    private TcpClient _tcpClient;
    private NetworkStream _networkStream;
    private StreamWriter writer;
    private User loginUserInfo;

    [SerializeField] private DataManager dataManager;
    [SerializeField] private MakingRoomPopUp roomPopUp;

    [Header("Make Room")]
    [SerializeField] private TextMeshProUGUI roomName;
    public GameObject makeRoom;
    private string mode;

    private string hostname = "k10c209.p.ssafy.io";
    private int port = 1370;

    private bool isConnected;

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
        }
    }

    void Start()
    {
    }

    void Update()
    {
        if(isConnected)
        {
            while (_networkStream != null && _networkStream.DataAvailable)
            {
                Debug.Log("B.S: Incoming");
                ReadMessageFromServer();
            }
        }

        mode = roomPopUp.modeName;
    }

    public void ConnectToServer()
    {
        try
        {
            _tcpClient = new TcpClient(hostname, port);
            _networkStream = _tcpClient.GetStream();
            writer = new StreamWriter(_networkStream);
            loginUserInfo = DataManager.Instance.loginUserInfo;

            InitialMessage message = new InitialMessage
            {   
                Command = Command.CLIENT,
                UserName = loginUserInfo.dataBody.nickname,
                EscapeString = "\n"
            };

            byte[] bytes = MessagePackSerializer.Serialize(message);
             
            SendMessageToServer(bytes);
            isConnected = true;
            Debug.Log("ConnectToServer");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    private void SendMessageToServer(byte[] message)
    {
        if (_tcpClient == null) return;

        _networkStream.Write(message, 0, message.Length);
        _networkStream.Flush();
    }

    // 서버에서 메시지 읽기
    private void ReadMessageFromServer()
    {
        if (_tcpClient == null || !_tcpClient.Connected) return;
        try
        {
            if (_networkStream == null)
            {
                _networkStream = _tcpClient.GetStream();
            }
            
            List<string> userList = new List<string>();
            List<string> roomList = new List<string>();
            ChannelInfo channelInfo = new ChannelInfo();
            
            while (_networkStream.DataAvailable)
            {
                // 메시지 길이만큼 데이터를 읽습니다.
                byte[] messageBuffer = new byte[4];
                int bytesRead = _networkStream.Read(messageBuffer, 0, 4);
                Array.Reverse(messageBuffer);
                
                if (bytesRead == 4)
                {
                    int messageLength = BitConverter.ToInt32(messageBuffer, 0);
                    
                    byte[] buffer = new byte[messageLength];
                    bytesRead = _networkStream.Read(buffer, 0, messageLength);
                    if (bytesRead > 0)
                    {
                        // MessagePackSerializer를 사용하여 메시지 역직렬화
                        RecieveLogin receivedMessage =
                            MessagePackSerializer.Deserialize<RecieveLogin>(buffer.AsSpan().Slice(0, messageLength)
                                .ToArray());

                        // 얻은 타입
                        string type = receivedMessage.Type;
                        if (type.Equals("userList"))
                        {
                            // 대괄호 제거
                            string trimmedString = receivedMessage.List.TrimStart('[').TrimEnd(']');
                            // 쉼표로 분리하여 리스트로 변환
                            userList = new List<string>(trimmedString.Split(','));
                            LobbyManagerScript.getAllUsers(userList);
                        }
                        else if (type.Equals("roomList"))
                        {
                            // 대괄호 제거
                            string trimmedString = receivedMessage.List.TrimStart('[').TrimEnd(']');
                            // 쉼표로 분리하여 리스트로 변환
                            roomList = new List<string>(trimmedString.Split(new string[] { "}, " }, StringSplitOptions.None));
                            LobbyManagerScript.getRoomList(roomList);
                        }
                        else if (type.Equals("roomInfo"))
                        {
                            string roomUserList = receivedMessage.List.TrimStart('[').TrimEnd(']');
                            string isReadyList = receivedMessage.IsReady.TrimStart('[').TrimEnd(']');
                            string teamColorList = receivedMessage.TeamColor.TrimStart('[').TrimEnd(']');

                            channelInfo.RoomIndex = receivedMessage.RoomIndex;
                            dataManager.channelIndex = channelInfo.RoomIndex;
                            channelInfo.RoomName = receivedMessage.RoomName;
                            dataManager.channelName = channelInfo.RoomName;
                            channelInfo.RoomUserList = new List<string>(roomUserList.Split(','));
                            dataManager.roomUserList = channelInfo.RoomUserList;
                            dataManager.cnt = channelInfo.RoomUserList.Count;
                            channelInfo.RoomManager = receivedMessage.RoomManager;
                            channelInfo.MapName = receivedMessage.MapName;
                            channelInfo.IsReady = isReadyList.Split(',').Select(s => bool.Parse(s)).ToList();
                            channelInfo.TeamColor = teamColorList.Split(',').Select(s => int.Parse(s)).ToList();
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("응답 읽기 실패 : " + e.Message);
        }
    }

    public void OnApplicationQuit()
    {

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
    
    //===================방======================
    
    // 방 만들기
    public void createRoom()
    {
        CreateRoom message = new CreateRoom
        {
            Command = Command.CREATE,
            UserName = loginUserInfo.dataBody.nickname, // 방 만드는 유저 닉네임
            RoomName = roomName.text,         // 방 제목
            MapName = "선택해서 체인지 되는 값",          // 맵 이름
            GameMode = mode,
            EscapeString = "\n"
        };

        byte[] bytes = MessagePackSerializer.Serialize(message);
             
        SendMessageToServer(bytes);

        makeRoom.SetActive(false);

        SceneManager.LoadScene("WaitingRoom");
    }
    
    // 방 들어가기 or 떠나기 or 레디상태 변경
    public void jlrRoom(Command command,int roomIndex)
    {
        JLRRoom message = new JLRRoom
        {   
            Command = command,
            UserName = loginUserInfo.dataBody.nickname, // 유저 닉네임
            RoomIndex = roomIndex,      // 방 번호
            EscapeString = "\n"
        };

        byte[] bytes = MessagePackSerializer.Serialize(message);
        
        SendMessageToServer(bytes);

        if (command.Equals(Command.JOIN))
        {
            SceneManager.LoadScene("WaitingRoom");
        }
    }

    // 게임 시작  or 게임 끝
    public void startOrEndGame(Command command)
    {
        StartOrEndGame message = new StartOrEndGame
        {   
            Command = command,
            RoomIndex = 1,      // 방 번호
            EscapeString = "\n"
        };

        byte[] bytes = MessagePackSerializer.Serialize(message);
             
        SendMessageToServer(bytes);
    }
    
    // 맵 바꾸기
    public void changeMap()
    {
        ChanegeMap message = new ChanegeMap
        {   
            Command = Command.MAP,
            MapName = "체인지해서 맵 바꿈",
            RoomIndex = 1,      // 방 번호
            EscapeString = "\n"
        };

        byte[] bytes = MessagePackSerializer.Serialize(message);
             
        SendMessageToServer(bytes);
    }
    // 팀 컬러 바꾸기
    public void teamChange(int roomIndex)
    {
        TEAMCHANGE message = new TEAMCHANGE
        {
            Command = Command.TEAMCHANGE,
            RoomIndex = roomIndex,      // 방 번호
            UserName = loginUserInfo.dataBody.nickname, // 유저 닉네임
            EscapeString = "\n"
        };

        byte[] bytes = MessagePackSerializer.Serialize(message);

        SendMessageToServer(bytes);
    }
}