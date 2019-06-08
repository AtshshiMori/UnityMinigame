using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    public GameObject target;
    public float slowDistance;
    public float slowRate;

    NavMeshAgent agent;
    Animator animator;
    float life;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        life = 100;
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = target.transform.position;

        if (life < 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void damaged(float damage)
    {
        life -= damage;
    }
}
