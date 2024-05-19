// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using UnityEngine.SceneManagement;
//
// public class WaitingRoomManager : MonoBehaviour
// {
//     [Header("Map")]
//     public List<Sprite> sprites = new List<Sprite>();
//     public TMP_Dropdown dropdown;
//     public Image mapImage;
//
//     [Header("User")]
//     public List<TMP_Text> nicknames;
//     public GameObject selectCharacters;
//     public List<Button> characterButtons;
//     public List<Image> checkMarks;
//     public List<Image> characters;
//     public List<Sprite> characterImages;
//     public List<Image> isReady;
//     private int selectedCharacterIndex = 0;
//
//     [Header("Script")]
//     [SerializeField] private BusinessManager businessManager;
//     [SerializeField] private DataManager dataManager;
//     [SerializeField] private GameObject LobbyCanvas;
//     private User loginUserInfo;
//
//     void Awake()
//     {
//         businessManager = FindObjectOfType<BusinessManager>();
//         dataManager = FindObjectOfType<DataManager>();
//
//         LobbyCanvas = GameObject.Find("Lobby Canvas");
//     }
//
//     private void OnEnable()
//     {
//         LobbyCanvas.SetActive(false);
//     }
//
//     void Start()
//     {
//
//         // 드롭다운의 선택 변화에 대한 리스너 추가
//         dropdown.onValueChanged.AddListener(delegate 
//         {
//             ChangeImage(dropdown.value);
//         });
//         selectCharacters.SetActive(false);
//     }
//
//     void Update()
//     {
//         nickNameUpdate();
//         ImageCall();
//     }
//
//     void ChangeImage(int index)
//     {
//         // 선택된 인덱스에 해당하는 스프라이트로 이미지 변경
//         mapImage.sprite = sprites[index];
//     }
//
//     public void LeaveRoom()
//     {
//         businessManager.jlrRoom(Highlands.Server.Command.LEAVE, dataManager.channelIndex);
//
//         LobbyCanvas.SetActive(true);
//
//         RoomClear();
//
//         SceneManager.LoadScene("Lobby");
//     }
//
//     public void RoomClear()
//     {
//         dataManager.channelIndex = -1;
//         dataManager.channelName = "";
//         dataManager.roomUserList = null;
//         dataManager.cnt = 0;
//     }
//
//     private void nickNameUpdate()
//     {
//         if (dataManager.roomUserList != null)
//         {
//             for (int i = 0; i < dataManager.roomUserList.Count; i++)
//             {
//                 nicknames[i].text = dataManager.roomUserList[i];
//             }
//
//             for (int i = 0; i < nicknames.Count; i++)
//             {
//                 if (i > dataManager.roomUserList.Count - 1)
//                 {
//                     nicknames[i].text = "";
//                 }
//             }
//         }
//     }
//
//     public void PopUp()
//     {
//         if (!selectCharacters.activeSelf)
//         {
//             selectCharacters.SetActive(true);
//         }
//         else if (selectCharacters.activeSelf)
//         {
//             selectCharacters.SetActive(false);
//
//             for (int i = 0; i < checkMarks.Count; i++)
//             {
//                 checkMarks[i].gameObject.SetActive(false);
//             }
//         }
//     }
//
//     public void ImageCall()
//     {
//         if (dataManager.roomUserList != null)
//         {
//             for (int i = 0; i < dataManager.roomUserList.Count; i++)
//             {
//                 characters[i].gameObject.SetActive(true);
//             }
//         }
//     }
//
//     // 버튼 클릭 시 호출될 메소드. 인덱스를 매개변수로 받음
//     public void ClickCharacter(int buttonIndex)
//     {
//         // 모든 체크마크를 먼저 비활성화
//         for (int i = 0; i < checkMarks.Count; i++)
//         {
//             checkMarks[i].gameObject.SetActive(false);
//         }
//
//         // 클릭된 버튼에 해당하는 체크마크만 활성화
//         checkMarks[buttonIndex].gameObject.SetActive(true);
//     }
//
//     public void SelectCharacter(int buttonIndex)
//     {
//         // 선택한 캐릭터 인덱스를 업데이트
//         selectedCharacterIndex = buttonIndex;
//     }
//
//     // 오케이 버튼에 연결할 함수
//     public void ConfirmCharacterSelection()
//     {
//         // 사용자가 선택한 캐릭터로 실제 이미지를 변경
//         characters[dataManager.myIndex].sprite = characterImages[selectedCharacterIndex];
//         characters[dataManager.myIndex].SetNativeSize();
//     }
//
//     public void MyIndexUpdate()
//     {
//         for (int i = 0; i < dataManager.roomUserList.Count; i++)
//         {
//             if (dataManager.roomUserList[i] == loginUserInfo.dataBody.nickname)
//             {
//                 dataManager.myIndex = i;
//             }
//         }
//     }
//
//     public void Ready()
//     {
//         if (!isReady[dataManager.myIndex].gameObject.activeSelf)
//         {
//             isReady[dataManager.myIndex].gameObject.SetActive(true);
//             dataManager.isReady = true;
//         }
//         else
//         {
//             isReady[dataManager.myIndex].gameObject.SetActive(false);
//             dataManager.isReady = false;
//         }
//     }
// }
