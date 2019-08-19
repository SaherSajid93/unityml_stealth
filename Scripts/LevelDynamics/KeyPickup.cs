using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public AudioClip keyGrab;                           // Audioclip to play when the key is picked up.


    private GameObject player;                          // Reference to the player.
    private PlayerInventory playerInventory;        // Reference to the player's inventory.

    [HideInInspector]
    /// <summary>
    /// The associated agent.
    /// This will be set by the agent script on Initialization. 
    /// Don't need to manually set.
    /// </summary>
    public AgentStealth agentStealth;



    void Awake()
    {
        // Setting up the references.
        player = GameObject.FindGameObjectWithTag(Tags.player);
        playerInventory = player.GetComponent<PlayerInventory>();
    }


    void OnTriggerEnter(Collider other)
    {
        // If the colliding gameobject is the player...
        if (other.gameObject == player)
        {
            // ... play the clip at the position of the key...
            AudioSource.PlayClipAtPoint(keyGrab, transform.position);

            // ... the player has a key ...
            playerInventory.hasKey = true;

            //reward for collecting key
            agentStealth.ICollectedKey();

            // ... and destroy this gameobject.
            //Destroy(gameObject);
            // not destroying
            gameObject.SetActive(false);
        }
    }
}
