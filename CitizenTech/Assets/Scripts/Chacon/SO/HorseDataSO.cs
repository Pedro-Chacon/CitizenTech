using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Fazenda/Horse Data")]
public class HorseDataSO : InstallationDataSO
{
    [Header("Viagens")]
    [Tooltip("Dias fora por nível. Posição 0 = cavalo, 1 = depois do 1º upgrade (carroça), 2 = depois do 2º (carro)...")]
    public List<int> tripDaysPerLevel = new() { 3, 2, 1 };
}