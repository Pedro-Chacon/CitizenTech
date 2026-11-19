
using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    [Header("Mouse Cursor")]
    [SerializeField] private Texture2D handCursor;

    [SerializeField] private Vector2 hotSpot = Vector2.zero;

    private void Start()
    {
        Cursor.SetCursor(
            handCursor,
            hotSpot,
            CursorMode.Auto
        );
    }
}
