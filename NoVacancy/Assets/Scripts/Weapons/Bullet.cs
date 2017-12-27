using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    [HideInInspector] public float damage;
    private Rigidbody bullet;
    private Camera cam;
    private float distance;

	// Use this for initialization
	void Start ()
    {
        //cam = GameObject.FindGameObjectWithTag("Gun").GetComponent<Camera>();
        bullet = GetComponent<Rigidbody>();
        var ray = Camera.main.ViewportPointToRay(new Vector3(.5f,.5f,0));
        bullet.velocity = ray.direction * speed;
        Destroy(gameObject, 2);
	}
	
	// Update is called once per frame
	void Update ()
    {
        distance = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);
        if(distance > 75)
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Enemy>())
        {
            collision.gameObject.GetComponent<Enemy>().takeDamage(damage);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
