using Highlands.Server;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChannelController : MonoBehaviour
{
    // User 
    [SerializeField] private GameObject[] playerCard;

    // Chatting
    [SerializeField] private GameObject chattingList;
    [SerializeField] private TMP_Text chattingMessage;
    [SerializeField] private TMP_InputField chattingInput;
    [SerializeField] private Button chattingSendButton;

    // Map 
    [SerializeField] private Image mapImage;
    [SerializeField] private TMP_Dropdown dropdown;

    // Button
    [SerializeField] private Button characterSelectButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button exitButton;

    void Start()
    {
        TMP_Text nickname = playerCard[0].transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        nickname.text = "asdf";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (chattingInput.isFocused)
            {
                // 채팅창 포커스인 경우 채팅 보내기
                SendChattingMessage();
            }
            else
            {
                // 채팅창 포커스 시키기
                chattingInput.Select();
            }
        }
    }

    #region 채팅

    private void SendChattingMessage()
    {
        string myName = GameManager.Instance.loginUserInfo.dataBody.nickname;
        int channelIndex;
        var buffer = MessageHandler.PackChatMessage(chattingInput, channelIndex, myName);
        
        NetworkManager.Instance.SendChatMessage(buffer);
        throw new System.NotImplementedException();
    }

    #endregion
    
}
