using System.Collections;
using System.Collections.Generic;
using Highlands.Server;
using PG;
using UnityEngine;

public class IngameController : MonoBehaviour
{
    [SerializeField] private GameObject[] PlayerPrefabs; // 캐릭터 프리팹
    [SerializeField] private Transform[] SpawnPoints; // 스폰장소
    [SerializeField] private CheckPointsController[] _checkPointsControllers; // 체크포인트 초기화 위함
    [SerializeField] private Boostpad[] _boostpads; // 부스트패드

    [SerializeField] public List<GameObject> PlayerCharacters; // 실제 플레이어 캐릭터

    [SerializeField] public int myIndex; // 내 인덱스
    
    public bool isReceiving = false;

    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Instance.IngameController = this;
        SpawnPlayer();
        
        if (!isReceiving)
        {
            isReceiving = true;
            StartCoroutine(NetworkManager.Instance.UpdateLiveLogCoroutine());
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // 내 현재 좌표 계속 보내기
        NetworkManager.Instance.SendLiveMessage(
            MessageHandler.PackUDPPointMessage(
                NetworkManager.Instance.currentChannelInfo.channelIndex,
                NetworkManager.Instance.currentChannelInfo.myIndex,
                PlayerCharacters[myIndex].transform.position,
                PlayerCharacters[myIndex].transform.rotation
            ));
    }

    // 플레이어 캐릭터 생성 및 배치
    private void SpawnPlayer()
    {
        List<int> characterNumber = NetworkManager.Instance.currentChannelInfo.userCharacter;
        myIndex = NetworkManager.Instance.currentChannelInfo.myIndex;

        // 다른 플레이어들의 필요 없는 컴포넌트 해제
        for (int i = 0; i < NetworkManager.Instance.currentChannelInfo.channelUserList.Count; i++)
        {
            PlayerCharacters.Add(Instantiate(PlayerPrefabs[characterNumber[i]]));
            PlayerCharacters[i].transform.position = SpawnPoints[i].position;
            // 내가 아닐 경우
            if (i != myIndex)
            {
                PlayerCharacters[i].tag = "Untagged";
                PlayerCharacters[i].GetComponent<PlayerScript>().enabled = false;
                PlayerCharacters[i].GetComponent<LapController>().enabled = false;
                PlayerCharacters[i].GetComponent<AudioSource>().enabled = false;
                PlayerCharacters[i].transform.GetChild(2).SetActive(false);
                PlayerCharacters[i].transform.GetChild(7).SetActive(false);
                PlayerCharacters[i].transform.GetChild(8).SetActive(false);
                PlayerCharacters[i].transform.GetChild(9).SetActive(false);
            }
        }

        // 초기화
        FinishLineController finishLineController = GameObject.Find("Finish Line").GetComponent<FinishLineController>();
        finishLineController.controller = PlayerCharacters[myIndex].GetComponent<LapController>();

        for (int i = 0; i < _checkPointsControllers.Length; i++)
        {
            _checkPointsControllers[i].lapController = PlayerCharacters[myIndex].GetComponent<LapController>();
        }

        for (int i = 0; i < _boostpads.Length; i++)
        {
            _boostpads[i].script = PlayerCharacters[myIndex].GetComponent<PlayerScript>();
        }
    }

    // 다른 플레이어 좌표 업데이트
    public void SetOtherPlayerPosition(UDPMessageForm messageForm)
    {
        float interpolationFactor = 0.1f; // 조절 가능

        Vector3 receivedPosition = new Vector3(messageForm.x_, messageForm.y_, messageForm.z_);
        Quaternion receivecRotation =
            new Quaternion(messageForm.rx_, messageForm.ry_, messageForm.rz_, messageForm.rw_);

        PlayerCharacters[messageForm.user_index_].transform.position = Vector3.Lerp(
            PlayerCharacters[messageForm.user_index_].transform.position,
            receivedPosition,
            interpolationFactor);
        PlayerCharacters[messageForm.user_index_].transform.rotation = Quaternion.Lerp(
            PlayerCharacters[messageForm.user_index_].transform.rotation,
            receivecRotation,
            interpolationFactor);
    }
}