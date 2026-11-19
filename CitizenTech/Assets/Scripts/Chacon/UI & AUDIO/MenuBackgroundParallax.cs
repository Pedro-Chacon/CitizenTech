
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class MenuBackgroundParallax : MonoBehaviour
{
    [Header("Configurações do Parallax")]

    [SerializeField]
    private Vector2 maxOffset = new Vector2(30f, 20f);

    [SerializeField]
    private float smoothSpeed = 5f;

    private RectTransform rectTransform;
    private Vector2 initialPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        initialPosition = rectTransform.anchoredPosition;
    }

    private void LateUpdate()
    {
        // Posição normalizada do mouse
        Vector2 mousePosition = new Vector2(
            Input.mousePosition.x / Screen.width,
            Input.mousePosition.y / Screen.height
        );

        // Converte a posição para valores entre -1 e 1
        Vector2 direction = (mousePosition - new Vector2(0.5f, 0.5f)) * 2f;

        // Calcula o deslocamento desejado
        Vector2 targetPosition = initialPosition + Vector2.Scale(direction, maxOffset);

        // Movimento suave
        float smooth = 1f - Mathf.Exp(-smoothSpeed * Time.unscaledDeltaTime);

        rectTransform.anchoredPosition = Vector2.Lerp(
            rectTransform.anchoredPosition,
            targetPosition,
            smooth
        );
    }

    private void OnDisable()
    {
        if (rectTransform != null)
            rectTransform.anchoredPosition = initialPosition;
    }
}
