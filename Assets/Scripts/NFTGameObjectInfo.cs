using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.InteropServices;

public enum WeaponType
{
    None = -1,
    Sword = 0,
    Axe = 1,
    Shield = 2
}

public class NFTGameObjectInfo : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void OpenURLInExternalWindow(string url);

    private readonly string[] WEAPON_KEY_WORDS = {"sword", "axe", "shield"};
    private readonly string TOTAL_POWER_KEY_WORD = "Total Power";
    public NFTMetadata metadata;
    public WeaponType weaponType = WeaponType.None;
    public int totalPower;
    public bool isInteractable = true;

    public int Initialize(NFTMetadata metadata) {
        this.metadata = metadata;
        foreach (NFTAttribute attribute in metadata.attributes) {
            if (weaponType == WeaponType.None){
                for (int i = 0; i < WEAPON_KEY_WORDS.Length; i++) {
                    if (attribute.trait_type.ToLower().Contains(WEAPON_KEY_WORDS[i])) {
                        weaponType = (WeaponType) i;
                    }
                }
            }
            if (attribute.trait_type.ToLower().Contains(TOTAL_POWER_KEY_WORD.ToLower())) {
                totalPower = int.Parse(attribute.value);
            }
        }

        transform.Find("Canvas/Button/Id").GetComponent<TextMeshProUGUI>().text = metadata.edition.ToString();

        return totalPower;
    }

    private void OnMouseDown() {
        if (isInteractable) {
            OpenURLInExternalWindow("https://opensea.io/assets/matic/0x158b808cb4c585017b6276e9d2753350e67363ec/" + metadata.edition);
        }
    }
}
