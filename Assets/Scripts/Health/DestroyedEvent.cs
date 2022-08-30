using System;
using UnityEngine;

public class DestroyedEvent : MonoBehaviour
{
    public event Action<DestroyedEvent, DestroyedEventArgs> OnDestroyed;

    public void CallDestroyedEvent(bool playerDied){
        OnDestroyed?.Invoke(this, new DestroyedEventArgs(){
            playerDied = playerDied
        });
    }
}

public class DestroyedEventArgs: EventArgs{
    public bool playerDied;
}


