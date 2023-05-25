using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEditor;

public class BallAgent : MonoBehaviour
{
    public GameObject Target;
    private GameObject targetInstance;
    public GameObject Area;

    private Rigidbody rb;

    private Vector3 agentInitialPos;
    private Quaternion agentInitialRot;

    private float speed = 20.0f;
    private float rotateSpeed = 110.0f;
    private float fallingLimit = -0.5f;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        agentInitialPos = transform.position;
        agentInitialRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        float verticalInput = Input.GetAxis("Vertical");
        //float horizontalInput = Input.GetAxis("Horizontal");

        //transform.Translate(Vector3.forward * verticalInput * speed * Time.deltaTime);
        //transform.Rotate(Vector3.up *  horizontalInput * rotateSpeed * Time.deltaTime);

        // Déplacement vers l'avant
        Vector3 forwardMovement = transform.forward * speed * Time.deltaTime * verticalInput;
        rb.MovePosition(rb.position + forwardMovement);

        // Rotation
        float rotation = Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime;
        Quaternion deltaRotation = Quaternion.Euler(0f, rotation, 0f);
        rb.MoveRotation(rb.rotation * deltaRotation);

        if (rb.position.y < fallingLimit)
        {
            ResetAgentPosition();
        }
    }

    private void ResetAgentPosition()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.position = agentInitialPos;
        rb.rotation = agentInitialRot;

    }

}
