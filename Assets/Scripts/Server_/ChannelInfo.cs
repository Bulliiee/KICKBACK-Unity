using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ChannelInfoo
{
    [Header("ChannelInfo")]
    [SerializeField] private int channelIndex; // 방 번호
    [SerializeField] private string channelName; // 방 제목
    [SerializeField] private List<string> channelUserList; // 방 유저 목록
    [SerializeField] private string channelManager; // 방장 닉네임
    [SerializeField] private string mapName; // 선택된 맵 이름
    [SerializeField] private List<bool> isReady; //  준비 상태 리스트
    [SerializeField] private List<int> teamColor; //  팀 컬러 리스트
    [SerializeField] private List<int> userCharacter;

    [Header("Chat")] [SerializeField] private TMP_Text MessageElement; // 채팅 메세지
    [SerializeField] private GameObject ChannelChattingList; // 채널 채팅 리스트
    [SerializeField] private TMP_InputField ChannelChat; // 채널 입력 메세지
    [SerializeField] private Button ChannelChatSendBtn; // 채널 메세지 전송 버튼

    public int ChannelIndex
    {
        get { return channelIndex; }
        set { channelIndex = value; }
    }

    public string ChannelName
    {
        get { return channelName; }
        set { channelName = value; }
    }

    public List<string> ChannelUserList
    {
        get { return channelUserList; }
        set { channelUserList = value; }
    }

    public string ChannelManager
    {
        get { return channelManager; }
        set { channelManager = value; }
    }

    public string MapName
    {
        get { return mapName; }
        set { mapName = value; }
    }

    public List<bool> IsReady
    {
        get { return isReady; }
        set { isReady = value; }
    }

    public List<int> TeamColor
    {
        get { return teamColor; }
        set { teamColor = value; }
    }

    public List<int> UserCharacter
    {
        get { return userCharacter; }
        set { userCharacter = value; }
    }
}