using System;
using Highlands.Server;
using UnityEngine;
using UnityEngine.UI;

public class CharactersSelect : MonoBehaviour
{
    [SerializeField] private GameObject charactersSelect;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button selectButton;
    [SerializeField] private int currentSelect;
    
    private void Start()
    {
        closeButton.onClick.AddListener(CloseButtonClicked);
        selectButton.onClick.AddListener(SelectButtonClicked);
    }

    #region 버튼

    public void CloseButtonClicked()
    {
        charactersSelect.SetActive(false);
    }

    public void SelectButtonClicked()
    {
        NetworkManager.Instance.SendBusinessMessage(
            MessageHandler.PackCharacterChangeMessage(NetworkManager.Instance.currentChannelInfo.channelIndex, currentSelect));
        
        CloseButtonClicked();
    }

    #endregion
}