using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.WebRequestMethods;

public class LoginManager : MonoBehaviour
{
    [Header ("Commons")]
    [SerializeField] private GameObject SignUpPopUp; // 회원 가입 팝업 오브젝트
    [SerializeField] private GameObject LoginCanvas; // 로그인 캔버스
    [SerializeField] private GameObject LobbyCanvas; // 로비 캔버스

    [Header ("Login")]
    [SerializeField] private TMP_InputField LoginEmail; // 로그인 이메일 인풋필드
    [SerializeField] private TMP_InputField LoginPassword; // 패스워드 인풋 필드
    [SerializeField] private Button LoginSignUpBtn; // 회원 가입 팝업 띄울 버튼
    [SerializeField] private Button LoginBtn; // 로그인 버튼
    [SerializeField] private Button LoginExit; // 종료 버튼
    [SerializeField] private GameObject ErrorPopup; // 로그인 실패 시 팝업

    [Header ("SignUp")]
    [SerializeField] private TMP_InputField SignUpEmail; // 회원가입 이메일 인풋필드
    [SerializeField] private TMP_InputField SignUpName; // 회원가입 이름 인풋필드
    [SerializeField] private TMP_InputField SignUpPassword; // 회원가입 비밀번호 인풋필드
    [SerializeField] private TMP_InputField SignUpPasswordCheck; // 회원가입 비밀번호 재입력 인풋필드
    [SerializeField] private TMP_Text SignUpFeedback; // 회원가입 성공 / 실패 여부 피드백 텍스트
    [SerializeField] private Button SignUpConfirmBtn; // 회원 가입 하기 버튼
    [SerializeField] private Button SignUpCancelBtn; // 회원가입 종료 버튼

    private string url = "https://k10c209.p.ssafy.io/api/v1"; // 요청 URL

    // 만약 로그인 한 상태에서 로그인 매니저 켜질 시 로그인 필요 없으니 로비로 바로 이동
    private void OnEnable()
    {
        if (DataManager.Instance.loginUserInfo != null && DataManager.Instance.loginUserInfo.UserId != 0)
        {
            goToLobby();
        }
    }

    #region 회원 가입

    // 회원 가입으로 가기
    public void GoSignUp()
    {
        closeErrPopUp();
        SignUpPopUp.SetActive(true);
        SignUpConfirmBtn.interactable = true;
    }

