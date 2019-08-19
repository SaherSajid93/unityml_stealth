using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPlayerDetection : MonoBehaviour
{
    private GameObject player;
    private LastPlayerSighting lastPlayerSighting;
    private AgentStealth agentStealth;
    

    void Awake()
    {
        // Setting up references.
        player = GameObject.FindGameObjectWithTag(Tags.player);
        agentStealth = player.GetComponent<AgentStealth>();
        lastPlayerSighting = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<LastPlayerSighting>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            agentStealth.ITriggeredAlarm();
            Debug.Log("Alarm Triggered");
        }
    }

    void OnTriggerStay(Collider other)
    {
        // If the beam is on...
        if (GetComponent<Renderer>().enabled)
            // ... and if the colliding gameobject is the player...
            if (other.gameObject == player)
            { // ... set the last global sighting of the player to the colliding object's position.
                lastPlayerSighting.position = other.transform.position;
            }
    }
}
