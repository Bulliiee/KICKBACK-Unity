using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    [Header ("LoginUserInfo")]
    public string accessToken; // access Token
    //public string refreshToken; // refresh Token
    public User loginUserInfo; // 로그인 한 유저 정보

    [Header("ChannelInfo")]
    public int channelIndex = -1; // 접속 채널 인덱스
    public string channelName; // 접속 채널 이름
    public int cnt; // 채널 참가 인원
    public bool isOnGame; // 게임 중 여부
    public List<string> roomUserList; // 방에 접속한 인원들을 전부 string으로 받아 parsing
    public int myIndex;
    public List<SessionData> sessionList; // 참가 중인 유저 정보 (players와 인덱스 공유)
    public int clearUserId = -1; // 클리어 한 유저 아이디
    public bool isReady; //레디 상태 여부

    [Header("Player")]
    public int myIdx = -1; // sessionList 중 나의 인덱스
    public List<GameObject> players; // 플레이어들 정보 (sessionList와 인덱스 공유)
    public int[] score; // 인덱스 별 점수

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool isMe(string userName)
    {
        return sessionList[myIdx].UserName == userName;
    }

    public bool isMe(int userId)
    {
        return sessionList[myIdx].UserId == userId;
    }

    public int getUserIndex(string userName)
    {
        for (int i = 0; i < cnt; i++)
        {
            if (sessionList[i].UserName == userName)
            {
                return i;
            }
        }

        return -1;
    }

    public int getUserIndex(int userId)
    {
        for (int i = 0; i < cnt; i++)
        {
            if (sessionList[i].UserId == userId)
            {
                return i;
            }
        }

        return -1;
    }

    public void dataClear()
    {
        accessToken = "";
        //refreshToken = "";
        loginUserInfo = null;
        channelIndex = -1;
        channelName = "";
        cnt = -1;
        isOnGame = false;
        //sessionList.Clear();
        clearUserId = -1;
        myIdx = -1;
        players.Clear();
        score = null;
    }

    public void gameDataClear()
    {
        channelIndex = -1;
        channelName = "";
        cnt = -1;
        isOnGame = false;
        //sessionList.Clear();
        clearUserId = -1;
        myIdx = -1;
        players.Clear();
        score = null;
    }
}
