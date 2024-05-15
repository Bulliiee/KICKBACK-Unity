using System;
using UnityEngine;


[Serializable]
public class DataBody
{
    public string nickname;
}

[Serializable]
public class User
{
    [SerializeField] private int userId;
    [SerializeField] private string email;
    [SerializeField] private string password;
    [SerializeField] private string nickname;
    [SerializeField] public DataBody dataBody;
    //[SerializeField] private string profileImg;

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

    public string Email
    {
        get
        {
            return email;
        }
        set
        {
            email = value;
        }
    }

    public string Password
    {
        get
        {
            return password;
        }
        set
        {
            password = value;
        }
    }

    public string NickName
    {
        get
        {
            return nickname;
        }
        set
        {
            nickname = value;
        }
    }

    //public string ProfileImg
    //{
    //    get
    //    {
    //        return profileImg;
    //    }
    //    set
    //    {
    //        profileImg = value;
    //    }
    //}
}