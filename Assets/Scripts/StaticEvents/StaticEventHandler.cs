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

    public static event Action<PointsScoredAgrs> OnPointsScored;

    public static void CallPointScoredEvent(int points){
        OnPointsScored?.Invoke(new PointsScoredAgrs(){
            points = points
        });
    }

    public static event Action<ScoreChangedArgs> OnScoreChanged;

    public static void CallScoreChangedEvent(long score){
        OnScoreChanged?.Invoke(new ScoreChangedArgs(){
            score = score
        });
    }
}

public class RoomChangedEventArgs : EventArgs{
    public Room room;
}

public class RoomEnemiesDefeatedArgs : EventArgs{
    public Room room;
}

public class PointsScoredAgrs : EventArgs{
    public int points;
}

public class ScoreChangedArgs : EventArgs{
    public long score;
}
