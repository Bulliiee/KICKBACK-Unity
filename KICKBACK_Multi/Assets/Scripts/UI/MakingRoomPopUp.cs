using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MakingRoomPopUp : MonoBehaviour
{
    [SerializeField] private GameObject m_Room;
    [SerializeField] private TMP_InputField roomName;
    [SerializeField] private TMP_InputField roomPwd;
    public List<Button> modeButtons;
    public List<Image> checkMarks;
    public List<TMP_Text> modeTxts;
    public string modeName;

    void Start()
    {
        m_Room.SetActive(false);
    }

    public void OpenPopUp()
    {
        roomName.text = "";
        roomPwd.text = "";
        m_Room.SetActive(true);

        checkMarks[0].gameObject.SetActive(true);
    }

    public void CloseBtn()
    {  
        roomName.text = "";
        roomPwd.text = "";
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
        modeName = modeTxts[buttonIndex].text;
    }
}
