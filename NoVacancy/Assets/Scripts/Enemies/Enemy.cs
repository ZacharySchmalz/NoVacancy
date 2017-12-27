using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health;                    // enemy health
    public float speed;                     // enemy movement speed
    public float detectionRange;            // range at which enemy detects player
    public float damage;                    // how much damage does enemy do
    public float rotationSpeed;             // speed at which enemy rotates towards player
    public GameObject[] itemPool;           // item pool that this enery can choose from to spawn at random;

    protected GameObject target;            // what is the enemys' target
    protected Animator anim;                // enemy animator controller
    protected float distance;                 // distance from player
    protected bool isDead;

	protected virtual void Start ()
    {
        isDead = false;
        target = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
	}
	
	protected virtual void Update ()
    {
        distance = Vector3.Distance(transform.position, target.transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.transform.position - transform.position), rotationSpeed * Time.deltaTime);
    }

    // Deals damage to enemy, checks if enemy dies, play animation if so
    public virtual void takeDamage(float amount)
    {
        if (isDead)
            return;

        health -= amount;
        if(health <= 0)
        {
            anim.SetTrigger("death1");
            isDead = true;
            GameObject item = Instantiate(itemPool[Random.Range(0, itemPool.Length)], transform.position, Quaternion.identity);
            Physics.IgnoreCollision(item.GetComponent<Collider>(), GetComponentInChildren<Collider>());
            gameObject.GetComponent<Enemy>().enabled = false;
            gameObject.GetComponent<Rigidbody>().freezeRotation = true;
        }
    }
}
