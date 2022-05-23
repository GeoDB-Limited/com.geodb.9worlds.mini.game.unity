using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardSelectorController : MonoBehaviour
{
    public int amountSelected = 1;
    public int maxAmount;

    public void SetMaxAmount(int max)
    {
        maxAmount = max <= 5 ? max : 5;
    }

    public void IncreaseAmount()
    {
        if (amountSelected < maxAmount)
        {
            amountSelected++;
        }
        SetCurrentAmount();
    }

    public void DecreaseAmount()
    {
        if (amountSelected > 1)
        {
            amountSelected--;
        }
        SetCurrentAmount();
    }

    public void SetCurrentAmount()
    {
        GetComponent<TMP_InputField>().text = amountSelected.ToString();
    }
}
