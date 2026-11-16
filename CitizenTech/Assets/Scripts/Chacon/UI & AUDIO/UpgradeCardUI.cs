using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCardUI : MonoBehaviour
{
    public enum CardState { Purchased, Available, Locked }

    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text effectText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Button buyButton;

    [Tooltip("Opcional: o texto dentro do botão (Comprar / Comprado / Bloqueado).")]
    [SerializeField] private TMP_Text buttonLabel;

    public void Setup(UpgradeLevelData upgrade, Action onBuy)
    {
        nameText.text = upgrade.displayName;
        descriptionText.text = upgrade.description;
        effectText.text = upgrade.GetEffectSummary();
        costText.text = $"{upgrade.cost} moedas";

        iconImage.sprite = upgrade.image;
        iconImage.enabled = upgrade.image != null;

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => onBuy?.Invoke());
    }

    public void SetState(CardState state, bool canAfford)
    {
        buyButton.interactable = state == CardState.Available && canAfford;

        if (buttonLabel == null)
            return;

        switch (state)
        {
            case CardState.Purchased: buttonLabel.text = "Comprado"; break;
            case CardState.Locked: buttonLabel.text = "Bloqueado"; break;
            default: buttonLabel.text = "Comprar"; break;
        }
    }
}