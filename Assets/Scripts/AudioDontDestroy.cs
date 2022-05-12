using UnityEngine;
using TMPro;

public class AudioDontDestroy : MonoBehaviour
{
    public void ToggleMute() {
        gameObject.GetComponent<AudioSource>().mute = !gameObject.GetComponent<AudioSource>().mute;
        GameObject.Find("MuteText").GetComponent<TextMeshProUGUI>().text = gameObject.GetComponent<AudioSource>().mute ? "Unmute sound" : "Mute sound";
        PlayerPrefs.SetInt("mute", gameObject.GetComponent<AudioSource>().mute ? 1 : 0);
    }

    private void Awake() {
        if (PlayerPrefs.HasKey("mute")) {
            if (PlayerPrefs.GetInt("mute") == 1) {
                ToggleMute();
            }
        } else {
            PlayerPrefs.SetInt("mute", 0);
        };
        DontDestroyOnLoad(gameObject);
    }
}
