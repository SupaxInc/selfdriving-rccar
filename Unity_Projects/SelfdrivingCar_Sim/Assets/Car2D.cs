using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MachineLearning;

public class Car2D : Species {


    //public struct LineDrawer            // LineDrawer from https://stackoverflow.com/questions/42819071/debug-drawline-not-showing-in-the-gameview
    //{
    //    private LineRenderer lineRenderer;
    //    private float lineSize;

    //    public LineDrawer(float lineSize = 0.03f)
    //    {
    //        GameObject lineObj = new GameObject("LineObj");
    //        lineRenderer = lineObj.AddComponent<LineRenderer>();
    //        //Particles/Additive
    //        lineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));

    //        this.lineSize = lineSize;
    //    }

    //    private void init(float lineSize = 0.03f)
    //    {
    //        if (lineRenderer == null)
    //        {
    //            GameObject lineObj = new GameObject("LineObj");
    //            lineRenderer = lineObj.AddComponent<LineRenderer>();
    //            //Particles/Additive
    //            lineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));

    //            this.lineSize = lineSize;
    //        }
    //    }

    //    //Draws lines through the provided vertices
    //    public void DrawLineInGameView(Vector3 start, Vector3 end, Color color)
    //    {
    //        if (lineRenderer == null)
    //        {
    //            init(0.03f);
    //        }

    //        //Set color
    //        lineRenderer.startColor = color;
    //        lineRenderer.endColor = color;

    //        //Set width
    //        lineRenderer.startWidth = lineSize;
    //        lineRenderer.endWidth = lineSize;

    //        //Set line count which is 2
    //        lineRenderer.positionCount = 2;

    //        //Set the postion of both two lines
    //        lineRenderer.SetPosition(0, start);
    //        lineRenderer.SetPosition(1, end);
    //    }

    //    public void Destroy()
    //    {
    //        if (lineRenderer != null)
    //        {
    //            UnityEngine.Object.Destroy(lineRenderer.gameObject);
    //        }
    //    }
    //}

    //byte Red, Green, Blue;

    //// https://docs.unity3d.com/Manual/class-WheelCollider.html Extremum is max stickyness of the car wheels
    //float driftingForceSlippery = 1;                          // The magnitude when to start drifting
    //float driftingForceSticky = 0.9f;                          // Traction 'spec' of the car
    //float maxStickyVelocity = 2f;                            // Max magnitude for the car so it doesn't lose traction, we start drifting when we exceed this value
    ////float minSlipperyVelocity = 1.5f;                          // Min magnitude for the car to start drifting
    //float speedForce = 13.5f;
    //float torqueForce = -135f;

    //Rigidbody2D rbCar;
    //LineDrawer frontCenterLineDrawer, frontRightLineDrawer, frontLeftLineDrawer, backCenterLineDrawer, backRightLineDrawer, backLeftLineDrawer;
    //RaycastHit2D frontCenterDistanceHitDetector, frontRightDistanceHitDetector, frontLeftDistanceHitDetector, backCenterDistanceHitDetector,
    //    backRightDistanceHitDetector, backLeftDistanceHitDetector;

    //[Header("Back Sensors")]
    //public Transform backCenterSensorLength, backCenterSensorStart, backRightAngleSensorLength, backRightAngleSensorStart, backLeftAngleSensorLength, backLeftAngleSensorStart;
    //public bool backCenterHitDetection = false, backRightHitDetection = false, backLeftHitDetection = false;

    //[Header("Front Sensors")]
    //public Transform frontCenterSensorLength, frontCenterSensorStart, frontRightAngleSensorLength, frontRightAngleSensorStart, frontLeftAngleSensorLength, frontLeftAngleSensorStart;
    //public bool frontCenterHitDetection = false, frontRightHitDetection = false, frontLeftHitDetection = false;

    public Car2D(Functions.ActivationFunction actf, params int[] layersInfo) : base(actf, layersInfo)
    {
    }

    //public void changeSpeed(float acc)
    //{
    //    float newSpeed;
    //    newSpeed = (float)Functions.Map(acc, -1.0, 1.0, 0.0, 90.0);         // max reverse speed and forward speed
    //}

    //public void changeAngle(float ang)
    //{
    //    Functions.Map(ang, -1.0, 1.0, -90.0, 90.0);     // actual number, next 2 initial range get axis basically,
    //}

    //public void updateEveryFrame()
    //{
    //    Sensors();
    //}

    //public void fixedUpdate()
    //{

    //}

    //private void applyManualDriver(float stoppedTorqueForce)
    //{
    //    //rbCar.AddForce(transform.up * speedForce);                                   // Not transform.forward (2d space) so transform.up, Linear Drag property helps add realistic physics to stop car

    //    //rbCar.AddForce(transform.up * -speedForce / 2);

    //    //rbCar.angularVelocity = Input.GetAxis("Horizontal") * stoppedTorqueForce;                // GetAxis is left and right keys ( a and d )
    //    //// We don't multiply torqueForce with Time.deltaTime because it adds a bit more force every single frame.
    //}

