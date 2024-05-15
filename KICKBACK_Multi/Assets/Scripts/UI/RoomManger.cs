using PG;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomManger : MonoBehaviour
{
    public GameObject[] rooms;
    public Image[] checkMarks;
    public GameObject tutorialPopUp;

    private bool isCebu;
    private bool isMexico;
    private bool isDownhill;

    void Start()
    {
        isCebu = false;
        isMexico = false;
        isDownhill = false;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            QuitTutorial();
        }
    }

    public void ClickCebu()
    {
        checkMarks[0].SetActive(true);
        isCebu = true;

        isMexico = false;
        isDownhill = false;

        checkMarks[1].SetActive(false);
        checkMarks[2].SetActive(false);
    }

    public void ClickMexico()
    {
        checkMarks[1].SetActive(true);
        isMexico = true;

        isCebu = false;
        isDownhill = false;

        checkMarks[0].SetActive(false);
        checkMarks[2].SetActive(false);
    }
    
    public void ClickDownhill()
    {
        checkMarks[2].SetActive(true);
        isDownhill = true;

        isCebu = false;
        isMexico = false;

        checkMarks[0].SetActive(false);
        checkMarks[1].SetActive(false);
    }

    public void GameStart()
    {
        if (isCebu)
        {
            SceneManager.LoadScene("Cebu Track");
        }
        else if (isMexico)
        {
            SceneManager.LoadScene("Mexico Track");
        }
        else if (isDownhill)
        {
            SceneManager.LoadScene("Downhill Track");
        }
    }

    public void goToLogin()
    {
        SceneManager.LoadScene("Login");
        DataManager.Instance.dataClear();
    }

    public void Tutorial()
    {
        tutorialPopUp.SetActive(true);
    }

    public void QuitTutorial()
    {
        tutorialPopUp.SetActive(false);
    }

    public void QuitBtnClicked()
    {
        Application.Quit();
    }
}
