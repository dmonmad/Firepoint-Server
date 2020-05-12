using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static List<Transform> itemSpawnPositions = new List<Transform>();
    public static List<Transform> playerSpawnPositions = new List<Transform>();

    public static void Initialize(NetworkManager _nm)
    {
        foreach (GameObject itemSpawnerPos in GameObject.FindGameObjectsWithTag("WeaponSpawner"))
        {
            itemSpawnPositions.Add(itemSpawnerPos.GetComponent<Transform>());
        }


        foreach (GameObject playerSpawnerPos in GameObject.FindGameObjectsWithTag("PlayerSpawner"))
        {
            playerSpawnPositions.Add(playerSpawnerPos.GetComponent<Transform>());
        }

        SetUp(_nm);
    }

    private static void SetUp(NetworkManager _nm)
    {
        foreach (Transform itemSpawnerTransform in itemSpawnPositions)
        {
            Instantiate(_nm.weaponPrefabs[UnityEngine.Random.Range(0, _nm.weaponPrefabs.Length)], itemSpawnerTransform.position, Quaternion.identity);
        }
    }

    public static Vector3 GetRandomPlayerSpawner()
    {
        return playerSpawnPositions[UnityEngine.Random.Range(0, playerSpawnPositions.Count - 1)].position;
    }

    public static Vector3 GetRandomWeaponSpawner()
    {
        return itemSpawnPositions[UnityEngine.Random.Range(0, playerSpawnPositions.Count - 1)].position;
    }
}
