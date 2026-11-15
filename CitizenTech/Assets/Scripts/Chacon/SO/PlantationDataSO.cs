using UnityEngine;

[CreateAssetMenu(menuName = "Fazenda/Plantation Data")]
public class PlantationDataSO : InstallationDataSO
{
    [Header("Plantação")]
    [Tooltip("Produto que esta plantação gera ao colher.")]
    public ProductSO product;

    [Tooltip("Quantas unidades vão pro inventário a cada colheita.")]
    [Min(1)] public int yieldPerHarvest = 5;

    [Tooltip("Quantos dias entre plantar e a colheita ficar pronta.")]
    [Min(1)] public int growthDays = 3;

    [Tooltip("Ícone da semente mostrado no billboard quando a plantação está esperando.")]
    public Sprite seedIcon;
}