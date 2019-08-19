using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSwitchDeactivation : MonoBehaviour
{
    public GameObject laser;
    public Material unlockedMat;

    private GameObject player;

    [HideInInspector]
    /// <summary>
    /// The associated agent.
    /// This will be set by the agent script on Initialization. 
    /// Don't need to manually set.
    /// </summary>
    public AgentStealth agentStealth;


    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag(Tags.player);
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject == player)
        {
            if (Input.GetButton("Switch"))
            {
                LaserDeactivation();
                agentStealth.IDisabledLaser();
            }
        }
    }

    void LaserDeactivation()
    {
        laser.SetActive(false);

        Renderer screen = transform.Find("prop_switchUnit_screen").gameObject.GetComponent<Renderer>();
        screen.material = unlockedMat;
        GetComponent<AudioSource>().Play();
    }

}
