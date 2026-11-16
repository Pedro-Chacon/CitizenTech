using UnityEngine;
using UnityEngine.UI;

public class OvenBillboard : MonoBehaviour
{
    [Header("Referências (filhos do Billboard)")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private Button button;
    [SerializeField] private Image iconImage;

    private OvenInstallation oven;

    private void Start()
    {
        oven = GetComponentInParent<OvenInstallation>();

        // Cópia do preview (Installation desativado): o canvas fica escondido.
        if (oven == null || !oven.enabled)
            return;

        canvas.worldCamera = Camera.main;
        button.onClick.AddListener(OnButtonClicked);
        oven.OnChanged += Refresh;
        Refresh(oven);
    }

    private void OnDestroy()
    {
        if (oven != null)
            oven.OnChanged -= Refresh;
    }

    // Quando pronto, mostra o ícone do que foi assado (o ícone do produto de saída).
    private void Refresh(Installation installation)
    {
        if (oven.State == OvenState.Ready && oven.OutputProduct != null)
        {
            iconImage.sprite = oven.OutputProduct.icon;
            canvas.gameObject.SetActive(true);
        }
        else
        {
            canvas.gameObject.SetActive(false);
        }
    }

    private void OnButtonClicked()
    {
        if (TurnManager.instance != null && TurnManager.instance.IsBusy)
            return;

        oven.CollectOutput();
    }
}