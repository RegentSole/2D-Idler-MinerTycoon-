#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class EditorGlobalEvents
{
    static EditorGlobalEvents()
    {
        // Вызывается при загрузке проекта
        Debug.Log("Project loaded in editor");

        // Подписываемся на события редактора
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
        EditorApplication.projectChanged += OnProjectChanged;
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
        Selection.selectionChanged += OnSelectionChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        // Срабатывает при изменении режима игры
        if (state == PlayModeStateChange.ExitingEditMode)
            Debug.Log("Entering Play Mode");
        else if (state == PlayModeStateChange.EnteredEditMode)
            Debug.Log("Returned to Edit Mode");
    }

    private static void OnProjectChanged()
    {
        // При изменении файлов проекта
        Debug.Log("Project assets changed");
    }

    private static void OnHierarchyChanged()
    {
        // При изменении иерархии объектов
        Debug.Log("Hierarchy changed");
    }

    private static void OnSelectionChanged()
    {
        // При изменении выделения
        Debug.Log($"Selected: {Selection.activeGameObject?.name}");
    }
}
#endif