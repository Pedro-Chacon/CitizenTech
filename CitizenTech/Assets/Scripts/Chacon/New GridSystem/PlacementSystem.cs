
using System.Collections.Generic;
using UnityEngine;
using static JuicyButton;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator, cellIndicator;
    [SerializeField] private InputManager inputManager;
    [SerializeField] Grid grid;

    [SerializeField] private ObjectsDatabaseSO database;
    private int selectedObjectIndex = -1;

    [SerializeField] private GameObject gridVisualization;

    private GridData floorData, furnitureData;

    private Renderer previewRenderer;

    private List<GameObject> placedGameObjects = new();

     void Start()
    {
        StopPlacement();
        floorData = new();
        furnitureData = new();
        previewRenderer = cellIndicator.GetComponentInChildren<Renderer>();

    }
    public void StartPlacement(int ID)
    {
        StopPlacement();
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if(selectedObjectIndex < 0)
        {
            Debug.LogError($"Nenhum ID encontrado {ID}");
            return;
        }
        gridVisualization.SetActive(true);
        cellIndicator.SetActive(true);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
 
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPonterOverUI())
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (placementValidity == false)
            return;

        AudioManager.instance.ObjectPlacementSFX(); 
        GameObject newObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
        newObject.transform.position = grid.CellToWorld(gridPosition);
        placedGameObjects.Add(newObject);
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ?
             floorData :
             furnitureData;
        selectedData.AddObjectAt(gridPosition,
            database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID,
            placedGameObjects.Count - 1);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ?
            floorData :
            furnitureData;

        return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }

    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        cellIndicator.SetActive(false);
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
    }

    private void Update()
    {
        if (selectedObjectIndex < 0)
            return;

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        previewRenderer.material.color = placementValidity ? Color.white : Color.red;

        mouseIndicator.transform.position = mousePosition;
        cellIndicator.transform.position = grid.CellToWorld(gridPosition);
    }


}
