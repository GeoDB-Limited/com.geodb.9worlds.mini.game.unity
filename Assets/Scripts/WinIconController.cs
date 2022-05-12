using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinIconController : MonoBehaviour
{
    public IconState iconState = IconState.Tie;
    public Sprite win;
    public Sprite lose;

    void Update()
    {
        switch (iconState) {
            case IconState.Tie:
                GetComponent<Image>().enabled = false;
                break;
            case IconState.Win:
                GetComponent<Image>().enabled = true;
                GetComponent<Image>().sprite = win;
                break;
            case IconState.Lose:
                GetComponent<Image>().enabled = true;
                GetComponent<Image>().sprite = lose;
                break;
        }
    }
}
