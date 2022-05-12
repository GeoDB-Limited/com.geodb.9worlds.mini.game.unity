using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NFTController : MonoBehaviour
{
    private const string GAME_CONTROLLER_NAME = "GameController";
    private const string OUTLINE_NAME = "Outline";
    
    public bool isSelected = false;
    public float lerpSpeed = 5f;
    public Vector3 objectivePosition = Vector3.zero;
    public Vector3 startingPosition = Vector3.zero;
    public float lerp = 1;
    private SpriteRenderer spriteRenderer;
    private GameController gameController;
    
    public void Swap(Vector3 objectivePosition) {
        startingPosition = transform.position;
        this.objectivePosition = objectivePosition;
        lerp = 0;
        spriteRenderer.enabled = false;
    }

    private void Start()
    {
        spriteRenderer = transform.Find(OUTLINE_NAME).gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        gameController = GameObject.Find(GAME_CONTROLLER_NAME).GetComponent<GameController>();
    }

    private void Update() {
        if (lerp < 1) {
            Vector3 positon = Vector3.Lerp(startingPosition, objectivePosition, lerp);
            transform.position = positon;
            lerp += Mathf.Min(lerpSpeed * Time.deltaTime, 1f);
        } else if (lerp >= 1 && objectivePosition != Vector3.zero) {
            Vector3 positon = Vector3.Lerp(startingPosition, objectivePosition, 1);
            transform.position = positon;
        }
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
