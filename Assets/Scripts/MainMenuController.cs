using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using TMPro;
using Newtonsoft.Json;

public class MainMenuController : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void Web3Connect();
    [DllImport("__Internal")]
    private static extern string ConnectAccount();
    [DllImport("__Internal")]
    private static extern void SetConnectAccount(string value);

    private const string CONNECTING_TEXT = "Connecting...";
    private const string GETTING_MATCH_INFO_TEXT = "Getting\nmatch\ninfo...";
    private const string CREATING_MATCH_TEXT = "Creating\nmatch...";
    private const string INITIALIZING_MATCH_TEXT = "Initializing\nmatch...";
    private const string WAITING_RANDOM_SEED_TEXT = "Waiting for\nrandom seed...";
    private const string BOARD_GAME_SCENE_NAME = "BoardGame";

    public GameObject connectButton;
    public GameObject playButton;
    public GameObject optionsButton;
    public GameObject infoText;
    public string account;
    public GameObject cardSelector;
    public CardSelectorController selectorController;

    private EVMService evmService;

    public void Connect()
    {
        evmService = new EVMService();
        connectButton.SetActive(false);
        playButton.SetActive(false);
        optionsButton.SetActive(false);
        infoText.SetActive(true);
        infoText.GetComponent<TextMeshProUGUI>().text = CONNECTING_TEXT;
        StartCoroutine(ConnectToMetamask());
    }

    public void Play()
    {
        connectButton.SetActive(false);
        playButton.SetActive(false);
        optionsButton.SetActive(false);
        infoText.SetActive(true);
        infoText.GetComponent<TextMeshProUGUI>().text = GETTING_MATCH_INFO_TEXT;
        CreatingMatch();
    }

    private IEnumerator ConnectToMetamask()
    {
        Web3Connect();
        account = ConnectAccount();
        while (account == "")
        {
            yield return new WaitForSeconds(1f);
            account = ConnectAccount();
        };
        PlayerPrefs.SetString("Account", account);
        SetConnectAccount("");
        connectButton.SetActive(false);
        playButton.SetActive(true);
        optionsButton.SetActive(true);
        infoText.SetActive(false);
    }

    private async void CreatingMatch()
    {
        string txStatus = "";
        string balanceOf = await evmService.BalanceOf(account);
        selectorController.SetMaxAmount(int.Parse(balanceOf));
        string userLastMatchId = await evmService.GetUserLastMatch(account);
        Debug.Log("UserLastMatch: " + userLastMatchId);
        if (userLastMatchId == "5")
        {
            cardSelector.SetActive(true);
            infoText.SetActive(false);
        }
        else
        {
            try
            {
                string response = await evmService.GetValidPlayerNft(userLastMatchId, "0");
            }
            catch (Exception e)
            {
                txStatus = "";
                infoText.GetComponent<TextMeshProUGUI>().text = INITIALIZING_MATCH_TEXT;
                string initMatchTxHash = await evmService.InitializeMatch(account);
                while (txStatus == "" || txStatus == "pending")
                {
                    txStatus = await evmService.TxStatus(initMatchTxHash);
                    Debug.Log("Status:" + txStatus);
                }
            }
            SceneManager.LoadScene(BOARD_GAME_SCENE_NAME);
            PlayerPrefs.SetString("MatchId", userLastMatchId);
        }
    }

    public async void ContinueCreatingMatch()
    {
        cardSelector.SetActive(false);
        infoText.SetActive(true);
        string txStatus = "";
        infoText.GetComponent<TextMeshProUGUI>().text = CREATING_MATCH_TEXT;
        string createMatchTxHash = await evmService.CreateMatch(selectorController.amountSelected);
        while (txStatus == "" || txStatus == "pending")
        {
            txStatus = await evmService.TxStatus(createMatchTxHash);
        }
        infoText.GetComponent<TextMeshProUGUI>().text = WAITING_RANDOM_SEED_TEXT;
        string userLastMatchId = await evmService.GetUserLastMatch(account);
        string randomSeed = "0";
        int count = 0;
        while (randomSeed == "0")
        {
            await new WaitForSeconds(1f);
            Debug.Log(count);
            count++;
            string matchInfoResponse = await evmService.GetMatchInfoById(userLastMatchId);
            MatchInfo matchInfo = JsonConvert.DeserializeObject<MatchInfo>(matchInfoResponse);
            randomSeed = matchInfo.matchRandomSeed;
        }
        infoText.GetComponent<TextMeshProUGUI>().text = INITIALIZING_MATCH_TEXT;
        txStatus = "";
        string initMatchTxHash = await evmService.InitializeMatch(account);
        while (txStatus == "" || txStatus == "pending")
        {
            txStatus = await evmService.TxStatus(initMatchTxHash);
        }
        SceneManager.LoadScene(BOARD_GAME_SCENE_NAME);
        PlayerPrefs.SetString("MatchId", userLastMatchId);
    }
}
