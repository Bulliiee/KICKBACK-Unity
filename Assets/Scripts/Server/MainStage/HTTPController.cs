using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Networking;

public class HTTPController
{
    private string url = "https://k10c209.p.ssafy.io/api/v1";
    private string accessToken = "";

    // 요청 보내기
    public IEnumerator SendPostRequest<T>(T t, string requestUrl, Action<long> resultCallback)
    {
        // 요청 body 생성
        string jsonRequestBody = JsonUtility.ToJson(t);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonRequestBody);
        
        // 요청 생성
        using (UnityWebRequest request = new UnityWebRequest(url + requestUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", accessToken);

            yield return request.SendWebRequest();

            // 요청 성공 시
            if (request.result == UnityWebRequest.Result.Success)
            {
                if (accessToken == "")
                {
                    accessToken = request.GetResponseHeader("accessToken");
                }
                resultCallback?.Invoke(200);
            }
            // 요청 실패 시 에러 코드
            else
            {
                resultCallback?.Invoke(request.responseCode);
            }
        }
        
    }
    
    // 결과 반환 위해 Action<string> 델리게이트 사용
    // result는 JSON형태의 문자열 혹은 문자열 형태의 숫자(에러코드)
    public IEnumerator SendGetRequest(string requestData, string requestUrl, Action<string> resultCallback)
    {
        // 요청 생성
        using (UnityWebRequest request = UnityWebRequest.Get(url + requestUrl + requestData))
        {
            request.SetRequestHeader("Authorization", accessToken);

            yield return request.SendWebRequest();
            
            // 요청 성공 시
            if (request.result == UnityWebRequest.Result.Success)
            {
                resultCallback?.Invoke(request.downloadHandler.text);
            }
            // 요청 실패 시 에러 코드
            else
            {
                resultCallback?.Invoke(request.responseCode.ToString());
            }
        }
    }
}
