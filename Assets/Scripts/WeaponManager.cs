using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject[] Inventory = new GameObject[3];
    public GameObject primary;
    public GameObject secondary;
    public GameObject knife;
    public int selectedIndex = -1;
    public int lastSelectedIndex = -1;
    public float dropForce = 30f;

    public void Start()
    {
        selectedIndex = -1;
        dropForce = Constants.DROP_WEAPON_FORCE;
    }

    public bool AttemptToPickUp(Weapon _weapon, Transform _weaponHolder, int _byPlayer)
    {
        for (int i = 0; i < Inventory.Length; i++)
        {
            if (Inventory[i] == null)
            {
                Inventory[i] = _weapon.gameObject;

                if (selectedIndex == -1)
                {
                    selectedIndex = i;
                }

                Rigidbody _rb = _weapon.GetComponent<Rigidbody>();
                if (_rb)
                {
                    _rb.isKinematic = true;
                }
                _weapon.gameObject.transform.parent = _weaponHolder;
                _weapon.gameObject.transform.localPosition = Vector3.zero;
                _weapon.gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
                ServerSend.ItemPickedUp(_weapon.itemId, _byPlayer);
                return true;
            }
        }

        return false;
    }

    public bool DropActualWeapon(Vector3 _dropVector, Transform _weaponDropper)
    {
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
                        _rb.AddForce(_dropVector * dropForce, ForceMode.Force);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void DropAllWeapons(Transform _weaponDropper)
    {
        for (int i = 0; i < Inventory.Length; i++)
        {
            if (Inventory[i])
            {
                selectedIndex = i;
                Debug.Log(Inventory[selectedIndex].name);
                Inventory[selectedIndex].SetActive(true);
                DropActualWeapon(gameObject.transform.forward, _weaponDropper);
            }
        }
    }

    public void FireWeapon(Transform _shootOrigin, Vector3 _shootDirection)
    {
        if (selectedIndex != -1)
        {
            Inventory[selectedIndex].GetComponent<Weapon>().Shoot(_shootOrigin, _shootDirection);
        }

    }

    public void ChangeWeapon(int _index)
    {
        if (Inventory[_index] != null)
        {
            if (_index != selectedIndex)
            {
                ServerSend.PlayerChangeWeapon(GetComponent<Player>().id, _index);
                if (selectedIndex != -1)
                    Inventory[selectedIndex].SetActive(false);
                selectedIndex = _index;
                Inventory[selectedIndex].SetActive(true);
            }
        }
    }
}
