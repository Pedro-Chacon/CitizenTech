using UnityEngine;

public class DronePatrol : MonoBehaviour
{
    [Header("Pontos (dois objetos vazios)")]
    [Tooltip("Têm que ser FILHOS do prefab da plantação (irmãos do drone), e NÃO filhos do drone.")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Movimento")]
    [SerializeField] private float speed = 1.5f;
    [Tooltip("Segundos parado em cada ponta antes de voltar.")]
    [SerializeField] private float pauseAtEnds = 0.5f;

    [Header("Rotação do modelo")]
    [Tooltip("Vira o drone para a direção em que ele está indo.")]
    [SerializeField] private bool faceDirection = true;
    [SerializeField] private float turnSpeed = 8f;
    [Tooltip("Ajuste se o modelo 3D não 'olha' para o eixo Z (ex.: 90 ou -90 no Y).")]
    [SerializeField] private Vector3 modelRotationOffset = Vector3.zero;

    [Header("Flutuação (opcional)")]
    [SerializeField] private float bobAmplitude = 0.05f;
    [SerializeField] private float bobFrequency = 2f;

    private bool goingToB = true;
    private float pauseTimer;
    private Vector3 basePosition; 
    private float bobTime;

    private Transform Parent => transform.parent;

    private void OnEnable()
    {
      
        if (pointA == null || pointB == null)
            return;

        goingToB = true;
        pauseTimer = 0f;
        basePosition = LocalPointPosition(pointA);
        transform.localPosition = basePosition;
    }

    private void Update()
    {
        if (pointA == null || pointB == null)
            return;

        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.deltaTime;
            ApplyBob();
            return;
        }

        Vector3 target = LocalPointPosition(goingToB ? pointB : pointA);
        Vector3 toTarget = target - basePosition;

        if (faceDirection)
            FaceLocalDirection(toTarget);

        basePosition = Vector3.MoveTowards(basePosition, target, speed * Time.deltaTime);

        if ((target - basePosition).sqrMagnitude < 0.0001f)
        {
            basePosition = target;
            goingToB = !goingToB;
            pauseTimer = pauseAtEnds;
        }

        ApplyBob();
    }


    private Vector3 LocalPointPosition(Transform point)
    {
        return Parent != null ? Parent.InverseTransformPoint(point.position) : point.position;
    }

    private void ApplyBob()
    {
        bobTime += Time.deltaTime;
        float bob = Mathf.Sin(bobTime * bobFrequency) * bobAmplitude;
        transform.localPosition = basePosition + Vector3.up * bob;
    }

    private void FaceLocalDirection(Vector3 localDir)
    {
        localDir.y = 0f; 
        if (localDir.sqrMagnitude < 0.0001f)
            return;

        Quaternion look = Quaternion.LookRotation(localDir.normalized, Vector3.up)
                          * Quaternion.Euler(modelRotationOffset);

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation, look, 1f - Mathf.Exp(-turnSpeed * Time.deltaTime));
    }

    private void OnDrawGizmosSelected()
    {
        if (pointA == null || pointB == null)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(pointA.position, pointB.position);
        Gizmos.DrawWireSphere(pointA.position, 0.1f);
        Gizmos.DrawWireSphere(pointB.position, 0.1f);
    }
}