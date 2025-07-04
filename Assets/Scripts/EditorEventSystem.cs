using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EditorEventSystem : MonoBehaviour
{
    // Событие только для редактора
    public event System.Action OnEditorUpdate;

#if UNITY_EDITOR
    private void OnEnable()
    {
        // Подписываемся на обновление сцены
        EditorApplication.update += EditorUpdate;
    }

    private void OnDisable()
    {
        // Отписываемся при отключении компонента
        EditorApplication.update -= EditorUpdate;
    }

    private void EditorUpdate()
    {
        // Вызывается каждый кадр в редакторе
        OnEditorUpdate?.Invoke();

        // Пример: проверка изменений
        if (transform.hasChanged)
        {
            Debug.Log("Object moved in editor");
            transform.hasChanged = false;
        }
    }
#endif
}