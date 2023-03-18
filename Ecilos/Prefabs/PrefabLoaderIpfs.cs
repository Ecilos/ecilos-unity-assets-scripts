using System.Collections;
using UnityEditor;
using UnityEngine.Networking;
using UnityEngine;

[ExecuteInEditMode]
public class PrefabLoaderIpfs : MonoBehaviour {
  public bool hasBeenInitialized = false;
  public float timeout = 10f;
  // Define a public field to hold the prefab object.
  public string prefabObjectCID;
  // Reference to the instance of the prefab.
  private GameObject prefabInstance;
  private string ipfsApiUrl = "https://cloudflare-ipfs.com/ipfs/";

  // Get the bounds of the current object.
  Bounds GetBounds() { return GetComponent<Renderer>().bounds; }

  // Calculate the size of the bounds of the current object.
  Vector3 GetSize() {
    Vector3 size = GetBounds().size;

    // Print the size of the bounds.
    // Debug.Log("Size: " + size);
    return size;
  }

  IEnumerator LoadObjectViaIpfs(string ipfsCid) {
    using (UnityWebRequest request =
               UnityWebRequest.Get(ipfsApiUrl + ipfsCid)) {
      yield return request.SendWebRequest();

      if (request.result == UnityWebRequest.Result.ConnectionError ||
          request.result == UnityWebRequest.Result.ProtocolError) {
        Debug.LogError("REST request error: " + request.error);
      } else {
        // Parse the JSON data
        string jsonData = request.downloadHandler.text;
        ObjectData objectData = JsonUtility.FromJson<ObjectData>(jsonData);

        // Instantiate a new GameObject based on the object data
        GameObject newObject = new GameObject(objectData.name);
        newObject.transform.position = objectData.position;
        newObject.transform.rotation = objectData.rotation;
        newObject.transform.localScale = objectData.scale;
      }
      /*
        // Instantiate the prefab object in the scene using the
        InstantiatePrefab method. var prefabInstance =
            PrefabUtility.InstantiatePrefab(prefabObject) as GameObject;
        if (prefabInstance != null) {
          // Set the prefab instance's position to match the position of this
        game
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
      */
    }
  }

  void OnDestroy() {
    if (prefabObjectCID != null && hasBeenInitialized) {
      DestroyImmediate(prefabInstance);
    }
  }

  // Called when game object is instantiated in the editor.
  private void Start() {
    InvokeRepeating("UpdatePerMinute", 2.0f, 60.0f);
    InvokeRepeating("UpdatePerSecond", 1.0f, 1.0f);
    // Check if the prefab object has been assigned.
    if (prefabObjectCID.Length > 0 && !hasBeenInitialized) {
      StartCoroutine(LoadObjectViaIpfs(prefabObjectCID));
    }
  }

  // Called once per frame.
  private void Update() {}

  // Update is called once per minute.
  void UpdatePerMinute() {}

  // Update is called once per second.
  void UpdatePerSecond() {}
}

[System.Serializable]
public class ObjectData {
  public string name;
  public Vector3 position;
  public Quaternion rotation;
  public Vector3 scale;
}