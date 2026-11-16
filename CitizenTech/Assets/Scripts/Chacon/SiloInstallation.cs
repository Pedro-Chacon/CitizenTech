using UnityEngine;

// O silo é o inventário: a bolha extra do menu abre a lista de tudo que foi colhido.
// Os produtos ficam guardados no InventoryManager, então mover o silo não perde nada.
public class SiloInstallation : Installation
{
    [Header("Referências de cena")]
    [SerializeField] private InventoryPanel inventoryPanel;
    [SerializeField] private Sprite extraActionIcon;

    public override bool HasExtraAction => true;
    public override Sprite ExtraActionIcon => extraActionIcon;

    public override void DoExtraAction()
    {
        if (inventoryPanel != null)
            inventoryPanel.Open();
    }
}