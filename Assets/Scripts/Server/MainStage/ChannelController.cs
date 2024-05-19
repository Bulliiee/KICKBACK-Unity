using System.Collections;
using System.Collections.Generic;
using System.Text;
using Highlands.Server;
using PG;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChannelController : MonoBehaviour
{
    // User 
    [Header("유저")] [SerializeField] private GameObject[] playerCard;

    // Chatting
    [Header("채팅")] [SerializeField] private ObjectPool chattingObjectPool;
    [SerializeField] private GameObject chattingListContent;
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

    [Header("캐릭터 선택")] [SerializeField] private GameObject characterSelectPopup;

    [Header("기타")] [SerializeField] private List<Sprite> mapSprites;
    [SerializeField] private List<Sprite> characterSprites;

    private bool chatFocus = false;

    void OnEnable()
    {
        // 채팅 리스트 초기화
        int chattingCount = chattingListContent.transform.childCount;
        for (int i = chattingCount - 1; i >= 0; i--)
        {
            chattingObjectPool.ReturnObject(chattingListContent.transform.GetChild(i).gameObject);
        }
        
        // 방장 여부에 따라 버튼 변경
        if (NetworkManager.Instance.currentChannelInfo.channelManager.Equals(
                GameManager.Instance.loginUserInfo.NickName))
        {
            startButton.SetActive(true);
            readyButton.SetActive(false);
            dropdown.gameObject.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
            readyButton.SetActive(true);
            dropdown.gameObject.SetActive(false);
        }

        // 게임 모드에 따라 버튼 변경
        if (NetworkManager.Instance.currentChannelInfo.gameMode == "speed")
        {
            changeTeamButton.gameObject.SetActive(false);
            dropdown.gameObject.SetActive(true);
        }
        else if (NetworkManager.Instance.currentChannelInfo.gameMode == "soccer")
        {
            changeTeamButton.gameObject.SetActive(true);
            dropdown.gameObject.SetActive(false);
        }
        
        // 드롭다운 초기화
        dropdown.value = 0;

        // 선택한 맵에 따라 관련된 것 설정
        SetMap();

        // 플레이어 카드 설정
        SetPlayerCard();
    }

    void Start()
    {
        exitButton.onClick.AddListener(ExitButtonClicked);
        readyButton.onClick.AddListener(ReadyButtonClicked);
        startButton.onClick.AddListener(StartButtonClicked);
        chattingSendButton.onClick.AddListener(ChattingSendButtonClicked);
        characterSelectButton.onClick.AddListener(CharacterSelectPopupOpen);

        dropdown.onValueChanged.AddListener(delegate { SelectMapDropdown(dropdown); });
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

    #region 설정

    // 채널 정보 설정
    public void SetChannelInfo(ChannelInfo channelInfo)
    {
        NetworkManager.Instance.currentChannelInfo = channelInfo;
        SetPlayerCard();
        SetMap();
    }

    // 드롭다운 설정
    public void SelectMapDropdown(TMP_Dropdown changedDropdown)
    {
        string tempName = changedDropdown.options[changedDropdown.value].text;
        int currChannelIndex = NetworkManager.Instance.currentChannelInfo.channelIndex; 
        
        NetworkManager.Instance.currentChannelInfo.mapName = tempName;
        // 서버에 맵 바꿨다고 알려주기
        NetworkManager.Instance.SendBusinessMessage(MessageHandler.PackChangeMapMessage(tempName, currChannelIndex));
        // SetMap();   // 서버에서 인포 쏴주면 SetChannelInfo() -> SetMap() 해서 안해도 됨
    }

    // 맵 이미지, 이름 설정
    private void SetMap()
    {
        string currentMapName = NetworkManager.Instance.currentChannelInfo.mapName;

        string[] mapNames =
        {
            "Cebu",
            "Mexico",
            "Downhill",
            "Football Stadium"
        };

        for (int i = 0; i < mapNames.Length; i++)
        {
            if (currentMapName == mapNames[i])
            {
                mapImage.sprite = mapSprites[i];
                mapName.text = mapNames[i];
            }
        }
    }
    
    // 플레이어 카드 설정
    private void SetPlayerCard()
    {
        // 기존 플레이어 카드 지우기
        for (int i = 0; i < 6; i++)
        {
            TMP_Text nickname = playerCard[i].transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
            nickname.text = "";

            Image playerCharacter = playerCard[i].transform.GetChild(1).GetComponent<Image>();
            playerCharacter.SetActive(false);
        }

        // 재설정
        for (int i = 0; i < NetworkManager.Instance.currentChannelInfo.channelUserList.Count; i++)
        {
            TMP_Text nickname = playerCard[i].transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
            nickname.text = NetworkManager.Instance.currentChannelInfo.channelUserList[i];

            if (!nickname.text.Equals(""))
            {
                Image playerCharacter = playerCard[i].transform.GetChild(1).GetComponent<Image>();
                playerCharacter.sprite = characterSprites[NetworkManager.Instance.currentChannelInfo.userCharacter[i]];
                playerCharacter.SetActive(true);
            }
        }
    }

    #endregion
    
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

        // 오브젝트 풀링
        GameObject chattingElement = chattingObjectPool.GetObject();
        // 값 설정 및 채팅 보이기
        chattingElement.GetComponent<TMP_Text>().text = sb.ToString();
        // 부모 정하기
        chattingElement.transform.SetParent(chattingListContent.transform);
        // 사이즈 조절
        chattingElement.transform.localScale = new Vector3(1f, 1f, 1f);

        // 20개 이상 위에서부터 제거
        if (chattingListContent.transform.childCount >= 20)
        {
            chattingObjectPool.ReturnObject(chattingListContent.transform.GetChild(0).gameObject);
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
        // 먼저 나가야 로비에서 필요한 정보 받는데 순서 맞음
        GameManager.Instance.ChangeMainStageCanvas("Lobby Canvas");

        NetworkManager.Instance.SendBusinessMessage(
            MessageHandler.PackJLRMessage(NetworkManager.Instance.currentChannelInfo.channelIndex, Command.LEAVE));

        NetworkManager.Instance.SendChatMessage(
            MessageHandler.PackChatLeaveMessage());

        NetworkManager.Instance.currentPlayerLocation = CurrentPlayerLocation.Lobby;
    }

    private void ReadyButtonClicked()
    {
        NetworkManager.Instance.SendBusinessMessage(
            MessageHandler.PackJLRMessage(NetworkManager.Instance.currentChannelInfo.channelIndex, Command.READY));
    }

    private void StartButtonClicked()
    {
        NetworkManager.Instance.SendBusinessMessage(
            MessageHandler.PackStartMessage(Command.START, 
                NetworkManager.Instance.currentChannelInfo.channelIndex, 
                NetworkManager.Instance.currentChannelInfo.gameMode));
    }

    // 캐릭터 선택 팝업 켜기
    private void CharacterSelectPopupOpen()
    {
        characterSelectPopup.SetActive(true);
    }

    #endregion
}