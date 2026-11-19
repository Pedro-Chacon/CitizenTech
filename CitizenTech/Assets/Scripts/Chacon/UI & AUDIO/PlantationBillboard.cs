using UnityEngine;
using UnityEngine.UI;

public class PlantationBillboard : MonoBehaviour
{
    [Header("Referências (filhos do Billboard)")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private Button button;
    [SerializeField] private Image iconImage;

    [Header("Ícones")]
    [Tooltip("Ícone de colher, igual pra todas as plantações. O de plantar vem do PlantationDataSO.")]
    [SerializeField] private Sprite harvestIcon;

    private FarmInstallation farm;

    private void Start()
    {
        farm = GetComponentInParent<FarmInstallation>();

        if (farm == null || !farm.enabled)
            return;

        canvas.worldCamera = Camera.main;
        button.onClick.AddListener(OnButtonClicked);
        farm.OnChanged += Refresh;
        Refresh(farm);
    }

    private void OnDestroy()
    {
        if (farm != null)
            farm.OnChanged -= Refresh;
    }

    private void Refresh(Installation installation)
    {
        switch (farm.State)
        {
            case PlantationState.Idle:
                PlantationDataSO plantData = farm.Data as PlantationDataSO;
                Show(plantData != null ? plantData.seedIcon : null);
                break;

            case PlantationState.Ready:
                Show(harvestIcon);
                break;

            default: // Growing: nada pra clicar
                canvas.gameObject.SetActive(false);
                break;
        }
    }

    private void Show(Sprite icon)
    {
        iconImage.sprite = icon;
        canvas.gameObject.SetActive(true);
    }

    private void OnButtonClicked()
    {
        // Não deixa interagir enquanto o dia está passando.
        if (TurnManager.instance != null && TurnManager.instance.IsBusy)
            return;

        switch (farm.State)
        {
            case PlantationState.Idle:
                farm.Plant();
                break;
            case PlantationState.Ready:
                farm.Harvest();
                break;
        }
    }
}