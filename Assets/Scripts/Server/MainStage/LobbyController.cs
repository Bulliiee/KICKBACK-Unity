using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyController : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button exitGameButton;
    [SerializeField] private Button chattingSendButton;

    [Header("인풋필드")]
    [SerializeField] private TMP_InputField chattingInput;

    [Header("게임오브젝트")]
    [SerializeField] private GameObject channelListContent;
    [SerializeField] private GameObject userListContent;
    [SerializeField] private GameObject chattingListContent;
    [SerializeField] private GameObject tutorialPopup;

    [Header("프리팹")]
    [SerializeField] private GameObject channelElement;
    [SerializeField] private GameObject userElement;
    [SerializeField] private GameObject chattingElement;

    private List<string> _userList;
    private List<string> _channelList;
    private ChannelInfo _channelInfo;

    #region 채널목록

    public void SetUserList(List<string> userList)
    {
        
    }

    public void SetChannelList(List<string> channelList)
    {
        _channelList = channelList;
        for (int i = 0; i < _channelList.Count; i++)
        {
            Debug.Log(_channelList[i]);
        }
    }

    public void SetChannelInfo()
    {
        
    }

    #endregion

    #region 유저목록



    #endregion

    #region 채팅



    #endregion
}
