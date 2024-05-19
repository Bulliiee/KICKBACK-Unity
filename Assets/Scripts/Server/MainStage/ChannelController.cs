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
        }
        else
        {
            startButton.SetActive(false);
            readyButton.SetActive(true);
        }

        // 게임 모드에 따라 버튼 변경
        if (NetworkManager.Instance.currentChannelInfo.gameMode == "speed")
        {
            changeTeamButton.gameObject.SetActive(false);

            if (NetworkManager.Instance.currentChannelInfo.channelManager.Equals(
                    GameManager.Instance.loginUserInfo.NickName))
            {
                dropdown.gameObject.SetActive(true);
            }
            else
            {
                dropdown.gameObject.SetActive(false);
            }
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
        changeTeamButton.onClick.AddListener(ChangeTeamButtonClicked);

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
        StartButtonActive();

        // 만약 게임 시작이 되었다면 UDP 설정 및 UDP 내부 채널에 JOIN
        if (NetworkManager.Instance.currentChannelInfo.isOnGame)
        {
            SendGameStartToUDP(NetworkManager.Instance.currentChannelInfo.channelIndex);
        }
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

            Image team = playerCard[i].transform.GetChild(0).GetComponent<Image>();
            team.color = new Color(0.6584118f, 0.5814791f, 0.6886792f);
            
            Image playerCharacter = playerCard[i].transform.GetChild(1).GetComponent<Image>();
            playerCharacter.SetActive(false);

            Image ready = playerCard[i].transform.GetChild(2).GetComponent<Image>();
            ready.SetActive(false);
        }

        // 재설정
        for (int i = 0; i < NetworkManager.Instance.currentChannelInfo.channelUserList.Count; i++)
        {
            TMP_Text nickname = playerCard[i].transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
            nickname.text = NetworkManager.Instance.currentChannelInfo.channelUserList[i];

            if (NetworkManager.Instance.currentChannelInfo.gameMode.Equals("soccer"))
            {
                Image color = playerCard[i].transform.GetChild(0).GetComponent<Image>();

                if (NetworkManager.Instance.currentChannelInfo.teamColor[i] == 0)
                {
                    color.color = new Color(0f, 0.07142854f, 1f);
                }
                else
                {
                    color.color = new Color(1f, 0f, 0.0947485f);
                }
            }
            

            if (!nickname.text.Equals(""))
            {
                Image playerCharacter = playerCard[i].transform.GetChild(1).GetComponent<Image>();
                playerCharacter.sprite = characterSprites[NetworkManager.Instance.currentChannelInfo.userCharacter[i]];
                playerCharacter.SetActive(true);
                Image ready = playerCard[i].transform.GetChild(2).GetComponent<Image>();

                // 레디 했을 때 레디 이미지 표시
                if (NetworkManager.Instance.currentChannelInfo.isReady[i] && i != 0)
                {
                    ready.SetActive(true);
                }
                else
                {
                    ready.SetActive(false);
                }

                if (nickname.text.Equals(GameManager.Instance.loginUserInfo.NickName))
                {
                    // 레디 했을 때 캐릭터 선택 비활성화
                    if (NetworkManager.Instance.currentChannelInfo.isReady[i])
                    {
                        if (i != 0)
                        {
                            characterSelectButton.interactable = false;
                        }
                    }
                    else
                    {
                        characterSelectButton.interactable = true;
                    }

                    // 방장이 바꼈을 때 시작버튼으로 변경
                    if (NetworkManager.Instance.currentChannelInfo.channelManager.Equals(GameManager.Instance
                            .loginUserInfo.NickName))
                    {
                        readyButton.SetActive(false);
                        startButton.SetActive(true);
                    }
                }
            }
        }
    }

    // 모든 플레이어가 레디를 했을 경우에만 시작 버튼 활성화
    private void StartButtonActive()
    {
        int count = 0;

        for (int i = 0; i < NetworkManager.Instance.currentChannelInfo.isReady.Count; i++)
        {
            if (NetworkManager.Instance.currentChannelInfo.isReady[i])
            {
                count++;
            }
        }

        if (count == 6)
        {
            startButton.interactable = true;
        }
        else
        {
            startButton.interactable = false;
        }
    }

    // UDP 서버에 연결 설정 및 JOIN 요청
    private void SendGameStartToUDP(int channelIndex)
    {
        // UDP 연결 설정
        NetworkManager.Instance.ConnectLiveServer();
        
        // live에 join 요청
        NetworkManager.Instance.SendLiveMessage(
            MessageHandler.PackUDPInitialMessage(channelIndex));

        NetworkManager.Instance.currentPlayerLocation = CurrentPlayerLocation.InGame;
        
        // TEST: 좌표 데이터 보내기
        NetworkManager.Instance.SendLiveMessage(
            MessageHandler.PackUDPPointMessage(channelIndex,
                NetworkManager.Instance.currentChannelInfo.myIndex,
                new Vector3(0.1f, 0.2f, 0.3f),
                new Quaternion(0.4f, 0.5f, 0.6f, 0.7f)));
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
        // business에 시작 요청
        NetworkManager.Instance.SendBusinessMessage(
            MessageHandler.PackStartMessage(Command.START,
                NetworkManager.Instance.currentChannelInfo.channelIndex,
                NetworkManager.Instance.currentChannelInfo.gameMode));
    }

    // 팀 변경
    private void ChangeTeamButtonClicked()
    {
        NetworkManager.Instance.SendBusinessMessage(
            MessageHandler.PackTeamChangeMessage(
                NetworkManager.Instance.currentChannelInfo.channelIndex));
    }
    
    // 캐릭터 선택 팝업 켜기
    private void CharacterSelectPopupOpen()
    {
        characterSelectPopup.SetActive(true);
    }

    #endregion
}