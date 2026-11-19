using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class BlockedClickSound : MonoBehaviour, IPointerDownHandler, IPointerClickHandler
{
    private Button button;
    private bool wasInteractableOnPress;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

   
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