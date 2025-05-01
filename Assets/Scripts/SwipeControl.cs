using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class SwipeControl : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;

    private Vector2 startPosition;
    private float startTime;
    private Vector2 endPosition;
    private float endTime;

    [SerializeField]
    private float minimumDistance =.2f;
    [SerializeField]
    private float maxTime = 1f;
    [SerializeField]
    private float directionalThreshold = 0.9f;
    [SerializeField]
    private GameObject trail;
    [SerializeField]
    private GameObject ball;
    [SerializeField]
    private GameObject player;
    private Coroutine coroutine;

    private void OnEnable()
    {
        inputManager.OnStartTouch += SwipeStart;
        inputManager.OnEndTouch += SwipeEnd;
    }
    private void OnDisable()
    {

        inputManager.OnStartTouch -= SwipeStart;
        inputManager.OnEndTouch -= SwipeEnd;
    }
    
    private void SwipeStart(Vector2 position, float time)
    {
        
        startPosition = position;
        startTime = time;
        trail.SetActive(true);
        trail.transform.position = position;
        coroutine =  StartCoroutine(Trail());
    }
    private IEnumerator Trail()
    {
        while (true)
        {
            trail.transform.position = inputManager.PrimaryPosition(1f);
            yield return null;
        }
        
    }
    private void SwipeEnd(Vector2 position, float time)
    {
        trail.SetActive(false);
        StopCoroutine(coroutine);
        endPosition = position;
        endTime = time;
        DetectSwipe();
    }

    private void DetectSwipe()
    {
        if (Vector2.Distance(startPosition, endPosition) >= minimumDistance && (endTime - startTime) <= maxTime)
        {
            Debug.DrawLine(startPosition, endPosition, Color.red, 5f);
           Vector3 direction = endPosition - startPosition;
           Vector2 direction2D = new Vector2(direction.x, direction.y).normalized;
         //  Vector2 middlePosition = (startPosition + endPosition) / 2f + Vector2.up * 100f;

            //making it endPosition & middlePosition Vector3
            Vector3 endPoint = new Vector3(endPosition.x, endPosition.y, 0);
            Vector3 midPoint = new Vector3(endPoint.x, endPoint.y, 0);

            SwipeDirection(direction2D);

           DrawQuadraticBezierPoint(startPosition, midPoint, endPoint);
            //Debug.Log(direction+"direction");
        }
    }
    public void DrawQuadraticBezierPoint(Vector3 start, Vector3 middle, Vector3 end)
    {

        List<Vector3> path = new List<Vector3>();
        int noOfPoints = 20;

        float distance2D = Vector2.Distance(start, end);
        GameObject playerBall = Instantiate(ball);

        end.z = (end.z * 10 * distance2D);//making it endPosition extent to z * multiple of the swipe distance
        Debug.Log(end);
        middle = (start + end) /2f + Vector3.up * distance2D;
        for (int i = 0; i <= noOfPoints; i++)
        {
            float t = i / (float)noOfPoints;

            //Vector3 point
            path.Add(CalculateQuadraticBezierPoint(start, middle, end, t));

            //point.z = distance2D * (4 * t * (1 - t)); // nice z curve
            //path.Add(point);

        }
        StartCoroutine(MoveAlongPath(playerBall, path, 1.0f));
    }
   public IEnumerator MoveAlongPath(GameObject ball, List<Vector3> path, float duration)
    {
        float stepTime = duration / (path.Count - 1);
        for (int i = 0; i < path.Count; i++)
        {
            ball.transform.position = path[i];
            yield return new WaitForSeconds(stepTime);
        }
    }

    private Vector3 CalculateQuadraticBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        //B(t) = (1 - t)²P0  +   2(1 - t)tP1        + t²P2 
        //          u               u                 tt
        //          uu * p0  +   u * 2 * t * p1     + tt * p2
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 point = uu * p0;
        point += u * 2 * t * p1;
        point += tt * p2;
        //point.z = point.z * t; //adding the z axis for depth
        return point;
    }

    private void SwipeDirection(Vector2 direction)
    {

        if(Vector3.Dot(Vector3.up, direction) > directionalThreshold)
        {
            Debug.Log("swipedUp");
        }
        else if(Vector3.Dot(Vector3.down, direction) > directionalThreshold)
        {
            Debug.Log("swipedDown");
        }
        else if(Vector3.Dot(Vector3.left, direction) > directionalThreshold)
        {
            Debug.Log("swipedLeft");
        }
        else if(Vector3.Dot(Vector3.right, direction) > directionalThreshold)
        {
            Debug.Log("swipedRight");
        }
    }
   
    
}

