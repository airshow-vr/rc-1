using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlaneControl : IPlaneControl {

    private PlaneController plane;
    private List<AIPlaneControl> slaves;

    public PlaneController Plane
    {
        get
        {
            return plane;
        }

        set
        {
            plane = value;
        }
    }

    public PlayerPlaneControl()
    {
        slaves = new List<AIPlaneControl>();
    }

    void IPlaneControl.Update() {
        Debug.Log("PlayerPlaneControl Update");
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        plane.Aileron = moveHorizontal;
        plane.Elevator = moveVertical;

        AIPlaneControl.AICommand cmd = AIPlaneControl.AICommand.None;

        if (Input.GetKeyDown(KeyCode.T))
        {
            plane.Throttle = Mathf.Min(plane.Throttle + 0.02f, 1.0f);
            cmd = AIPlaneControl.AICommand.IncreaseSpeed;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            plane.Throttle = Mathf.Max(plane.Throttle - 0.02f, 0.0f);
            cmd = AIPlaneControl.AICommand.DecreaseSpeed;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            plane.Rudder = Mathf.Min(plane.Rudder + 0.02f, 1.0f);
            cmd = AIPlaneControl.AICommand.TurnRight;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            plane.Rudder = Mathf.Max(plane.Rudder - 0.02f, 0.0f);
            cmd = AIPlaneControl.AICommand.TurnRight;
        }

        float eps = 0.1f;
        if (plane.Aileron > eps)
        {
            cmd = AIPlaneControl.AICommand.RollRight;
        }
        else if (plane.Aileron < -eps)
        {
            cmd = AIPlaneControl.AICommand.RollLeft;
        }

        if (plane.Elevator > eps)
        {
            cmd = AIPlaneControl.AICommand.GoUp;
        }
        else if (plane.Elevator < -eps)
        {
            cmd = AIPlaneControl.AICommand.GoDown;
        }

        foreach (AIPlaneControl s in slaves)
        {
            s.ExecuteCommand(cmd);
        }
    }

    public void AddSlave(AIPlaneControl slave)
    {
        slaves.Add(slave);
    }
}
