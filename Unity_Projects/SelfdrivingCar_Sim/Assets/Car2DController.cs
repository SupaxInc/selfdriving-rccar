using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MachineLearning;
using System;
using System.Linq;

public class Car2DController : MonoBehaviour
{
    public LevelManager levelManager;
    public Species brain;

    byte Red, Green, Blue, Alpha;

    // https://docs.unity3d.com/Manual/class-WheelCollider.html Extremum is max stickyness of the car wheels
    public float driftingForceSlippery;                          // The magnitude when to start drifting
    public float driftingForceSticky;                          // Traction 'spec' of the car
    public float maxStickyVelocity;                            // Max magnitude for the car so it doesn't lose traction, we start drifting when we exceed this value
    //float minSlipperyVelocity = 1.5f;                          // Min magnitude for the car to start drifting
    public float speedForce;
    public float torqueForce;

    Rigidbody2D rbCar;
    LineDrawer frontCenterLineDrawer, frontRightLineDrawer, frontLeftLineDrawer, backCenterLineDrawer, backRightLineDrawer, backLeftLineDrawer;
    RaycastHit2D frontCenterDistanceHitDetector, frontRightDistanceHitDetector, frontLeftDistanceHitDetector, backCenterDistanceHitDetector,
        backRightDistanceHitDetector, backLeftDistanceHitDetector;

    [Header("Back Sensors")]
    public Transform backCenterSensorLength, backCenterSensorStart, backRightAngleSensorLength, backRightAngleSensorStart, backLeftAngleSensorLength, backLeftAngleSensorStart;
    public bool backCenterHitDetection = false, backRightHitDetection = false, backLeftHitDetection = false;

    [Header("Front Sensors")]
    public Transform frontCenterSensorLength, frontCenterSensorStart, frontRightAngleSensorLength, frontRightAngleSensorStart, frontLeftAngleSensorLength, frontLeftAngleSensorStart;
    public bool frontCenterHitDetection = false, frontRightHitDetection = false, frontLeftHitDetection = false;
    
    public float sensorLength;

    public float stoppedTorqueForce;

    public float TravelledDistance;
    public float TimeAlive;
    public double CumulativeSensoryData;

    public float Speed
    {
        get
        {
            Vector2 vel = rbCar.velocity;
            vel /= 7.0f;
            float angle = (rbCar.rotation * -1.0f) % 360.0f;
            vel = vel.Rotate(angle);
            float speed = vel.y;
            return speed;
        }
    }


    void Start()
    {
        driftingForceSlippery = 1.0f;                          // The magnitude when to start drifting
        driftingForceSticky = 0.9f;                          // Traction 'spec' of the car
        maxStickyVelocity = 2f;                            // Max magnitude for the car so it doesn't lose traction, we start drifting when we exceed this value
        //float minSlipperyVelocity = 1.5f;                          // Min magnitude for the car to start drifting
        speedForce = 10.8f;
        torqueForce = -200f;
        sensorLength = 4.5f;
        TravelledDistance = 0;
        CumulativeSensoryData = 0;
        TimeAlive = 1;
        rbCar = GetComponent<Rigidbody2D>();                                 // Rigidbody2D creates physics for 2D objects
        levelManager = FindObjectOfType<LevelManager>();
        frontCenterLineDrawer = new LineDrawer();
        frontRightLineDrawer = new LineDrawer();
        frontLeftLineDrawer = new LineDrawer();
        backCenterLineDrawer = new LineDrawer();
        backRightLineDrawer = new LineDrawer();
        backLeftLineDrawer = new LineDrawer();
    }

    void Update()
    {
    }

