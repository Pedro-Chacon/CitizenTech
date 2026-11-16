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

    [Header("Efeitos (somam ao valor base; deixe 0 se não se aplica)")]
    [Tooltip("Plantação: dias a mais/menos pra crescer. Ex.: -1")]
    public int growthDaysChange;
    [Tooltip("Plantação: unidades a mais/menos por colheita. Ex.: +3")]
    public int yieldChange;
    [Tooltip("Cavalo: dias a mais/menos de viagem. Ex.: -1")]
    public int tripDaysChange;
    [Tooltip("Forno: dias a mais/menos pra assar. Ex.: -1")]
    public int bakeDaysChange;
    [Tooltip("Forno: unidades a mais/menos por fornada. Ex.: +2")]
    public int ovenOutputChange;

    // Texto automático dos efeitos, mostrado no card.
    public string GetEffectSummary()
    {
        List<string> lines = new();
        if (growthDaysChange != 0) lines.Add($"Crescimento: {Signed(growthDaysChange)} dia(s)");
        if (yieldChange != 0) lines.Add($"Colheita: {Signed(yieldChange)}");
        if (tripDaysChange != 0) lines.Add($"Viagem: {Signed(tripDaysChange)} dia(s)");
        if (bakeDaysChange != 0) lines.Add($"Assado: {Signed(bakeDaysChange)} dia(s)");
        if (ovenOutputChange != 0) lines.Add($"Produção: {Signed(ovenOutputChange)}");
        return string.Join("\n", lines);
    }

    private static string Signed(int value) => value > 0 ? $"+{value}" : value.ToString();
}