using System.Collections;
using System.Collections.Generic;
using Highlands.Server;
using PG;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoginController : MonoBehaviour
{
    [SerializeField] private Button signupPopupButton;
    [SerializeField] private Button signupConfirmButton;
    [SerializeField] private Button signupExitButton;
    [SerializeField] private Button loginConfirmButton;
    [SerializeField] private Button quitButton;
    
    [SerializeField] private TMP_InputField loginEmailInput;
    [SerializeField] private TMP_InputField loginPasswordInput;
    [SerializeField] private TMP_InputField signupNicknameInput;
    [SerializeField] private TMP_InputField signupEmailInput;
    [SerializeField] private TMP_InputField signupPasswordInput;
    [SerializeField] private TMP_InputField signupPasswordConfirmInput;
    
    [SerializeField] private TMP_Text SignupFeedbackText;
    
    [SerializeField] private GameObject signupPopup;
    [SerializeField] private GameObject loginErrorPopup;
    
    // Start is called before the first frame update
    void Start()
    {
        // 이벤트 붙이기
        signupPopupButton.onClick.AddListener(SignupPopupButtonClicked);
        signupExitButton.onClick.AddListener(SignupExitButtonClicked);
        signupConfirmButton.onClick.AddListener(SignupConfiemButton);
    }

    #region UI이벤트

    // signup ***************************************************************************
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
    private void SignupConfiemButton()
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
            StartCoroutine(ShowSignupFeedback("모든 필드를 입력해주세요."));
        }
        else if (password != passwordConfirm)
        {
            StartCoroutine(ShowSignupFeedback("비밀번호가 일치하지 않습니다."));
            return;
        }
        else
        {
            SignupFeedbackText.text = "";
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
                StartCoroutine(ShowSignupFeedback("회원 가입이 완료되었습니다."));
            }
            else
            {
                if (result == 409)
                {
                    StartCoroutine(ShowSignupFeedback("중복된 아이디입니다."));
                }
                else
                {
                    StartCoroutine(ShowSignupFeedback("서버에 요청할 수 없습니다."));
                }
            }
        });
    }

    // 회원가입 메시지 표시
    private IEnumerator ShowSignupFeedback(string message)
    {
        SignupFeedbackText.text = message;
        SignupFeedbackText.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        SignupFeedbackText.SetActive(false);
    }
    
    // login ***************************************************************************

    #endregion

}
