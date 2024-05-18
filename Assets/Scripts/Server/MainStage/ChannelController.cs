using System.Collections;
using System.Collections.Generic;
using System.Text;
using Highlands.Server;
using PG;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChannelController : MonoBehaviour
{
    // User 
    [Header("유저")] [SerializeField] private GameObject[] playerCard;

    // Chatting
    [Header("채팅")] [SerializeField] private GameObject chattingListContent;
    [SerializeField] private GameObject chattingElement;
    [SerializeField] private TMP_InputField chattingInput;
    [SerializeField] private Button chattingSendButton;

    // Map 
    [Header("맵")] [SerializeField] private Image mapImage;
    [SerializeField] private TMP_Text mapName;
    [SerializeField] private TMP_Dropdown dropdown;

    // Button
    [Header("버튼")] [SerializeField] private Button changeTeamButton;
    [SerializeField] private Button characterSelectButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button exitButton;

    [Header("기타")] public List<Sprite> speedMapSprites; // 변경할 스프라이트 목록
    public List<Sprite> soccerMapSprites;

    private bool chatFocus = false;

    void OnEnable()
    {
        if (NetworkManager.Instance.currentChannelInfo.channelManager.Equals(
                GameManager.Instance.loginUserInfo.NickName))
        {
            startButton.SetActive(true);
            readyButton.SetActive(false);
        }
        else
        {
            startButton.SetActive(false);
            readyButton.SetActive(true);
        }
        
        SetPlayerCard();
    }

    void Start()
    {
        exitButton.onClick.AddListener(ExitButtonClicked);
        readyButton.onClick.AddListener(ReadyButtonClicked);
        startButton.onClick.AddListener(StartButtonClicked);
        chattingSendButton.onClick.AddListener(ChattingSendButtonClicked);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (chatFocus)
            {
                ChattingSendButtonClicked();
            }
            else
            {
                // 채팅창 포커스 시키기
                chattingInput.Select();
                chatFocus = true;
            }
        }
    }

    // 채널 정보 설정
    public void SetChannelInfo(ChannelInfo channelInfo)
    {
        NetworkManager.Instance.currentChannelInfo = channelInfo;
        SetPlayerCard();
    }

    // 플레이어 카드 설정
    private void SetPlayerCard()
    {
        // 기존 플레이어 카드 지우기
        for (int i = 0; i < 6; i++)
        {
            TMP_Text nickname = playerCard[i].transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
            nickname.text = "";
        }

        // 재설정
        for (int i = 0; i < NetworkManager.Instance.currentChannelInfo.channelUserList.Count; i++)
        {
            TMP_Text nickname = playerCard[i].transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
            nickname.text = NetworkManager.Instance.currentChannelInfo.channelUserList[i];
        }
    }

    #region 채팅

    private void ChattingSendButtonClicked()
    {
        if (chattingInput.text == "")
        {
            return;
        }

        var message = MessageHandler.PackChatMessage(chattingInput.text,
            NetworkManager.Instance.currentChannelInfo.channelIndex);

        NetworkManager.Instance.SendChatMessage(message);

        chattingInput.text = "";
        chattingInput.Select();
        chattingInput.ActivateInputField();
    }

    public void UpdateChatMessage(ChatMessage message)
    {
        var sb = new StringBuilder();
        var myName = GameManager.Instance.loginUserInfo.NickName;

        if (myName.Equals(message.UserName))
        {
            sb.Append(message.UserName).Append("(나): ").Append(message.Message);
        }
        else
        {
            sb.Append(message.UserName).Append(": ").Append(message.Message);
        }

        var content = chattingListContent.transform;
        var temp = Instantiate(chattingElement, content, true);

        var tempTextComponent = temp.GetComponent<TMP_Text>();
        if (tempTextComponent != null)
        {
            tempTextComponent.text = sb.ToString();
        }

        // 20개 이상 위에서부터 제거
        if (content.childCount >= 20)
        {
            Destroy(content.GetChild(0).gameObject);
        }

        StartCoroutine(ScrollToBottom());
    }

    IEnumerator ScrollToBottom()
    {
        yield return null;

        var content = chattingListContent.transform;

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)content);

        content.parent.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
    }

    #endregion

    #region 버튼

    private void ExitButtonClicked()
    {
        NetworkManager.Instance.SendBusinessMessage(
            MessageHandler.PackJLRMessage(NetworkManager.Instance.currentChannelInfo.channelIndex, Command.LEAVE));
    }

    private void ReadyButtonClicked()
    {
        NetworkManager.Instance.SendBusinessMessage(
            MessageHandler.PackJLRMessage(NetworkManager.Instance.currentChannelInfo.channelIndex, Command.READY));
    }

    private void StartButtonClicked()
    {
        NetworkManager.Instance.SendBusinessMessage(
            MessageHandler.PackJLRMessage(NetworkManager.Instance.currentChannelInfo.channelIndex, Command.START));
    }

#endregion
}