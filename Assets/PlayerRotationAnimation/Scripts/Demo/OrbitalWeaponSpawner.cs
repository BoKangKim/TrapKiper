using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalWeaponSpawner : MonoBehaviour
{
    [Tooltip("The prefab orbital weapon that will be spawned and attached to the remote when spawn weapon is set to true.")]
    public GameObject weaponPrefab;
    [Tooltip("Spawns the weapon prefab. This can be set at any time and any existing weapon will be destroyed.")]
    public bool spawnWeapon = true;
    //Private hard spawn lock for error handling
    private bool canSpawn = false;
    void Start()
    {
        //Checks if a weapon should be spawned
        if (spawnWeapon)
        StartCoroutine(DelayedSpawn());
    }

    private void Update()
    {
        //Checks if a weapon should be spawned
        if (spawnWeapon)
        {
            SpawnWeapon();
        }
    }

    //Waits a small amount of time to spawn a weapon to prevent startup problems
    IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(0.1f);
        canSpawn = true;
        SpawnWeapon();
    }

    //Spawns the prefab weapon
    void SpawnWeapon()
    {
        if (!canSpawn) return;
        spawnWeapon = false;
        if (weaponPrefab != null)
        {
            OrbitalRemote orbitalRemote = GetComponent<OrbitalRemote>();
            if (orbitalRemote == null)
            {
                Debug.LogError("Weapon Creation Failed!");
            }
            else
            {
                orbitalRemote.SpawnWeaponFromPrefab(weaponPrefab);
            }
        }
    }

    //Spawns a specified weapon not related to the prefab variable 
    public void SpawnWeapon(GameObject weapon)
    {
        weaponPrefab = weapon;
        spawnWeapon = false;
        if (weaponPrefab != null)
        {
            OrbitalRemote orbitalRemote = GetComponent<OrbitalRemote>();
            if (orbitalRemote == null)
            {
                Debug.LogError("Weapon Creation Failed!");
            }
            else
            {
                orbitalRemote.SpawnWeaponFromPrefab(weaponPrefab);
            }
        }
    }
}
