using System.Collections;
using System.Collections.Generic;
using Highlands.Server;
using PG;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MakingRoomPopUp : MonoBehaviour
{
    [SerializeField] private GameObject m_Room;
    [SerializeField] private TMP_InputField roomName;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button createChannelButton;
    [SerializeField] private TMP_Text warningText;
    public List<Button> modeButtons;
    public List<Image> checkMarks;
    public List<TMP_Text> modeTxts;
    public string modeName;

    void Start()
    {
        m_Room.SetActive(false);
        warningText.text = "";
        warningText.SetActive(false);

        closeButton.onClick.AddListener(CloseBtn);
        createChannelButton.onClick.AddListener(CreateChannelButtonClicked);

        for (int i = 0; i < checkMarks.Count; i++)
        {
            int index = i;
            modeButtons[i].onClick.AddListener(() => ClickMode(index));
        }
    }

    public void OpenPopUp()
    {
        roomName.text = "";
        m_Room.SetActive(true);

        checkMarks[0].gameObject.SetActive(true);
    }

    public void CloseBtn()
    {  
        roomName.text = "";
        m_Room.SetActive(false);

        // 모든 체크마크를 먼저 비활성화
        for (int i = 0; i < checkMarks.Count; i++)
        {
            checkMarks[i].gameObject.SetActive(false);
        }
    }

    // 버튼 클릭 시 호출될 메소드. 인덱스를 매개변수로 받음
    public void ClickMode(int buttonIndex)
    {
        // 모든 체크마크를 먼저 비활성화
        for (int i = 0; i < checkMarks.Count; i++)
        {
            checkMarks[i].gameObject.SetActive(false);
        }

        // 클릭된 버튼에 해당하는 체크마크만 활성화
        checkMarks[buttonIndex].gameObject.SetActive(true);
        // 모드 이름 할당
        if (modeTxts[buttonIndex].text == "스피드 모드")
        {
            modeName = "speed";
        }
        else if (modeTxts[buttonIndex].text == "축구 모드")
        {
            modeName = "soccer";
        }
        
    }

    // 방 생성 버튼 클릭 시
    public void CreateChannelButtonClicked()
    {
        // 방 제목 없을 때
        if (roomName.text == "")
        {
            warningText.text = "방 제목을 입력 하세요!";
            warningText.SetActive(true);
        }
        // 방 제목 있을 때(정상 생성
        else
        {
            warningText.text = "";
            warningText.SetActive(false);
            NetworkManager.Instance.SendBusinessMessage(MessageHandler.PackCreateMessage(roomName.text, "", modeName));
        }
    }
}
