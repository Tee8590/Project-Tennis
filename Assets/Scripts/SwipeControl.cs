using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwipeControl : MonoBehaviour
{
    public static event Action<SwipeControl> OnSwipe;
    [SerializeField]
    private InputManager inputManager;

    private Vector2 direction2D;
    private Vector2 startPosition;
    private float startTime;
    private Vector2 endPosition;
    private float endTime;
    private Rigidbody ballrb;
    [SerializeField]
    private float minimumDistance =.2f;
    [SerializeField]
    private float maxTime = 1f;
    [SerializeField]
    private float directionalThreshold = 0.9f;
    [SerializeField]
    private GameObject trail;
    [SerializeField]
    private GameObject ballPrefab;
    [SerializeField]
    private GameObject player;
    private Coroutine coroutine;
    private Coroutine slowBallCoroutine;
    private BallHitDetection ballHitDetection;
    private InputAction fireAction;
    private GameObject createdBallPrefab = null;
    private float swipeTime;

    private Vector3 direction;
    private Vector2 middlePosition;
    private Vector3 velocity;
    private Vector3 landingPos;
    private Vector2 swipStart;
    private Vector2 swipEnd;
    //[SerializeField]
    //private Camera mainCamera;
    private void OnEnable()
    {
        inputManager.OnStartTouch += SwipeStart;
        inputManager.OnEndTouch += SwipeEnd;
        BallHitDetection.OnPlayer1Hit += Player1SwingAction;
        fireAction = inputManager.FireAction;
        //  Ball.BallStartAndEndpositions += MakeBallMovement;
        BallHitDetection.OnPlayer2Hit += OpponentPlayerServing;
    }
    private void OnDisable()
    {

        inputManager.OnStartTouch -= SwipeStart;
        inputManager.OnEndTouch -= SwipeEnd;
        BallHitDetection.OnPlayer1Hit -= Player1SwingAction;
        // Ball.BallStartAndEndpositions -= MakeBallMovement;
        BallHitDetection.OnPlayer2Hit -= OpponentPlayerServing;
    }
    private void Awake()
    {
       ballHitDetection =  player.GetComponentInChildren<BallHitDetection>();
        CreateBall();
    }
    public void Player1SwingAction(Collider collider)
    {
        
        ballrb = collider.GetComponent<Rigidbody>();
    }
   
    private void SwipeStart(Vector2 position, float time)
    {
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
            CreateBall();
            /*//SimulateSwipeRHS();*/
            //OpponentBallDebug();
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
      
        trail.SetActive(false);
        StopCoroutine(coroutine);
        endPosition = position;
        endTime = time;
        DetectSwipe();
    }
    public void OpponentPlayerServing(Collider collider)
    {
       
       
        //Todo Randomize Swing Direction
        //Vector3 enemy = new Vector3(-9, 45, -100).normalized;
        /*ballRb.linearVelocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;*/
       
        /*landingPos.z = landingPos.z / 6;
        middlePosition.y += middlePosition.y + 10f;*/
        /*MakeBallMovement(ballobj.transform.position, middlePosition, landingPos);
        GameManager.isPlayerOneServing = false;*/

        if (Vector2.Distance(swipStart, swipEnd) >= minimumDistance && (endTime - startTime) <= maxTime)
        {
            float swipeTime = (endTime - startTime);
            direction = swipEnd - swipStart;
            direction2D = new Vector2(direction.x, direction.y).normalized;

            float arcFactor = 0.5f; // Adjust to control arc height (try 0.3 to 0.7)
            float swipeDistance = Vector2.Distance(startPosition, endPosition);
            middlePosition = (startPosition + endPosition) / 2f + Vector2.up * swipeDistance * arcFactor;
            Vector3 endPoint = new Vector3(swipEnd.x, swipEnd.y, 0);
            Vector3 midPoint = new Vector3(0, 0, 0);
            SwipeDirection(direction2D);;

            Ball ballobj = GameObject.Find("Ball").GetComponent<Ball>();
            Rigidbody ballRb = collider.gameObject.GetComponent<Rigidbody>();
            ballRb.useGravity = true;
            velocity = ballobj.CreateBallVelocity(swipStart, direction, swipeTime);
            landingPos = ballobj.CalculateLandingPoint(swipStart, velocity, 18.24f);
            landingPos.z = landingPos.z / 6;
            ballobj.BallLandingPositionMarker(landingPos);
            MakeBallMovement(ballobj.transform.position, middlePosition, landingPos);
        }
    }
   /* public void OpponentPlayerServing(Collider collider)
    {
        Ball ballobj = collider.gameObject.GetComponent<Ball>();
        Rigidbody ballRb = collider.gameObject.GetComponent<Rigidbody>();
        //Todo Randomize Swing Direction
        Vector3 enemy = new Vector3(-9, 45, -100).normalized;
        ballRb.linearVelocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
        ballRb.useGravity = true;
        //ballRb.AddForce(enemy * 3f, ForceMode.Impulse);
        Vector3 middlePosition =new Vector3 (297f, 18f, 3f );
        Vector3 direction = new Vector3(0.93f, 18f, -3.56f);
        Vector3 zValue = ballobj.CreateBallVelocity(ballobj.transform.position, direction, 0.2f);
        Debug.Log(zValue.z + "landingPosz1");

        Vector3 landingPos = (ballobj.CalculateLandingPoint(ballobj.transform.position, zValue, 18.24f));
        landingPos.z = landingPos.z / 6;
        Debug.Log(landingPos.z +"landingPos");
        MakeBallMovement(ballobj.transform.position, middlePosition, landingPos);
        GameManager.isPlayerOneServing = false;


    }*/
    public void DetectSwipe()
    {
        if (Vector2.Distance(startPosition, endPosition) >= minimumDistance && (endTime - startTime) <= maxTime)
        {
            float swipeTime = (endTime - startTime);
            swipStart = startPosition;
            swipEnd = endPosition;
            //direction of the ballPrefab in 2D, z is 0 currently
           direction = endPosition - startPosition;
           direction2D = new Vector2(direction.x, direction.y).normalized;

            float arcFactor = 0.5f; // Adjust to control arc height (try 0.3 to 0.7)
            float swipeDistance = Vector2.Distance(startPosition, endPosition);
            middlePosition = (startPosition + endPosition) / 2f + Vector2.up * swipeDistance * arcFactor;
            Debug.Log("middlePosition" + middlePosition);
            //making it endPosition & middlePosition Vector3
            Vector3 endPoint = new Vector3(endPosition.x, endPosition.y, 0);
            Vector3 midPoint = new Vector3(0,0,0);
            SwipeDirection(direction2D);

            
            Ball ballobj= GameObject.Find("Ball").GetComponent<Ball>();
            Rigidbody ballRb = ballobj.GetComponent<Rigidbody>();
            ballRb.useGravity = true;
            velocity = ballobj.CreateBallVelocity(startPosition, direction, swipeTime);
            landingPos = ballobj.CalculateLandingPoint(startPosition, velocity, 18.24f);

            MakeBallMovement(ballobj.transform.position, middlePosition, landingPos);
           

        }
    }
    public void MakeBallMovement( Vector3 startPoint, Vector3 middle,  Vector3 landingPoint)
    {
        /* swipeTime = (endTime - startTime);
        //direction of the ballPrefab in 2D, z is 0 currently
        direction = endPosition - startPosition;*/
        direction2D = new Vector2(direction.x, direction.y).normalized;
        if (GameManager.isPlayerOneServing)
        {
            CreateBall();
            // BallMovement(ballrb, direction, swipeTime);
            DrawQuadraticBezierPoint(startPoint, middle, landingPoint);
            GameManager.isPlayerOneServing = false;
        }
        if (!GameManager.isPlayerOneServing)
        {
            CreateBall();
            // BallMovement(ballrb, direction, swipeTime);
            DrawQuadraticBezierPoint(startPoint, middle, landingPoint);
            GameManager.isPlayerOneServing = true;
        }
       
    }
    //public void SimulateSwipeLHS()
    //{
    //    UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
    //    Vector2 simStartSwipe = new Vector2(UnityEngine.Random.Range(291, 294.5f), UnityEngine.Random.Range(11f, 14f));
    //    Vector2 simEndSwipe   = new Vector2(UnityEngine.Random.Range(293, 296.5f), UnityEngine.Random.Range(0.5f, .75f));
    //    float simSwipeLength = UnityEngine.Random.Range(0.12f, 0.16f);

    //    SwipeStart(simStartSwipe, simSwipeLength);
    //    SwipeEnd(simEndSwipe, simSwipeLength);

    //}
    //public void SimulateSwipeRHS()
    //{
    //    UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
    //    Vector2 simStartSwipe = new Vector2(UnityEngine.Random.Range(297.5f, 400), UnityEngine.Random.Range(0.8f, 1f));
    //    Vector2 simEndSwipe   = new Vector2(UnityEngine.Random.Range(297.5f, 600), UnityEngine.Random.Range(0.8f, 1f));
    //    float simSwipeLength = UnityEngine.Random.Range(1, 4);

    //    SwipeStart(simStartSwipe, simSwipeLength);
    //    SwipeEnd(simEndSwipe, simSwipeLength);

    //}

    public void DrawQuadraticBezierPoint(Vector3 start, Vector3 middle, Vector3 end)
    {

        List<Vector3> path = new List<Vector3>();
        int noOfPoints = 50;

        float distance2D = Vector2.Distance(start, end);

        // GameObject playerBall = Instantiate(ballPrefab);
        CreateBall();
        
       // end.z = (0.2f * distance2D);//making the endPosition extent to z * multiple of the swipe distance
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
        StartCoroutine(MoveAlongPath(ballPrefab, path, 1.0f));
    }

    public Rigidbody CreateBall()
    {
        if (GameObject.FindGameObjectWithTag("Ball") == null)
        {
            ballPrefab.transform.position = new Vector3(player.transform.position.x,
                player.transform.position.y,
                player.transform.position.z + 2f);
            GameObject ball = Instantiate(ballPrefab);  ball.name = "Ball";
           
            ballrb = ball.GetComponent<Rigidbody>();
            BallPhysicsOff();
            return ballrb;
        }
        else{
            ballrb.transform.position = new Vector3(player.transform.position.x,
              player.transform.position.y,
              player.transform.position.z + 2f);
            ballrb = GameObject.FindGameObjectWithTag("Ball").GetComponent<Rigidbody>();
           
            BallPhysicsOff();
            return ballrb;
        }
    }
    public Rigidbody BallPhysicsOff()
    {
        ballrb.linearVelocity = Vector3.zero;
        ballrb.angularVelocity = Vector3.zero;
        ballrb.useGravity = false; return ballrb;
    }

    public IEnumerator MoveAlongPath(GameObject ball, List<Vector3> path, float duration)
    {
        float totalLength = path.Count - 1;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            
            float t = elapsed / duration * totalLength;
            int i = Mathf.FloorToInt(t);
            float u = t - i;              // local interpolation between path[i] -> path[i+1]
             ball = GameObject.Find("Ball");
            Rigidbody rb =ball.GetComponent<Rigidbody>();
            rb.useGravity = true;
            if (i < path.Count - 1)
                ball.transform.position = Vector3.Lerp(path[i], path[i + 1], u);

            elapsed += Time.deltaTime;
            yield return null;
        }

        AddForceAtTheEnd(path, ball);

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
    public void AddForceAtTheEnd(List<Vector3> path, GameObject ball)
    {
        Vector3 start = path[path.Count - 2];
        Vector3 end = path[path.Count - 1];
        Vector3 direction = (end - start);

        ballrb = ball.GetComponent<Rigidbody>();
        ballrb.WakeUp();
       ballrb.rotation = Quaternion.LookRotation(direction, Vector3.down);
        Debug.Log("Force direction: " + direction);
        ballrb.AddForce(direction * 8f, ForceMode.Impulse);
        /* Debug.Log("Force direction: " + direction);*/
    }
    public void BallMovement(Rigidbody rb,Vector3 direction, float swipeTime)
    {
        rb = ballrb;
        if (rb != null)
        {
            //if(!ballHitDetection == true)
            //    StopCoroutine(slowBallCoroutine);
            // rb.gameObject.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z + 2f);
            rb.useGravity = true;
            rb.gameObject.GetComponent<Ball>().CreateBallVelocity(startPosition, direction, swipeTime);
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

