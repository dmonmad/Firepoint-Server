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
    public int holdedByPlayer;

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
        isReloading = false;
        isNextShotReady = true;
        preparingNextShot = false;
        isFiring = false;
        shotsPerSecond = 1f / fireRate;
        holdedByPlayer = -1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Player _player = other.GetComponent<Player>();
            WeaponManager _wm = other.GetComponent<WeaponManager>();
            if (_wm.AttemptToPickUp(this, _player.weaponHolder, _player.id) && _player)
            {
                ItemPickedUp(_player.id);
            }

        }
    }

    private void ItemPickedUp(int id)
    {
        Debug.Log("Item guardado, enviando ");
        weaponCollider.enabled = false;
        modelCollider.enabled = false;
        holdedByPlayer = id;
    }

    public void ItemDropped()
    {
        weaponCollider.enabled = true;
        modelCollider.enabled = true;
        holdedByPlayer = -1;
    }

    public bool Shoot(Transform _shootOrigin, Vector3 _shootDirection)
    {
        if (isNextShotReady)
        {
            if (clip > 0 && !isReloading)
            {

                switch (weaponShootType)
                {
                    case WeaponShootType.Semi:
                        SemiShot(_shootOrigin, _shootDirection);
                        if (clip <= 0)
                        {
                            TryToReload();
                        }
                        ServerSend.UpdateWeaponBullets(this);
                        return true;

                    case WeaponShootType.Automatic:
                        AutomaticShot(_shootOrigin, _shootDirection);
                        if (clip <= 0)
                        {
                            TryToReload();
                        }
                        ServerSend.UpdateWeaponBullets(this);
                        return true;
                }
            }
        }
        else
        {
            Debug.LogWarning("Packet is arriving too fast");
            if (!preparingNextShot)
            {
                StartCoroutine(PrepareNextShot());
            }
        }
        return false;
    }

    IEnumerator PrepareNextShot()
    {
        preparingNextShot = true;
        yield return new WaitForSeconds(shotsPerSecond);
        preparingNextShot = false;
        isNextShotReady = true;
    }

    public void AutomaticShot(Transform _shootOrigin, Vector3 _shootDirection)
    {
        RayShoot(_shootOrigin, _shootDirection);
        isFiring = true;
        isNextShotReady = false;
        clip--;

        if (!preparingNextShot)
        {
            StartCoroutine(PrepareNextShot());
        }
    }

    public void SemiShot(Transform _shootOrigin, Vector3 _shootDirection)
    {
        RayShoot(_shootOrigin, _shootDirection);
        isNextShotReady = false;
        clip--;

        if (!preparingNextShot)
        {
            StartCoroutine(PrepareNextShot());
        }
    }

    public void TryToReload()
    {
        if (!isReloading)
        {
            if (ammo > 0)
            {
                Debug.Log("!isReloading = false and starting reload");
                StartCoroutine(Reload());
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);

        int newClipAfterReload = 0;
        int newAmmoAfterReload = 0;

        if (ammo == 0)
        {
            newClipAfterReload = clip;
            newAmmoAfterReload = 0;
        }
        else 
        {
            int ammoDifference = (clip + ammo) - maxClip;
            if (ammoDifference >= 0)
            {
                newClipAfterReload = maxClip;
                newAmmoAfterReload = Mathf.Abs(ammoDifference);
            }
            else
            {
                newClipAfterReload = clip + ammo;
                newAmmoAfterReload = 0;
            }
        }

        clip = newClipAfterReload;
        ammo = newAmmoAfterReload;
        isReloading = false;
        Debug.Log("Reloading and sending new bullets [" + clip + "] and ammo = [" + ammo + "]");
        ServerSend.UpdateWeaponBullets(this);
    }

    public void RayShoot(Transform _shootOrigin, Vector3 _shootDirection)
    {

        if (Physics.Raycast(_shootOrigin.position, _shootDirection, out RaycastHit _hit, 999f))
        {
            if (_hit.collider.CompareTag("Player"))
            {
                _hit.collider.GetComponent<Player>().TakeDamage(damagePerShot);
            }
            else
            {
                ServerSend.ApplyDecal(_hit.point, Quaternion.FromToRotation(Vector3.forward, _hit.normal));
            }
        }

    }

    private void OnEnable()
    {
        isReloading = false;

        if (preparingNextShot)
        {
            StartCoroutine(PrepareNextShot());
        }

        if (clip < 0)
        {
            StartCoroutine(Reload());
        }
    }

}
