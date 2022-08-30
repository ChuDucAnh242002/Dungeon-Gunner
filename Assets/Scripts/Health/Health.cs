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
    private Player player;
    [HideInInspector] public bool isDamageable = true;
    [HideInInspector] public Enemy enemy;

    private void Awake(){
        healthEvent = GetComponent<HealthEvent>();
    }

    private void Start(){
        CallHealthEvent(0);

        player = GetComponent<Player>();
        enemy = GetComponent<Enemy>();
    }

    private void CallHealthEvent(int damageAmount){
        healthEvent.CallHealthChangedEvent(((float) currentHealth / (float) startingHealth), currentHealth, damageAmount);
    }

    public void TakeDamge(int damageAmount){
        bool isRolling = false;

        if (player != null){
            isRolling = player.playerControl.isPlayerRolling;
        }

        if (isDamageable && !isRolling){
            currentHealth -= damageAmount;
            CallHealthEvent(damageAmount);
        }

        if (isDamageable && isRolling){
            Debug.Log("Dodge bullet by rolling");
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
