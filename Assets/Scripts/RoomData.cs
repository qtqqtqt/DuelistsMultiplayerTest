using Unity.Netcode;
using Unity.Collections;
using System;

public struct RoomData : INetworkSerializable, IEquatable<RoomData>
{
    public FixedString32Bytes roomName;
    public FixedString32Bytes password; 
    public ulong hostId;
    public bool isPrivate;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref roomName);
        serializer.SerializeValue(ref password);
        serializer.SerializeValue(ref hostId);
        serializer.SerializeValue(ref isPrivate);
    }

    public bool Equals(RoomData other)
    {
        return roomName.Equals(other.roomName)
            && password.Equals(other.password)
            && hostId.Equals(other.hostId)
            && isPrivate == other.isPrivate;
    }

    public override bool Equals(object obj)
    {
        return obj is RoomData other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + roomName.GetHashCode();
            hash = hash * 31 + password.GetHashCode();
            hash = hash * 31 + hostId.GetHashCode();
            hash = hash * 31 + isPrivate.GetHashCode();
            return hash;
        }
    }
}
