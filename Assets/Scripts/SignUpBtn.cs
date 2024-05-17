using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignUpBtn : MonoBehaviour
{
    [SerializeField]
    private GameObject SignUpPanel;

    void Start()
    {
        SignUpPanel.SetActive(false);
    }

    public void SignUp()
    {
        if (!SignUpPanel.activeSelf)
        {
            SignUpPanel.SetActive(true);
        }
        else if (SignUpPanel.activeSelf)
        {
            SignUpPanel.SetActive(false);
        }
    }
}
