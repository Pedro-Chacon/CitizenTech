using UnityEngine;
using UnityEngine.UI;

public class HorseBillboard : MonoBehaviour
{
    [Header("Referências (filhos do Billboard)")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private Button button;
    [SerializeField] private Image iconImage;

    [Header("Ícone")]
    [Tooltip("Ícone da moeda, mostrado quando o cavalo voltou com o lucro.")]
    [SerializeField] private Sprite coinIcon;

    private HorseInstallation horse;

    private void Start()
    {
        horse = GetComponentInParent<HorseInstallation>();

        // Cópia do preview (Installation desativado): o canvas fica escondido.
        if (horse == null || !horse.enabled)
            return;

        canvas.worldCamera = Camera.main;
        button.onClick.AddListener(OnButtonClicked);
        horse.OnChanged += Refresh;
        Refresh(horse);
    }

    private void OnDestroy()
    {
        if (horse != null)
            horse.OnChanged -= Refresh;
    }

    private void Refresh(Installation installation)
    {
        if (horse.State == HorseState.Returned)
        {
            iconImage.sprite = coinIcon;
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

        horse.CollectProfit();
    }
}