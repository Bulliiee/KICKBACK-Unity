using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundController : MonoBehaviour
{
    [SerializeField]
    private Image Background;
    [SerializeField]
    private Sprite[] BackgroundSprites;
    private int RandomIndex;

    void Start()
    {
        RandomIndex = Random.Range(0, BackgroundSprites.Length);

        // BackgroundImages 배열에 요소가 있는지 확인
        if (Background != null && BackgroundSprites != null && BackgroundSprites.Length > 0)
        {
            Background.sprite = BackgroundSprites[RandomIndex];
        }
    }
}
