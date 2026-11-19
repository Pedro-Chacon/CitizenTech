using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Toca o som de "negado" quando o jogador clica num botão que JÁ estava bloqueado.
[RequireComponent(typeof(Button))]
public class BlockedClickSound : MonoBehaviour, IPointerDownHandler, IPointerClickHandler
{
    private Button button;
    private bool wasInteractableOnPress;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    // Guarda se o botão estava liberado no instante em que o mouse apertou.
    // Um clique válido pode bloquear o próprio botão (ex.: gastar o último trigo, passar o dia),
    // e isso não é um clique negado.
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        wasInteractableOnPress = button.interactable;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (!wasInteractableOnPress && AudioManager.instance != null)
            AudioManager.instance.PlayDeniedSFX();
    }
}