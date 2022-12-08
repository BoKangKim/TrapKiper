using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalTestFire : MonoBehaviour
{
    [HideInInspector, Tooltip("The Orbital Remote script attached to the character")]
    public OrbitalRemote orbitalRemote;
    [Tooltip("The amount of time in seconds in-between 'shots'")]
    public float fireRate = 0.2f;
    [Tooltip("Should audio be played when the gun is fired?")]
    public bool playAudio = false;
    [Tooltip("Demo Muzzle Flash")]
    public bool enableDemoMuzzleFlash = false;
    private float currentFireRate = 0;
    // Start is called before the first frame update
    void Start()
    {
        //Gets the remote if it has not been set
        orbitalRemote = GetComponent<OrbitalRemote>();
    }

    // Update is called once per frame
    void Update()
    {
        currentFireRate = Mathf.Clamp(currentFireRate - Time.deltaTime , 0, 100);

        //Plays the fire animation if left clicked and is in aim mode
        if (orbitalRemote.GetComponent<Animator>().GetBool("OrbitalAimMode") && Input.GetMouseButton(0) && currentFireRate == 0)
        {
            //Resets the fire timer
            currentFireRate = fireRate;
            //Plays the fire animation
            orbitalRemote.PlayFireAnimation(true);

            if (playAudio)
            {
                AudioSource audioSource = GetComponent<AudioSource>();
                if (audioSource != null)
                    audioSource.Play();
            }

            if (enableDemoMuzzleFlash)
            {
                orbitalRemote.recommendedSettings.orbitalTargeting.localValues.orbitalWeapon.transform.GetComponent<DemoMuzzleFlash>().Play();
            }
        }
        //Toggles the weapon ready state if right clicked
        if (Input.GetMouseButtonDown(1))
        {
            orbitalRemote.GetComponent<Animator>().SetBool("OrbitalAimMode", !orbitalRemote.GetComponent<Animator>().GetBool("OrbitalAimMode"));
        }
    }
}
