using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticEventHandler 
{
    // Room changed event
    public static event Action<RoomChangedEventArgs> OnRoomChanged;

    public static void CallRoomChangedEvent(Room room){
        OnRoomChanged?.Invoke(new RoomChangedEventArgs(){
            room = room
        });
    }

    public static event Action<RoomEnemiesDefeatedArgs> OnRoomEnemiesDefeated;

    public static void CallRoomEnemiesDefeatedEvent(Room room){
        OnRoomEnemiesDefeated?.Invoke(new RoomEnemiesDefeatedArgs(){
            room = room
        });
    }
}

public class RoomChangedEventArgs : EventArgs{
    public Room room;
}

public class RoomEnemiesDefeatedArgs : EventArgs{
    public Room room;
}
