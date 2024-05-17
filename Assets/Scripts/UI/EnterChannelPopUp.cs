using System.Collections;
using System.Collections.Generic;
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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenEnterChannelPopup(ReceiveChannelElement channelInfo)
    {
        this.channelIndex = channelInfo.channelIndex;
        channelName.text = channelInfo.channelName;
        userCountText.text = "( " + channelInfo.channelUser + " / 6)";
        
        enterChannelPopup.SetActive(true);
    }

    private void EnterChannelButtonClicked()
    {
        Debug.Log("채널번호 " + channelIndex + "에 " + GameManager.Instance.loginUserInfo.NickName + " 참가!");
    }

    private void CloseButtonClicked()
    {
        enterChannelPopup.SetActive(false);
    }
}