    void FixedUpdate()
    {                                                                // FixedUpdate and Update runs on separate threads, Update runs per tick of graphics, FixedUpdate runs per tick of physics engine
        if (GetComponentInParent<Respawn>().isAlive)
        {
            stoppedTorqueForce = Mathf.Lerp(0, torqueForce, rbCar.velocity.magnitude / 10);       // Stops the car from changing directions when it is stopped

            List<double> data = GetSensorData().ToList();       // list could be able to be appended, and data needs to be appended

            foreach (double sensorData in data)
            {
                CumulativeSensoryData += (sensorData + 1.0) * 0.5;
            }
            CumulativeSensoryData /= data.Count;

            data.Add(Speed);

            float userSpeed = Input.GetAxis("Vertical");
            float userDirection = Input.GetAxis("Horizontal");
            if ((userDirection != 0.0 || userSpeed != 0.0) || levelManager.manualMode)
            {
                //Debug.Log(userSpeed + " " + userDirection);
                double[] userInput = new double[] { userSpeed == 0.0f ? Speed : userSpeed, userDirection == 0 ? rbCar.angularVelocity / 100.0 : userDirection };
                for (int i = 0; i < levelManager.lessons; i++)
                {
                    brain.Train(data.ToArray(), userInput, levelManager.learningRate, levelManager.regularizationRate);
                }
                changeSpeed(userSpeed);
                changeAngle(userDirection);
            }
            else
            {
                double[] newVelAngle = brain.Guess(data.ToArray());
                float nnSpeed = (float)newVelAngle[0];
                float nnDirection = (float)newVelAngle[1];
                //Debug.Log(userSpeed + " " + userDirection);
                changeSpeed(nnSpeed);           // Thinking that index 0 is speed for NN
                changeAngle(nnDirection);           // Thinking that index 1 is angle for NN
            }

            applyDriftPhysics();
        }
    }

    public void changeSpeed(float deltaSpeed)
    {
        float newSpeed = speedForce * deltaSpeed;
        rbCar.AddForce(transform.up * newSpeed);
        TravelledDistance += newSpeed * Time.deltaTime;
        //Debug.Log(TravelledDistance);
    }

    public void changeAngle(float angle)
    {
        angle = Speed > 0.0f ? angle * 1.0f : angle * -1.0f;
        rbCar.angularVelocity = angle * stoppedTorqueForce;
    }

    // ForwardVelocity() and RightVelocity() stops car from sliding sideways and makes the Vector magnitude ONLY in forward direction
    Vector2 GetForwardVelocity()                                                                   // takes total velocity and gets how much of that component is in up axis
    {
        return transform.up * Vector2.Dot(GetComponent<Rigidbody2D>().velocity, transform.up);     // Dot gets product of two vectors and gets magnitude of our forward direction 
    }

    Vector2 GetRightVelocity()                                                                     // takes total velocity and gets how much of that component is in right axis
    {
        return transform.right * Vector2.Dot(GetComponent<Rigidbody2D>().velocity, transform.right);
    }

    private void applyDriftPhysics()
    {
        float carMagnitude = GetRightVelocity().magnitude;
        float driftingForce = driftingForceSticky;
        //Debug.Log(carMagnitude);
        if (carMagnitude > maxStickyVelocity)                                            // If the car magnitude is greater than the traction of the car, we START drifting
        {
            driftingForce = driftingForceSlippery;
        }
        rbCar.velocity = GetForwardVelocity() + GetRightVelocity() * driftingForce;      // kills the sliding movement and puts the velocity ONLY in forward direction and creates drifting motion
    }

