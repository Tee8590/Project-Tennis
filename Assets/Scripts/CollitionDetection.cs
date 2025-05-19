using System;
using UnityEngine;

public class CollitionDetection : MonoBehaviour
{
    public CourtZoneType zoneType;
    public static event Action<CourtZoneType, Collision> OnZoneHit;
    public bool hasCollided = false;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            
            if (!hasCollided)
            {
                //Debug.Log($"{gameObject.name} has zoneType: {zoneType}");
                OnZoneHit?.Invoke(zoneType, collision);
                
                hasCollided = true;
            }
        }
    }
}
