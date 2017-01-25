using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class PlaneController : MonoBehaviour {

    private IPlaneControl control;
    static PlayerPlaneControl master = null;

    private Rigidbody rb;
    public float wspolczynnikTarcia = 20.0f;

    public Text speedLabel;
    public Text rollLabel;
    public Text altitudeLabel;

    private float rudder;       //wychylenie steru kierunku [-1,1]
    private float aileron;      //wychylenie lotek [-1,1]
    private float elevator;     //wychylenie steru wysokości [-1,1]
    private float throttle;     //pozycja przepustnicy  [0,1]

    private float speed;

    private float maxRudderAngle = 45;      //maksymalne wychylenie steru kierunku w stopniach
    private float maxAileronAngle = 45;     //maksymalne wychylenie lotek w stopniach
    private float maxElevatorAngle = 45;    //maksymalne wychylenie steru wysokości w stopniach
    private float maxThrottle = 200;       //maksymalny ciąg przepustnicy
    private float liftCoef = 5;

    private float maxSpeed = 200;           //maksymalna prędkość samolotu w m/s

    #region Properties

    public float Rudder
    {
        get
        {
            return rudder;
        }

        set
        {
            rudder = Mathf.Clamp(value, -1.0f, 1.0f);
        }
    }

    public float Aileron
    {
        get
        {
            return aileron;
        }

        set
        {
            aileron = Mathf.Clamp(value, -1.0f, 1.0f);
        }
    }

    public float Elevator
    {
        get
        {
            return elevator;
        }

        set
        {
            elevator = Mathf.Clamp(value, -1.0f, 1.0f);
        }
    }

    public float Throttle
    {
        get
        {
            return throttle;
        }

        set
        {
            throttle = Mathf.Clamp(value, 0.0f, 1.0f);
        }
    }

    #endregion

    void Update()
    {
        control.Update();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = 100;
        rb.velocity = transform.forward * 20;   //prędkość początkowa

        if (master == null)
        {
            master = new PlayerPlaneControl();
            control = master;
        }
        else
        {
            AIPlaneControl ai = new AIPlaneControl();
            control = ai;
            master.AddSlave(ai);
        }
        control.Plane = this;

        rudder = 0;
        aileron = 0;
        elevator = 0;
        throttle = 0.5f;  //TODO: temporarily, should be 0
    }

    void FixedUpdate()
    {
        
        transform.Rotate(elevator, rudder, -aileron);

        //rb.velocity = Vector3.zero;

        speed = rb.velocity.magnitude;
        if (speed > maxSpeed)
        {
            rb.velocity *= maxSpeed / speed;
        }


        Vector3 myForward = transform.forward * throttle * maxThrottle;
        Vector3 myFriction = -transform.forward * wspolczynnikTarcia * speed;


        Vector3 airVel = Vector3.Dot(rb.velocity, transform.forward) * transform.forward;
        float airSpeed = airVel.magnitude;
        Debug.DrawLine(transform.position, transform.position + 1000 * rb.velocity, Color.red, 1, true);

        Vector3 myLift = liftCoef * transform.up * airSpeed * airSpeed * 0.5f;
        Vector3 myGravity = - Vector3.up * 9.81f * rb.mass;

        
        rb.AddForce(myForward);
        rb.AddForce(myFriction);
        rb.AddForce(myLift);
        rb.AddForce(myGravity);


        //update gui debug
        if (speedLabel != null)
        {


            speedLabel.text = myLift + " " + myGravity;
            speedLabel.text += "Speed:" + speed + " AS: " + airSpeed + " m/s";
            rollLabel.text = "R: " + rudder + " A: " + aileron + " E: " + elevator + " T: " + throttle;
            altitudeLabel.text = "Alt: " + transform.position.y + "m";
        }

        //Debug.Log(nosna.magnitude);
        //Debug.DrawRay(transform.position, forward, Color.green);
        //Debug.DrawRay(transform.position, nosna, Color.red);
        //Debug.DrawRay(transform.position, tarcia, Color.blue);
    }
}
