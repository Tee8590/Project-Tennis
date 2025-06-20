using System;
using Unity.VisualScripting;
using UnityEngine;

public class CollitionDetection : MonoBehaviour
{
    public CourtZoneType zoneType;
    public static event Action<CourtZoneType, Collision> OnZoneHit;
    public static event Action<CourtZoneType, bool, string> LandingPoint;
    public bool hasCollided = false;
    public GameObject trailBallPrefab;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            if(!hasCollided) 
          {
                if (GameManager.Instance.GameStarted)
                {
                    //Debug.Log($"{gameObject.name} has zoneType: {zoneType}");
                    OnZoneHit?.Invoke(zoneType, collision);
                    GameManager.Instance.SetBallTouched(true);
                    hasCollided= true;
                }
            }
        }
       

    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("BallPos")) return;
        if (GameManager.Instance.hasCollidedFromColliders) return;
        //if (gameObject.CompareTag("CommonCort")) return;

        Debug.Log($"[Trigger] gameObject.name: {gameObject.name}, tag: {gameObject.tag}");
        Debug.Log($"[Trigger] other.name: {other.name}, other.tag: {other.tag}");
        if (gameObject.CompareTag("ServiceBox"))
        {
            //hasCollided = true;
            LandingPoint?.Invoke(zoneType, GameManager.Instance.hasCollidedFromColliders, this.gameObject.tag);
            Debug.Log("OnTriggerEnter Success..................." + GameManager.Instance.hasCollidedFromColliders);
            GameManager.Instance.hasCollidedFromColliders = true;
            Debug.Log("OnTriggerEnter Success..............." + GameManager.Instance.hasCollidedFromColliders);

        }
        else if (gameObject.CompareTag("CommonCort") && gameObject.CompareTag("ServiceBox"))
        {
            //if (!GameManager.Instance.hasCollidedFromColliders)
            {
                GameManager.Instance.hasCollidedFromColliders = true;
                LandingPoint?.Invoke(zoneType, false, this.gameObject.tag);
                Debug.Log("CommonCort triggered and handled.");
            }
        }
    }
   
}
