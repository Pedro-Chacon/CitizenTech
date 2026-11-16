using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Fazenda/Horse Data")]
public class HorseDataSO : InstallationDataSO
{
    [Header("Viagens")]
    [Tooltip("Dias de viagem sem nenhum upgrade. Os upgrades somam 'Trip Days Change'.")]
    [Min(1)] public int baseTripDays = 3;
}