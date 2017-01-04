using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    private Rigidbody rb;
    public float maxCiag, currentCiag, wspolczynnikTarcia, przechylenie;
    private Vector3 previousForward, previousNosna;
	void Update()
    {

    }

    void Start()
    {
        previousForward = new Vector3(0, 0, 0);
        previousNosna = new Vector3(0, 0, 0);
        rb = GetComponent<Rigidbody>();
        currentCiag = 0.0000f;
        maxCiag = 350000.0f;
        wspolczynnikTarcia = 1.1f;
        przechylenie = 0.0f;
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        

        if (Input.GetKeyDown(KeyCode.T))
        {
            currentCiag = Mathf.Min(currentCiag + 0.1f, 1.0f);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            currentCiag = Mathf.Max(currentCiag - 0.1f, 0.0f);
        }
        /*if (Input.GetKeyDown(KeyCode.X))
        {
            przechylenieDronga = Mathf.Min(przechylenieDronga + 0.1f, 3.0f);
            transform.Rotate(-1, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            przechylenieDronga = Mathf.Max(przechylenieDronga - 0.1f, -3.0f);
            transform.Rotate(1, 0, 0);
        }*/
        //if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.Rotate(moveVertical, 0, 0);
            transform.Rotate(0,0, -moveHorizontal);
        }
       // if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //transform.Rotate(0,0, -moveHorizontal);
        }
        rb.velocity = Vector3.zero;
        Vector3 forward = rb.rotation * new Vector3(0.0f, 0.0f, currentCiag * maxCiag);
        Vector3 nosna = rb.rotation * new Vector3(0.0f, 1.5f * ((0.11f - moveVertical) * Mathf.Min(500,rb.velocity.magnitude) * Mathf.Min(500, rb.velocity.magnitude) / 2), 0.0f);
        Vector3 tarcia = rb.rotation * new Vector3(-wspolczynnikTarcia * rb.velocity.x, -wspolczynnikTarcia * rb.velocity.y, -wspolczynnikTarcia * rb.velocity.z);
        if (rb.velocity.magnitude <= 500)
            rb.AddForce(forward); // sila ciagu
        rb.AddForce(nosna); //sila nosna
        rb.AddForce(0, -rb.mass * 9.81f * 7.5f, 0);
        //if (rb.velocity.magnitude <= 500)
        rb.AddForce(tarcia); // sila tarcia
        previousForward = forward;
        previousNosna = nosna;

        
        Debug.Log(nosna.magnitude);
        Debug.DrawRay(transform.position, forward, Color.green);
        Debug.DrawRay(transform.position, nosna, Color.red);
        //Debug.DrawRay(transform.position, tarcia, Color.blue);
    }
}
