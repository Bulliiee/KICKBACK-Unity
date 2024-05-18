using System.Collections;
using System.Collections.Generic;
using Highlands.Server;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnterChannelPopUp : MonoBehaviour
{
    [SerializeField] private GameObject enterChannelPopup;
    [SerializeField] private TMP_InputField channelName;
    [SerializeField] private TMP_Text modeText;
    [SerializeField] private TMP_Text userCountText;
    [SerializeField] private TMP_Text warningText;
    [SerializeField] private Button enterChannelButton;
    [SerializeField] private Button closeButton;

    private int channelIndex;

    // Start is called before the first frame update
    void Start()
    {
        warningText.text = "";
        modeText.text = "모드 텍스트 뿌려주기";
        enterChannelPopup.SetActive(false);
        
        enterChannelButton.onClick.AddListener(EnterChannelButtonClicked);
        closeButton.onClick.AddListener(CloseButtonClicked);
    }

    // 채널 요소 눌렀을 때 팝업 띄우기
    public void OpenEnterChannelPopup(ChannelInfo channelInfo)
    {
        this.channelIndex = channelInfo.channelIndex;
        channelName.text = channelInfo.channelName;
        userCountText.text = "( " + channelInfo.channelUser + " / 6)";
        if (channelInfo.gameMode == "speed")
        {
            modeText.text = "스피드 모드";
        }
        else if (channelInfo.gameMode == "soccer")
        {
            modeText.text = "축구 모드";
        }
        
        
        enterChannelPopup.SetActive(true);
    }

    // 채널 입장 버튼 클릭 시
    private void EnterChannelButtonClicked()
    {
        // Debug.Log("채널번호 " + channelIndex + "에 " + GameManager.Instance.loginUserInfo.NickName + " 참가!");
        NetworkManager.Instance.SendBusinessMessage(MessageHandler.PackJLRMessage(channelIndex, Command.JOIN));
    }

    // 채널 닫기 버튼 클릭 시
    private void CloseButtonClicked()
    {
        enterChannelPopup.SetActive(false);
    }
}
