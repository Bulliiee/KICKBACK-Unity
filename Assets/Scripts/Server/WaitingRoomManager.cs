using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WaitingRoomManager : MonoBehaviour
{
    [Header("Map")]
    public List<Sprite> sprites = new List<Sprite>();
    public TMP_Dropdown dropdown;
    public Image mapImage;

    [Header("User")]
    public List<TMP_Text> nicknames;
    public GameObject selectCharacters;
    public List<Button> characterButtons;
    public List<Image> checkMarks;
    public List<Image> characters;
    public List<Sprite> characterImages;
    public List<Image> isReady;
    private int selectedCharacterIndex = 0;

    [Header("Script")]
    [SerializeField] private BusinessManager businessManager;
    [SerializeField] private DataManager dataManager;
    [SerializeField] private GameObject LobbyCanvas;
    private User loginUserInfo;

    void Awake()
    {
        businessManager = FindObjectOfType<BusinessManager>();
        dataManager = FindObjectOfType<DataManager>();

        LobbyCanvas = GameObject.Find("Lobby Canvas");
    }

    private void OnEnable()
    {
        LobbyCanvas.SetActive(false);
    }

    void Start()
    {

        // ��Ӵٿ��� ���� ��ȭ�� ���� ������ �߰�
        dropdown.onValueChanged.AddListener(delegate 
        {
            ChangeImage(dropdown.value);
        });
        selectCharacters.SetActive(false);
    }

    void Update()
    {
        nickNameUpdate();
        ImageCall();
    }

    void ChangeImage(int index)
    {
        // ���õ� �ε����� �ش��ϴ� ��������Ʈ�� �̹��� ����
        mapImage.sprite = sprites[index];
    }

    public void LeaveRoom()
    {
        businessManager.jlrRoom(Highlands.Server.Command.LEAVE, dataManager.channelIndex);

        LobbyCanvas.SetActive(true);

        RoomClear();

        SceneManager.LoadScene("Lobby");
    }

    public void RoomClear()
    {
        dataManager.channelIndex = -1;
        dataManager.channelName = "";
        dataManager.roomUserList = null;
        dataManager.cnt = 0;
    }

    private void nickNameUpdate()
    {
        if (dataManager.roomUserList != null)
        {
            for (int i = 0; i < dataManager.roomUserList.Count; i++)
            {
                nicknames[i].text = dataManager.roomUserList[i];
            }

            for (int i = 0; i < nicknames.Count; i++)
            {
                if (i > dataManager.roomUserList.Count - 1)
                {
                    nicknames[i].text = "";
                }
            }
        }
    }

    public void PopUp()
    {
        if (!selectCharacters.activeSelf)
        {
            selectCharacters.SetActive(true);
        }
        else if (selectCharacters.activeSelf)
        {
            selectCharacters.SetActive(false);

            for (int i = 0; i < checkMarks.Count; i++)
            {
                checkMarks[i].gameObject.SetActive(false);
            }
        }
    }

    public void ImageCall()
    {
        if (dataManager.roomUserList != null)
        {
            for (int i = 0; i < dataManager.roomUserList.Count; i++)
            {
                characters[i].gameObject.SetActive(true);
            }
        }
    }

    // ��ư Ŭ�� �� ȣ��� �޼ҵ�. �ε����� �Ű������� ����
    public void ClickCharacter(int buttonIndex)
    {
        // ��� üũ��ũ�� ���� ��Ȱ��ȭ
        for (int i = 0; i < checkMarks.Count; i++)
        {
            checkMarks[i].gameObject.SetActive(false);
        }

        // Ŭ���� ��ư�� �ش��ϴ� üũ��ũ�� Ȱ��ȭ
        checkMarks[buttonIndex].gameObject.SetActive(true);
    }

    public void SelectCharacter(int buttonIndex)
    {
        // ������ ĳ���� �ε����� ������Ʈ
        selectedCharacterIndex = buttonIndex;
    }

    // ������ ��ư�� ������ �Լ�
    public void ConfirmCharacterSelection()
    {
        // ����ڰ� ������ ĳ���ͷ� ���� �̹����� ����
        characters[dataManager.myIndex].sprite = characterImages[selectedCharacterIndex];
        characters[dataManager.myIndex].SetNativeSize();
    }

    public void MyIndexUpdate()
    {
        for (int i = 0; i < dataManager.roomUserList.Count; i++)
        {
            if (dataManager.roomUserList[i] == loginUserInfo.dataBody.nickname)
            {
                dataManager.myIndex = i;
            }
        }
    }

    public void Ready()
    {
        if (!isReady[dataManager.myIndex].gameObject.activeSelf)
        {
            isReady[dataManager.myIndex].gameObject.SetActive(true);
            dataManager.isReady = true;
        }
        else
        {
            isReady[dataManager.myIndex].gameObject.SetActive(false);
            dataManager.isReady = false;
        }
    }
}
