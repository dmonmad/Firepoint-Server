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

    /// <summary>Changes the selected weapon.</summary>
    /// <param name="_weapon">The Weapon that the player will try to pick up.</param>
    /// <param name="_weaponHolder">The position where the weapon should be when picked up.</param>
    /// <param name="_byPlayer">The id of the player that tries to pick the weapon up.</param>
    /// <returns>Returns true if the weapon was picked up, otherwise false</returns>
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

    /// <summary>Drops the selected weapon.</summary>
    /// <param name="_dropVector">The direction the weapon will follow when dropped.</param>
    /// <param name="_weaponDropper">The position where the weapons will be initially dropped.</param>
    /// <returns>If the weapon could be dropped, it will return true, otherwise false</returns>
    public bool DropActualWeapon(Vector3 _dropVector, Transform _weaponDropper)
    {
        if (Inventory[selectedIndex])
        {
            if (Inventory[selectedIndex].activeInHierarchy)
            {
                Rigidbody _rb = Inventory[selectedIndex].GetComponent<Rigidbody>();
                if (_rb)
                {
                    Weapon _weapon = Inventory[selectedIndex].GetComponent<Weapon>();
                    if (_weapon)
                    {
                        _weapon.ItemDropped();
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

    /// <summary>Drops all the weapons holded by the player.</summary>
    /// <param name="_weaponDropper">The position where the weapons will be initially dropped.</param>
    public void DropAllWeapons(Transform _weaponDropper)
    {
        for (int i = 0; i < Inventory.Length; i++)
        {
            if (Inventory[i])
            {
                selectedIndex = i;
                Inventory[selectedIndex].SetActive(true);
                DropActualWeapon(gameObject.transform.forward, _weaponDropper);
            }
        }
    }

    /// <summary>Tries to shoot the weapon and, if it does, sends the event to the other clients.</summary>
    /// <param name="_shootOrigin">The origin where the shot will be originated.</param>
    /// <param name="_shootDirection">The vector that the shot will travel.</param>
    public void FireWeapon(Transform _shootOrigin, Vector3 _shootDirection)
    {
        if (selectedIndex != -1)
        {
            if(Inventory[selectedIndex].GetComponent<Weapon>().Shoot(_shootOrigin, _shootDirection))
            {

                ServerSend.PlayerShot(GetComponent<Player>().id);
            }
        }
    }

    /// <summary>Tries to reload the weapon if one is selected.</summary>
    public void ReloadWeapon(int _id)
    {        
        if(selectedIndex != -1)
        {
            if (Inventory[selectedIndex])
            {
                Debug.Log(Server.clients[_id].player.username+ " is reloading");
                Inventory[selectedIndex].GetComponent<Weapon>().TryToReload();
            }
        }
    }

    /// <summary>Changes the selected weapon to the new one.</summary>
    /// <param name="_index">The index of the weapon to change in the inventory.</param>
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
