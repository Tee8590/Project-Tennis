using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwipeControl : MonoBehaviour
{
    [SerializeField]
    private InputManager inputManager;

    private Vector2 direction2D;
    private Vector2 startPosition;
    private float startTime;
    private Vector2 endPosition;
    private float endTime;
    private Rigidbody ballrb;
    private Rigidbody currentBallRb;
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
    private Coroutine slowBallCoroutine;
    private BallHitDetection ballHitDetection;
    private InputAction fireAction;
    [SerializeField]
    private Camera mainCamera;
    private void OnEnable()
    {
        inputManager.OnStartTouch += SwipeStart;
        inputManager.OnEndTouch += SwipeEnd;
        fireAction = inputManager.FireAction;
    }
    private void OnDisable()
    {

        inputManager.OnStartTouch -= SwipeStart;
        inputManager.OnEndTouch -= SwipeEnd;
    }
    private void Awake()
    {
        player.GetComponentInChildren<BoxCollider>();
       ballHitDetection =  player.GetComponentInChildren<BallHitDetection>();
    }
    private float ComputeZDepth(float planeY = 0f)
    {
        float camY = mainCamera.transform.position.y;
        float forwardY = mainCamera.transform.forward.y;
        // Avoid division by zero
        if (Mathf.Approximately(forwardY, 0f))
            forwardY = 0.0001f;
        return (camY - planeY) / forwardY;
    }
    private void SwipeStart(Vector2 position, float time)
    {/*
        float zDepth = ComputeZDepth(0f);
        startPosition = Utils.ScreenToWorld(
            mainCamera,
            new Vector3(position.x, position.y, zDepth)
        );*/
        startPosition = position;
        startTime = time;
        trail.SetActive(true);
        trail.transform.position = position;
        coroutine =  StartCoroutine(Trail());
    }
    private void Update()
    {
     
         if (fireAction != null && fireAction.WasPressedThisFrame())
        {
            //SimulateSwipeRHS();
            OpponentBallDebug();
        }
    }
    private IEnumerator Trail()
    {
        while (true)
        {
            trail.transform.position = inputManager.PrimaryPosition(10f);
            yield return null;
        }
        
    }
    private void SwipeEnd(Vector2 position, float time)
    {
      /*  float zDepth = ComputeZDepth(0f);
        endPosition = Utils.ScreenToWorld(
            mainCamera,
            new Vector3(position.x, position.y, zDepth)
        );*/

        trail.SetActive(false);
        StopCoroutine(coroutine);
        endPosition = position;
        endTime = time;
        DetectSwipe();
    }

    public void DetectSwipe()
    {
        if (Vector2.Distance(startPosition, endPosition) >= minimumDistance && (endTime - startTime) <= maxTime)
        {
        //    Debug.Log("Start :"+startPosition+", "+"End :"+endPosition);
        //    Debug.Log("TIme"+(endTime - startTime));
            //Debug.DrawLine(startPosition, endPosition, Color.red, 5f);

            //direction of the ball in 2D, z is 0 currently
           Vector3 direction = endPosition - startPosition;
           direction2D = new Vector2(direction.x, direction.y).normalized;
         //  Vector2 middlePosition = (startPosition + endPosition) / 2f + Vector2.up * 100f;

            //making it endPosition & middlePosition Vector3
            Vector3 endPoint = new Vector3(endPosition.x, endPosition.y, 0);
            Vector3 midPoint = new Vector3(0,0,0);
            SwipeDirection(direction2D);

            CreateBall();

            BallMovement(currentBallRb, direction);
            
            //DrawQuadraticBezierPoint(player.transform.position, midPoint, endPoint);

        }
    }

    public void SimulateSwipeLHS()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        Vector2 simStartSwipe = new Vector2(Random.Range(293, 296.5f), Random.Range(0.5f, .75f));
        Vector2 simEndSwipe   = new Vector2(Random.Range(293, 296.5f), Random.Range(0.5f, .75f));
        float simSwipeLength = Random.Range(0.12f, 0.15f);

        SwipeStart(simStartSwipe, simSwipeLength);
        SwipeEnd(simEndSwipe, simSwipeLength);

    }
    public void SimulateSwipeRHS()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        Vector2 simStartSwipe = new Vector2(Random.Range(297.5f, 300), Random.Range(0.8f, 1f));
        Vector2 simEndSwipe   = new Vector2(Random.Range(297.5f, 300), Random.Range(0.8f, 1f));
        float simSwipeLength = Random.Range(0.12f, 0.15f);

        SwipeStart(simStartSwipe, simSwipeLength);
        SwipeEnd(simEndSwipe, simSwipeLength);

    }
    //public void SimulateSwipeRHS()
    //{
    //    Vector2 simStartSwipe = new Vector2(Random.Range(293, 300), Random.Range(0.5f, 1f));
    //    Vector2 simEndSwipe   = new Vector2(Random.Range(293, 300), Random.Range(0.5f, 1f));
    //    float simSwipeLength = Random.Range(0.12f, 0.15f);

    //    SwipeStart(simStartSwipe, simSwipeLength);
    //    SwipeEnd(simEndSwipe, simSwipeLength);

    //}
   
    public void DrawQuadraticBezierPoint(Vector3 start, Vector3 middle, Vector3 end)
    {

        List<Vector3> path = new List<Vector3>();
        int noOfPoints = 50;

        float distance2D = Vector2.Distance(start, end);

        // GameObject playerBall = Instantiate(ball);
        CreateBall();
        
        end.z = (0.2f * distance2D);//making the endPosition extent to z * multiple of the swipe distance
        //Debug.Log("end position after" + end);
        middle = (start + end) /2f + Vector3.up * distance2D;
        for (int i = 0; i <= noOfPoints; i++)
        {
            float t = i / (float)noOfPoints;

            //Vector3 point
            path.Add(CalculateQuadraticBezierPoint(start, middle, end, t));

        }
        for (int i = 1; i < path.Count; i++)
        {
            Debug.DrawLine(path[i - 1], path[i], Color.red, 2f);
        }
        StartCoroutine(MoveAlongPath(ball, path, 1.0f));
    }

    public void CreateBall()
    {
        if (GameObject.FindGameObjectWithTag("Ball") == null)
        {
             ball = Instantiate(ball);
        }
    }
    public void OpponentBallDebug()
    {
        if (GameObject.FindGameObjectWithTag("Ball") == null)
        {
            Vector3 position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z + 20f);
            ball = Instantiate(ball, position, Quaternion.identity);

            Rigidbody rb = ball.GetComponent<Rigidbody>();
            rb.AddForce(-Vector3.forward * 50f * Time.deltaTime, ForceMode.Impulse);
        }
    }
    public IEnumerator MoveAlongPath(GameObject ball, List<Vector3> path, float duration)
    {
        float totalLength = path.Count - 1;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            // figure out which segment we’re in, and the local t
            float t = elapsed / duration * totalLength;
            int i = Mathf.FloorToInt(t);
            float u = t - i;              // local interpolation between path[i] -> path[i+1]

            if (i < path.Count - 1)
                ball.transform.position = Vector3.Lerp(path[i], path[i + 1], u);

            elapsed += Time.deltaTime;
            yield return null;
        }
        
        AddForceAtTheEnd(path, ball);

    }
    public void AddForceAtTheEnd(List<Vector3> path, GameObject ball)
    {
        Vector3 start = path[path.Count - 2];
        Vector3 end    = path[path.Count - 1];
        Vector3 direction = end - start;

        ballrb = ball.GetComponent<Rigidbody>();
        ballrb.WakeUp();
        ballrb.AddForce(direction *8f, ForceMode.Impulse);
       /* Debug.Log("Force direction: " + direction);*/
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
    public void BallMovement(Rigidbody rb,Vector3 direction )
    {
        rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            //if(!ballHitDetection == true)
            //    StopCoroutine(slowBallCoroutine);
            rb.gameObject.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z + 2f);

            rb.gameObject.GetComponent<Ball>().CreateBallMovement(startPosition, direction);
        }
    }

    private void SwipeDirection(Vector2 direction)
    {

        if(Vector3.Dot(Vector3.up, direction) > directionalThreshold)
        {
          //  Debug.Log("swipedUp");
        }
        else if(Vector3.Dot(Vector3.down, direction) > directionalThreshold)
        {
            //Debug.Log("swipedDown");
        }
        else if(Vector3.Dot(Vector3.left, direction) > directionalThreshold)
        {
            //Debug.Log("swipedLeft");
        }
        else if(Vector3.Dot(Vector3.right, direction) > directionalThreshold)
        {
            //Debug.Log("swipedRight");
        }
    }
   
    
}

