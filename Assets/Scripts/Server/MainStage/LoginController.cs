using System;
using System.Collections;
using System.Collections.Generic;
using Highlands.Server;
using PG;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class LoginController : MonoBehaviour
{
    [SerializeField] private Button signupPopupButton;
    [SerializeField] private Button signupConfirmButton;
    [SerializeField] private Button signupExitButton;
    [SerializeField] private Button loginConfirmButton;
    [SerializeField] private Button loginErrorPopupCloseButton;
    [SerializeField] private Button quitButton;
    
    [SerializeField] private TMP_InputField loginEmailInput;
    [SerializeField] private TMP_InputField loginPasswordInput;
    [SerializeField] private TMP_InputField signupNicknameInput;
    [SerializeField] private TMP_InputField signupEmailInput;
    [SerializeField] private TMP_InputField signupPasswordInput;
    [SerializeField] private TMP_InputField signupPasswordConfirmInput;
    
    [SerializeField] private TMP_Text signupFeedbackText;
    
    [SerializeField] private GameObject signupPopup;
    [SerializeField] private GameObject loginErrorPopup;
    
    private TMP_InputField[] _loginInputFields;
    private TMP_InputField[] _signupInputFields;
    private Coroutine _feedbackCoroutine;
    
    // Start is called before the first frame update
    void Start()
    {
        // 인풋필드 배열 설정
        _loginInputFields = new TMP_InputField[2]
        {
            loginEmailInput,
            loginPasswordInput
        };
        _signupInputFields = new TMP_InputField[4]
        {
            signupNicknameInput,
            signupEmailInput,
            signupPasswordInput,
            signupPasswordConfirmInput
        };
        
        // 이벤트 붙이기
        signupPopupButton.onClick.AddListener(SignupPopupButtonClicked);
        signupExitButton.onClick.AddListener(SignupExitButtonClicked);
        signupConfirmButton.onClick.AddListener(SignupConfirmButtonClicked);
        loginConfirmButton.onClick.AddListener(LoginConfirmButtonClicked);
        loginErrorPopupCloseButton.onClick.AddListener(LoginErrorPopupCloseButtonClicked);
    }

    void Update()
    {
        // 탭 눌렀을 때 다음 인풋필드로 이동
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // 로그인 켜져있는 경우
            if (!signupPopup.activeSelf)
            {
                for (int i = 0; i < _loginInputFields.Length - 1; i++)
                {
                    if (_loginInputFields[i].isFocused)
                    {
                        _loginInputFields[i + 1].Select();
                    }
                }
            }
            // 회원가입 켜져있는 경우
            else
            {
                for (int i = 0; i < _signupInputFields.Length - 1; i++)
                {
                    if (_signupInputFields[i].isFocused)
                    {
                        _signupInputFields[i + 1].Select();
                    }
                }
            }
        }

        // 엔터 눌렀을 때 요청 보내기
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // 로그인 켜져 있는 경우 로그인 하기
            if (!signupPopup.activeSelf)
            {
                LoginConfirmButtonClicked();
            }
            // 회원가입 켜져 있는 경우 회원가입 하기
            else
            {
                SignupConfirmButtonClicked();
            }
        }
    }

    #region 회원가입
    
    // 회원가입 팝업 띄우기
    private void SignupPopupButtonClicked()
    {
        signupPopup.SetActive(true);
        loginErrorPopup.SetActive(false);
    }

    // 회원가입 팝업 끄기
    private void SignupExitButtonClicked()
    {
        signupPopup.SetActive(false);
        signupEmailInput.text = "";
        signupNicknameInput.text = "";
        signupPasswordInput.text = "";
        signupPasswordConfirmInput.text = "";
    }

    // 회원가입 버튼 클릭 시
    private void SignupConfirmButtonClicked()
    {
        // 입력값 가져오기
        string email = signupEmailInput.text;
        string nickname = signupNicknameInput.text;
        string password = signupPasswordInput.text;
        string passwordConfirm = signupPasswordConfirmInput.text;
        
        loginErrorPopup.SetActive(false);
        
        // 입력값 검증
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(nickname) ||
            string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordConfirm))
        {
            DisplaySignupFeedback("모든 필드를 입력해주세요.");
            return;
        }
        else if (password != passwordConfirm)
        {
            DisplaySignupFeedback("비밀번호가 일치하지 않습니다.");
            return;
        }
        else
        {
            signupFeedbackText.text = "";
        }
        
        // 요청 객체 만들기
        User user = new User
        {
            Email = signupEmailInput.text,
            Password = signupPasswordInput.text,
            NickName = signupNicknameInput.text
        };
        
        // 요청 보내기
        NetworkManager.Instance.PostRequest(user, "/member/signup", (result) =>
        {
            if (result == 200)
            {
                DisplaySignupFeedback("회원 가입이 완료되었습니다.");
            }
            else
            {
                if (result == 409)
                {
                    DisplaySignupFeedback("중복된 아이디 혹은 닉네임 입니다.");
                }
                else
                {
                    DisplaySignupFeedback("서버에 요청할 수 없습니다.");
                }
            }
        });
    }

    private void DisplaySignupFeedback(string message)
    {
        // 이전 코루틴 있으면 멈추기
        if (_feedbackCoroutine != null)
        {
            StopCoroutine(_feedbackCoroutine);
        }

        _feedbackCoroutine = StartCoroutine(ShowSignupFeedback(message));
    }
    
    // 회원가입 메시지 표시
    private IEnumerator ShowSignupFeedback(string message)
    {
        signupFeedbackText.text = message;
        signupFeedbackText.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        signupFeedbackText.SetActive(false);
    }

    #endregion

    #region 로그인

    private void LoginConfirmButtonClicked()
    {
        // 입력값 가져오기
        string email = loginEmailInput.text;
        string password = loginPasswordInput.text;
        
        // 입력 확인
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            return;
        }
        
        // 요청 객체 만들기
        User user = new User
        {
            Email = loginEmailInput.text,
            Password = loginPasswordInput.text,
        };
        
        // 요청보내기
        NetworkManager.Instance.PostRequest(user, "/member/login", (result) =>
        {
            if (result == 200)
            {
                loginErrorPopup.SetActive(false);
                
                // 유저 정보 불러오기
                NetworkManager.Instance.GetRequest<User>("", "/member/get", (userInfo) =>
                {
                    GameManager.Instance.loginUserInfo = userInfo;
                });

                GameManager.Instance.ChangeMainStageCanvas("Lobby Canvas");
                NetworkManager.Instance.currentPlayerLocation = CurrentPlayerLocation.Lobby;
            }
            else
            {
                loginErrorPopup.SetActive(true);
            }
        });
    }

    #endregion

    #region 기타

    // 로그인 에러 팝업 닫기
    private void LoginErrorPopupCloseButtonClicked()
    {
        loginErrorPopup.SetActive(false);
    }

    #endregion

}
