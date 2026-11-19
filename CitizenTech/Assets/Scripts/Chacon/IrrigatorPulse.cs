using System.Collections;
using UnityEngine;


public class IrrigatorPulse : MonoBehaviour
{
    [Header("Efeitos")]
    [Tooltip("Um ou mais Particle Systems (ex.: vários bicos de água).")]
    [SerializeField] private ParticleSystem[] effects;

    [Header("Ciclo (segundos)")]
    [SerializeField] private float activeTime = 3f;
    [SerializeField] private float inactiveTime = 2f;

    [Header("Opções")]
    [Tooltip("Sorteia um atraso no começo, pra vários irrigadores não ligarem todos juntos.")]
    [SerializeField] private float randomStartDelay = 0f;
    [Tooltip("Marcado: ao desligar, as gotas que já saíram terminam de cair. Desmarcado: somem na hora.")]
    [SerializeField] private bool letParticlesFinish = true;

    private void OnEnable()
    {
       
        StartCoroutine(CycleRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        SetEffects(false, true);
    }

    private IEnumerator CycleRoutine()
    {
        SetEffects(false, true);

        if (randomStartDelay > 0f)
            yield return new WaitForSeconds(Random.Range(0f, randomStartDelay));

        while (true)
        {
            SetEffects(true, false);
            yield return new WaitForSeconds(Mathf.Max(0.01f, activeTime));

            SetEffects(false, !letParticlesFinish);
            yield return new WaitForSeconds(Mathf.Max(0.01f, inactiveTime));
        }
    }

    private void SetEffects(bool on, bool clearParticles)
    {
        if (effects == null)
            return;

        foreach (ParticleSystem ps in effects)
        {
            if (ps == null)
                continue;

            if (on)
            {
                if (!ps.isPlaying)
                    ps.Play();
            }
            else
            {
                ps.Stop(true, clearParticles
                    ? ParticleSystemStopBehavior.StopEmittingAndClear
                    : ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }
}