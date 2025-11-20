using UnityEngine;
using UnityEditor;
using System.Linq;

public class SetSpikeTriggers : MonoBehaviour
{
    //[MenuItem("Tools/Set All Spikes Child Colliders To Trigger (Scene + Prefabs)")]
    //private static void SetAllSpikes()
    //{
    //    int total = 0;

    //    // --- Szene durchsuchen ---
    //    GameObject[] sceneSpikes = GameObject.FindGameObjectsWithTag("Spikes");
    //    foreach (GameObject spike in sceneSpikes)
    //    {
    //        total += SetColliders(spike);
    //    }

    //    // --- Prefabs im Projekt durchsuchen ---
    //    string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab");

    //    foreach (string guid in prefabGUIDs)
    //    {
    //        string path = AssetDatabase.GUIDToAssetPath(guid);
    //        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

    //        if (prefab == null) continue;

    //        // Prefab-Root hat Tag "Spikes" → behandeln
    //        bool prefabIsSpikes = prefab.CompareTag("Spikes");

    //        // Oder ein Child hat Tag "Spikes"
    //        GameObject[] children = prefab.GetComponentsInChildren<Transform>(true)
    //                                      .Select(t => t.gameObject)
    //                                      .ToArray();

    //        bool hasSpikeChild = children.Any(go => go.CompareTag("Spikes"));

    //        if (prefabIsSpikes || hasSpikeChild)
    //        {
    //            // Prefab bearbeiten
    //            GameObject prefabRoot = PrefabUtility.LoadPrefabContents(path);

    //            // Alle Spikes-Objekte darin holen
    //            var spikeObjects = prefabRoot
    //                .GetComponentsInChildren<Transform>(true)
    //                .Where(t => t.CompareTag("Spikes"))
    //                .Select(t => t.gameObject);

    //            foreach (var spikeObj in spikeObjects)
    //                total += SetColliders(spikeObj);

    //            // Änderungen speichern
    //            PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
    //            PrefabUtility.UnloadPrefabContents(prefabRoot);
    //        }
    //    }

    //    Debug.Log($"FERTIG! Insgesamt {total} Collider2D wurden auf Trigger gesetzt (Szene + Prefabs).");
    //}

    //private static int SetColliders(GameObject root)
    //{
    //    int count = 0;
    //    Collider2D[] colliders = root.GetComponentsInChildren<Collider2D>(true);

    //    foreach (var col in colliders)
    //    {
    //        Undo.RecordObject(col, "Set Collider IsTrigger");
    //        col.isTrigger = true;
    //        EditorUtility.SetDirty(col);
    //        count++;
    //    }
    //    return count;
    //}
}
