using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlaneControl {
    

    PlaneController Plane { get; set; }
    void Update();
}
