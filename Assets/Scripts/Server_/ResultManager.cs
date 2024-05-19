using System.Collections;
using System.Collections.Generic;
using Highlands.Server;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    [SerializeField] private TMP_Text LapTime;
    
    private IEnumerator LapTimeUpdate()
    {
        string url = "https://k10c209.p.ssafy.io/api/v1"; // 요청 URL
        string requestUrl = url + "/record/updateSpeedRecord2";

        string jsonRequestBody = "";

        if (SceneManager.GetActiveScene().name == "Cebu Track")
        {
            jsonRequestBody = "{" +
                $"\"mapName\":\"Cebu\"," +
                $"\"time\":\"{LapTime.text}\"" + 
                "}";
        }
        else if (SceneManager.GetActiveScene().name == "Mexico Track")
        {
            jsonRequestBody = "{" +
                $"\"mapName\":\"Mexico\"," +
                $"\"time\":\"{LapTime.text}\"" +
                "}";
        }
        else if (SceneManager.GetActiveScene().name == "Downhill Track")
        {
            jsonRequestBody = "{" +
                $"\"mapName\":\"Downhill\"," +
                $"\"time\":\"{LapTime.text}\"" +
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
        // SceneManager.LoadScene("Room");
        NetworkManager.Instance.SendBusinessMessage(MessageHandler.PackEndMessage(Command.END, NetworkManager.Instance.currentChannelInfo.channelIndex));
        GameManager.Instance.ChangeMainStageCanvas("lobby");
        StartCoroutine(LapTimeUpdate());
    }
}
