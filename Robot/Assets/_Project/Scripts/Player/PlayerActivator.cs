using System.Collections.Generic;
using UnityEngine;

public class PlayerActivator : MonoBehaviour
{
   [Header("Configuración")]
    public float radius = 15f;
    public LayerMask targetLayers; // Capas de los objetos a gestionar (ej. "Activable")

    private List<GameObject> managedObjects = new List<GameObject>();

    void Start()
    {
        // Buscar TODOS los objetos con las capas especificadas al inicio
        Collider[] allColliders = FindObjectsOfType<Collider>();
        foreach (Collider col in allColliders)
        {
            if (col != null && ((1 << col.gameObject.layer) & targetLayers) != 0)
            {
                managedObjects.Add(col.gameObject);
            }
        }

        Debug.Log($"Objetos gestionados encontrados: {managedObjects.Count}");

        // Aplicar estado inicial (desactivar todo lo que esté fuera del radio)
        UpdateAllObjects();
    }

    void Update()
    {
        // Actualizar estado de todos los objetos gestionados cada frame
        UpdateAllObjects();
    }

    void UpdateAllObjects()
    {
        Vector3 playerPos = transform.position;
        
        for (int i = managedObjects.Count - 1; i >= 0; i--)
        {
            GameObject obj = managedObjects[i];
            
            // Si el objeto fue destruido, quitarlo de la lista
            if (obj == null)
            {
                managedObjects.RemoveAt(i);
                continue;
            }

            float distance = Vector3.Distance(playerPos, obj.transform.position);
            bool shouldBeActive = distance <= radius;

            // Activar o desactivar solo si es necesario
            if (obj.activeSelf != shouldBeActive)
            {
                obj.SetActive(shouldBeActive);
            }
        }
    }

    // Método público para registrar objetos creados dinámicamente
    public void RegisterObject(GameObject obj)
    {
        if (obj == null || managedObjects.Contains(obj)) return;
        
        managedObjects.Add(obj);
        
        // Activar/desactivar según distancia actual
        float distance = Vector3.Distance(transform.position, obj.transform.position);
        obj.SetActive(distance <= radius);
    }

    // Método público para eliminar objetos de la lista
    public void UnregisterObject(GameObject obj)
    {
        if (managedObjects.Contains(obj))
        {
            managedObjects.Remove(obj);
        }
    }

    // Visualización en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.15f);
        Gizmos.DrawSphere(transform.position, radius);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
