using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OvenPanel : MonoBehaviour
{
    public static OvenPanel instance { get; private set; }

    [Header("Referências")]
    [SerializeField] private JuicyPanel panel;
    [SerializeField] private TMP_Text recipeText;
    [SerializeField] private TMP_Text stockText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private Button bakeButton;
    [SerializeField] private Button cancelButton;

    private OvenInstallation oven;

    // O SelectionManager consulta isso pra ignorar cliques no mundo enquanto a janela está aberta.
    public bool IsOpen { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("Tem mais de um OvenPanel na cena!");
            return;
        }
        instance = this;
    }

    private void Start()
    {
        bakeButton.onClick.AddListener(Bake);
        cancelButton.onClick.AddListener(Close);

        if (InventoryManager.instance != null)
            InventoryManager.instance.OnInventoryChanged += Refresh;

        panel.Hide(true);
    }

    private void OnDestroy()
    {
        if (InventoryManager.instance != null)
            InventoryManager.instance.OnInventoryChanged -= Refresh;

        if (instance == this)
            instance = null;
    }

    public void Open(OvenInstallation targetOven)
    {
        oven = targetOven;
        IsOpen = true;

        Refresh();
        panel.Show();
    }

    public void Close()
    {
        IsOpen = false;
        oven = null;
        panel.Hide();
    }

    private void Refresh()
    {
        if (!IsOpen || oven == null || oven.InputProduct == null || oven.OutputProduct == null)
            return;

        int stock = InventoryManager.instance != null
            ? InventoryManager.instance.GetAmount(oven.InputProduct)
            : 0;

        recipeText.text = $"{oven.InputAmount} {oven.InputProduct.displayName} -> " +
                          $"{oven.OutputAmount} {oven.OutputProduct.displayName}";
        stockText.text = $"No silo: {stock}";
        timeText.text = $"Tempo: {oven.BakeDays} dias";

        bakeButton.interactable = stock >= oven.InputAmount;
    }

    private void Bake()
    {
        // Não começa enquanto o dia está passando.
        if (TurnManager.instance != null && TurnManager.instance.IsBusy)
            return;

        if (oven != null && oven.TryStartBaking())
            Close();
    }
}