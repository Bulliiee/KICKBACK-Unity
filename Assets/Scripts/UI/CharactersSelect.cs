using System;
using Highlands.Server;
using PG;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class CharactersSelect : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private GameObject charactersSelect;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button selectButton;

    [Header("캐릭터")] 
    [SerializeField] private Button[] characters;
    [SerializeField] private int currentSelect;
    
    private void Start()
    {
        closeButton.onClick.AddListener(CloseButtonClicked);
        selectButton.onClick.AddListener(SelectButtonClicked);
        
        for (int i = 0; i < characters.Length; i++)
        {
            int index = i;
            characters[i].onClick.AddListener(() => SetCharacterIndex(index));
        }
    }

    private void OnEnable()
    {
        int myIndex = NetworkManager.Instance.currentChannelInfo.myIndex;
        CharacterCheckMark(NetworkManager.Instance.currentChannelInfo.userCharacter[myIndex]);
    }

    #region 캐릭터
    
    private void SetCharacterIndex(int index)
    {
        currentSelect = index;

        CharacterCheckMark(index);
    }

    // 캐릭터 선택 표시
    private void CharacterCheckMark(int index)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].transform.GetChild(1).SetActive(false);
        }
        
        characters[index].transform.GetChild(1).SetActive(true);
    }
    

    #endregion
    
    #region 버튼

    private void CloseButtonClicked()
    {
        charactersSelect.SetActive(false);
    }

    private void SelectButtonClicked()
    {
        NetworkManager.Instance.SendBusinessMessage(
            MessageHandler.PackCharacterChangeMessage(NetworkManager.Instance.currentChannelInfo.channelIndex, currentSelect));
        
        CloseButtonClicked();
    }

    #endregion
}