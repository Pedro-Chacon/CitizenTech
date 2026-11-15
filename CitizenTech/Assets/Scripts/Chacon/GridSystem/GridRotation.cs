using UnityEngine;

// Geometria da rotação em passos de 90°.
// O pivô dos prefabs fica no canto inferior esquerdo da área ocupada. Ao girar em volta dele,
// a área "escapa" das células; o offset abaixo traz o objeto de volta pra cima das células certas.
public static class GridRotation
{
    public static int Next(int rotationIndex) => (rotationIndex + 1) % 4;

    public static Quaternion Rotation(int rotationIndex)
        => Quaternion.Euler(0f, 90f * rotationIndex, 0f);

    // Em 90° e 270° a largura e a profundidade trocam de lugar.
    public static Vector2Int RotatedSize(Vector2Int size, int rotationIndex)
        => rotationIndex % 2 == 0 ? size : new Vector2Int(size.y, size.x);

    public static Vector3 PivotOffset(Vector2Int size, int rotationIndex, Vector3 cellSize)
    {
        float w = size.x * cellSize.x; // extensão do modelo em X
        float d = size.y * cellSize.z; // extensão do modelo em Z

        switch (rotationIndex % 4)
        {
            case 1: return new Vector3(0f, 0f, w);   // 90°
            case 2: return new Vector3(w, 0f, d);    // 180°
            case 3: return new Vector3(d, 0f, 0f);   // 270°
            default: return Vector3.zero;             // 0°
        }
    }
}