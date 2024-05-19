using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Highlands.Server;
using PG;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class LobbyController : MonoBehaviour
{
    [Header("버튼")] 
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button tutorialCloseButton;
    [SerializeField] private Button exitGameButton;
    [SerializeField] private Button chattingSendButton;

    [Header("인풋필드")] 
    [SerializeField] private TMP_InputField chattingInput;

    [Header("게임오브젝트")] 
    [SerializeField] private GameObject channelListContent;
    [SerializeField] private GameObject userListContent;
    [SerializeField] private GameObject chattingListContent;
    [SerializeField] private GameObject createChannelPopup;
    [SerializeField] private GameObject tutorialPopup;
    [SerializeField] private GameObject enterChannelPopup;

    [Header("기타")] 
    [SerializeField] private ObjectPool channelObjectPool;
    [SerializeField] private ObjectPool userObjectPool;
    [SerializeField] private ObjectPool chattingObjectPool;

    private bool chatFocus = false;

    void Start()
    {
        tutorialButton.onClick.AddListener(TutorialButtonClicked);
        tutorialCloseButton.onClick.AddListener(TutorialCloseButtonClicked);
        chattingSendButton.onClick.AddListener(ChattingSendButtonClicked);
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable Lobby");
        // 팝업 끄기
        createChannelPopup.SetActive(false);
        enterChannelPopup.SetActive(false);
        tutorialPopup.SetActive(false);
        
        // 채팅 리스트 초기화
        int chattingCount = chattingListContent.transform.childCount;
        for (int i = chattingCount - 1; i >= 0; i--)
        {
            chattingObjectPool.ReturnObject(chattingListContent.transform.GetChild(i).gameObject);
        }
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
                chattingInput.Select();
                chatFocus = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && tutorialPopup.activeSelf)
        {
            TutorialCloseButtonClicked();
        }
    }

    #region 채널

    // 채널 목록 불러오기
    public void SetChannelList(List<string> receiveChannelListJson)
    {
        // 기존 방 목록 제거
        int channelCount = channelListContent.transform.childCount;
        for (int i = channelCount - 1; i >= 0; i--)
        {
            channelObjectPool.ReturnObject(channelListContent.transform.GetChild(i).gameObject);
        }

        // 새로운 방 목록 생성
        for (int i = 0; i < receiveChannelListJson.Count; i++)
        {
            // 받은 데이터 파싱
            string tempJson = receiveChannelListJson[i];
            if (i != receiveChannelListJson.Count - 1)
            {
                tempJson += "}";
            }

            ChannelInfo temp = JsonUtility.FromJson<ChannelInfo>(tempJson);

            // 오브젝트 풀링
            GameObject channelElement = channelObjectPool.GetObject();
            // 값 설정 및 텍스트 보이기
            channelElement.GetComponent<ChannelListElement>().SetDatas(temp);
            channelElement.GetComponent<ChannelListElement>().SetText();
            // 부모 정하기
            channelElement.transform.SetParent(channelListContent.transform);
            // 사이즈 조절
            channelElement.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    // 채널 정보 설정 및 채널 화면 이동
    public void SetChannelInfo(ChannelInfo channelInfo)
    {
        NetworkManager.Instance.currentChannelInfo = channelInfo;
        
        NetworkManager.Instance.SendChatMessage(MessageHandler.PackChatMoveMessage(NetworkManager.Instance.currentChannelInfo.channelIndex));

        GameManager.Instance.ChangeMainStageCanvas("Channel Canvas");
        NetworkManager.Instance.currentPlayerLocation = CurrentPlayerLocation.WaitingRoom;
    }

    #endregion

    #region 유저목록

    public void SetUserList(List<string> userList)
    {
        // 기존 유저 목록 제거
        int userCount = userListContent.transform.childCount;
        for (int i = userCount - 1; i >= 0; i--)
        {
            userObjectPool.ReturnObject(userListContent.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < userList.Count; i++)
        {
            // 오브젝트 풀링
            GameObject userElement = userObjectPool.GetObject();
            // 값 설정 및 텍스트 보이기
            userElement.transform.GetChild(1).GetComponent<TMP_Text>().text = userList[i];
            // 부모 정하기
            userElement.transform.SetParent(userListContent.transform);
            // 사이즈 조절
            userElement.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    #endregion

    #region 채팅

    public void ChattingSendButtonClicked()
    {
        if (chattingInput.text == "")
        {
            return;
        }
        var message = MessageHandler.PackChatMessage(chattingInput.text, 0);
        
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

    #region 기타

    // 튜토리얼 창 열기
    private void TutorialButtonClicked()
    {
        tutorialPopup.SetActive(true);
    }

    // 튜토리얼 창 닫기
    private void TutorialCloseButtonClicked()
    {
        tutorialPopup.SetActive(false);
    }

    #endregion
}