    // 회원 가입 버튼 클릭 시
    public void SignUpConfirm()
    {
        string email = SignUpEmail.text;
        string nickname = SignUpName.text;
        string password = SignUpPassword.text;
        string passwordCheck = SignUpPasswordCheck.text;

        closeErrPopUp();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(nickname) ||
            string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordCheck))
        {
            SignUpFeedback.text = "모든 필드를 입력해주세요.";
        }
        else if (password != passwordCheck)
        {
            SignUpFeedback.text = "비밀번호가 일치하지 않습니다.";
            return;
        }
        else
        {
            SignUpFeedback.text = "";
        }

        SignUpConfirmBtn.interactable = false;
        // 회원 가입 요청 보내기
        StartCoroutine(SignUpRequest(email, password, nickname));
        StartCoroutine(SignUpConfirmCancel());
    }

    // 회원 가입 취소 시
    public void SignUpCancel()
    {
        // 회원 가입 취소 시 입력 필드 및 피드백 텍스트 초기화
        // 회원가입 취소 시 입력 필드 및 피드백 텍스트 초기화
        SignUpEmail.text = "";
        SignUpName.text = "";
        SignUpPassword.text = "";
        SignUpPasswordCheck.text = "";
        SignUpFeedback.text = "";

        closeErrPopUp();

        // 로그인 페이지로 이동
        SignUpPopUp.SetActive(false);
    }

    public IEnumerator SignUpConfirmCancel()
    {
        yield return new WaitForSeconds(1.0f);

        // 회원 가입 취소 시 입력 필드 및 피드백 텍스트 초기화
        // 회원가입 취소 시 입력 필드 및 피드백 텍스트 초기화
        SignUpEmail.text = "";
        SignUpName.text = "";
        SignUpPassword.text = "";
        SignUpPasswordCheck.text = "";
        SignUpFeedback.text = "";

        closeErrPopUp();

        // 로그인 페이지로 이동
        SignUpPopUp.SetActive(false);
    }

    // 회원 가입 요청 보내기
    private IEnumerator SignUpRequest(string email, string password, string nickname)
    {
        closeErrPopUp();

        string requestUrl = url + "/member/signup";
        User user = new User
        {
            Email = email,
            Password = password,
            NickName = nickname
        };

        string jsonRequestBody = JsonUtility.ToJson(user);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonRequestBody);

        // 요청 생성
        using (UnityWebRequest request = new UnityWebRequest(requestUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            // 요청 성공 시
            if (request.result == UnityWebRequest.Result.Success)
            {
                SignUpFeedback.text = "회원 가입이 완료되었습니다.";
            }
            // 요청 실패 시
            else
            {
                Debug.LogError("Error : " + request.error);
                // 아이디 중복
                if (request.responseCode == 409)
                {
                    SignUpFeedback.text = "중복된 아이디입니다.";
                }
                // 그 외
                else
                {
                    SignUpFeedback.text = "서버에 연결할 수 없습니다.";
                }

                SignUpConfirmBtn.interactable = true;
                yield break;
            }
        }
    }

    #endregion

    #region 로그인

    // 로그인 버튼 클릭 시
    public void LoginConfirm()
    {
        string email = LoginEmail.text;
        string password = LoginPassword.text;

        // 입력 확인
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            return;
        }

        //LoginBtn.interactable = true;
        StartCoroutine(LoginRequest(email, password));
    }


    // 로그인 요청 보내기
    IEnumerator LoginRequest(string email, string password)
    {
        string requestUrl = url + "/member/login";

        User user = new User
        {
            Email = email,
            Password = password
        };

        string json = JsonUtility.ToJson(user);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        // 요청 생성
        using (UnityWebRequest request = new UnityWebRequest(requestUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");


            yield return request.SendWebRequest();

            // 요청 성공 시
            if (request.result == UnityWebRequest.Result.Success)
            {
                closeErrPopUp();

                DataManager.Instance.accessToken = request.GetResponseHeader("accessToken");
                //DataManager.Instance.refreshToken = request.GetResponseHeader("refreshToken");

                // 로그인 한 유저 정보 불러오기
                using (UnityWebRequest userProfileRequest = UnityWebRequest.Get(url + "/member/get"))
                {
                    userProfileRequest.SetRequestHeader("Authorization", DataManager.Instance.accessToken);

                    yield return userProfileRequest.SendWebRequest();

                    // 정보 요청 성공 시
                    if (userProfileRequest.result == UnityWebRequest.Result.Success)
                    {
                        DataManager.Instance.loginUserInfo = JsonUtility.FromJson<User>(userProfileRequest.downloadHandler.text);
                    }
                    // 정보 요청 성공 시
                    else
                    {
                        Debug.Log(userProfileRequest.error);

                    }
                }

                goToLobby();
            }
            // 요청 실패 시
            else
            {
                Debug.LogError("Error : " + request.error);
                ErrorPopup.SetActive(true);

                // 아이디 중복
                if (request.responseCode == 400)
                {
                    Debug.Log("로그인 실패");
                }
                // 그 외
                else
                {
                    Debug.Log("서버에 연결할 수 없습니다.");
                }

                LoginBtn.interactable = true;
                yield break;
            }
        }
    }
    #endregion

    #region 기타 기능
    // 로그인 실패 팝업 닫기
    public void closeErrPopUp()
    {
        ErrorPopup.SetActive(false);
    }

    // 로그인 끄고 로비 켜기
    public void goToLobby()
    {
        closeErrPopUp();
        // SceneManager.LoadScene("Room");
        SceneManager.LoadScene("Lobby");
        LoginCanvas.SetActive(false);
        LobbyCanvas.SetActive(true);
    }

    public void QuitBtnClicked()
    {
        Application.Quit();
    }
    #endregion
}
