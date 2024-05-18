using System;
using Highlands.Server;
using UnityEngine;
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
            int index = i + 1;
            characters[i].onClick.AddListener(() => SetCharacterIndex(index));
        }
    }

    #region 캐릭터
    
    private void SetCharacterIndex(int index)
    {
        currentSelect = index;
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