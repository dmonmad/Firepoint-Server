using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum WeaponID
    {
        Cuchillo = 1,
        Glock,
        M3,
        AK47,
        M4,
        L96
    }

    public enum WeaponTier
    {
        Primary,
        Secondary,
        Cuchillo,
        Equipment
    }

    public enum WeaponShootType
    {
        Semi,
        Automatic
    }

    public static Dictionary<int, GameObject> weapons = new Dictionary<int, GameObject>();
    public static Dictionary<int, Weapon> items = new Dictionary<int, Weapon>();


    private static int nextItemId = 1;

    public WeaponID weaponName = WeaponID.Cuchillo;
    public WeaponTier weaponTier = WeaponTier.Cuchillo;
    public WeaponShootType weaponShootType = WeaponShootType.Automatic;
    public BoxCollider weaponCollider;
    public MeshCollider modelCollider;
    public int weaponId;
    public int itemId;

    public int damagePerShot = 0;
    public bool isReloading;
    public bool preparingNextShot;
    public bool isFiring;
    public bool isNextShotReady;
    public float reloadTime = 0;
    public int maxAmmo = 0;
    public int ammo = 0;
    public int maxClip = 0;
    public int clip = 0;
    public float fireRate = 0;
    public float shotsPerSecond = 0;

    private void Start()
    {
        itemId = nextItemId;
        nextItemId++;
        items.Add(itemId, this);
        weaponCollider = GetComponent<BoxCollider>();
        modelCollider = GetComponentInChildren<MeshCollider>();
        ammo = maxAmmo;
        clip = maxClip;
        isReloading = true;
        isNextShotReady = true;
        preparingNextShot = false;
        isFiring = false;
        shotsPerSecond = 1f / fireRate;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Tag = Player");
            Player _player = other.GetComponent<Player>();
            WeaponManager _wm = other.GetComponent<WeaponManager>();
            if (_wm.AttemptToPickUp(this) && _player && _wm)
            {
                Debug.Log("Picked up");
                ItemPickedUp(_player.id, _player.weaponHolder);
            }

        }
    }

    private void ItemPickedUp(int _byPlayer, Transform _weaponHolder)
    {
        Debug.Log("Item guardado, enviando ");
        gameObject.transform.parent = _weaponHolder;
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
        weaponCollider.enabled = false;
        modelCollider.enabled = false;
        ServerSend.ItemPickedUp(itemId, _byPlayer);
        
    }

    public void ItemDropped()
    {
        weaponCollider.enabled = true;
        modelCollider.enabled = true;
    }

    public void Shoot(Transform _shootOrigin, Vector3 _shootDirection)
    {
        if (isNextShotReady)
        {
            if (clip > 0)
            {
                switch (weaponShootType)
                {
                    case WeaponShootType.Semi:
                        SemiShot(_shootOrigin.forward);

                        break;

                    case WeaponShootType.Automatic:
                        AutomaticShot(_shootOrigin.forward);

                        break;
                }
            }
            else
            {
                Reload();
            }
        }
        else
        {
            if (!preparingNextShot)
            {
                StartCoroutine(PrepareNextShot());
            }
        }        
    }

    IEnumerator PrepareNextShot()
    {
        preparingNextShot = true;
        yield return new WaitForSeconds(shotsPerSecond);
        preparingNextShot = false;
        isNextShotReady = true;
    }

    public void AutomaticShot(Vector3 _shootDirection)
    {
        ClientSend.PlayerShoot(_shootDirection);
        isFiring = true;
        isNextShotReady = false;
        clip--;

        if (!preparingNextShot)
        {
            StartCoroutine(PrepareNextShot());
        }
    }

    public void SemiShot(Vector3 _shootDirection)
    {
        ClientSend.PlayerShoot(_shootDirection);
        isNextShotReady = false;
        clip--;

        if (!preparingNextShot)
        {
            StartCoroutine(PrepareNextShot());
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        clip = maxClip;
        isReloading = false;
    }

}
