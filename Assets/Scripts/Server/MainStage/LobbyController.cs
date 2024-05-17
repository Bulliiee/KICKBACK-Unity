using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class LobbyController : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button exitGameButton;
    [SerializeField] private Button chattingSendButton;

    [Header("인풋필드")]
    [SerializeField] private TMP_InputField chattingInput;

    [Header("게임오브젝트")]
    [SerializeField] private GameObject channelListContent;
    [SerializeField] private GameObject userListContent;
    [SerializeField] private GameObject chattingListContent;
    [SerializeField] private GameObject tutorialPopup;

    [Header("프리팹")]
    [SerializeField] private GameObject channelElement;
    [SerializeField] private GameObject userElement;
    [SerializeField] private GameObject chattingElement;

    [Header("기타")] 
    [SerializeField] private ObjectPool channelObjectPool;

    private List<string> _userList;
    private List<string> _channelList;
    private ChannelInfo _channelInfo;

    #region 채널목록

    public void SetUserList(List<string> userList)
    {
        
    }

    public void SetChannelList(List<string> receiveChannelListJson)
    {
        // 기존 방 목록 제거
        for (int i = 0; i < channelListContent.transform.childCount; i++)
        {
            channelObjectPool.ReturnObject(channelListContent.transform.GetChild(i).gameObject);
        }
        
        // 새로운 방 생성
        for (int i = 0; i < receiveChannelListJson.Count; i++)
        {
            // 받은 데이터 파싱
            string tempJson = receiveChannelListJson[i];
            if (i != receiveChannelListJson.Count-1)
            {
                tempJson += "}";
            }
            ReceiveChannelElement temp = JsonUtility.FromJson<ReceiveChannelElement>(tempJson);
            
            // 오브젝트 풀링
            GameObject channelElement = channelObjectPool.GetObject();
            // 값 설정 및 텍스트 보이기
            channelElement.GetComponent<ChannelListElement>().SetDatas(temp);
            channelElement.GetComponent<ChannelListElement>().SetText();
            // 부모 정하기
            channelElement.transform.SetParent(channelListContent.transform);
            // 사이즈 조절
            channelElement.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    public void SetChannelInfo()
    {
        
        
    }

    #endregion

    #region 유저목록



    #endregion

    #region 채팅



    #endregion
}
