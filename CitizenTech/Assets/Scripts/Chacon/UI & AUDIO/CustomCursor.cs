
using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    private static CustomCursor instance;

    [Header("Mouse Cursor")]
    [SerializeField] private Texture2D handCursor;
    [SerializeField] private Vector2 hotSpot = Vector2.zero;

    private void Awake()
    {

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;


        DontDestroyOnLoad(gameObject);

        Cursor.SetCursor(handCursor, hotSpot, CursorMode.Auto);
    }
}
