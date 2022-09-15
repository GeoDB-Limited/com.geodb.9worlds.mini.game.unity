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
    private static extern void OpenURLInExternalWindow(string url);
    [DllImport("__Internal")]
    private static extern void Web3Connect();
    [DllImport("__Internal")]
    private static extern string ConnectAccount();
    [DllImport("__Internal")]
    private static extern void SetConnectAccount(string value);
    [DllImport("__Internal")]
    private static extern void ReloadWindow();

    private const string CONNECTING_TEXT = "Connecting...";
    private const string GETTING_MATCH_INFO_TEXT = "Getting\nmatch\ninfo...";
    private const string CREATING_MATCH_TEXT = "Creating\nmatch...";
    private const string INITIALIZING_MATCH_TEXT = "Initializing\nmatch...";
    private const string WAITING_RANDOM_SEED_TEXT = "Waiting for\nrandom seed...\n\n0 secs\n\nup to\n60 seconds";
    private const string BOARD_GAME_SCENE_NAME = "BoardGame";
    private const string EXCEEDED_DAILY_MATCHES = "NineWorldsMinigame: Match nft amount exceed user valid nfts to play";
    private const string COME_BACK_TOMORROW_TEXT = "Excedeed\ndaily matches.\n\nCome back\nagain\ntomorrow";
    private const string DONT_OWN_ANY_NFT_TEXT = "You don't own\nany 9 worlds\nmidgard nfts.\n\nGo buy at\nleast one on";


    public GameObject connectButton;
    public GameObject playButton;
    public GameObject optionsButton;
    public GameObject infoText;
    public GameObject openseaButton;
    public GameObject userInfoBox;
    public string account;
    public GameObject cardSelector;
    public CardSelectorController selectorController;

    private EVMService evmService;
    private int totalGems;

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

    private async void SetUserInfoPanel()
    {
        string balance = await evmService.pointBalanceOf(account);
        int totalGems = int.Parse(balance);
        PlayerPrefs.SetInt("TotalGems", totalGems);
        PlayerPrefs.SetString("ShortAccount", account.Substring(0, 6) + "........." + account.Substring(37, 4));
        userInfoBox.SetActive(true);
        userInfoBox.transform.Find("UserAddress").GetComponent<TextMeshProUGUI>().text = account.Substring(0, 6) + "........." + account.Substring(37, 4);
        userInfoBox.transform.Find("WeakBalance").GetComponent<TextMeshProUGUI>().text = "Total O9W tickets:\n" + totalGems;
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
        SetUserInfoPanel();
    }

    private async void CreatingMatch()
    {
        string txStatus = "";
        string balanceOf = await evmService.BalanceOf(account);
        if (balanceOf == "0")
        {
            infoText.GetComponent<TextMeshProUGUI>().text = DONT_OWN_ANY_NFT_TEXT;
            openseaButton.SetActive(true);
        }
        else
        {
            selectorController.SetMaxAmount(int.Parse(balanceOf));
            string userLastMatchId = await evmService.GetUserLastMatch(account);
            string matchInfoResponse = await evmService.GetMatchInfoById(userLastMatchId);
            MatchInfo matchInfo = JsonConvert.DeserializeObject<MatchInfo>(matchInfoResponse);
            Debug.Log("UserLastMatch: " + userLastMatchId);
            if (userLastMatchId == "0" || matchInfo.isMatchFinished)
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
                    userLastMatchId = await evmService.GetUserLastMatch(account);
                    string randomSeed = "0";
                    int count = 0;
                    while (randomSeed == "0")
                    {
                        await new WaitForSeconds(1f);
                        Debug.Log(count);
                        count++;
                        if (count % 5 == 0 || count == 1)
                        {
                            Debug.Log("Asking seed...");
                            matchInfoResponse = await evmService.GetMatchInfoById(userLastMatchId);
                            matchInfo = JsonConvert.DeserializeObject<MatchInfo>(matchInfoResponse);
                            randomSeed = matchInfo.matchRandomSeed;
                        }
                        infoText.GetComponent<TextMeshProUGUI>().text = "Waiting for\nrandom seed...\n\n" + count + " secs\n\nup to\n60 seconds";
                    }
                    infoText.GetComponent<TextMeshProUGUI>().text = INITIALIZING_MATCH_TEXT;
                    string initMatchTxHash = "";
                    try
                    {
                        initMatchTxHash = await evmService.InitializeMatch(account);
                    }
                    catch (Exception er)
                    {
                        ReloadWindow();
                    }
                    while (txStatus == "" || txStatus == "pending")
                    {
                        txStatus = await evmService.TxStatus(initMatchTxHash);
                        Debug.Log("Status:" + txStatus);
                    }
                }
                PlayerPrefs.SetString("MatchId", userLastMatchId);
                SceneManager.LoadScene(BOARD_GAME_SCENE_NAME);
            }
        }
    }

    public async void ContinueCreatingMatch()
    {
        cardSelector.SetActive(false);
        infoText.SetActive(true);
        string txStatus = "";
        infoText.GetComponent<TextMeshProUGUI>().text = CREATING_MATCH_TEXT;
        string createMatchTxHash = "";
        string dryCreateMatchTxHash = await evmService.DryCreateMatch(selectorController.amountSelected, account);
        if (dryCreateMatchTxHash.Contains(EXCEEDED_DAILY_MATCHES))
        {
            infoText.GetComponent<TextMeshProUGUI>().text = COME_BACK_TOMORROW_TEXT;
        }
        else
        {
            try
            {
                createMatchTxHash = await evmService.CreateMatch(selectorController.amountSelected);
            }
            catch (Exception e)
            {
                ReloadWindow();
            }
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
                if (count % 5 == 0 || count == 1)
                {
                    Debug.Log("Asking seed...");
                    string matchInfoResponse = await evmService.GetMatchInfoById(userLastMatchId);
                    MatchInfo matchInfo = JsonConvert.DeserializeObject<MatchInfo>(matchInfoResponse);
                    randomSeed = matchInfo.matchRandomSeed;
                }
                infoText.GetComponent<TextMeshProUGUI>().text = "Waiting for\nrandom seed...\n\n" + count + " secs\n\nup to\n60 seconds";
            }
            infoText.GetComponent<TextMeshProUGUI>().text = INITIALIZING_MATCH_TEXT;
            txStatus = "";
            string initMatchTxHash = "";
            try
            {
                initMatchTxHash = await evmService.InitializeMatch(account);
            }
            catch (Exception e)
            {
                ReloadWindow();
            }
            while (txStatus == "" || txStatus == "pending")
            {
                txStatus = await evmService.TxStatus(initMatchTxHash);
            }
            SceneManager.LoadScene(BOARD_GAME_SCENE_NAME);
            PlayerPrefs.SetString("MatchId", userLastMatchId);
        }
    }

    public void OpenOpensea()
    {
        OpenURLInExternalWindow("https://opensea.io/collection/o9wmidgard");
    }
}
