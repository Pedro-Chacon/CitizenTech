using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Button buyButton;

    public void Setup(string itemName, Sprite icon, int price, Action onBuy)
    {
        nameText.text = itemName;
        priceText.text = $"{price} moedas";

        iconImage.sprite = icon;
        iconImage.enabled = icon != null;

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => onBuy?.Invoke());
    }

    public void SetAffordable(bool affordable)
    {
        buyButton.interactable = affordable;
    }
}