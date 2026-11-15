using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExportRowUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text availableText;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private Button minusButton;
    [SerializeField] private Button plusButton;

    public void Setup(ProductSO product, Action onMinus, Action onPlus)
    {
        nameText.text = product.displayName;
        priceText.text = $"{product.sellPrice} moedas cada";

        iconImage.sprite = product.icon;
        iconImage.enabled = product.icon != null;

        minusButton.onClick.RemoveAllListeners();
        minusButton.onClick.AddListener(() => onMinus?.Invoke());
        plusButton.onClick.RemoveAllListeners();
        plusButton.onClick.AddListener(() => onPlus?.Invoke());
    }

    public void SetValues(int available, int selected)
    {
        availableText.text = $"No silo: {available}";
        quantityText.text = selected.ToString();
        minusButton.interactable = selected > 0;
        plusButton.interactable = selected < available;
    }
}