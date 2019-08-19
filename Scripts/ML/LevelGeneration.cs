using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelGeneration : MonoBehaviour
{
    // Access to game objects to be used in the scene..
    public GameObject keyCard;          // for key pick up

    // Key Card positions array

    public Vector3[] keyPositions;


    // objects names , positions
    // rooms
    public GameObject[] rooms;

    public GameObject obstacle1;
    public GameObject obstacle2;

    // robot guards
    public GameObject[] robotGuards;

    // lasers
    public GameObject[] lasers;

    // cameras
    public GameObject[] cctvCameras;

    public Vector3[] room1_positions;
    public Vector3[] room2_positions;
    public Vector3[] room3_positions;
    public Vector3[] room4_positions;

    public void Awake()
    {
        keyPositions = new[] {
        new Vector3(-6.86f, -4.39f, 9.17f), // original position
        new Vector3(-3.5f, -4.39f, -18f), // near the removable obstacle
        new Vector3(12f, -4.39f, -18f), //close to agent 
       new Vector3(1f, -4.39f, -12f), //room 1
        new Vector3(-6f, -4.39f, -9f) //near the fence
//           new Vector3(-11f, -4.39f, -16f), //test position
//        new Vector3(15f, -4.39f, -7f), // test position 2
         };

        room1_positions = new[] {
            new Vector3(0f, 0f, 0f),
            new Vector3(-10f,0f,0f),
            new Vector3(-5f,0f,22f),
        };
        room2_positions = new[] {
            new Vector3(0f, 0f, 0f),
            new Vector3(2f, 0f, 33f),
            new Vector3(-9f, 0f, 0f),
        };
        room3_positions = new[] {
            new Vector3(0f, 0f, 0f),
            new Vector3(10f, 0f, -22f), // if rooms 1 and 2 disabled
            new Vector3(8f, 0f, 6f), // if rooms 1 and 2 disabled .. it is kind of messy
        };
        room4_positions = new[] {
            new Vector3(0f, 0f, 0f),
            
        };
    }

    // Level generation 

    public void generateLevel(int levelMask)
    {
        switch (levelMask)
        {
            // All rooms disabled.
            case 1:
                randomObstacleGeneration();
                spawnKey();
                break;
            // Rooms enabled
            case 2:
                randomObstacleGeneration();
                generateRandomRoom(1);
                spawnKey();
                break;
            case 3:
                randomObstacleGeneration();
                generateRandomRoom(2);
                spawnKey();
                break;
            case 4:
                randomObstacleGeneration();
                generateRandomRoom(3);
                spawnKey();
                break;
            // Lasers This point on
            // Switch deactivation units to be enabled along with lasers
            case 5:
                randomObstacleGeneration();
                generateRandomRoom(3);
                laserEnable(1);
                spawnKey();
                break;
            case 6:
                randomObstacleGeneration();
                generateRandomRoom(3);
                laserEnable(2);
                spawnKey();
                break;
            case 7:
                randomObstacleGeneration();
                generateRandomRoom(3);
                laserEnable(3);
                spawnKey();
                break;
            case 8:
                randomObstacleGeneration();
                generateRandomRoom(3);
                laserEnable(4);
                spawnKey();
                break;
            case 9:
                randomObstacleGeneration();
                generateRandomRoom(3);
                laserEnable(5);
                spawnKey();
                break;
            case 10:
                randomObstacleGeneration();
                generateRandomRoom(3);
                laserEnable(6);
                spawnKey();
                break;
            // CCTV Cameras from now on
            case 11:
                randomObstacleGeneration();
                generateRandomRoom(3);
                laserEnable(6);
                cctvCamEnable(1);
                spawnKey();
                break;
            case 12:
                randomObstacleGeneration();
                generateRandomRoom(3);
                laserEnable(6);
                cctvCamEnable(2);
                spawnKey();
                break;
            case 13:
                randomObstacleGeneration();
                generateRandomRoom(3);
                laserEnable(6);
                cctvCamEnable(3);
                spawnKey();
                break;            // Robot Guards enabled from here on
            case 14:
                randomObstacleGeneration();
                generateRandomRoom(3);
                laserEnable(6);
                cctvCamEnable(3);
                robotGuardEnable(1);
                spawnKey();
                break;
            case 15:
                randomObstacleGeneration();
                generateRandomRoom(3);
                laserEnable(6);
                cctvCamEnable(3);
                robotGuardEnable(2);
                spawnKey();
                break;
            case 16:
                randomObstacleGeneration();
                generateRandomRoom(3);
                laserEnable(6);
                cctvCamEnable(3);
                robotGuardEnable(3);
                spawnKey();
                break;
            default: // full level
                randomObstacleGeneration();
                generateRandomRoom(3);
                laserEnable(6);
                cctvCamEnable(3);
                robotGuardEnable(3);
                spawnKey();
                break;
        }

    }
    
    public void generateRandomRoom(int num)
    {
        Shuffle(rooms);
        for (int i = 0; i < num; i++)
        {
            if (rooms[i].name == "room1")
            {
                Shuffle(room1_positions);
                rooms[i].transform.localPosition = room1_positions[0];
                rooms[i].SetActive(true);
            }
            else if (rooms[i].name == "room2")
            {
                Shuffle(room2_positions);
                rooms[i].transform.localPosition = room2_positions[0];
                rooms[i].SetActive(true);

            }
            else if (rooms[i].name == "room3")
            {
                Shuffle(room3_positions);
                rooms[i].transform.localPosition = room3_positions[0];
                rooms[i].SetActive(true);

            }
            else // if it's room 4.. just generate on the default position
            {
                rooms[i].transform.localPosition = room4_positions[0];
                rooms[i].SetActive(true);

            }

        }
    }


public void spawnKey()
    {
        // Randomise the position of the key
        keyCard.transform.localPosition = keyPositions[Random.Range(0, keyPositions.Length)];

    }

    public void randomObstacleGeneration()
    {
        // Randomly enable / disable the following    
        bool boolean;
        boolean = (Random.value > 0.5f);
        obstacle1.SetActive(boolean);
        boolean = (Random.value > 0.5f);
        obstacle2.SetActive(boolean);

    }

    public void laserEnable (int num)
    {
        // enable num number of lasers at random.
                Shuffle(lasers);
        for(int i = 0; i<num; i++)
        {
            lasers[i].SetActive(true);
        }
    }

    public void cctvCamEnable(int num)
    {
        // enable num number of cameras next to the activated rooms/props only.
       
       Shuffle(cctvCameras);
        for (int i = 0; i < num; i++)
        {
            cctvCameras[i].SetActive(true);
        }

    }

    public void robotGuardEnable(int num)
    {
        // enable num number of guards at random

        Shuffle(robotGuards);
        for (int i = 0; i < num; i++)
        {
            robotGuards[i].SetActive(true);
        }
    }


    // ************************  UTILS ************************
    // ********************************************************
    public void Shuffle(GameObject [] array)
    {
        GameObject tempGO;
        for (int i = 0; i < array.Length; i++)
        {
            int rnd = Random.Range(0, array.Length);
            tempGO = array[rnd];
            array[rnd] = array[i];
            array[i] = tempGO;
        }
    }
    public void Shuffle(Vector3[] array)
    {
        Vector3 tempGO;
        for (int i = 0; i < array.Length; i++)
        {
            int rnd = Random.Range(0, array.Length);
            tempGO = array[rnd];
            array[rnd] = array[i];
            array[i] = tempGO;
        }
    }

    public void DisableAll()
    {
        foreach (GameObject enemy in robotGuards)
            enemy.SetActive(false);

        foreach (GameObject room in rooms)
            room.SetActive(false);

        obstacle1.SetActive(false);
        obstacle2.SetActive(false);

        foreach (GameObject laser in lasers)
            laser.SetActive(false);

        foreach (GameObject cctvCam in cctvCameras)
            cctvCam.SetActive(false);
    }

}