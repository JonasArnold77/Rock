using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;

public class ReplaceDefaultSpriteMaterial : EditorWindow
{
    private Material newMaterial;
    [SerializeField] private List<GameObject> selectedObjects = new List<GameObject>();
    private ReorderableList reorderableList;
    private Vector2 scrollPosition; // Scroll-Position speichern

    [MenuItem("Tools/Replace Default Sprite Material in Children")]
    public static void ShowWindow()
    {
        GetWindow<ReplaceDefaultSpriteMaterial>("Replace Sprite Material");
    }

    private void OnEnable()
    {
        reorderableList = new ReorderableList(selectedObjects, typeof(GameObject), true, true, true, true)
        {
            drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Selected Objects");
            },
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                selectedObjects[index] = (GameObject)EditorGUI.ObjectField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    selectedObjects[index],
                    typeof(GameObject),
                    true);
            },
            onAddCallback = (ReorderableList list) =>
            {
                selectedObjects.Add(null);
            },
            onRemoveCallback = (ReorderableList list) =>
            {
                if (list.index >= 0) selectedObjects.RemoveAt(list.index);
            }
        };
    }

    private void OnGUI()
    {
        GUILayout.Label("Replace 'Sprite-Lit-Default' Material", EditorStyles.boldLabel);

        newMaterial = (Material)EditorGUILayout.ObjectField("New Material:", newMaterial, typeof(Material), false);

        GUILayout.Space(10);

        // 🎯 Scrollbereich hinzufügen
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200)); // Höhe begrenzen
        reorderableList.DoLayoutList();
        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);

        if (GUILayout.Button("Replace Material in Children"))
        {
            ReplaceMaterial();
        }

        // Unterstützt Drag & Drop für mehrere Objekte
        HandleDragAndDrop();
    }

    private void HandleDragAndDrop()
    {
        Event evt = Event.current;
        Rect dropArea = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drag & Drop Objects Here", EditorStyles.helpBox);

        if (evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
        {
            if (dropArea.Contains(evt.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    foreach (Object draggedObject in DragAndDrop.objectReferences)
                    {
                        if (draggedObject is GameObject gameObject)
                        {
                            selectedObjects.Add(gameObject);
                        }
                    }
                    evt.Use();
                }
            }
        }
    }

    private void ReplaceMaterial()
    {
        if (newMaterial == null)
        {
            Debug.LogError("Please assign a new material.");
            return;
        }

        int count = 0;

        foreach (GameObject obj in selectedObjects)
        {
            if (obj == null) continue;

            SpriteRenderer[] spriteRenderers = obj.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (SpriteRenderer sr in spriteRenderers)
            {
                if (sr.sharedMaterial != null && sr.sharedMaterial.name == "Sprite-Lit-Default")
                {
                    sr.material = newMaterial;
                    count++;
                }
            }
        }

        Debug.Log($"Replaced material for {count} Sprite Renderers.");
    }
}