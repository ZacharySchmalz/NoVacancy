using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  This script controls the behaviour of resources.
 *  It handles spawn behavior (float in air then fall back down), and collisions with the player
 */

public class Resource : Item
{
    public int amount;
    public ResourceType type;
    private Rigidbody body;
    private GameObject player;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.velocity = new Vector3(Random.Range(-.5f,.5f), 2f, Random.Range(-.5f,.5f));
        body.AddTorque(-1f, 1.5f, -1.5f);

        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (Vector3.Distance(GetComponent<Transform>().position, player.GetComponent<Transform>().position) < 2.5f)
        {
            body.AddForce((player.transform.position - transform.position) * 2);
        }
    }

    public ResourceType getType()
    {
        return type;
    }
}
