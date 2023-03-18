using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class PrefabLoader : MonoBehaviour {
  public bool hasBeenInitialized = false;
  // Define a public field to hold the prefab object.
  public GameObject prefabObject;
  // Reference to the instance of the prefab.
  private GameObject prefabInstance;

  // Get the bounds of the current object.
  Bounds GetBounds() { return GetComponent<Renderer>().bounds; }

  // Calculate the size of the bounds of the current object.
  Vector3 GetSize() {
    Vector3 size = GetBounds().size;

    // Print the size of the bounds.
    // Debug.Log("Size: " + size);
    return size;
  }

  void OnDestroy() {
    if (prefabObject != null && hasBeenInitialized) {
      DestroyImmediate(prefabInstance);
    }
  }

  // Called when game object is instantiated in the editor.
  private void Start() {
    InvokeRepeating("UpdatePerMinute", 2.0f, 60.0f);
    InvokeRepeating("UpdatePerSecond", 1.0f, 1.0f);
    // Check if the prefab object has been assigned.
    if (prefabObject != null && !hasBeenInitialized) {
      // Instantiate the prefab object in the scene using the InstantiatePrefab
      // method.
      var prefabInstance =
          PrefabUtility.InstantiatePrefab(prefabObject) as GameObject;
      if (prefabInstance != null) {
        // Set the prefab instance's position to match the position of this game
        // object.
        // prefabInstance.transform.position = new Vector3(0, 0, 0); //
        // transform.position;
        // Set current game object as a parent.
        prefabInstance.transform.SetParent(transform, false);
        // Gets bounds.
        Bounds bounds = GetBounds();
        // Position the prefab instance at the center of the plane.
        prefabInstance.transform.position = bounds.center;
        // Scale the prefab instance to fit within the plane.
        Vector3 scale = prefabInstance.transform.localScale;
        float scaleFactor =
            Mathf.Min(bounds.size.x / scale.x, bounds.size.y / scale.y,
                      bounds.size.z / scale.z);
        prefabInstance.transform.localScale = scaleFactor * scale;
        // Set that already has been initialized.
        hasBeenInitialized = true;
      }
    }
  }

  // Called once per frame.
  private void Update() {}

  // Update is called once per minute.
  void UpdatePerMinute() {}

  // Update is called once per second.
  void UpdatePerSecond() {}
}