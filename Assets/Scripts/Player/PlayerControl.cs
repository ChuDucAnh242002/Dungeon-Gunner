using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class PlayerControl : MonoBehaviour
{
    #region Tooltip
    [Tooltip("MovementDetailsSO scriptable object containing movement details such as speed")]
    #endregion
    [SerializeField] private MovementDetailsSO movementDetails;

    private Player player;
    private bool leftMouseDownPreviousFrame = false;
    private int currentWeaponIndex = 1;
    private float moveSpeed;
    private Coroutine playerRollCoroutine;
    private WaitForFixedUpdate waitForFixedUpdate;
    [HideInInspector] public bool isPlayerRolling = false;
    private float playerRollCooldownTimer = 0f;
    [HideInInspector] private bool isPlayerMovementDisabled = false;

    private bool reverse = false;
    private int previousWeaponIndex = 0;
    private float fireTime = 0;

    private void Awake(){
        player = GetComponent<Player>();

        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Start(){
        waitForFixedUpdate = new WaitForFixedUpdate();

        SetStartingWeapon();

        SetPlayerAnimationSpeed();
    }

    private void SetStartingWeapon(){
        int index = 1;
        foreach(Weapon weapon in player.weaponList){
            if(weapon.weaponDetails == player.playerDetails.startingWeapon){
                SetWeaponByIndex(index);
                break;
            }
            index++ ;
        }
    }

    private void SetPlayerAnimationSpeed(){
        player.animator.speed = moveSpeed / Settings.baseSpeedForPlayerAnimation;
    }

    private void Update() {

        if (isPlayerMovementDisabled) return;

        if (isPlayerRolling) return;

        MovementInput();

        WeaponInput();

        UseItemInput();

        PlayerRollCooldownTimer();
    }

    private void MovementInput(){
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");
        bool rightMouseButtonDown = Input.GetMouseButtonDown(1);

        Vector2 direction = new Vector2(horizontalMovement, verticalMovement);

        if (horizontalMovement != 0f && verticalMovement != 0f){
            direction *= 0.7f;
        }

        // If there is movement either move or roll
        if (direction != Vector2.zero){

            // move
            if (!rightMouseButtonDown){
                player.movementByVelocityEvent.CallMovementByVelocityEvent(direction, moveSpeed);
            }
            else if(playerRollCooldownTimer <= 0f){
                PlayerRoll((Vector3) direction);
            }

        } else {
            player.idleEvent.CallIdleEvent();
        }

    }

    private void PlayerRoll(Vector3 direction){
        playerRollCoroutine = StartCoroutine(PlayerRollRoutine(direction));
    }

    private IEnumerator PlayerRollRoutine(Vector3 direction){
        float minDistance = 0.1f;

        isPlayerRolling = true;
        Vector3 targetPosition = player.transform.position + (Vector3)direction * movementDetails.rollDistance;

        while(Vector3.Distance(player.transform.position, targetPosition) > minDistance){
            player.movementToPositionEvent.CallMovementToPositionEvent(
                targetPosition,
                player.transform.position,
                movementDetails.rollSpeed,
                direction,
                isPlayerRolling
            );

            yield return waitForFixedUpdate;
        }

        isPlayerRolling = false;

        playerRollCooldownTimer = movementDetails.rollCooldownTime;
        player.transform.position = targetPosition;
    }

    private void PlayerRollCooldownTimer(){
        if(playerRollCooldownTimer >= 0f){
            playerRollCooldownTimer -= Time.deltaTime;
        }
    }

    private void WeaponInput(){
        Vector3 weaponDirection;
        float weaponAngleDegrees, playerAngleDegrees;
        AimDirection playerAimDirection;

        AimWeaponInput(out weaponDirection, out weaponAngleDegrees, out playerAngleDegrees, out playerAimDirection);

        FireWeaponInput(weaponDirection, weaponAngleDegrees, playerAngleDegrees, playerAimDirection);

        SwitchWeaponInput();

        ReloadWeaponInput();
    }

    private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees, out AimDirection playerAimDirection){
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();

        weaponDirection = (mouseWorldPosition - player.activeWeapon.GetShootPosition());

        Vector3 playerDirection = (mouseWorldPosition - transform.position);

        weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);

        playerAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirection);

        playerAimDirection = HelperUtilities.GetAimDirection(playerAngleDegrees);

        player.aimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
    } 

    private void FireWeaponInput(Vector3 weaponDirection, float weaponAngleDegrees, float playerAngleDegrees, AimDirection playerAimDirection){
        if(Input.GetMouseButton(0)){
            fireTime += Time.deltaTime;
            player.fireWeaponEvent.CallFireWeaponEvent(true, leftMouseDownPreviousFrame , playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection, fireTime);
            leftMouseDownPreviousFrame = true;
        } else {
            fireTime = 0;
            leftMouseDownPreviousFrame = false;
        }
    }

    private void SwitchWeaponInput(){
        if (Input.mouseScrollDelta.y < 0f){
            PreviousWeapon();
        }
        if (Input.mouseScrollDelta.y > 0f){
            NextWeapon();
        }
        if (Input.GetKeyDown(KeyCode.LeftControl)){
            SwitchBetweenTwoWeapon();
        }
        if (Input.GetKeyDown(KeyCode.Q)){
            FastSwitchWeapon();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)){
            SetWeaponByIndex(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)){
            SetWeaponByIndex(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)){
            SetWeaponByIndex(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)){
            SetWeaponByIndex(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5)){
            SetWeaponByIndex(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6)){
            SetWeaponByIndex(6);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7)){
            SetWeaponByIndex(7);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8)){
            SetWeaponByIndex(8);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9)){
            SetWeaponByIndex(9);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0)){
            SetWeaponByIndex(10);
        }
        if (Input.GetKeyDown(KeyCode.Minus)){
            SetCurrentWeaponToFirstInTheList();
        }
    }

    private void SetWeaponByIndex(int weaponIndex){
        Weapon currentWeapon = player.activeWeapon.GetCurrentWeapon();
        
        if (weaponIndex -1 < player.weaponList.Count){
            previousWeaponIndex = currentWeaponIndex;
            currentWeaponIndex = weaponIndex;
            Weapon weapon = player.weaponList[weaponIndex - 1];
            if(weapon == currentWeapon) return;
            player.stopReloadWeaponEvent.CallStopReloadWeapon(currentWeapon);
            player.setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);
        }
    }

    private void PreviousWeapon(){
        currentWeaponIndex--;
        if(currentWeaponIndex < 1){
            currentWeaponIndex = player.weaponList.Count;
        }
        SetWeaponByIndex(currentWeaponIndex);

    }

    private void NextWeapon(){
        currentWeaponIndex++;
        if(currentWeaponIndex > player.weaponList.Count){
            currentWeaponIndex = 1;
        }
        SetWeaponByIndex(currentWeaponIndex);
    }

    private void SwitchBetweenTwoWeapon(){
        if(!reverse){
            NextWeapon();
        } else {
            PreviousWeapon();
        }
        reverse = !reverse;
    }

    private void FastSwitchWeapon(){
        SetWeaponByIndex(previousWeaponIndex);
    }

    private void ReloadWeaponInput(){
        Weapon currentWeapon = player.activeWeapon.GetCurrentWeapon();
        int weaponClipAmmoCapacity = currentWeapon.weaponDetails.weaponClipAmmoCapacity;
        int weaponClipRemainingAmmo = currentWeapon.weaponClipRemainingAmmo;
        bool hasInfiniteAmmo = currentWeapon.weaponDetails.hasInfiniteAmmo;
        bool hasInfiniteClipCapacity = currentWeapon.weaponDetails.hasInfiniteClipCapacity;

        if (currentWeapon.isWeaponReloading) return;
        
        // Don't have enough ammo in clip
        // if (weaponClipRemainingAmmo < weaponClipAmmoCapacity && !hasInfiniteAmmo) return;

        if (weaponClipRemainingAmmo == weaponClipAmmoCapacity) return;

        if (Input.GetKeyDown(KeyCode.R)){
            player.reloadWeaponEvent.CallReloadWeaponEvent(currentWeapon, 0);
        }
    }

    private void UseItemInput(){
        if (Input.GetKeyDown(KeyCode.E)){
            float useItemRadius = 2f;

            Collider2D[] collider2DArray = Physics2D.OverlapCircleAll(player.GetPlayerPosition(), useItemRadius);

            foreach (Collider2D collider2D in collider2DArray){
                IUseable iUseable = collider2D.GetComponent<IUseable>();

                if (iUseable != null){
                    iUseable.UseItem();
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision){
        StopPlayerRollRountine();
    }
    private void OnCollisionStay2D(Collision2D collision){
        StopPlayerRollRountine();
    }

    private void StopPlayerRollRountine(){
        if(playerRollCoroutine != null){
            StopCoroutine(playerRollCoroutine);

            isPlayerRolling = false;
        }
    }

    public void EnablePlayer(){
        isPlayerMovementDisabled = false;
    }

    public void DisablePlayer(){
        isPlayerMovementDisabled = true;
        player.idleEvent.CallIdleEvent();
    }

    private void SetCurrentWeaponToFirstInTheList(){
        List<Weapon> tempWeaponList = new List<Weapon>();

        Weapon currentWeapon = player.weaponList[currentWeaponIndex - 1];
        currentWeapon.weaponListPosition = 1;
        tempWeaponList.Add(currentWeapon);

        int index = 2;
        foreach (Weapon weapon in player.weaponList){
            if(weapon == currentWeapon) continue;
            tempWeaponList.Add(weapon);
            weapon.weaponListPosition = index;
            index++;
        }

        player.weaponList = tempWeaponList;
        
        currentWeaponIndex = 1;

        SetWeaponByIndex(currentWeaponIndex);
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate(){
        HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
    }

#endif
    #endregion
}
