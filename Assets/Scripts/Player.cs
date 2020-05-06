using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;
    public CharacterController controller;
    public WeaponManager weaponManager;
    public float gravity = -9.81f;
    public Transform shootOrigin;
    public Transform weaponHolder;
    public Transform weaponDropper;
    public float moveSpeed = 5f;
    public float jumpSpeed = 5f;
    public float health;
    public float maxHealth = 100;

    private bool[] inputs;
    private float yVelocity = 0;

    private void Start()
    {
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;
        jumpSpeed *= Time.fixedDeltaTime;
    }

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        health = maxHealth;

        inputs = new bool[5];
    }

    /// <summary>Processes player input and moves the player.</summary>
    public void FixedUpdate()
    {
        if (health <= 0)
        {
            return;
        }

        Vector2 _inputDirection = Vector2.zero;
        if (inputs[0]) //w
        {
            _inputDirection.y += 1;
        }
        if (inputs[1]) //s
        {
            _inputDirection.y -= 1;
        }
        if (inputs[2]) //a
        {
            _inputDirection.x -= 1;
        }
        if (inputs[3]) //d
        {
            _inputDirection.x += 1;
        }
        

        Move(_inputDirection);
    }

    /// <summary>Calculates the player's desired movement direction and moves him.</summary>
    /// <param name="_inputDirection"></param>
    private void Move(Vector2 _inputDirection)
    {
        Vector3 _moveDirection = transform.right * _inputDirection.x + transform.forward * _inputDirection.y;
        _moveDirection *= moveSpeed;

        if (controller.isGrounded)
        {
            yVelocity = 0f;
            if (inputs[4])
            {
                yVelocity = jumpSpeed;
            }
        }
        yVelocity += gravity;

        _moveDirection.y = yVelocity;
        controller.Move(_moveDirection);

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }

    /// <summary>Updates the player input with newly received input.</summary>
    /// <param name="_inputs">The new key inputs.</param>
    /// <param name="_rotation">The new rotation.</param>
    public void SetInput(bool[] _inputs, Quaternion _rotation)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
    }

    public void DropWeapon(Vector3 _dropVector)
    {
        Debug.Log("DropWeapon");
        if (health <= 0)
        {
            return;
        }

        if (weaponManager.DropActualWeapon(_dropVector, weaponDropper))
        {
            ServerSend.PlayerThrowWeapon(_dropVector, id);
        }
    }

    public void Shoot(Vector3 _viewDirection)
    {
        if (health <= 0)
        {
            return;
        }

        weaponManager.FireWeapon(shootOrigin, _viewDirection);
        
    }

    public void TakeDamage(float _damage)
    {
        Debug.Log("Take Damage on "+username);
        if (health <= 0)
        {
            return;
        }

        Debug.Log("Take Damage health - damage " + username);
        health -= _damage;
        if (health <= 0f)
        {
            Debug.Log("Take Damage on " + username);
            health = 0f;
            controller.enabled = false;
            transform.position = new Vector3(0f, 25f, 0f);
            ServerSend.PlayerPosition(this);
            StartCoroutine(Respawn());
        }

        ServerSend.PlayerHealth(this);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);

        health = maxHealth;
        controller.enabled = true;
        ServerSend.PlayerRespawned(this);
    }
    


}