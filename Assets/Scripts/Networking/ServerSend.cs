using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend
{

    public static int nextUpdateWeaponBulletsPacketId = 1;
    /// <summary>Sends a packet to a client via TCP.</summary>
    /// <param name="_toClient">The client to send the packet the packet to.</param>
    /// <param name="_packet">The packet to send to the client.</param>
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }

    /// <summary>Sends a packet to a client via UDP.</summary>
    /// <param name="_toClient">The client to send the packet the packet to.</param>
    /// <param name="_packet">The packet to send to the client.</param>
    private static void SendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].udp.SendData(_packet);
    }

    /// <summary>Sends a packet to all clients via TCP.</summary>
    /// <param name="_packet">The packet to send.</param>
    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(_packet);
        }
    }

    /// <summary>Sends a packet to all clients except one via TCP.</summary>
    /// <param name="_exceptClient">The client to NOT send the data to.</param>
    /// <param name="_packet">The packet to send.</param>
    private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
    }

    /// <summary>Sends a packet to all clients via UDP.</summary>
    /// <param name="_packet">The packet to send.</param>
    private static void SendUDPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].udp.SendData(_packet);
        }
    }

    /// <summary>Sends a packet to all clients except one via UDP.</summary>
    /// <param name="_exceptClient">The client to NOT send the data to.</param>
    /// <param name="_packet">The packet to send.</param>
    private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].udp.SendData(_packet);
            }
        }
    }

    #region Packets
    /// <summary>Sends a welcome message to the given client.</summary>
    /// <param name="_toClient">The client to send the packet to.</param>
    /// <param name="_msg">The message to send.</param>
    public static void Welcome(int _toClient, string _msg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            _packet.Write(_msg);
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);
        }
    }

    /// <summary>Tells a client to spawn a player.</summary>
    /// <param name="_toClient">The client that should spawn the player.</param>
    /// <param name="_player">The player to spawn.</param>
    public static void SpawnPlayer(int _toClient, Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.username);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.transform.rotation);
            //_packet.Write(_player.weaponManager.Inventory[_player.weaponManager.selectedIndex].GetComponent<Weapon>().itemId);

            SendTCPData(_toClient, _packet);
        }
    }

    /// <summary>Sends a player's updated position to all clients.</summary>
    /// <param name="_player">The player whose position to update.</param>
    public static void PlayerPosition(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.position);
            _packet.Write(Time.deltaTime);

            SendUDPDataToAll(_packet);
        }
    }

    /// <summary>Sends a player's updated rotation to all clients except to himself (to avoid overwriting the local player's rotation).</summary>
    /// <param name="_player">The player whose rotation to update.</param>
    public static void PlayerRotation(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRotation))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.rotation);

            SendUDPDataToAll(_player.id, _packet);
        }
    }

    /// <summary>Sends a player's disconnection to all clients except to himself (since he's already disconnected).</summary>
    /// <param name="_playerId">The player id that disconnected.</param>
    public static void PlayerDisconnect(int _playerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            _packet.Write(_playerId);

            SendTCPDataToAll(_packet);
        }
    }

    /// <summary>Sends a player's updated health to all clients.</summary>
    /// <param name="_player">The player whose health to update.</param>
    public static void PlayerHealth(Player _player, int _attackerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerHealth))
        {
            _packet.Write(_player.id);
            _packet.Write(_attackerId);
            _packet.Write(_player.health);

            SendTCPDataToAll(_packet);
        }
    }

    /// <summary>Sends a player's updated rotation to all clients except to himself (to avoid overwriting the local player's rotation).</summary>
    /// <param name="_player">The player whose rotation to update.</param>
    public static void PlayerRespawned(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRespawned))
        {
            _packet.Write(_player.id);

            SendTCPDataToAll(_packet);
        }
    }

    /// <summary>Sends a player's updated rotation to all clients except to himself (to avoid overwriting the local player's rotation).</summary>
    /// <param name="_player">The player whose rotation to update.</param>
    public static void ApplyDecal(Vector3 _hitPosition, Quaternion _hitRotation)
    {
        using (Packet _packet = new Packet((int)ServerPackets.applyDecal))
        {
            _packet.Write(_hitPosition);
            _packet.Write(_hitRotation);

            SendTCPDataToAll(_packet);
        }
    }

    public static void SpawnItem(int _toClient, int _weaponId, int _itemId, Vector3 _itemPosition, int _heldBy, int _bullets, int _ammo)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnItem))
        {
            _packet.Write(_weaponId);
            _packet.Write(_itemId);
            _packet.Write(_itemPosition);
            _packet.Write(_heldBy);
            _packet.Write(_bullets);
            _packet.Write(_ammo);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void ItemPickedUp(int _itemId, int _byPlayer)
    {
        using (Packet _packet = new Packet((int)ServerPackets.itemPickedUp))
        {
            _packet.Write(_itemId);
            _packet.Write(_byPlayer);

            SendTCPDataToAll(_packet);
        }
    }

    public static void PlayerThrowWeapon(Vector3 _facing, int _byPlayer)
    {
        using (Packet _packet = new Packet((int)ServerPackets.itemThrown))
        {
            _packet.Write(_facing);
            _packet.Write(_byPlayer);

            SendTCPDataToAll(_byPlayer, _packet);
        }
    }

    public static void UpdateWeaponBullets(Weapon _weapon)
    {
        int _itemId = _weapon.itemId;
        int _actualClip = _weapon.clip;
        int _actualAmmo = _weapon.ammo;

        using (Packet _packet = new Packet((int)ServerPackets.updateWeaponBullets))
        {
            _packet.Write(nextUpdateWeaponBulletsPacketId);
            _packet.Write(_itemId);
            _packet.Write(_actualClip);
            _packet.Write(_actualAmmo);

            SendTCPDataToAll(_packet);
        }

        nextUpdateWeaponBulletsPacketId++;
    }

    public static void PlayerChangeWeapon(int _byPlayer, int _index)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerChangedWeapon))
        {
            _packet.Write(_byPlayer);
            _packet.Write(_index);

            SendTCPDataToAll(_packet);
        }
    }

    public static void PlayerShot(int _byPlayer)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerShot))
        {
            _packet.Write(_byPlayer);

            SendTCPDataToAll(_byPlayer, _packet);
        }
    }

    #endregion
}