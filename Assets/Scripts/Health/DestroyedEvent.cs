using System;
using UnityEngine;

public class DestroyedEvent : MonoBehaviour
{
    public event Action<DestroyedEvent, DestroyedEventArgs> OnDestroyed;

    public void CallDestroyedEvent(bool playerDied, int points){
        OnDestroyed?.Invoke(this, new DestroyedEventArgs(){
            playerDied = playerDied,
            points = points
        });
    }
}

public class DestroyedEventArgs: EventArgs{
    public bool playerDied;
    public int points;
}


