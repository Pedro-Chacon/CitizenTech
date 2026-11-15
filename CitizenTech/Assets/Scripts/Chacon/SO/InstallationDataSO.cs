using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Fazenda/Installation Data")]
public class InstallationDataSO : ScriptableObject
{
    [Header("Geral")]
    public string displayName;
    public Sprite icon;
    [Min(0)] public int price = 100;

    [Tooltip("Porcentagem do total gasto (compra + upgrades) devolvida ao vender.")]
    [Range(0f, 1f)] public float refundPercent = 0.5f;

    [Header("Upgrades (em ordem, do 1º ao último)")]
    public List<UpgradeLevelData> upgrades = new();
}

[Serializable]
public class UpgradeLevelData
{
    public string displayName;
    public Sprite image;
    [TextArea] public string description;
    [Min(0)] public int cost = 100;

    // O EFEITO do upgrade (rendimento, redução de turnos...) é definido pelo game designer.
    // Adicione os campos dele aqui (ex.: public float yieldMultiplier = 1f;)
    // e aplique em Installation.OnUpgraded / nas subclasses.
}