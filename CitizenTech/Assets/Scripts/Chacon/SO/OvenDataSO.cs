using UnityEngine;

[CreateAssetMenu(menuName = "Fazenda/Oven Data")]
public class OvenDataSO : InstallationDataSO
{
    [Header("Receita (uma fornada)")]
    [Tooltip("Produto que entra no forno (ex.: Trigo).")]
    public ProductSO inputProduct;
    [Min(1)] public int inputAmount = 10;

    [Tooltip("Produto que sai do forno (ex.: Pão).")]
    public ProductSO outputProduct;
    [Min(1)] public int outputAmount = 5;

    [Tooltip("Quantos dias a fornada leva pra ficar pronta.")]
    [Min(1)] public int bakeDays = 2;
}