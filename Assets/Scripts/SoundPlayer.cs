using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public void play()
    {
        GetComponent<AudioSource>().Play(0);
    }
}
