using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlaneControl : IPlaneControl {
    private PlaneController plane;

    public enum AICommand {
        None,
        TurnLeft, TurnRight,
        RollLeft, RollRight,
        GoUp, GoDown,
        IncreaseSpeed, DecreaseSpeed
    };

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

    public void ExecuteCommand(AICommand cmd)
    {
        switch (cmd)
        {
            case AICommand.TurnLeft:
                plane.Rudder = -1;
                break;
            case AICommand.TurnRight:
                plane.Rudder = 1;
                break;
            case AICommand.RollLeft:
                plane.Aileron = -1;
                break;
            case AICommand.RollRight:
                plane.Aileron = 1;
                break;
            case AICommand.GoUp:
                plane.Elevator = 1;
                break;
            case AICommand.GoDown:
                plane.Elevator = -1;
                break;
            case AICommand.IncreaseSpeed:
                plane.Throttle += 0.1f;
                break;
            case AICommand.DecreaseSpeed:
                plane.Throttle -= 0.1f;
                break;
            case AICommand.None:
                plane.Rudder = 0;
                plane.Aileron = 0;
                plane.Elevator = 0;
                break;
            default:
                Debug.Log("Unknown command");
                break;
        }
    }

	void IPlaneControl.Update() {
		
	}
}
