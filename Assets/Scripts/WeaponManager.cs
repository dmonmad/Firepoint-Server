﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject[] Inventory = new GameObject[3];
    public int selectedIndex = -1;
    public int lastSelectedIndex = -1;
    public float dropForce = 30f;

    public void Start()
    {
        selectedIndex = -1;
    }

    public bool AttemptToPickUp(Weapon _weapon)
    {
        Debug.Log("AttempToPickup");
        for (int i = 0; i < Inventory.Length; i++)
        {
            if (Inventory[i] == null)
            {
                Debug.Log("Guardando arma en el slot " + i);
                Inventory[i] = _weapon.gameObject;
                selectedIndex = i;
                Rigidbody _rb = Inventory[i].GetComponent<Rigidbody>();
                if (_rb)
                {
                    _rb.isKinematic = true;
                    return true;
                }
                return true;
            }
        }

        return false;
    }

    public bool DropActualWeapon(Vector3 _dropVector, Transform _weaponDropper)
    {
        Debug.Log("DropActualWeapon");
        if (Inventory[selectedIndex])
        {
            Debug.Log("DropActualWeapon - Selected Weapon");
            if (Inventory[selectedIndex].activeInHierarchy)
            {
                Debug.Log("DropActualWeapon - Selected Weapon - Active");
                Rigidbody _rb = Inventory[selectedIndex].GetComponent<Rigidbody>();
                if (_rb)
                {
                    Debug.Log("DropActualWeapon - Selected Weapon - Active - RB");
                    Weapon _weapon = Inventory[selectedIndex].GetComponent<Weapon>();
                    if (_weapon)
                    {
                        _weapon.ItemDropped();
                        Debug.Log("DropActualWeapon - Selected Weapon - Active - RB - WEAPON");
                        Inventory[selectedIndex].transform.parent = null;
                        Inventory[selectedIndex].transform.position = _weaponDropper.position;
                        Inventory[selectedIndex] = null;
                        selectedIndex = -1;
                        _rb.isKinematic = false;
                        _rb.AddForce(gameObject.transform.forward * dropForce, ForceMode.Force);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void FireWeapon(Transform _shootOrigin, Vector3 _shootDirection)
    {
        if (selectedIndex != -1)
        {
            Inventory[selectedIndex].GetComponent<Weapon>().Shoot(_shootOrigin, _shootDirection);
        }

    }

}