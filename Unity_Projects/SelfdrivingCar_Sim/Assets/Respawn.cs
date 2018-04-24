using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    private Car2DController car2D;
        
    public Transform respawnPoint;
    public Rigidbody2D thisRbCar;      // gets the rigidbody of only THIS car

    public bool isAlive;
    public float elapsedTime = 0;

    // Use this for initialization
    void Start () {
        isAlive = true;

        thisRbCar = GetComponent<Rigidbody2D>();
        
        //car2D = FindObjectOfType<Car2DController>();
        car2D = GetComponent<Car2DController>();

        respawnPoint = GameObject.Find("Respawn Point").transform;

        RespawnCar();
    }
	
	// Update is called once per frame
	//void Update () {
 //       elapsedTime += Time.deltaTime;
 //       string minutes = ((int)elapsedTime / 60).ToString();
 //       string seconds = (elapsedTime % 60).ToString("f2");
 //       if (elapsedTime >= 20.00)
 //       {
 //           RespawnCar();
 //           elapsedTime = 0;
 //       }
 //       // when every car dies it respawns
 //   }

    public void RespawnCar()    // Respawns car when time passes
    {
        isAlive = true;
        GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        car2D.TravelledDistance = 0;
        car2D.TimeAlive = 1;
        car2D.CumulativeSensoryData = 0;
        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;
        thisRbCar.velocity = Vector2.zero;
        thisRbCar.angularVelocity = 0;
        thisRbCar.constraints = RigidbodyConstraints2D.None;    // stops freezing the car to let it move again
    }

    private void OnCollisionEnter2D(Collision2D collision)      // WHEN THE SPECIFIC CAR HITS THE COLLISION
    {
        if (collision.gameObject.tag == "Boundary")
        {
            isAlive = false;
            GetComponent<SpriteRenderer>().color = new Color32(255, 0, 0, 48);
            car2D.GetSensorData();
            // if GameObject (Rigidbody of the car that connects with this script) will stop in its spot when it hits the collider
            thisRbCar.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

}
