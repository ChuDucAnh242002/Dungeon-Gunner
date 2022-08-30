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
    private Coroutine immunityCoroutine;
    private bool isImmuneAfterHit = false;
    private float immunityTime = 0f;
    private SpriteRenderer spriteRenderer = null;
    private const float spriteFlashInterval = 0.2f;
    private WaitForSeconds WaitForSecondsSpriteFlashInterval = new WaitForSeconds(spriteFlashInterval);

    [HideInInspector] public bool isDamageable = true;
    [HideInInspector] public Enemy enemy;

    private void Awake(){
        healthEvent = GetComponent<HealthEvent>();
    }

    private void Start(){
        CallHealthEvent(0);

        player = GetComponent<Player>();
        enemy = GetComponent<Enemy>();

        if (player != null){
            if (player.playerDetails.isImmuneAfterHit){
                isImmuneAfterHit = true;
                immunityTime = player.playerDetails.hitImmunityTime;
                spriteRenderer = player.spriteRenderer;
            }
        }
        // Optional for enemy, especially bosses
        else if (enemy != null){
            if (enemy.enemyDetails.isImmuneAfterHit){
                isImmuneAfterHit = true;
                immunityTime = enemy.enemyDetails.hitImmunityTime;
                spriteRenderer = enemy.spriteRendererArray[0];
            }
        }
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

            PostHitImmunity();
        }

    }

    private void PostHitImmunity(){
        if (gameObject.activeSelf == false){
            return;
        }

        if (isImmuneAfterHit){
            if (immunityCoroutine != null){
                StopCoroutine(immunityCoroutine);
            }

            immunityCoroutine = StartCoroutine(PostHitImmunityRoutine(immunityTime, spriteRenderer));
        }
    }

    private IEnumerator PostHitImmunityRoutine(float immunityTime, SpriteRenderer spriteRenderer){
        int iterations = Mathf.RoundToInt(immunityTime / spriteFlashInterval / 2f);
        isDamageable = false;

        while (iterations > 0){
            spriteRenderer.color = Color.red;
            yield return WaitForSecondsSpriteFlashInterval;
            spriteRenderer.color = Color.white;
            yield return WaitForSecondsSpriteFlashInterval;
            iterations--;
            yield return null;
        }

        isDamageable = true;
    }

    public void SetStartingHealth(int startingHealth){
        this.startingHealth = startingHealth;
        currentHealth = startingHealth;
    }

    public int GetStartingHealth(){
        return startingHealth;
    }
}
