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

    /// <summary>Initializes the player with the given values and sets the health to the max value possible.</summary>
    /// <param name="_id">The player's id.</param>
    /// <param name="_username">The player's username.</param>
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

    /// <summary>Try to drop the weapon and, if it does, sends the command to the other players. .</summary>
    /// <param name="_dropVector">The origin where the shot will be originated.</param>
    public void DropWeapon(Vector3 _dropVector)
    {
        if (health <= 0)
        {
            return;
        }

        if (weaponManager.DropActualWeapon(_dropVector, weaponDropper))
        {
            ServerSend.PlayerThrowWeapon(_dropVector, id);
            Debug.Log(username + " dropped a gun");
        }
    }

    /// <summary>Shoots the weapon.</summary>
    /// <param name="_viewDirection">The vector the shoot will follow.</param>
    public void Shoot(Vector3 _viewDirection)
    {
        if (health <= 0)
        {
            return;
        }

        weaponManager.FireWeapon(shootOrigin, _viewDirection);
        
    }

    /// <summary>Takes damage and kills the player if under 0.</summary>
    /// <param name="_damage">The origin where the shot will be originated.</param>
    /// <param name="_attackerId">The vector that the shot will travel.</param>
    public void TakeDamage(float _damage, int _attackerId)
    {
        
        if (health <= 0)
        {
            return;
        }

        health -= _damage;
        Debug.Log(username + "takes " + _damage + " of damage");
        if (health <= 0f)
        {
            health = 0f;
            controller.enabled = false;
            transform.position = new Vector3(0f, 25f, 0f);
            ServerSend.PlayerPosition(this);
            StartCoroutine(Respawn());
        }

        ServerSend.PlayerHealth(this, _attackerId);
    }

    /// <summary>Respawns after delay, setting full health and enabling controller 
    /// then sends the event to the other players.</summary>
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);

        health = maxHealth;
        controller.enabled = true;
        ServerSend.PlayerRespawned(this);
    }
    


}