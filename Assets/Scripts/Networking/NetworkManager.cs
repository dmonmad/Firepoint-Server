using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    public GameObject[] weaponPrefabs;

    public GameObject playerPrefab;

    //public Transform[] spawners;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }

        PopulateDictionaryWithWeapons();

        MapManager.Initialize(this);
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        Server.Start(Constants.MAX_PLAYERS, Constants.PORT);
        ConnectWithMasterServer();
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    /// <summary>Spawns a player.</summary>
    /// <returns>Player object</returns>
    public Player InstantiatePlayer()
    {
        return Instantiate(playerPrefab, MapManager.GetRandomPlayerSpawner(), Quaternion.identity).GetComponent<Player>();
    }

    /// <summary>Spawns a weapon.</summary>
    /// <returns>Weapon object</returns>
    public Weapon InstantiateWeapon()
    {
        return Instantiate(playerPrefab, MapManager.GetRandomWeaponSpawner(), Quaternion.identity).GetComponent<Weapon>();
    }

    /// <summary>Get a GameObject weapon from it's id.</summary>
    /// <returns>GameObject of the weapon's id</returns>
    public GameObject GetWeapon(int _id)
    {
        return Weapon.weapons[_id];
    }

    /// <summary>Populates the Weapon's Dictionary with weapons by ID.</summary>
    private void PopulateDictionaryWithWeapons()
    {

        foreach (GameObject weapon in weaponPrefabs)
        {
            Weapon.weapons.Add((int)weapon.GetComponent<Weapon>().weaponName, weapon);
        }
    }

    /// <summary>Sends this server information to the master server</summary>
    private void ConnectWithMasterServer()
    {
        Debug.Log("Connecting with master server...");
        StartCoroutine(RestClient._instance.Post(Constants.MASTERSERVER_URL, new ServerModel(Constants.HOST_NAME, Time.time)));
    }
}