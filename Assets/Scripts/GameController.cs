using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;
using System.Runtime.InteropServices;

public enum SingleMatchState
{
    Tie = -1,
    AIWinner = 0,
    PlayerWinner = 1
}

public class GameController : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void OpenURLInExternalWindow(string url);

    [DllImport("__Internal")]
    private static extern void ReloadWindow();

    private const string BASE_URI = "https://odin9worldsmidgard.mypinata.cloud/ipfs/QmTgJNNnaF2tcaXNufmkjKHEUquJA45sW9V9bBw2MdgEDn/";
    private const string POLYGON_SCAN_BASE_URL = "https://mumbai.polygonscan.com/tx/";
    private const string BASE_EXTENSION = ".json";
    private const string NFT_PREFIX = "NFT";

    private const string AI_HAND_NAME = "AIHand";
    private const string PLAYER_HAND_NAME = "PlayerHand";
    private const string AI_TOTAL_POWER_NAME = "TotalPowerAI";
    private const string PLAYER_TOTAL_POWER_NAME = "TotalPowerPlayer";
    private const string REORDER_BUTTON_NAME = "ReorderButton";
    private const string OUTLINE_NAME = "Outline";
    private const string FRAME_NAME = "Frame";
    private const string TX_INFO_TEXT_NAME = "TxInfoText";
    private const string WIN_INDICATOR_NAME = "WinIndicator";
    private const string WIN_TEXT_NAME = "WinnerText";
    private const string RESULT_INFO_TEXT_NAME = "ResultInfoText";
    private const string WIN_ICON_TAG = "WinIcon";
    private const string WATING_FOR_TX_CONFIRM_TEXT = "Wating for tx confirmation...";
    private const string SUCCESS_TEXT = "Success!";

    private const int LAST_NFT_ID = 999;

    public int cardAmount;
    public List<GameObject> boards;
    public GameObject powerWinIndicator;
    public GameObject userInfoBox;
    public GameObject reorderButton;
    public GameObject loadingScreen;
    public GameObject loadingText;
    public GameObject cancelReorderButton;
    public GameObject swapButton;
    public GameObject txInfoWindow;
    public GameObject result;
    public GameObject aiHand;
    public GameObject playerHand;
    public List<GameObject> aiCards;
    public List<GameObject> playerCards;
    public List<GameObject> playerWinIndicators;
    public int aiTotalPower = 0;
    public int playerTotalPower = 0;

    public List<int> selectedNfts;
    public List<int> playerNfts;
    public List<int> aiNfts;
    public SingleMatchState[] singleMatchesState;
    public bool isReorderSelectionActive = false;
    public List<GameObject> reorderSelectedNfts;

    private Dictionary<int, NFTMetadata> metadata;
    private Texture2D nftImage;
    private EVMService evmService;
    private string userLastMatchId;
    private MatchInfo matchInfo;
    private string resolveTxHash = "";
    private int loadedCards = 1;
    private int firstSelectedIndex = -1;
    private int secondSelectedIndex = -1;
    private int playerWinnerGems = 100;
    private int playerTieGems = 10;
    private int totalGems = 0;


    async void Start()
    {
        loadingScreen.SetActive(true);
        selectedNfts = new List<int>();
        evmService = new EVMService();
        metadata = new Dictionary<int, NFTMetadata>();
        aiCards = new List<GameObject>();
        playerCards = new List<GameObject>();
        reorderSelectedNfts = new List<GameObject>();
        List<Coroutine> coroutines = new List<Coroutine>();

        string account = PlayerPrefs.GetString("Account");
        totalGems = PlayerPrefs.GetInt("TotalGems");
        string shortAccount = PlayerPrefs.GetString("ShortAccount");
        userInfoBox.SetActive(true);
        userInfoBox.transform.Find("UserAddress").GetComponent<TextMeshProUGUI>().text = shortAccount;
        userInfoBox.transform.Find("WeakBalance").GetComponent<TextMeshProUGUI>().text = "Total O9W tickets:\n" + totalGems;
        string userLastMatchId = PlayerPrefs.GetString("MatchId");
        string matchInfoResponse = await evmService.GetMatchInfoById(userLastMatchId);
        matchInfo = JsonConvert.DeserializeObject<MatchInfo>(matchInfoResponse);
        cardAmount = int.Parse(matchInfo.nftMatchCount);
        singleMatchesState = new SingleMatchState[cardAmount];
        Instantiate(boards[cardAmount - 1]);
        aiHand = GameObject.Find(AI_HAND_NAME);
        playerHand = GameObject.Find(PLAYER_HAND_NAME);
        for (int i = 0; i < cardAmount; i++)
        {
            string playerResponse = await evmService.GetValidPlayerNft(userLastMatchId, i.ToString());
            selectedNfts.Add(int.Parse(playerResponse));
        }
        for (int i = 0; i < cardAmount; i++)
        {
            string computerResponse = await evmService.GetValidComputerNft(userLastMatchId, i.ToString());
            selectedNfts.Add(int.Parse(computerResponse));
        }
        playerNfts = selectedNfts.GetRange(0, cardAmount);
        aiNfts = selectedNfts.GetRange(cardAmount, cardAmount);
        foreach (Transform item in aiHand.GetComponentsInChildren<Transform>())
        {
            if (item.name.Contains(NFT_PREFIX))
            {
                Debug.Log(item.name);
                aiCards.Add(item.gameObject);
            }
        }
        foreach (Transform item in playerHand.GetComponentsInChildren<Transform>())
        {
            if (item.name.Contains(NFT_PREFIX))
            {
                Debug.Log(item.name);
                playerCards.Add(item.gameObject);
            }
            else if (item.name == WIN_INDICATOR_NAME)
            {
                Debug.Log(item.name);
                playerWinIndicators.Add(item.gameObject);
            }
        }
        for (int i = 0; i < aiNfts.Count; i++)
        {
            coroutines.Add(StartCoroutine(GetRequest(aiNfts[i], i, true)));
        }
        for (int i = 0; i < playerNfts.Count; i++)
        {
            coroutines.Add(StartCoroutine(GetRequest(playerNfts[i], i, false)));
        }
    }

    public void Reload()
    {
        ReloadWindow();
    }

    public void Battle()
    {
        int aiPoints = 0;
        int playerPoints = 0;
        for (int i = 0; i < aiCards.Count; i++)
        {
            int aiType = (int)aiCards[i].GetComponent<NFTGameObjectInfo>().weaponType;
            int playerType = (int)playerCards[i].GetComponent<NFTGameObjectInfo>().weaponType;
            if (aiType == 0 && playerType == 1)
            {
                aiPoints++;
                singleMatchesState[i] = SingleMatchState.AIWinner;
            }
            else if (aiType == 0 && playerType == 2)
            {
                playerPoints++;
                singleMatchesState[i] = SingleMatchState.PlayerWinner;
            }
            else if (aiType == 1 && playerType == 2)
            {
                aiPoints++;
                singleMatchesState[i] = SingleMatchState.AIWinner;
            }
            else if (aiType == 1 && playerType == 0)
            {
                playerPoints++;
                singleMatchesState[i] = SingleMatchState.PlayerWinner;
            }
            else if (aiType == 2 && playerType == 0)
            {
                aiPoints++;
                singleMatchesState[i] = SingleMatchState.AIWinner;
            }
            else if (aiType == 2 && playerType == 1)
            {
                playerPoints++;
                singleMatchesState[i] = SingleMatchState.PlayerWinner;
            }
            else
            {
                singleMatchesState[i] = SingleMatchState.Tie;
            }
        }
        result.SetActive(true);
        TextMeshProUGUI resultInfoText = GameObject.Find(RESULT_INFO_TEXT_NAME).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI winText = GameObject.Find(WIN_TEXT_NAME).GetComponent<TextMeshProUGUI>();
        GameObject[] winIcons = GameObject.FindGameObjectsWithTag(WIN_ICON_TAG);
        if (aiPoints > playerPoints)
        {
            winText.text = "Defeat!";
            resultInfoText.text = playerPoints + " - " + aiPoints + "\nNo gems won!";
            winIcons[0].GetComponent<WinIconController>().iconState = IconState.Win;
            winIcons[1].GetComponent<WinIconController>().iconState = IconState.Win;
        }
        else if (aiPoints < playerPoints)
        {
            winText.text = "Victory!";
            resultInfoText.text = playerPoints + " - " + aiPoints + "\n+" + playerWinnerGems * cardAmount + " gems!";
            userInfoBox.transform.Find("WeakBalance").GetComponent<TextMeshProUGUI>().text = "Total O9W tickets:\n" + (totalGems + (playerWinnerGems * cardAmount));
            winIcons[0].GetComponent<WinIconController>().iconState = IconState.Lose;
            winIcons[1].GetComponent<WinIconController>().iconState = IconState.Lose;
        }
        else
        {
            winText.text = "Tie!";
            resultInfoText.text = playerPoints + " - " + aiPoints + "\n+" + playerTieGems * cardAmount + " gems!";
            userInfoBox.transform.Find("WeakBalance").GetComponent<TextMeshProUGUI>().text = "Total O9W tickets:\n" + (totalGems + (playerTieGems * cardAmount));
            winIcons[0].GetComponent<WinIconController>().iconState = IconState.Tie;
            winIcons[1].GetComponent<WinIconController>().iconState = IconState.Tie;
        }
    }

    public void ToggleReorderSelection()
    {
        isReorderSelectionActive = !isReorderSelectionActive;
        reorderButton.SetActive(!isReorderSelectionActive);
        cancelReorderButton.SetActive(isReorderSelectionActive);
    }

    public bool selectReorderNFT(GameObject selectedItem)
    {
        bool wasItemAdded = false;
        if (reorderSelectedNfts.Count < 2 && !reorderSelectedNfts.Contains(selectedItem))
        {
            reorderSelectedNfts.Add(selectedItem);
            wasItemAdded = true;
        }
        else
        {
            if (reorderSelectedNfts.Contains(selectedItem))
            {
                reorderSelectedNfts.Remove(selectedItem);
            }
        }
        swapButton.SetActive(reorderSelectedNfts.Count == 2);
        return wasItemAdded;
    }

    public void SwapNFTs()
    {
        firstSelectedIndex = playerCards.IndexOf(reorderSelectedNfts[0]);
        secondSelectedIndex = playerCards.IndexOf(reorderSelectedNfts[1]);
        playerCards[firstSelectedIndex] = reorderSelectedNfts[1];
        playerCards[secondSelectedIndex] = reorderSelectedNfts[0];
        reorderSelectedNfts[0].GetComponent<NFTController>().Swap(reorderSelectedNfts[1].transform.position);
        reorderSelectedNfts[1].GetComponent<NFTController>().Swap(reorderSelectedNfts[0].transform.position);
        GameObject tmp = playerWinIndicators[firstSelectedIndex];
        playerWinIndicators[firstSelectedIndex] = playerWinIndicators[secondSelectedIndex];
        playerWinIndicators[secondSelectedIndex] = tmp;
        swapButton.SetActive(false);
        reorderButton.SetActive(false);
        cancelReorderButton.SetActive(false);
        isReorderSelectionActive = false;
    }

    public void OpenBrowserTx()
    {
        OpenURLInExternalWindow(POLYGON_SCAN_BASE_URL + resolveTxHash);
    }

    public void ResolveMatch()
    {
        txInfoWindow.SetActive(true);
        ResolveMatchTransaction();
    }

    private void GenerateRandomMatch()
    {
        selectedNfts = new List<int>();
        HashSet<int> alreadyUsedIds = new HashSet<int>();
        alreadyUsedIds.Add(0);
        for (int i = 0; i < cardAmount * 2; i++)
        {
            int rand = 0;
            while (alreadyUsedIds.Contains(rand))
            {
                rand = Random.Range(1, LAST_NFT_ID + 1);
                selectedNfts.Add(rand);
            }
            alreadyUsedIds.Add(rand);
        }
        playerNfts = selectedNfts.GetRange(0, cardAmount);
        aiNfts = selectedNfts.GetRange(cardAmount, cardAmount);
    }

    private async void ResolveMatchTransaction()
    {
        string txStatus = "";
        TextMeshProUGUI txInfo = GameObject.Find(TX_INFO_TEXT_NAME).GetComponent<TextMeshProUGUI>();
        try
        {
            if (firstSelectedIndex != -1 && secondSelectedIndex != -1)
            {
                resolveTxHash = await evmService.ResolveMatchReorder(firstSelectedIndex, secondSelectedIndex);
            }
            else
            {
                resolveTxHash = await evmService.ResolveMatch();
            }
        }
        catch (System.Exception e)
        {
            txInfoWindow.SetActive(false);
            return;
        }
        txInfo.text = WATING_FOR_TX_CONFIRM_TEXT;
        while (txStatus == "" || txStatus == "pending")
        {
            txStatus = await evmService.TxStatus(resolveTxHash);
        }
        txInfo.text = SUCCESS_TEXT;
        Battle();
        Debug.Log(singleMatchesState.Length);
        for (int i = 0; i < singleMatchesState.Length; i++)
        {
            if (singleMatchesState[i] == SingleMatchState.Tie)
            {
                playerWinIndicators[i].GetComponent<WinIndicatorController>().iconState = IconState.Tie;
            }
            else if (singleMatchesState[i] == SingleMatchState.PlayerWinner)
            {
                playerWinIndicators[i].GetComponent<WinIndicatorController>().iconState = IconState.Win;
            }
            else
            {
                playerWinIndicators[i].GetComponent<WinIndicatorController>().iconState = IconState.Lose;
            }
        }
        txInfoWindow.SetActive(false);
    }

    private IEnumerator GetRequest(int nftId, int handIndex, bool isAI)
    {
        string uri = BASE_URI + nftId + BASE_EXTENSION;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;
            string nftMetadata = webRequest.downloadHandler.text;
            NFTMetadata currentMetadata = new NFTMetadata();
            currentMetadata = JsonConvert.DeserializeObject<NFTMetadata>(nftMetadata);
            metadata.Add(currentMetadata.edition, currentMetadata);

            using (UnityWebRequest textureRequest = UnityWebRequestTexture.GetTexture(currentMetadata.image))
            {
                yield return textureRequest.SendWebRequest();

                nftImage = DownloadHandlerTexture.GetContent(textureRequest);
                Sprite prefabSprite = aiCards[handIndex].GetComponent<SpriteRenderer>().sprite;
                Sprite nftSprite = Sprite.Create(nftImage, new Rect(0, 0, nftImage.width, nftImage.height), new Vector2(0.5f, 0.5f), prefabSprite.pixelsPerUnit);
                if (isAI)
                {
                    aiCards[handIndex].GetComponent<SpriteRenderer>().sprite = nftSprite;
                    aiTotalPower += aiCards[handIndex].GetComponent<NFTGameObjectInfo>().Initialize(currentMetadata);
                    GameObject.Find(AI_TOTAL_POWER_NAME).GetComponent<TextMeshProUGUI>().text = "Power: " + aiTotalPower;

                }
                else
                {
                    playerCards[handIndex].GetComponent<SpriteRenderer>().sprite = nftSprite;
                    playerTotalPower += playerCards[handIndex].GetComponent<NFTGameObjectInfo>().Initialize(currentMetadata);
                    GameObject.Find(PLAYER_TOTAL_POWER_NAME).GetComponent<TextMeshProUGUI>().text = "Power: " + playerTotalPower;
                }
                reorderButton.SetActive(playerTotalPower > aiTotalPower && cardAmount > 1);
                if (playerTotalPower > aiTotalPower)
                {
                    powerWinIndicator.GetComponent<WinIndicatorController>().iconState = IconState.Win;
                }
                else if (playerTotalPower < aiTotalPower)
                {
                    powerWinIndicator.GetComponent<WinIndicatorController>().iconState = IconState.Lose;
                }
                else
                {
                    powerWinIndicator.GetComponent<WinIndicatorController>().iconState = IconState.Tie;
                }

                if (loadedCards < cardAmount * 2)
                {
                    loadedCards++;
                    loadingText.GetComponent<TextMeshProUGUI>().text = "Loading cards...\n" + loadedCards + " out of " + cardAmount * 2;
                }
                else
                {
                    loadingScreen.SetActive(false);
                }
            }

            /* switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    break;
            } */
        }
    }
}
