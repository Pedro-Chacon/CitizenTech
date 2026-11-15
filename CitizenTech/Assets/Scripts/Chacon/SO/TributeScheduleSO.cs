using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Fazenda/Tribute Schedule")]
public class TributeScheduleSO : ScriptableObject
{
    [Tooltip("Em ordem. O jogo termina (vitória) quando o último for pago.")]
    public List<TributeData> tributes = new();
}

[Serializable]
public class TributeData
{
    [Tooltip("Dia em que o tributo é cobrado, no FIM desse dia. Use valores crescentes.")]
    [Min(1)] public int dueDay = 5;

    [Min(0)] public int amount = 100;
}