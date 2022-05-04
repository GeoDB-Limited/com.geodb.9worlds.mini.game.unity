using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NFTController : MonoBehaviour
{
    private const string GAME_CONTROLLER_NAME = "GameController";
    public bool isSelected = false;
    private SpriteRenderer spriteRenderer;
    private GameController gameController;
    void Start()
    {
        spriteRenderer = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        gameController = GameObject.Find(GAME_CONTROLLER_NAME).GetComponent<GameController>();
    }

    private void OnMouseEnter() {
        if (gameController.isReorderSelectionActive && gameController.reorderSelectedNfts.Count < 2) {
            spriteRenderer.enabled = true;
        }
    }

    private void OnMouseExit() {
        if (gameController.isReorderSelectionActive && !gameController.reorderSelectedNfts.Contains(gameObject)) {
            spriteRenderer.enabled = false;
        }
    }

    private void OnMouseDown() {
        if (gameController.isReorderSelectionActive) {
            bool wasItemAdded = gameController.selectReorderNFT(gameObject);
            spriteRenderer.enabled = wasItemAdded;
        }
    }
}
