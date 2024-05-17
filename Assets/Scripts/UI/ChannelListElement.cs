using System;
using System.Collections;
using System.Collections.Generic;
using Highlands.Server;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[Serializable]
public class ReceiveChannelElement
{
    public int channelIndex;
    public string channelName;
    public bool isOnGame;
    public string mapName;
    public int channelUser;
}

public class ChannelListElement : MonoBehaviour
{
    public Button channelListButton;
    public TMP_Text channelName_txt;
    public TMP_Text channelUser_txt;
    public TMP_Text isOnGame_txt;

    public int channelIndex { get; set; }
    public string channelName { get; set; }
    public bool isOnGame { get; set; }
    public string mapName { get; set; }
    public int channelUser { get; set; }

    void Start()
    {
        channelListButton.onClick.AddListener(ChannelListButtonClicked);
    }
    
    public void SetText()
    {
        channelName_txt.text = channelName;
        channelUser_txt.text = $"({channelUser} / 6)";

        if (!isOnGame)
        {
            isOnGame_txt.text = "(대기 중)";
        }
        else
        {
            isOnGame_txt.text = "(게임 중)";
        }
    }

    public void SetDatas(ReceiveChannelElement receiveChannelElement)
    {
        this.channelIndex = receiveChannelElement.channelIndex;
        this.channelName = receiveChannelElement.channelName;
        this.isOnGame = receiveChannelElement.isOnGame;
        this.mapName = receiveChannelElement.mapName;
        this.channelUser = receiveChannelElement.channelUser;
    }

    // 방 리스트에서 버튼 누르면
    private void ChannelListButtonClicked()
    {
        if (!isOnGame)
        {
            ReceiveChannelElement temp = new ReceiveChannelElement();
            temp.channelIndex = this.channelIndex;
            temp.channelName = this.channelName;
            temp.channelUser = this.channelUser;

            GameObject.Find("Enter Channel").GetComponent<EnterChannelPopUp>().OpenEnterChannelPopup(temp);
        }
        
        // BusinessManager.Instance.jlrRoom(Command.JOIN, channelIndex);  // chk
    }
}