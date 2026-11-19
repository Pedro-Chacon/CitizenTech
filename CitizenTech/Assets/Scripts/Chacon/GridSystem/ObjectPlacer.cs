using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> placedGameObjects = new(); 

    public int PlaceObject(GameObject prefab, Vector3 position)
        => PlaceObject(prefab, position, Quaternion.identity);

    public int PlaceObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject newObject = Instantiate(prefab, position, rotation);
        placedGameObjects.Add(newObject);
        return placedGameObjects.Count - 1;
    }

    // Registra um objeto que já estava na cena (silo, casa, cavalo). Devolve o índice dele.
    public int Register(GameObject existingObject)
    {
        placedGameObjects.Add(existingObject);
        return placedGameObjects.Count - 1;
    }

    public int GetIndexOf(GameObject placedObject)
    {
        return placedGameObjects.IndexOf(placedObject);
    }

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if (placedGameObjects.Count <= gameObjectIndex
            || placedGameObjects[gameObjectIndex] == null)
            return;
        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
    }
}