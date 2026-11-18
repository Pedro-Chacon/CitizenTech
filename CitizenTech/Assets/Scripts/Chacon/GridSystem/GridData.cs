using System;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new();
    private bool hasBounds;
    private Vector3Int boundsMin;
    private Vector2Int boundsSize;

    // minCell = célula do canto inferior esquerdo (x, z). size = quantas células de largura e profundidade.
    public void SetBounds(Vector2Int minCell, Vector2Int size)
    {
        hasBounds = true;
        boundsMin = new Vector3Int(minCell.x, 0, minCell.y);
        boundsSize = size;
    }

    private bool IsInsideBounds(Vector3Int pos)
    {
        if (!hasBounds)
            return true;

        return pos.x >= boundsMin.x && pos.x < boundsMin.x + boundsSize.x
            && pos.z >= boundsMin.z && pos.z < boundsMin.z + boundsSize.y;
    }
    public void AddObjectAt(Vector3Int gridPosition,
        Vector2Int objectSize,
        int ID, int placedObjectIndex,
        int rotationIndex = 0)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex, gridPosition, rotationIndex);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                throw new Exception($"Dictionary already contains this cell position {pos}");
            placedObjects[pos] = data;
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach (var pos in positionToOccupy)
        {
            if (!IsInsideBounds(pos))
                return false;

            if (placedObjects.ContainsKey(pos))
                return false;
        }
        return true;
    }

    internal int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition) == false)
            return -1;
        return placedObjects[gridPosition].PlacedObjectIndex;
    }

    internal PlacementData GetPlacementData(Vector3Int gridPosition)
    {
        return placedObjects.TryGetValue(gridPosition, out PlacementData data) ? data : null;
    }

    // Acha o registro de um objeto pelo índice dele no ObjectPlacer.
    internal PlacementData FindByPlacedIndex(int placedObjectIndex)
    {
        foreach (PlacementData data in placedObjects.Values)
        {
            if (data.PlacedObjectIndex == placedObjectIndex)
                return data;
        }
        return null;
    }

    internal void RemoveObjectAt(Vector3Int gridPosition)
    {
        foreach (var pos in placedObjects[gridPosition].occupiedPositions)
        {
            placedObjects.Remove(pos);
        }
    }
}

public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }

    // Célula do canto (menor X e Z) e rotação (0 a 3, passos de 90°) com que o objeto foi colocado.
    public Vector3Int Anchor { get; private set; }
    public int Rotation { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex,
                         Vector3Int anchor, int rotation)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
        Anchor = anchor;
        Rotation = rotation;
    }
}