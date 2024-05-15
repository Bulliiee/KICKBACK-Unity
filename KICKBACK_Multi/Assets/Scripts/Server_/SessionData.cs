using UnityEngine;

public class SessionData
{
    [SerializeField] private string userName;
    [SerializeField] private int userId;
    [SerializeField] private bool isReady;
    [SerializeField] private bool isHost;

    public string UserName
    {
        get
        {
            return userName;
        }
        set
        {
            userName = value;
        }
    }

    public int UserId
    {
        get
        {
            return userId;
        }
        set
        {
            userId = value;
        }
    }

    public bool IsReady
    {
        get
        {
            return isReady;
        }
        set
        {
            isReady = value;
        }
    }

    public bool IsHost
    {
        get
        {
            return isHost;
        }
        set
        {
            isHost = value;
        }
    }
}