    //private void Sensors()
    //{
    //    frontCenterHitDetection = Physics2D.Linecast(frontCenterSensorStart.position, frontCenterSensorLength.position, 1 << LayerMask.NameToLayer("InnerBoundary"));
    //    frontRightHitDetection = Physics2D.Linecast(frontRightAngleSensorStart.position, frontRightAngleSensorLength.position, 1 << LayerMask.NameToLayer("InnerBoundary"));
    //    frontLeftHitDetection = Physics2D.Linecast(frontLeftAngleSensorStart.position, frontLeftAngleSensorLength.position, 1 << LayerMask.NameToLayer("InnerBoundary"));
    //    backCenterHitDetection = Physics2D.Linecast(backCenterSensorStart.position, backCenterSensorLength.position, 1 << LayerMask.NameToLayer("InnerBoundary"));
    //    backRightHitDetection = Physics2D.Linecast(backRightAngleSensorStart.position, backRightAngleSensorLength.position, 1 << LayerMask.NameToLayer("InnerBoundary"));
    //    backLeftHitDetection = Physics2D.Linecast(backLeftAngleSensorStart.position, backLeftAngleSensorLength.position, 1 << LayerMask.NameToLayer("InnerBoundary"));
    //    Blue = 0;
    //    //Front Center Hit Detection
    //    Green = (byte)(frontCenterDistanceHitDetector.fraction * 255);
    //    Red = (byte)(255 - Green);
    //    frontCenterDistanceHitDetector = Physics2D.Linecast(frontCenterSensorStart.position, frontCenterSensorLength.position, 1 << LayerMask.NameToLayer("InnerBoundary"));
    //    frontCenterLineDrawer.DrawLineInGameView(frontCenterSensorStart.position, frontCenterSensorLength.position, new Color32(Red, Green, Blue, 255));
    //    if (frontCenterHitDetection)
    //    {
    //        Debug.Log(frontCenterDistanceHitDetector.fraction);
    //    }

    //    //Front Right Angle Hit Detection
    //    Green = (byte)(frontRightDistanceHitDetector.fraction * 255);
    //    Red = (byte)(255 - Green);
    //    frontRightDistanceHitDetector = Physics2D.Linecast(frontRightAngleSensorStart.position, frontRightAngleSensorLength.position, 1 << LayerMask.NameToLayer("InnerBoundary"));
    //    frontRightLineDrawer.DrawLineInGameView(frontRightAngleSensorStart.position, frontRightAngleSensorLength.position, new Color32(Red, Green, Blue, 255));
    //    if (frontRightHitDetection)
    //    {
    //        Debug.Log(frontRightDistanceHitDetector.fraction);
    //    }

    //    //Front Left Angle Hit Detection
    //    Green = (byte)(frontLeftDistanceHitDetector.fraction * 255);
    //    Red = (byte)(255 - Green);
    //    frontLeftDistanceHitDetector = Physics2D.Linecast(frontLeftAngleSensorStart.position, frontLeftAngleSensorLength.position, 1 << LayerMask.NameToLayer("InnerBoundary"));
    //    frontLeftLineDrawer.DrawLineInGameView(frontLeftAngleSensorStart.position, frontLeftAngleSensorLength.position, new Color32(Red, Green, Blue, 255));
    //    if (frontLeftHitDetection)
    //    {
    //        Debug.Log(frontLeftDistanceHitDetector.fraction);
    //    }

    //    //Back Center Hit Detection
    //    Green = (byte)(backCenterDistanceHitDetector.fraction * 255);
    //    Red = (byte)(255 - Green);
    //    backCenterDistanceHitDetector = Physics2D.Linecast(backCenterSensorStart.position, backCenterSensorLength.position, 1 << LayerMask.NameToLayer("InnerBoundary"));
    //    backCenterLineDrawer.DrawLineInGameView(backCenterSensorStart.position, backCenterSensorLength.position, new Color32(Red, Green, Blue, 255));
    //    if (backCenterHitDetection)
    //    {
    //        Debug.Log(backCenterDistanceHitDetector.fraction);
    //    }

    //    //Back Right Angle Hit Detection
    //    Green = (byte)(backRightDistanceHitDetector.fraction * 255);
    //    Red = (byte)(255 - Green);
    //    backRightLineDrawer.DrawLineInGameView(backRightAngleSensorStart.position, backRightAngleSensorLength.position, new Color32(Red, Green, Blue, 255));
    //    backRightDistanceHitDetector = Physics2D.Linecast(backRightAngleSensorStart.position, backRightAngleSensorLength.position, 1 << LayerMask.NameToLayer("InnerBoundary"));
    //    if (backRightHitDetection)
    //    {
    //        Debug.Log(backRightDistanceHitDetector.fraction);
    //    }

    //    //Back Left Angle Hit Detection
    //    Green = (byte)(backLeftDistanceHitDetector.fraction * 255);
    //    Red = (byte)(255 - Green);
    //    backLeftLineDrawer.DrawLineInGameView(backLeftAngleSensorStart.position, backLeftAngleSensorLength.position, new Color32(Red, Green, Blue, 255));
    //    backLeftDistanceHitDetector = Physics2D.Linecast(backLeftAngleSensorStart.position, backLeftAngleSensorLength.position, 1 << LayerMask.NameToLayer("InnerBoundary"));
    //    if (backLeftDistanceHitDetector)
    //    {
    //        Debug.Log(backLeftDistanceHitDetector.fraction);
    //    }
    //}
}
