using System;
using UnityEngine;

public class HealthBarEvent : MonoBehaviour
{
    public event Action<HealthBarEvent, HealthBarEventArgs> OnHealthBarChanged;

    public void CallHealthBarChangedEvent(float healthPerCent){
        OnHealthBarChanged?.Invoke(this, new HealthBarEventArgs(){
            healthPercent = healthPerCent
        });
    }
}

public class HealthBarEventArgs : EventArgs {
    public float healthPercent;
}