using UnityEngine;

public class RandomRotationOnStart : MonoBehaviour
{
    [Tooltip("Marque para girar também em X e Z. Desmarcado, gira só no eixo Y (o normal para árvores, arbustos, pedras...).")]
    [SerializeField] private bool randomizeAllAxes = false;

    private void Start()
    {
        if (randomizeAllAxes)
        {
            transform.rotation = Random.rotation;
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        }
    }
}