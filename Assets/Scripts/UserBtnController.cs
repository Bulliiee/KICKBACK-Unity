using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserBtnController : MonoBehaviour
{
    [SerializeField]
    private GameObject AllUsersPanel;
    [SerializeField]
    private GameObject FriendsPanel;
    [SerializeField]
    private Button AllUsersBtn;
    [SerializeField]
    private Button FriendsBtn;


    // Start is called before the first frame update
    void Start()
    {
        AllUsersPanel.SetActive(true);
        FriendsPanel.SetActive(false);
        AllUsersBtn.interactable = false;
    }

    public void ClickAllUsers()
    {
        if (!AllUsersPanel.activeSelf && FriendsPanel.activeSelf)
        {
            AllUsersPanel.SetActive(true);
            FriendsPanel.SetActive(false);
            AllUsersBtn.interactable = false;
            FriendsBtn.interactable = true;
        }
    }

    public void ClickFriends()
    {
        if (!FriendsPanel.activeSelf && AllUsersPanel.activeSelf)
        {
            FriendsPanel.SetActive(true);
            AllUsersPanel.SetActive(false);
            FriendsBtn.interactable = false;
            AllUsersBtn.interactable = true;
        }
    }
}