    public double[] GetSensorData()
    {
        //frontCenterHitDetection = Physics2D.Linecast(frontCenterSensorStart.position, frontCenterSensorLength.position, 1 << LayerMask.NameToLayer("InnerBoundary"));
        //frontRightHitDetection = Physics2D.Linecast(frontRightAngleSensorStart.position, frontRightAngleSensorLength.position, 1 << LayerMask.NameToLayer("InnerBoundary"));
        //frontLeftHitDetection = Physics2D.Linecast(frontLeftAngleSensorStart.position, frontLeftAngleSensorLength.position, 1 << LayerMask.NameToLayer("InnerBoundary"));
        //backCenterHitDetection = Physics2D.Linecast(backCenterSensorStart.position, backCenterSensorLength.position, 1 << LayerMask.NameToLayer("InnerBoundary"));
        //backRightHitDetection = Physics2D.Linecast(backRightAngleSensorStart.position, backRightAngleSensorLength.position, 1 << LayerMask.NameToLayer("InnerBoundary"));
        //backLeftHitDetection = Physics2D.Linecast(backLeftAngleSensorStart.position, backLeftAngleSensorLength.position, 1 << LayerMask.NameToLayer("InnerBoundary"));
        double[] sensorData = new double[6];
        float fraction = 0; //Percentage of distance to the nearest wall;
        Alpha = (byte)(GetComponent<Respawn>().isAlive ? 92 : 24);
        Blue = 0;

        //Front Center Hit Detection
        frontCenterDistanceHitDetector = Physics2D.Linecast(frontCenterSensorStart.position, frontCenterSensorStart.position + frontCenterSensorStart.up * sensorLength, 1 << LayerMask.NameToLayer("InnerBoundary"));
        fraction = frontCenterDistanceHitDetector.collider != null ? frontCenterDistanceHitDetector.fraction : 1.0f;    //On hit, fraction is based on RayCast. But if no hit occured, it is 100%, meaning no obsticale along the distance of sensor.
        Green = (byte)(fraction * 255);
        Red = (byte)(255 - Green);
        if (levelManager.isDebugMode)
        {
            frontCenterLineDrawer.DrawLineInGameView(frontCenterSensorStart.position, frontCenterSensorStart.position + frontCenterSensorStart.up * sensorLength, new Color32(Red, Green, Blue, Alpha));
        }
        else
        {
            frontCenterLineDrawer.Destroy();
        }
        //if (frontCenterHitDetection)
        //{
        //    Debug.Log(frontCenterDistanceHitDetector.fraction);
        //}
        sensorData[0] = Functions.Map(fraction, 0.0, 1.0, -1.0, 1.0);

        //Front Right Angle Hit Detection
        frontRightDistanceHitDetector = Physics2D.Linecast(frontRightAngleSensorStart.position, frontRightAngleSensorStart.position + frontRightAngleSensorStart.up * sensorLength, 1 << LayerMask.NameToLayer("InnerBoundary"));
        fraction = frontRightDistanceHitDetector.collider != null ? frontRightDistanceHitDetector.fraction : 1.0f;
        Green = (byte)(fraction * 255);
        Red = (byte)(255 - Green);
        if (levelManager.isDebugMode)
        {
            frontRightLineDrawer.DrawLineInGameView(frontRightAngleSensorStart.position, frontRightAngleSensorStart.position + frontRightAngleSensorStart.up * sensorLength, new Color32(Red, Green, Blue, Alpha));
        }
        else
        {
            frontRightLineDrawer.Destroy();
        }
        //if (frontRightHitDetection)
        //{
        //    //Debug.Log(frontRightDistanceHitDetector.fraction);
        //}
        sensorData[1] = Functions.Map(fraction, 0.0, 1.0, -1.0, 1.0);

        //Front Left Angle Hit Detection
        frontLeftDistanceHitDetector = Physics2D.Linecast(frontLeftAngleSensorStart.position, frontLeftAngleSensorStart.position + frontLeftAngleSensorStart.up * sensorLength, 1 << LayerMask.NameToLayer("InnerBoundary"));
        fraction = frontLeftDistanceHitDetector.collider != null ? frontLeftDistanceHitDetector.fraction : 1.0f;
        Green = (byte)(fraction * 255);
        Red = (byte)(255 - Green);
        if (levelManager.isDebugMode)
        {
            frontLeftLineDrawer.DrawLineInGameView(frontLeftAngleSensorStart.position, frontLeftAngleSensorStart.position + frontLeftAngleSensorStart.up * sensorLength, new Color32(Red, Green, Blue, Alpha));
        }
        else
        {
            frontLeftLineDrawer.Destroy();
        }
        //if (frontLeftHitDetection)
        //{
        //    Debug.Log(frontLeftDistanceHitDetector.fraction);
        //}
        sensorData[2] = Functions.Map(fraction, 0.0, 1.0, -1.0, 1.0);

        //Back Center Hit Detection
        backCenterDistanceHitDetector = Physics2D.Linecast(backCenterSensorStart.position, backCenterSensorStart.position + backCenterSensorStart.up * sensorLength, 1 << LayerMask.NameToLayer("InnerBoundary"));
        fraction = backCenterDistanceHitDetector.collider != null ? backCenterDistanceHitDetector.fraction : 1.0f;
        Green = (byte)(fraction * 255);
        Red = (byte)(255 - Green);
        if (levelManager.isDebugMode)
        {
            backCenterLineDrawer.DrawLineInGameView(backCenterSensorStart.position, backCenterSensorStart.position + backCenterSensorStart.up * sensorLength, new Color32(Red, Green, Blue, Alpha));
        }
        else
        {
            backCenterLineDrawer.Destroy();
        }
        //if (backCenterHitDetection)
        //{
        //    Debug.Log(backCenterDistanceHitDetector.fraction);
        //}
        sensorData[3] = Functions.Map(fraction, 0.0, 1.0, -1.0, 1.0);

        //Back Right Angle Hit Detection
        backRightDistanceHitDetector = Physics2D.Linecast(backRightAngleSensorStart.position, backRightAngleSensorStart.position + backRightAngleSensorStart.up * sensorLength, 1 << LayerMask.NameToLayer("InnerBoundary"));
        fraction = backRightDistanceHitDetector.collider != null ? backRightDistanceHitDetector.fraction : 1.0f;
        Green = (byte)(fraction * 255);
        Red = (byte)(255 - Green);
        if (levelManager.isDebugMode)
        {
            backRightLineDrawer.DrawLineInGameView(backRightAngleSensorStart.position, backRightAngleSensorStart.position + backRightAngleSensorStart.up * sensorLength, new Color32(Red, Green, Blue, Alpha));
        }
        else
        {
            backRightLineDrawer.Destroy();
        }
        //if (backRightHitDetection)
        //{
        //    Debug.Log(backRightDistanceHitDetector.fraction);
        //}
        sensorData[4] = Functions.Map(fraction, 0.0, 1.0, -1.0, 1.0);

        //Back Left Angle Hit Detection
        backLeftDistanceHitDetector = Physics2D.Linecast(backLeftAngleSensorStart.position, backLeftAngleSensorStart.position + backLeftAngleSensorStart.up * sensorLength, 1 << LayerMask.NameToLayer("InnerBoundary"));
        fraction = backLeftDistanceHitDetector.collider != null ? backLeftDistanceHitDetector.fraction : 1.0f;
        Green = (byte)(fraction * 255);
        Red = (byte)(255 - Green);
        if (levelManager.isDebugMode)
        {
            backLeftLineDrawer.DrawLineInGameView(backLeftAngleSensorStart.position, backLeftAngleSensorStart.position + backLeftAngleSensorStart.up * sensorLength, new Color32(Red, Green, Blue, Alpha));
        }
        else
        {
            backLeftLineDrawer.Destroy();
        }
        //if (backLeftDistanceHitDetector)
        //{
        //    Debug.Log(backLeftDistanceHitDetector.fraction);
        //}
        sensorData[5] = Functions.Map(fraction, 0.0, 1.0, -1.0, 1.0);

        return sensorData;
    }
}

