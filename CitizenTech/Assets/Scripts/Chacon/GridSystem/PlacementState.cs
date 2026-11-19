using UnityEngine;
using System;
public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    int ID;
    Grid grid;
    PreviewSystem previewSystem;
    ObjectsDatabaseSO database;
    GridData floorData;
    GridData furnitureData;
    ObjectPlacer objectPlacer;

    private int rotation;

    private Vector2Int BaseSize => database.objectsData[selectedObjectIndex].Size;
    private Vector2Int RotatedSize => GridRotation.RotatedSize(BaseSize, rotation);

    private readonly Action onFinished;
    private readonly int cost;

    public PlacementState(int iD,
                          Grid grid,
                          PreviewSystem previewSystem,
                          ObjectsDatabaseSO database,
                          GridData floorData,
                          GridData furnitureData,
                          ObjectPlacer objectPlacer,
                          Action onFinished = null,
                          int cost = 0)
    {
        this.onFinished = onFinished;
        this.cost = cost;

        ID = iD;
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.database = database;
        this.floorData = floorData;
        this.furnitureData = furnitureData;
        this.objectPlacer = objectPlacer;

        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex > -1)
        {
            previewSystem.StartShowingPlacementPreview(
                database.objectsData[selectedObjectIndex].Prefab,
                BaseSize);
            previewSystem.SetRotation(rotation, BaseSize, grid.cellSize);
        }
        else
        {
            throw new System.Exception($"No object with ID {iD}");
        }
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (placementValidity == false)
        {
            // INSERIR SOM WRONG PLACEMENT
            return;
        }
        if (cost > 0 && (EconomyManager.instance == null || !EconomyManager.instance.TrySpend(cost)))
        {
            // TODO: som de "sem dinheiro"
            return;
        }

        if (AudioManager.instance != null)
        {
              AudioManager.instance.ObjectPlacementSFX(); // INSERIR PLACE OBJECT
        }

        Vector3 position = grid.CellToWorld(gridPosition)
                           + GridRotation.PivotOffset(BaseSize, rotation, grid.cellSize);

        int index = objectPlacer.PlaceObject(
            database.objectsData[selectedObjectIndex].Prefab,
            position,
            GridRotation.Rotation(rotation));

        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ?
             floorData :
             furnitureData;
        selectedData.AddObjectAt(gridPosition,
            RotatedSize,
            database.objectsData[selectedObjectIndex].ID,
            index,
            rotation);
        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), false);

        onFinished?.Invoke(); // compra: um clique = uma instalação, e o estado encerra
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ?
            floorData :
            furnitureData;

        return selectedData.CanPlaceObjectAt(gridPosition, RotatedSize);
    }

    public void Rotate()
    {
        rotation = GridRotation.Next(rotation);
        previewSystem.SetRotation(rotation, BaseSize, grid.cellSize);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);

        previewSystem.UpdatePosition(grid.CellToWorld(gridPosition), placementValidity);
       
    }

}