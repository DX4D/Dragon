using UnityEngine;
using System.Collections;

public class Fireball : MonoBehaviour
{

    public float velocity = 500;
    public float force = 500;
    public float radius = 2;
    public GameObject explotion;
    protected GameObject owner;

    public GameObject Owner
    {
        set { owner = value; }
        get { return owner; }
    }

    Collider c;

    void Start()
    {
        Invoke("EnableCollider", 0.2f);
    }

    void EnableCollider()
    {
        c = GetComponent<Collider>();
        if (c) c.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 2) return; //Do not collide with Ignore Raycast!
        if (other.isTrigger) return; //Do not collide with Ignore Raycast!
        if (other.transform.root == owner.transform) return;    //Don't hit yourself

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

      

        foreach (var nearbyObjects in colliders)
        {
            if (nearbyObjects.GetComponent<Fireball>()) continue;

            Rigidbody rb = nearbyObjects.GetComponent<Rigidbody>();

            if (rb)
            {
                rb.AddExplosionForce(force, transform.position, radius);
            }
        }

        Destroy(gameObject);
        //create fireball explotion after collides
        GameObject fireballexplotion = Instantiate(explotion);
        fireballexplotion.transform.position = transform.position;

        Destroy(fireballexplotion, 2f);
    }
}
