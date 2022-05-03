using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class GameController : MonoBehaviour
{

    private const string BASE_URI = "https://odin9worldsmidgard.mypinata.cloud/ipfs/QmTgJNNnaF2tcaXNufmkjKHEUquJA45sW9V9bBw2MdgEDn/";
    private const string BASE_EXTENSION = ".json";
    private const int LAST_NFT_ID = 999;

    public GameObject nftPrefab;
    public GameObject aiHand;
    public GameObject playerHand;
    public List<GameObject> aiCards;
    public List<GameObject> playerCards;
    public int aiTotalPower = 0;
    public int playerTotalPower = 0;

    public int cardAmount = 3;
    public List<int> selectedNfts;
    public List<int> playerNfts;
    public List<int> aiNfts;

    private Dictionary<int, NFTMetadata> metadata;

    private Texture2D nftImage;
    void Start()
    {
        metadata = new Dictionary<int, NFTMetadata>();
        aiCards = new List<GameObject>();
        playerCards = new List<GameObject>();
        List<Coroutine> coroutines = new List<Coroutine>();
        GenerateRandomMatch();
        foreach( Transform item in aiHand.GetComponentsInChildren<Transform>()) {
            if (item.name != "AIHand") {
               aiCards.Add(item.gameObject);
            }
        }
        foreach( Transform item in playerHand.GetComponentsInChildren<Transform>()) {
            if (item.name != "PlayerHand") {
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

    

    void GenerateRandomMatch() {
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

    IEnumerator GetRequest(int nftId, int handIndex, bool isAI)
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

                } else {
                    playerCards[handIndex].GetComponent<SpriteRenderer>().sprite = nftSprite;
                    playerTotalPower += playerCards[handIndex].GetComponent<NFTGameObjectInfo>().Initialize(currentMetadata);
                }
            }

            switch (webRequest.result)
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
            }
        }
    }
}
