using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoMuzzleFlash : MonoBehaviour
{
    public Transform spawnPoint;
    public float duration;
    public GameObject prefab;

    public GameObject lineRenderer;
    public Vector3 forward = Vector3.forward;
    public float lineDuration;

    public void Play()
    {
        StartCoroutine(PlayFlash());
        if (lineRenderer != null)
            StartCoroutine(DrawLine());
    }
    IEnumerator PlayFlash() 
    {
        GameObject flash = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);

        yield return new WaitForSeconds(duration);
        Destroy(flash);
    }
    IEnumerator DrawLine()
    {
        GameObject line = Instantiate(lineRenderer, spawnPoint.position, spawnPoint.rotation, spawnPoint);
        line.GetComponent<LineRenderer>().SetPosition(0, line.transform.position);
        line.GetComponent<LineRenderer>().SetPosition(1, line.transform.position + line.transform.TransformDirection(forward));

        yield return new WaitForSeconds(lineDuration);
        Destroy(line);
    }
}
