using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public float AmbientSpeed = 100.0f;

    public float RotationSpeed = 200.0f;

    public Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Quaternion AddRot = Quaternion.identity;
        float roll = 0;
        float pitch = 0;
        float yaw = 0;
        roll = Input.GetAxis("Horizontal") * (Time.deltaTime * RotationSpeed);
        pitch = Input.GetAxis("Pitch") * (Time.deltaTime * RotationSpeed);
        yaw = Input.GetAxis("Yaw") * (Time.deltaTime * RotationSpeed);
        AddRot.eulerAngles = new Vector3(-pitch, yaw, -roll);
        rigidbody.rotation *= AddRot;
        Vector3 AddPos = Vector3.forward;
        AddPos = rigidbody.rotation * AddPos;
        rigidbody.velocity = AddPos * (Time.deltaTime * AmbientSpeed);
    }

    void UpdateFunction()
    {
        Quaternion AddRot = Quaternion.identity;
        float roll = 0;
        float pitch = 0;
        float yaw = 0;
        roll = Input.GetAxis("Roll") * (Time.deltaTime * RotationSpeed);
        pitch = Input.GetAxis("Pitch") * (Time.deltaTime * RotationSpeed);
        yaw = Input.GetAxis("Yaw") * (Time.deltaTime * RotationSpeed);
        AddRot.eulerAngles = new Vector3(-pitch, yaw, -roll);
        rigidbody.rotation *= AddRot;
        Vector3 forward = transform.forward;
        forward = rigidbody.rotation * forward;
        rigidbody.velocity = forward * (Time.deltaTime * AmbientSpeed);
    }
}
