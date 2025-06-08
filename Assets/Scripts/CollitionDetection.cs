using System;
using Unity.VisualScripting;
using UnityEngine;

public class CollitionDetection : MonoBehaviour
{
    public CourtZoneType zoneType;
    public static event Action<CourtZoneType, Collision> OnZoneHit;
    public bool hasCollided = false;
    public GameObject trailBallPrefab;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == ("Ball"))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                Vector3 position = contact.point;
                //GameObject trailBallPrefab = GameObject.Find("BallTrail");
               /* Instantiate(trailBallPrefab, position, Quaternion.identity);
                trailBallPrefab.GetComponent<TrailRenderer>().endColor = Color.green;*/
                Debug.Log(position);
            }
            if (!hasCollided)
            {
                //Debug.Log($"{gameObject.name} has zoneType: {zoneType}");
                OnZoneHit?.Invoke(zoneType, collision);
                
                hasCollided = true;
            }
        }
    }
}
