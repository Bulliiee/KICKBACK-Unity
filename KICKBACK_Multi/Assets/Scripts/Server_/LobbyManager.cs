using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private GameObject LoginCanvas; // 로그인 캔버스
    [SerializeField] private GameObject LobbyCanvas; // 로비 캔버스
    [SerializeField] private Button LogoutBtn; // 로그 아웃 버튼
    
    [Header("Lobby")]
    public GameObject ScrollViewLobbyList;  // 모든 유저 리스트 보일 스크롤뷰
    public GameObject UserListElement;      // 유저 리스트에 들어갈 유저 엘리먼트
    public Button LobbyAllListBtn;          // 모든 유저 리스트 보일 버튼

    [Header("Room")]
    public GameObject ScrollViewChannelList;    // 채널 리스트
    public GameObject ChannelListElement;       // 채널 리스트 항목
    public ChannelListElement SelectedChannel;  // 선택된 채널 리스트

    void Start()
    {
        TCPConnectManager.Instance.ConnectToServer();
        BusinessManager.Instance.ConnectToServer();
        BusinessManager.Instance.LobbyManagerScript = gameObject.GetComponent<LobbyManager>();
        
        // 로그인 캔버스와 로비 캔버스를 태그로 찾아서 할당
        LoginCanvas = GameObject.FindWithTag("Login Canvas");
        LobbyCanvas = GameObject.FindWithTag("Lobby Canvas");
        ScrollViewLobbyList = GameObject.Find("User List");
        ScrollViewChannelList = GameObject.Find("Room List");

        //// 씬에서 로그아웃 버튼을 찾아서 버튼 컴포넌트를 할당
        //GameObject logoutButtonObject = GameObject.Find("Logout Btn");
        //if (logoutButtonObject != null)
        //{
        //    LogoutBtn = logoutButtonObject.GetComponent<Button>();

        //    // 로그아웃 버튼에 클릭 리스너 추가
        //    if (LogoutBtn != null)
        //    {
        //        LogoutBtn.onClick.RemoveAllListeners(); // 기존 리스너 제거
        //        LogoutBtn.onClick.AddListener(LobbyOut); // 새로운 리스너 추가
        //    }
        //}
    }

    //public void LobbyOut()
    //{
    //    SceneManager.LoadScene("Login");

    //    // 로그인 캔버스 활성화, 로비 캔버스 비활성화
    //    //if (LoginCanvas != null)
    //        LoginCanvas.SetActive(true);
    //    //if (LobbyCanvas != null)
    //        LobbyCanvas.SetActive(false);

    //    // 기타 초기화 작업 수행
    //    TCPConnectManager.Instance.OnApplicationQuit();
    //    BusinessManager.Instance.OnApplicationQuit();
    //    DataManager.Instance.dataClear();
    //}
    
    // 전체 유저 불러오기
    public void getAllUsers(List<string> userList)
    {
        if(userList != null)
        {
            // 유저 리스트 들어갈 스크롤뷰(설정할 부모)
            Transform content = ScrollViewLobbyList.transform.Find("All Users Scroll View/Viewport/Content");
            // 기존 리스트 제거
            // TODO: 오브젝트 풀링
            foreach(Transform child in content) {
                Destroy(child.gameObject);
            }

            // 유저 리스트에 들어갈 각각의 컴포넌트 생성
            GameObject[] userListElements = new GameObject[userList.Count];

            // 유저 리스트 각각의 컴포넌트에 들어갈 데이터
            //GameObject[] activeUserData = new GameObject[userList.Length];
            for (int i = 0; i < userList.Count; i++)
            {
                // 자기 자신은 건너뛰기
                // if(userList[i] == DataManager.Instance.loginUserInfo.NickName)   // chk
                // {
                //     continue;
                // }
                // 요소 생성 및 부모 설정
                userListElements[i] = Instantiate(UserListElement);
                userListElements[i].transform.SetParent(content, false);
                // 유저 데이터 초기화
                UserListElement userListElementScript = userListElements[i].GetComponent<UserListElement>();
                userListElementScript.Nickname = userList[i];
                // 데이터 보이기
                userListElementScript.UserNameText.text = userList[i];
            }

            // 유저 접속 인원 텍스트 업데이트
            // ConnectedUserCount.text = "접속 인원: " + userList.Length;
        }
    }
    
    [Serializable]
    class ChannelInfo
    {
        public int roomIndex;   // 방 번호
        public string roomName; // 방 제목
        public string mapName; // 맵 이름
        public bool isOnGame; // 게임중인지 아닌지
        public int roomUser; // 몇명 참여중인지
    }
    
    // 방 리스트 불러오기
    public void getRoomList(List<string> roomList)
    {
        // 채널 리스트 들어갈 스크롤뷰(설정할 부모)
        Transform content = ScrollViewChannelList.transform.Find("Scroll View/Viewport/Content");
        // 기존 리스트 제거
        // TODO: 오브젝트 풀링
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        for(int i = 0; i < roomList.Count; i++)
        {   
            
            // 참가 인원 꽉차면 안보이게 하기
            // if(channelList.data[i].isOnGame || channelList.data[i].cnt == 6)
            // {
            //     continue;
            // }

            //Debug.Log("방이름: " + channelList.data[i].channelName);
            // 프리팹 만들기
            ChannelInfo roomInfo;
            if (i != roomList.Count - 1)
            {
                roomInfo = JsonUtility.FromJson<ChannelInfo>(roomList[i] + "}");
            }
            else
            {
                roomInfo = JsonUtility.FromJson<ChannelInfo>(roomList[i]);
            }
            Debug.Log(roomInfo.roomUser + "현재 방 인원수");
            GameObject channelListElement = Instantiate(ChannelListElement);
            ChannelListElement channelListElementScript = channelListElement.GetComponent<ChannelListElement>();
            channelListElementScript.RoomIndex = roomInfo.roomIndex;
            channelListElementScript.RoomName = roomInfo.roomName;
            channelListElementScript.mapName = roomInfo.mapName;
            channelListElementScript.roomUser = roomInfo.roomUser;
            channelListElementScript.IsOnGame = roomInfo.isOnGame;
            
            // 부모 붙이기
            channelListElement.transform.SetParent(content, false);
        }
    }
    
    
    //
    // // 방 만들기 버튼 클릭 시
    // public void CreateChannelBtnClicked()
    // {
    //     CreateChannel.SetActive(true);
    // }
    //
    // // 방 참가버튼 클릭 시
    // public void JoinChannelBtnClicked()
    // {
    //     TCPConnectManager.Instance.joinChannel();   // chk
    // }
}
