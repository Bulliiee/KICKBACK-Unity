using UnityEngine;

[System.Serializable]
public class ChatMessage
{
    //[SerializeField] private string type;
    [SerializeField] private string userName;
    [SerializeField] private string message;

    //public string Type
    //{
    //    get
    //    {
    //        return type;
    //    }
    //    set
    //    {
    //        type = value;
    //    }
    //}

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

    public string Message
    {
        get
        {
            return message;
        }
        set
        {
            message = value;
        }
    }
}
