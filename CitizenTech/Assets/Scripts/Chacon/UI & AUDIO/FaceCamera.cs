using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform cam;

    private void Start()
    {
        if (Camera.main != null)
            cam = Camera.main.transform;
    }

    private void LateUpdate()
    {
        // Câmera isométrica fixa: copiar a rotação dela deixa o canvas paralelo à tela, sem distorcer.
        if (cam != null)
            transform.rotation = cam.rotation;
    }
}