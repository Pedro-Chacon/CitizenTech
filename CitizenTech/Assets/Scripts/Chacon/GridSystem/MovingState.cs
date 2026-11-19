using System;
using UnityEngine;

public class MovingState : IBuildingState
{
    private readonly Installation installation;
    private readonly Grid grid;
    private readonly PreviewSystem previewSystem;
    private readonly Action onFinished;

    private GridData data;
    private Vector3Int originalAnchor;
    private int originalRotation;
    private int rotation;
    private Vector2Int baseSize;
    private int id;
    private int placedIndex;
    private bool moved;

    private Vector2Int RotatedSize => GridRotation.RotatedSize(baseSize, rotation);

    // Se for falso, o estado não foi iniciado e o PlacementSystem deve descartá-lo.
    public bool IsValid { get; private set; }

    public MovingState(Installation installation,
                       Grid grid,
                       PreviewSystem previewSystem,
                       ObjectsDatabaseSO database,
                       GridData floorData,
                       GridData furnitureData,
                       ObjectPlacer objectPlacer,
                       Action onFinished)
    {
        this.installation = installation;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.onFinished = onFinished;

        // Acha o registro da instalação pelo índice dela no ObjectPlacer.
        placedIndex = objectPlacer.GetIndexOf(installation.gameObject);
        PlacementData placement = null;
        if (placedIndex >= 0)
        {
            data = furnitureData;
            placement = furnitureData.FindByPlacedIndex(placedIndex);
            if (placement == null)
            {
                data = floorData;
                placement = floorData.FindByPlacedIndex(placedIndex);
            }
        }

        if (placement == null)
        {
            Debug.LogError($"[MovingState] {installation.name} não está registrada na grid.");
            return;
        }

        ObjectData entry = database.objectsData.Find(d => d.ID == placement.ID);
        if (entry == null)
        {
            Debug.LogError($"[MovingState] Nenhum objeto com ID {placement.ID} no database.");
            return;
        }

        id = placement.ID;
        baseSize = entry.Size;
        originalAnchor = placement.Anchor;
        originalRotation = placement.Rotation;
        rotation = originalRotation;

        // Libera as células (pra poder recolocar no mesmo lugar) e troca o objeto real pelo fantasma.
        data.RemoveObjectAt(originalAnchor);
        installation.gameObject.SetActive(false);

        previewSystem.StartShowingPlacementPreview(entry.Prefab, baseSize, installation);
        previewSystem.SetRotation(rotation, baseSize, grid.cellSize);
        previewSystem.UpdatePosition(grid.CellToWorld(originalAnchor), true);

        IsValid = true;
    }

    public void OnAction(Vector3Int gridPosition)
    {
        if (!data.CanPlaceObjectAt(gridPosition, RotatedSize))
        {
            // TODO: som de posição inválida
            return;
        }

        installation.transform.SetPositionAndRotation(
            grid.CellToWorld(gridPosition) + GridRotation.PivotOffset(baseSize, rotation, grid.cellSize),
            GridRotation.Rotation(rotation));
        installation.gameObject.SetActive(true);
        data.AddObjectAt(gridPosition, RotatedSize, id, placedIndex, rotation);
        moved = true;

        if (AudioManager.instance != null)
        {
              AudioManager.instance.ObjectPlacementSFX();
        }
        onFinished?.Invoke(); // o PlacementSystem encerra o estado (e chama EndState)
    }

    public void Rotate()
    {
        rotation = GridRotation.Next(rotation);
        previewSystem.SetRotation(rotation, baseSize, grid.cellSize);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity = data.CanPlaceObjectAt(gridPosition, RotatedSize);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), validity);
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();

        // Cancelou sem mover: o objeto nunca saiu do lugar, só reaparece e reocupa as células de origem.
        if (!moved && IsValid && installation != null)
        {
            installation.gameObject.SetActive(true);
            data.AddObjectAt(originalAnchor,
                GridRotation.RotatedSize(baseSize, originalRotation),
                id, placedIndex, originalRotation);
        }
    }
}