using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyController : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button createChannelPopupButton;
    [SerializeField] private Button exitGameButton;
    [SerializeField] private Button chattingSendButton;

    [Header("인풋필드")]
    [SerializeField] private TMP_InputField chattingInput;

    [Header("게임오브젝트")]
    [SerializeField] private GameObject channelListContent;
    [SerializeField] private GameObject userListContent;
    [SerializeField] private GameObject chattingListContent;
    [SerializeField] private GameObject createChannelPopup;

    [Header("프리팹")]
    [SerializeField] private GameObject channelElement;
    [SerializeField] private GameObject userElement;
    [SerializeField] private GameObject chattingElement;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
