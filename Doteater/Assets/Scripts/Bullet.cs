using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("tag: " + other.tag);
        if (other.tag == "Enemy")
        {
            Rigidbody rg_enemy = other.GetComponent<Rigidbody>();
            Rigidbody rg_bullet = this.GetComponent<Rigidbody>();
            Debug.Log(rg_bullet.velocity);
            rg_enemy.AddForce(rg_bullet.velocity.normalized, ForceMode.Impulse);

            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            enemy.damaged(10);
        }

        if (other.tag != "Player")
        {
            Destroy(this.gameObject);
        }
    }
}
