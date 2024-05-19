using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UserListElement : MonoBehaviour
{
    // public LobbyManager lobbyManagerScript;
    public Button DetailBtn;
    public TMP_Text UserNameText;
    [SerializeField] private new string nickname;
    
    public string Nickname
    {
        get { return nickname; }
        set { nickname = value; }
    }


    private void Start()
    {
        // 이벤트 붙이기
        // lobbyManagerScript = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
    }
}