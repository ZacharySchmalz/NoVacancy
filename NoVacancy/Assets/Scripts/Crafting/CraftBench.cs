using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *  This script controls the crafting bench functions.
 *  If the player is looking at the bench, and within the required distance, the bench is ready to be accessed.
 */
public class CraftBench : MonoBehaviour
{
    public float distance;

    private GameObject player;
    private Animator anim;
    private bool ready;
    private bool open;

	void Start ()
    {
        ready = false;
        open = false;
        player = GameObject.FindGameObjectWithTag("Player");
        //anim = GetComponent<Animator>();
	}

    void Update()
    {
        if (Vector3.Distance(GetComponent<Transform>().position, player.GetComponent<Transform>().position) < distance)
        {
            if (!open)
            {
                open = true;
                //anim.SetBool("open", open);
            }
        }
        else
        {
            if(open)
            {
                //anim.SetBool("close", open);
                open = false;
            }
        }
    }

    private void OnMouseOver()
    {
        if(Vector3.Distance(GetComponent<Transform>().position, player.GetComponent<Transform>().position) < distance)
        {
            ready = true;
        }
        else
        {
            ready = false; 
        }
    }

    private void OnMouseExit()
    {
        ready = false;
    }

    public bool isReady()
    {
        return ready;
    }
}
