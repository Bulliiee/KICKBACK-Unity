using System;
using System.Collections;
using System.Collections.Generic;
using Highlands.Server;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[Serializable]
public class ChannelInfo
{
    public int channelIndex;                // 채널 번호
    public string channelName;              // 채널 이름
    public List<string> channelUserList;    // 채널 유저 리스트
    public string channelManager;           // 방장
    public string mapName;                  // 선택된 맵 이름
    public List<bool> isReady;              // 레디 여부
    public List<int> teamColor;             // 축구모드 팀 색깔
    public List<int> userCharacter;         // 선택한 캐릭터
    
    public bool isOnGame;                   // 게임중 여부
    public int channelUser;                 // 채널 유저 수
    public string gameMode;                 // 게임 모드(스피드/축구)

    public int myIndex;                     // channelUserList에서 내 인덱스

    // 내 인덱스 찾기
    public void SetMyIndex()
    {
        for (int i = 0; i < channelUserList.Count; i++)
        {
            if (channelUserList[i] == GameManager.Instance.loginUserInfo.NickName)
            {
                myIndex = i;
                return;
            }
        }
    }
}

public class ChannelListElement : MonoBehaviour
{
    public Button channelListButton;
    public TMP_Text channelName_txt;
    public TMP_Text channelUser_txt;
    public TMP_Text isOnGame_txt;

    public ChannelInfo channelListInfo;

    void Start()
    {
        channelListButton.onClick.AddListener(ChannelListButtonClicked);
    }

    public void SetText()
    {
        channelName_txt.text = channelListInfo.channelName;
        channelUser_txt.text = $"({channelListInfo.channelUser} / 6)";

        if (!channelListInfo.isOnGame)
        {
            isOnGame_txt.text = "(대기 중)";
        }
        else
        {
            isOnGame_txt.text = "(게임 중)";
        }
    }

    public void SetDatas(ChannelInfo channelInfo)
    {
        channelListInfo = channelInfo;
    }

    // 방 리스트에서 버튼 누르면
    private void ChannelListButtonClicked()
    {
        if (!channelListInfo.isOnGame)
        {
            GameObject.Find("Enter Channel").GetComponent<EnterChannelPopUp>().OpenEnterChannelPopup(channelListInfo);
        }

        // BusinessManager.Instance.jlrRoom(Command.JOIN, channelIndex);  // chk
    }
}