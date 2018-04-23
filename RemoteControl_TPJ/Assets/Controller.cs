using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
    public HorizontalJoystick horizontal;
    public VerticalJoystick vertical;
    public Vector3 dirVertical;
    public Vector3 dirHorizontal;
    // Use this for initialization
    void Start () {
        horizontal = FindObjectOfType<HorizontalJoystick>();
        vertical = FindObjectOfType<VerticalJoystick>();
        dirHorizontal = Vector3.zero;
        dirVertical = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
        //dirHorizontal.x = horizontal.Horizontal();
        //dirVertical.z = vertical.Vertical();
        //Debug.Log("Horizontal axis: " + horizontal.Horizontal().ToString());
        //Debug.Log("Vertical axis: " + vertical.Vertical().ToString());
    }
}
