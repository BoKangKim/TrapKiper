using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Navigation : MonoBehaviour
{
    private NavMeshAgent agent = null;
    private GameObject player = null;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");

        StartCoroutine(FindTarget());
    }

    private IEnumerator FindTarget()
    {
        while(player != null)
        {
            agent.destination = player.transform.position;
            yield return new WaitForSeconds(0.5f);
        }
    }
}
