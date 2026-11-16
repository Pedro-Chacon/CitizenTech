using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryRowUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text quantityText;

    public void Setup(ProductSO product)
    {
        nameText.text = product.displayName;

        iconImage.sprite = product.icon;
        iconImage.enabled = product.icon != null;
    }

    public void SetAmount(int amount)
    {
        quantityText.text = $"x {amount}";
    }
}