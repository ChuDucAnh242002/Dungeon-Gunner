using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthEvent))]
[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    private int startingHealth;
    private int currentHealth;
    private HealthEvent healthEvent;
    [HideInInspector] public bool isDamageable = true;

    private void Awake(){
        healthEvent = GetComponent<HealthEvent>();
    }

    private void Start(){
        CallHealthEvent(0);
    }

    private void CallHealthEvent(int damageAmount){
        healthEvent.CallHealthChangedEvent(((float) currentHealth / (float) startingHealth), currentHealth, damageAmount);
    }

    public void TakeDamge(int damageAmount){
        if (isDamageable){
            currentHealth -= damageAmount;
            CallHealthEvent(damageAmount);
        }
    }

    public void SetStartingHealth(int startingHealth){
        this.startingHealth = startingHealth;
        currentHealth = startingHealth;
    }

    public int GetStartingHealth(){
        return startingHealth;
    }
}
