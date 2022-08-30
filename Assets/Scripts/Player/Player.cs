using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#region REQUIRE COMPONENTS
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(HealthEvent))]
[RequireComponent(typeof(DealContactDamage))]
[RequireComponent(typeof(ReceiveContactDamage))]
[RequireComponent(typeof(Destroyed))]
[RequireComponent(typeof(DestroyedEvent))]

[RequireComponent(typeof(PlayerControl))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(AimWeapon))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[RequireComponent(typeof(ActiveWeapon))]
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(FireWeapon))]
[RequireComponent(typeof(ReloadWeaponEvent))]
[RequireComponent(typeof(WeaponReloadedEvent))]
[RequireComponent(typeof(ReloadWeapon))]
[RequireComponent(typeof(StopReloadWeaponEvent))]

[RequireComponent(typeof(MovementByVelocityEvent))]
[RequireComponent(typeof(MovementByVelocity))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(AnimatePlayer))]

[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[DisallowMultipleComponent]
#endregion REQUIRE COMPONENTS
public class Player : MonoBehaviour
{
    [HideInInspector] public PlayerDetailsSO playerDetails;
    [HideInInspector] public Health health;
    [HideInInspector] public HealthEvent healthEvent;
    [HideInInspector] public DestroyedEvent destroyedEvent;
    [HideInInspector] public PlayerControl playerControl;
    [HideInInspector] public IdleEvent idleEvent;
    [HideInInspector] public AimWeaponEvent aimWeaponEvent;
    [HideInInspector] public SetActiveWeaponEvent setActiveWeaponEvent;
    [HideInInspector] public ActiveWeapon activeWeapon;
    [HideInInspector] public FireWeaponEvent fireWeaponEvent;
    [HideInInspector] public WeaponFiredEvent weaponFiredEvent;
    [HideInInspector] public ReloadWeaponEvent reloadWeaponEvent;
    [HideInInspector] public WeaponReloadedEvent weaponReloadedEvent;
    [HideInInspector] public StopReloadWeaponEvent stopReloadWeaponEvent;
    [HideInInspector] public MovementByVelocityEvent movementByVelocityEvent;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Animator animator;

    public List<Weapon> weaponList = new List<Weapon>();

    private void Awake(){
        health = GetComponent<Health>();
        healthEvent = GetComponent<HealthEvent>();
        destroyedEvent = GetComponent<DestroyedEvent>();
        playerControl = GetComponent<PlayerControl>();
        idleEvent = GetComponent<IdleEvent>();
        aimWeaponEvent = GetComponent<AimWeaponEvent>();
        setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
        activeWeapon = GetComponent<ActiveWeapon>();
        fireWeaponEvent = GetComponent<FireWeaponEvent>();
        weaponFiredEvent = GetComponent<WeaponFiredEvent>();
        reloadWeaponEvent = GetComponent<ReloadWeaponEvent>();
        weaponReloadedEvent = GetComponent<WeaponReloadedEvent>();
        stopReloadWeaponEvent = GetComponent<StopReloadWeaponEvent>();
        movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable(){
        healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
    }
    private void OnDisable(){
        healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    }

    private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs){

        if (healthEventArgs.healthAmount <= 0f){
            destroyedEvent.CallDestroyedEvent(true);
        }
    }

    public void Initialize(PlayerDetailsSO playerDetails){
        this.playerDetails = playerDetails;

        CreatePlayerStartingWeapon();

        SetPlayerHealth();
    }

    private void CreatePlayerStartingWeapon(){
        weaponList.Clear();

        foreach(WeaponDetailsSO weaponDetails in playerDetails.startingWeaponList){
            AddWeaponToPlayer(weaponDetails);
        }
    }

    private void SetPlayerHealth(){
        health.SetStartingHealth(playerDetails.playerHealthAmount);
    }

    public Vector3 GetPlayerPosition(){
        return transform.position;
    }

    private Weapon AddWeaponToPlayer(WeaponDetailsSO weaponDetails){
        Weapon weapon = new Weapon() {
            weaponDetails = weaponDetails,
            weaponReloadTimer = 0f,
            weaponClipRemainingAmmo = weaponDetails.weaponClipAmmoCapacity,
            weaponRemainingAmmo = weaponDetails.weaponAmmoCapacity,
            isWeaponReloading = false
        };

        weaponList.Add(weapon);

        weapon.weaponListPosition = weaponList.Count;

        setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);

        return weapon;
    }
}
