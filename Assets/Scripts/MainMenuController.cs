using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    private const string CONNECTING_TEXT = "Connecting...";
    private const string GETTING_MATCH_INFO_TEXT = "Getting\nmatch\ninfo...";
    private const string CREATING_MATCH_TEXT = "Creating\nmatch...";
    private const string INITIALIZING_MATCH_TEXT = "Initializing\nmatch...";
    private const string BOARD_GAME_SCENE_NAME = "BoardGame";

    public GameObject connectButton;
    public GameObject playButton;
    public GameObject optionsButton;
    public GameObject infoText;

    public void Connect() {
        connectButton.SetActive(false);
        playButton.SetActive(false);
        optionsButton.SetActive(false);
        infoText.SetActive(true);
        infoText.GetComponent<TextMeshProUGUI>().text = CONNECTING_TEXT;
        StartCoroutine(ConnectToMetamask());
    }

    public void Play() {
        connectButton.SetActive(false);
        playButton.SetActive(false);
        optionsButton.SetActive(false);
        infoText.SetActive(true);
        infoText.GetComponent<TextMeshProUGUI>().text = GETTING_MATCH_INFO_TEXT;
        StartCoroutine(CreatingMatch());
    }

    private IEnumerator ConnectToMetamask() {
        yield return new WaitForSeconds(1f);
        connectButton.SetActive(false);
        playButton.SetActive(true);
        optionsButton.SetActive(true);
        infoText.SetActive(false);
    }

    private IEnumerator CreatingMatch() {
        yield return new WaitForSeconds(1f);
        infoText.GetComponent<TextMeshProUGUI>().text = CREATING_MATCH_TEXT;
        yield return new WaitForSeconds(1f);
        infoText.GetComponent<TextMeshProUGUI>().text = INITIALIZING_MATCH_TEXT;
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(BOARD_GAME_SCENE_NAME);
    }
        
}
