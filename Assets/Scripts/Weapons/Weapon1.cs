using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon1 : MonoBehaviour
{
    public Dictionary<int, Weapon1> weapons = new Dictionary<int, Weapon1>();
    public static int nextWeaponId;

    public int weaponId;

    [Header("Throwing")]
    public float throwForce;
    public float throwExtraForce;
    public float rotationForce;

    [Header("Shooting")]
    public int maxAmmo;
    public int shotsPerSecond;
    public float reloadSpeed;
    public float damage;
    public bool tapable;

    [Header("Data")]
    public int weaponFxLayer;
    public GameObject weaponGfx;
    public Collider gfxCollider;

    private bool _held;
    private bool _reloading;
    private bool _shooting;
    private int _ammo;
    private Rigidbody _rb;
    private Transform _playerCamera;
 
    void Start()
    {
        weaponId = nextWeaponId;
        nextWeaponId++;
        weapons.Add(weaponId, this);
    }

    private void Update()
    {
        if (!_held) return;

        if(Input.GetKeyDown(KeyCode.R) && !_reloading && _ammo < maxAmmo)
        {
            StartCoroutine(ReloadCooldown());
        }

        if (tapable ? Input.GetMouseButtonDown(0) : Input.GetMouseButton(0) && !_shooting && !_reloading)
        {
            _ammo--;
            Shoot();
            StartCoroutine(_ammo <= 0 ? ReloadCooldown() : ShootingCooldown());
        }
    }

    private void Shoot()
    {
        Player p = transform.parent.gameObject.GetComponent<Player>();
        if(p != null)
        {
            
        }
    }

    private IEnumerator ShootingCooldown()
    {
        _shooting = true;
        yield return new WaitForSeconds(1f / shotsPerSecond);
        _shooting = false;
    }

    private IEnumerator ReloadCooldown()
    {
        _reloading = true;
        yield return new WaitForSeconds(reloadSpeed);
        _reloading = false;
    }
       
    public void Pickup(Transform _weaponHolder, Transform _playerCam)
    {
        if (_held) return;
        Destroy(_rb);
        transform.parent = _weaponHolder;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        gfxCollider.enabled = false;
        weaponGfx.layer = weaponFxLayer;
        _held = true;
        _playerCamera = _playerCam;
    }

    public void Drop(Transform _playerCamera)
    {
        if (!_held) return;
        _rb = gameObject.AddComponent<Rigidbody>();
        _rb.mass = 0.1f;
        Vector3 forward = _playerCamera.forward;
        forward.y = 0f;
        _rb.velocity = forward * throwForce;
        _rb.velocity += Vector3.up * throwExtraForce;
        _rb.angularVelocity = Random.onUnitSphere * rotationForce;
        gfxCollider.enabled = true;
        transform.parent = null;
        weaponGfx.layer = 0;
        _held = false;
    }
}
