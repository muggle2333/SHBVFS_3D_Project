using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public struct PlayerIdData : IEquatable<PlayerIdData>, INetworkSerializable
{

    public ulong clientId;
    public FixedString64Bytes playerName;
    public FixedString64Bytes playerId;


    public bool Equals(PlayerIdData other)
    {
        return
            clientId == other.clientId &&
            playerName == other.playerName &&
            playerId == other.playerId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref playerId);
    }

}