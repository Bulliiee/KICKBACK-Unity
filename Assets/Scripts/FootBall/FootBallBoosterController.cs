using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FootBallBoosterController : MonoBehaviour
{
    [SerializeField] private FootBallPlayerScript playerScript;
    [SerializeField] private Image BoosterImage;
    [SerializeField] private TMP_Text BoosterCount;

    // Start is called before the first frame update
    void Start()
    {
        if (BoosterImage != null) BoosterImage.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (BoosterCount != null && playerScript != null)
        {
            BoosterCount.SetText(playerScript.currentBoost.ToString());
        }

        ImageControl();
    }

    private void ImageControl()
    {
        if (playerScript.currentBoost == 0)
        {
            BoosterImage.gameObject.SetActive(false);
        }
        else
        {
            BoosterImage.gameObject.SetActive(true);
        }
    }
}
