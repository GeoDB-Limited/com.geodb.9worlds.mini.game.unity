using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IconState
{
    Tie = -1,
    Win = 0,
    Lose = 1
}

public class WinIndicatorController : MonoBehaviour
{
    public IconState iconState = IconState.Tie;
    public Sprite win;
    public Sprite lose;
    public Sprite tie;

    void Update()
    {
        switch (iconState) {
            case IconState.Tie:
                transform.localScale = new Vector3(0.1f, 0.1f, 1f);
                GetComponent<SpriteRenderer>().sprite = tie;
                break;
            case IconState.Win:
                transform.localScale = new Vector3(0.3f, 0.3f, 1f);
                GetComponent<SpriteRenderer>().sprite = win;
                break;
            case IconState.Lose:
                transform.localScale = new Vector3(0.4f, 0.4f, 1f);
                GetComponent<SpriteRenderer>().sprite = lose;
                break;
        }
    }
}
