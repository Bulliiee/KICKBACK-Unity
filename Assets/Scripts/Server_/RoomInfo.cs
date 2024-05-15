using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class RoomInfo
{
    [Header("RoomInfo")]
    [SerializeField] private int roomIndex;               // 방 번호
    [SerializeField] private string roomName;               // 방 제목
    [SerializeField] private List<string> roomUserList;     // 방 유저 목록
    [SerializeField] private string roomManager;            // 방장 닉네임
    [SerializeField] private string mapName;                // 선택된 맵 이름
    [SerializeField] private List<bool> isReady;             //  준비 상태 리스트
    [SerializeField] private List<int> teamColor;             //  팀 컬러 리스트

    [Header("Chat")]
    [SerializeField] private TMP_Text MessageElement; // 채팅 메세지
    [SerializeField] private GameObject ChannelChattingList; // 채널 채팅 리스트
    [SerializeField] private TMP_InputField ChannelChat; // 채널 입력 메세지
    [SerializeField] private Button ChannelChatSendBtn; // 채널 메세지 전송 버튼
 
    public int RoomIndex
    {
        get
        {
            return roomIndex;
        }
        set
        {
            roomIndex = value;
        }
    }
    public string RoomName
    {
        get
        {
            return roomName;
        }
        set
        {
            roomName = value;
        }
    }
    
    public List<string> RoomUserList
    {
        get
        {
            return roomUserList;
        }
        set
        {
            roomUserList = value;
        }
    }
    public string RoomManager
    {
        get
        {
            return roomManager;
        }
        set
        {
            roomManager = value;
        }
    }
    public string MapName
    {
        get
        {
            return mapName;
        }
        set
        {
            mapName = value;
        }
    }
    public List<bool> IsReady
    {
        get
        {
            return isReady;
        }
        set
        {
            isReady = value;
        }
    }

    public List<int> TeamColor
    {
        get
        {
            return teamColor;
        }
        set
        {
            teamColor = value;
        }
    }
}