public static class Vector2Extension
{
    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float tx = v.x;
        float ty = v.y;

        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }
}

public struct LineDrawer            // LineDrawer from https://stackoverflow.com/questions/42819071/debug-drawline-not-showing-in-the-gameview
{
    private LineRenderer lineRenderer;
    private float lineSize;

    public LineDrawer(float lineSize = 0.03f)
    {
        GameObject lineObj = new GameObject("LineObj");
        lineRenderer = lineObj.AddComponent<LineRenderer>();
        //Particles/Additive
        lineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));

        this.lineSize = lineSize;
    }

    private void init(float lineSize = 0.03f)
    {
        if (lineRenderer == null)
        {
            GameObject lineObj = new GameObject("LineObj");
            lineRenderer = lineObj.AddComponent<LineRenderer>();
            //Particles/Additive
            lineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));

            this.lineSize = lineSize;
        }
    }

    //Draws lines through the provided vertices
    public void DrawLineInGameView(Vector3 start, Vector3 end, Color color)
    {
        if (lineRenderer == null)
        {
            init(0.03f);
        }

        //Set color
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        //Set width
        lineRenderer.startWidth = lineSize;
        lineRenderer.endWidth = lineSize;

        //Set line count which is 2
        lineRenderer.positionCount = 2;

        //Set the postion of both two lines
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    public void Destroy()
    {
        if (lineRenderer != null)
        {
            UnityEngine.Object.Destroy(lineRenderer.gameObject);
        }
    }
}