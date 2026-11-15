using UnityEngine;

[CreateAssetMenu(menuName = "Fazenda/Product")]
public class ProductSO : ScriptableObject
{
    public string displayName;
    public Sprite icon;

    [Tooltip("Quanto a cidade paga por 1 unidade deste produto.")]
    [Min(0)] public int sellPrice = 10;
}