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

    public Player InstantiatePlayer()
    {
        return Instantiate(playerPrefab, MapManager.GetRandomPlayerSpawner(), Quaternion.identity).GetComponent<Player>();
    }

    public Weapon InstantiateWeapon()
    {
        return Instantiate(playerPrefab, MapManager.GetRandomWeaponSpawner(), Quaternion.identity).GetComponent<Weapon>();
    }

    public GameObject GetWeapon(int _id)
    {
        return Weapon.weapons[_id];
    }

    private void PopulateDictionaryWithWeapons()
    {

        foreach (GameObject weapon in weaponPrefabs)
        {
            Weapon.weapons.Add((int)weapon.GetComponent<Weapon>().weaponName, weapon);
        }
    }

    private void ConnectWithMasterServer()
    {
        Debug.Log("Connecting with master server...");
        StartCoroutine(RestClient._instance.Post(Constants.MASTERSERVER_URL, new ServerModel(Constants.HOST_NAME, Time.time)));
    }
}