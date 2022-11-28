using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator),typeof(NavMeshAgent))]
public class Monster : MonoBehaviour
{
    private Animator monsterAni = null;
    private NavMeshAgent agent = null;
    private GameObject dest = null;

    private void Awake()
    {
        monsterAni = GetComponent<Animator>();
        dest = GameObject.FindWithTag("Destination");
        agent.SetDestination(dest.transform.position);
    }

}
