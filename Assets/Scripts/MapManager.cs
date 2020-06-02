using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static List<Transform> itemSpawnPositions = new List<Transform>();
    public static List<Transform> playerSpawnPositions = new List<Transform>();

    /// <summary>Searchs the WeaponSpawners and PlayerSpawners then spawns the weapons.</summary>
    /// <param name="_nm">The object that has the weapon prefabs stored.</param>
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

    /// <summary>Spawns random weapons in the WeaponSpawners detected.</summary>
    /// <param name="_nm">The object that has the weapon prefabs stored.</param>
    private static void SetUp(NetworkManager _nm)
    {
        foreach (Transform itemSpawnerTransform in itemSpawnPositions)
        {
            Instantiate(_nm.weaponPrefabs[UnityEngine.Random.Range(0, _nm.weaponPrefabs.Length)], itemSpawnerTransform.position, Quaternion.identity);
        }
    }

    /// <summary>Returns a position of a random PlayerSpawner</summary>
    /// <returns>Position of random PlayerSpawner</returns>
    public static Vector3 GetRandomPlayerSpawner()
    {
        return playerSpawnPositions[UnityEngine.Random.Range(0, playerSpawnPositions.Count - 1)].position;
    }

    /// <summary>Returns a position of a random ItemSpawner</summary>
    /// <returns>Position of random ItemSpawner</returns>
    public static Vector3 GetRandomWeaponSpawner()
    {
        return itemSpawnPositions[UnityEngine.Random.Range(0, playerSpawnPositions.Count - 1)].position;
    }
}
