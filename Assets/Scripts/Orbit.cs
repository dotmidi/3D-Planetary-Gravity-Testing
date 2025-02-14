using UnityEngine;

public class Orbit : MonoBehaviour
{
    [SerializeField] private Attractor attractor;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (!other.CompareTag("Player")) return;

        Attractable attractable = other.GetComponentInParent<Attractable>();
        if (!attractor.attractedObjects.Contains(attractable))
            attractor.attractedObjects.Add(attractable);
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Attractable attractable = other.GetComponentInParent<Attractable>();
        if (attractor.attractedObjects.Contains(attractable))
            attractor.attractedObjects.Remove(attractable);
    }
}