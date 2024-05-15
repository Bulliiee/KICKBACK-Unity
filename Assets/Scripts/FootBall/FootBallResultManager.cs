using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class FootBallResultManager : MonoBehaviour
{
    [SerializeField] private TMP_Text ResultScore;

    private IEnumerator ScoreUpdate()
    {
        string url = "https://k10c209.p.ssafy.io/api/v1"; // 요청 URL
        string requestUrl = url + "/record/updateSpeedRecord2";

        string jsonRequestBody = "";

        if (SceneManager.GetActiveScene().name == "Cebu Track")
        {
            jsonRequestBody = "{" +
                $"\"mapName\":\"Cebu\"," +
                $"\"time\":\"{ResultScore.text}\"" +
                "}";
        }
        else if (SceneManager.GetActiveScene().name == "Mexico Track")
        {
            jsonRequestBody = "{" +
                $"\"mapName\":\"Mexico\"," +
                $"\"time\":\"{ResultScore.text}\"" +
                "}";
        }
        else if (SceneManager.GetActiveScene().name == "Downhill Track")
        {
            jsonRequestBody = "{" +
                $"\"mapName\":\"Downhill\"," +
                $"\"time\":\"{ResultScore.text}\"" +
                "}";
        }

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonRequestBody);

        // 요청 생성
        using (UnityWebRequest request = new UnityWebRequest(requestUrl, "PUT"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", DataManager.Instance.accessToken);
            yield return request.SendWebRequest();

            // 요청 성공 시
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("랩타임 갱신 성공");
            }
            // 요청 실패 시
            else
            {
                Debug.Log("랩타임 갱신 실패");
                yield break;
            }
        }
    }

    public void GotoRoom()
    {
        SceneManager.LoadScene("Room");
        StartCoroutine(ScoreUpdate());
    }
}
