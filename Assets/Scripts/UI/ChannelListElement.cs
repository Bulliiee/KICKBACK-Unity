using System.Collections;
using System.Collections.Generic;
using Highlands.Server;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms.Impl;

public class ChannelListElement : MonoBehaviour
{
    [FormerlySerializedAs("TCPConnectManagerScript")] public TCPConnectionController TcpConnectionControllerScript;

    public TMP_Text roomName_txt;
    public TMP_Text roomUser_txt;
    public TMP_Text isOnGame_txt;

    public int roomIndex;
    public string roomName;
    public string mapName;
    public int roomUser;
    public bool isOnGame;
    
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

    public int RoomUser
    {
        get
        {
            return roomUser;
        }
        set
        {
            roomUser = value;
        }
    }

    public bool IsOnGame
    {
        get
        {
            return isOnGame;
        }
        set
        {
            isOnGame = value;
        }
    }

    private void Start()
    {
        //TCPConnectManagerScript = GameObject.Find("TCPConnectManager").GetComponent<TCPConnectManager>();

        roomName_txt.text = roomName;
        roomUser_txt.text = $"({roomUser} / 6)";

        if (!isOnGame)
        {
            isOnGame_txt.text = "(대기 중)";
        }
        else
        {
            isOnGame_txt.text = "(게임 중)";
        }
    }

    // 방 리스트에서 버튼 누르면
    public void ChannelListElementClicked()
    {
        BusinessManager.Instance.jlrRoom(Command.JOIN, roomIndex);  // chk
    }
}