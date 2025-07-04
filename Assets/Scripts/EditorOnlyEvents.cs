using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EditorOnlyEvents : MonoBehaviour
{
#if UNITY_EDITOR
    private void OnValidate()
    {
        // Вызывается при изменении значений в инспекторе
        Debug.Log("Value changed in inspector for: " + gameObject.name);
        // Убрали вызов несуществующего метода EditorEvent()
    }

    private void Reset()
    {
        // Вызывается при добавлении компонента на объект
        Debug.Log("Component added to GameObject: " + gameObject.name);
    }

    private void OnDrawGizmos()
    {
        // Вызывается каждый кадр для отрисовки гизмо
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Вызывается при выделении объекта
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
#endif
}