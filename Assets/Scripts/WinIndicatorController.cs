using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinIndicatorController : MonoBehaviour
{
    public bool isWinner = true;
    public Sprite winSprite;
    public Sprite loseSprite;

    void Update()
    {
        if (isWinner) {
            transform.localScale = new Vector3(0.3f, 0.3f, 1f);
            GetComponent<SpriteRenderer>().sprite = winSprite;
        } else {
            transform.localScale = new Vector3(0.4f, 0.4f, 1f);
            GetComponent<SpriteRenderer>().sprite = loseSprite;
        }
    }
}
