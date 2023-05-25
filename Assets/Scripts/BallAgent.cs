using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEditor;

public class BallAgent : Agent
{
    public GameObject Target;
    private GameObject targetInstance;
    public GameObject Area;

    public GameObject[] spawnPoints;
    private float xLimitSpawn = 2.0f;
    private float zLimitSpawn = 2.0f;

    private Rigidbody rb;

    private Vector3 agentInitialPos;
    private Quaternion agentInitialRot;

    private float speed = 8.0f;
    private float rotateSpeed = 80.0f;
    private float fallingLimit = -0.5f;

    private float agentPositionX;
    private float agentPositionZ;
    private float targetPositionX;
    private float targetPositionZ;
    private float distance;

    public float currentStep;
    public float maxStep = 10000f;


    // Start is called before the first frame update
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        agentInitialPos = transform.position;
        agentInitialRot = transform.rotation;
        ResetAgentPosition();
    }

    public override void OnEpisodeBegin()
    {
        currentStep = 0;
        ResetAgentPosition();
        this.GetComponent<Rigidbody>().velocity = new Vector3();
        this.GetComponent <Rigidbody>().angularVelocity = new Vector3();

        if (targetInstance != null)
        {
            Destroy(targetInstance);
        }
        SpawnTarget();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Déplacement vers l'avant
        float verticalInput = actions.ContinuousActions[0];
        Vector3 forwardMovement = transform.forward * speed * Time.deltaTime * Mathf.Abs(verticalInput);
        rb.MovePosition(rb.position + forwardMovement);

        // Rotation
        float horizontalInput = actions.ContinuousActions[1];
        float rotation = horizontalInput * rotateSpeed * Time.deltaTime;
        Quaternion deltaRotation = Quaternion.Euler(0f, rotation, 0f);
        rb.MoveRotation(rb.rotation * deltaRotation);

        if (rb.position.y < fallingLimit)
        {
            ResetAgentPosition();
        }

        currentStep += 1;
        SetReward(-0.001f * distance);
        if (currentStep == maxStep)
        {
            SetReward(-1.0f);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        agentPositionX = rb.position.x;
        agentPositionZ = rb.position.z;
        targetPositionX = targetInstance.transform.position.x;
        targetPositionZ = targetInstance.transform.position.z;

        //xDistance = Mathf.Abs(targetPositionX - agentPositionX);
        //zDistance = Mathf.Abs(targetPositionZ - agentPositionZ);

        distance = Vector3.Distance(rb.position, targetInstance.transform.position);

        sensor.AddObservation(agentPositionX);
        sensor.AddObservation(agentPositionZ);
        sensor.AddObservation(targetPositionX);
        sensor.AddObservation(targetPositionZ);
        sensor.AddObservation(distance);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var agentContinuousAction = actionsOut.ContinuousActions;

        float verticalInput = Input.GetKey(KeyCode.UpArrow) ? 1f : 0f;

        float horizontalInput = Input.GetAxis("Horizontal");

        // Comparaison conditionnelle avec les bornes 0.5 et 1.0
        agentContinuousAction[0] = verticalInput ;
        agentContinuousAction[1] = horizontalInput;
    }

    private void ResetAgentPosition()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.position = agentInitialPos;
        rb.rotation = agentInitialRot;

    }

    private void SpawnTarget()
    { 
        GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        Vector3 spawnPos = spawnPoint.transform.position;

        float randomX = Random.Range(spawnPos.x - xLimitSpawn, spawnPos.x + xLimitSpawn);
        float staticY = spawnPos.y;
        float randomZ = Random.Range(spawnPos.z - zLimitSpawn, spawnPos.z + zLimitSpawn);

        Vector3 randomSpawnPos = new Vector3(randomX, staticY, randomZ);
        //Debug.Log("posrandom" + randomSpawnPos);

        targetInstance = Instantiate(Target, randomSpawnPos, Quaternion.identity);

        //targetInstance = Instantiate(Target, spawnPos, Quaternion.identity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            Destroy(targetInstance);
            SetReward(1.0f);
            EndEpisode();
        }
    }

}
