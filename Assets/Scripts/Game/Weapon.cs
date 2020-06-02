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

    /// <summary>Disables the colliders from the weapon's object so it doesn't collide while holded and set the holder's id.</summary>
    /// <param name="_playerId">The player's id who picked the weapon up.</param>
    private void ItemPickedUp(int _playerId)
    {
        weaponCollider.enabled = false;
        modelCollider.enabled = false;
        holdedByPlayer = _playerId;
    }

    /// <summary>Enables the colliders from the weapon's object and sets the holder's id to -1.</summary>
    public void ItemDropped()
    {
        weaponCollider.enabled = true;
        modelCollider.enabled = true;
        holdedByPlayer = -1;
    }

    /// <summary>Checks if the weapon can shoot and, if it do, shoots.</summary>
    /// <param name="_shootOrigin">The origin where the shot will be originated.</param>
    /// <param name="_shootDirection">The vector that the shot will travel.</param>
    /// <returns>Returns whether the shot was executed or not</returns>
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


    /// <summary>Prepares the next shot in the time specified.</summary>
    IEnumerator PrepareNextShot()
    {
        preparingNextShot = true;
        yield return new WaitForSeconds(shotsPerSecond);
        preparingNextShot = false;
        isNextShotReady = true;
    }

    /// <summary>Takes a shot in automatic mode.</summary>
    /// <param name="_shootOrigin">The origin where the shot will be originated.</param>
    /// <param name="_shootDirection">The vector that the shot will travel.</param>
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

    /// <summary>Takes a shot in semi-automatic mode.</summary>
    /// <param name="_shootOrigin">The origin where the shot will be originated.</param>
    /// <param name="_shootDirection">The vector that the shot will travel.</param>
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

    /// <summary>Tries to reload the weapon checking ammo left.</summary>
    public void TryToReload()
    {
        if (!isReloading)
        {
            if (ammo > 0 && clip < maxClip)
            {
                StartCoroutine(Reload());
            }
        }
    }

    /// <summary>Reloads the weapon substracting bullets from the ammo untill the magazine is full.</summary>
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

    /// <summary>Takes a shot. If a surface is hit, a decal is spawned. If a player is, the player takes damage.</summary>
    /// <param name="_shootOrigin">The origin where the shot will be originated.</param>
    /// <param name="_shootDirection">The vector that the shot will travel.</param>
    public void RayShoot(Transform _shootOrigin, Vector3 _shootDirection)
    {

        if (Physics.Raycast(_shootOrigin.position, _shootDirection, out RaycastHit _hit, 999f))
        {
            if (_hit.collider.CompareTag("Player"))
            {
                _hit.collider.GetComponent<Player>().TakeDamage(damagePerShot, holdedByPlayer);
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
