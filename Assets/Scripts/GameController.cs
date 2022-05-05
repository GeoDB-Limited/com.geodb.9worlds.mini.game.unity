using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;

public enum SingleMatchState
{
    Tie = -1,
    AIWinner = 0,
    PlayerWinner = 1
}

public class GameController : MonoBehaviour
{

    private const string BASE_URI = "https://odin9worldsmidgard.mypinata.cloud/ipfs/QmTgJNNnaF2tcaXNufmkjKHEUquJA45sW9V9bBw2MdgEDn/";
    private const string BASE_EXTENSION = ".json";
    private const string AI_HAND_NAME = "AIHand";
    private const string PLAYER_HAND_NAME = "PlayerHand";
    private const string AI_TOTAL_POWER_NAME = "TotalPowerAI";
    private const string PLAYER_TOTAL_POWER_NAME = "TotalPowerPlayer";
    private const string REORDER_BUTTON_NAME = "ReorderButton";
    private const string OUTLINE_NAME = "Outline";
    private const int LAST_NFT_ID = 999;

    public int cardAmount;
    public List<GameObject> boards;
    public GameObject reorderButton;
    public GameObject cancelReorderButton;
    public GameObject swapButton;
    public GameObject aiHand;
    public GameObject playerHand;
    public List<GameObject> aiCards;
    public List<GameObject> playerCards;
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

    void Start()
    {
        metadata = new Dictionary<int, NFTMetadata>();
        aiCards = new List<GameObject>();
        playerCards = new List<GameObject>();
        singleMatchesState = new SingleMatchState[cardAmount];
        reorderSelectedNfts = new List<GameObject>();
        List<Coroutine> coroutines = new List<Coroutine>();
        Instantiate(boards[cardAmount-1]);
        aiHand = GameObject.Find(AI_HAND_NAME);
        playerHand = GameObject.Find(PLAYER_HAND_NAME);
        GenerateRandomMatch();
        foreach( Transform item in aiHand.GetComponentsInChildren<Transform>()) {
            if (item.name != AI_HAND_NAME) {
               aiCards.Add(item.gameObject);
            }
        }
        foreach( Transform item in playerHand.GetComponentsInChildren<Transform>()) {
            if (item.name != PLAYER_HAND_NAME && !item.name.Contains(OUTLINE_NAME)) {
                playerCards.Add(item.gameObject);
            }
        }
        for (int i = 0; i < aiNfts.Count; i++) {
            coroutines.Add(StartCoroutine(GetRequest(aiNfts[i], i, true)));
        }
        for (int i = 0; i < playerNfts.Count; i++) {
            coroutines.Add(StartCoroutine(GetRequest(playerNfts[i], i, false)));
        }
    }

    public void RerollMatch() {
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);
    }

    public void Battle() {
        int aiPoints = 0;
        int playerPoints = 0;
        for (int i = 0; i < aiCards.Count; i++) {
            int aiType = (int) aiCards[i].GetComponent<NFTGameObjectInfo>().weaponType;
            int playerType = (int) playerCards[i].GetComponent<NFTGameObjectInfo>().weaponType;
            if (aiType == 0 && playerType == 1) {
                aiPoints++;
                singleMatchesState[i] = SingleMatchState.AIWinner;
            } else if (aiType == 0 && playerType == 2) {
                playerPoints++;
                singleMatchesState[i] = SingleMatchState.PlayerWinner;
            } else if (aiType == 1 && playerType == 2) {
                aiPoints++;
                singleMatchesState[i] = SingleMatchState.AIWinner;
            } else if (aiType == 1 && playerType == 0) {
                playerPoints++;
                singleMatchesState[i] = SingleMatchState.PlayerWinner;
            } else if (aiType == 2 && playerType == 0) {
                aiPoints++;
                singleMatchesState[i] = SingleMatchState.AIWinner;
            } else if (aiType == 2 && playerType == 1) {
                playerPoints++;
                singleMatchesState[i] = SingleMatchState.PlayerWinner;
            } else {
                singleMatchesState[i] = SingleMatchState.Tie;
            }
        }
        Debug.Log(aiPoints.ToString() + ", " + playerPoints.ToString());
    }

    public void ToggleReorderSelection() {
        isReorderSelectionActive = !isReorderSelectionActive;
        reorderButton.SetActive(!isReorderSelectionActive);
        cancelReorderButton.SetActive(isReorderSelectionActive);
    }

    public bool selectReorderNFT(GameObject selectedItem) {
        bool wasItemAdded = false;
        if (reorderSelectedNfts.Count < 2 && !reorderSelectedNfts.Contains(selectedItem)) {
            reorderSelectedNfts.Add(selectedItem);
            wasItemAdded = true;
        } else {
            if (reorderSelectedNfts.Contains(selectedItem)) {
                reorderSelectedNfts.Remove(selectedItem);
            }
        }
        swapButton.SetActive(reorderSelectedNfts.Count == 2);
        return wasItemAdded;
    }

    public void SwapNFTs() {
        int firstSelectedIndex = playerCards.IndexOf(reorderSelectedNfts[0]);
        int secondSelectedIndex = playerCards.IndexOf(reorderSelectedNfts[1]);
        playerCards[firstSelectedIndex] = reorderSelectedNfts[1];
        playerCards[secondSelectedIndex] = reorderSelectedNfts[0];
        reorderSelectedNfts[0].GetComponent<NFTController>().Swap(reorderSelectedNfts[1].transform.position);
        reorderSelectedNfts[1].GetComponent<NFTController>().Swap(reorderSelectedNfts[0].transform.position);
        swapButton.SetActive(false);
        reorderButton.SetActive(false);
        cancelReorderButton.SetActive(false);
    }

    private void GenerateRandomMatch() {
        selectedNfts = new List<int>();
        HashSet<int> alreadyUsedIds = new HashSet<int>();
        alreadyUsedIds.Add(0);
        for (int i = 0; i < cardAmount * 2; i++) {
            int rand = 0;
            while (alreadyUsedIds.Contains(rand)) {
                rand = Random.Range(1, LAST_NFT_ID + 1);
                selectedNfts.Add(rand);
            }
            alreadyUsedIds.Add(rand);
        }
        playerNfts = selectedNfts.GetRange(0,cardAmount);
        aiNfts = selectedNfts.GetRange(cardAmount,cardAmount);
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
                if (isAI) {
                    aiCards[handIndex].GetComponent<SpriteRenderer>().sprite = nftSprite;
                    aiTotalPower += aiCards[handIndex].GetComponent<NFTGameObjectInfo>().Initialize(currentMetadata);
                    GameObject.Find(AI_TOTAL_POWER_NAME).GetComponent<TextMeshProUGUI>().text = "Total power: " + aiTotalPower;

                } else {
                    playerCards[handIndex].GetComponent<SpriteRenderer>().sprite = nftSprite;
                    playerTotalPower += playerCards[handIndex].GetComponent<NFTGameObjectInfo>().Initialize(currentMetadata);
                    GameObject.Find(PLAYER_TOTAL_POWER_NAME).GetComponent<TextMeshProUGUI>().text = "Total power: " + playerTotalPower;
                }
                reorderButton.SetActive(playerTotalPower > aiTotalPower);
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
