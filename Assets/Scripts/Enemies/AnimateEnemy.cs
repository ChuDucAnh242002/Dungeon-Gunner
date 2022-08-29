using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
[DisallowMultipleComponent]
public class AnimateEnemy : MonoBehaviour
{
    private Enemy enemy;

    private void Awake(){
        enemy = GetComponent<Enemy>();
    }

    private void OnEnable(){
        enemy.movementToPositionEvent.OnMovementToPosition += MovementToPositionEvent_OnMovementToPosition;

        enemy.idleEvent.OnIdle += IdleEvent_OnIdle;
    }

    private void OnDisable(){
        enemy.movementToPositionEvent.OnMovementToPosition -= MovementToPositionEvent_OnMovementToPosition;

        enemy.idleEvent.OnIdle -= IdleEvent_OnIdle;
    }

    private void MovementToPositionEvent_OnMovementToPosition(MovementToPositionEvent movementToPositionEvent, MovementToPositionArgs movementToPositionArgs){
        float enemyPositionX = enemy.transform.position.x;
        float playerPositionX = GameManager.Instance.GetPlayer().transform.position.x;
        if (enemyPositionX < playerPositionX){
            SetAimWeaponAnimationParameters(AimDirection.Right);
        } else {
            SetAimWeaponAnimationParameters(AimDirection.Left);
        }
        SetMovementAnimationParameters();
    }

    private void IdleEvent_OnIdle(IdleEvent idleEvent){
        SetIdleAnimationParameters();
    }

    private void InitialiseAimAnimationParameters(){
        enemy.animator.SetBool(Settings.aimUp, false);
        enemy.animator.SetBool(Settings.aimUpRight, false);
        enemy.animator.SetBool(Settings.aimUpLeft, false);
        enemy.animator.SetBool(Settings.aimRight, false);
        enemy.animator.SetBool(Settings.aimLeft, false);
        enemy.animator.SetBool(Settings.aimDown, false);
    }

    private void SetMovementAnimationParameters(){
        enemy.animator.SetBool(Settings.isIdle, false);
        enemy.animator.SetBool(Settings.isMoving, true);
    }

    private void SetIdleAnimationParameters(){
        enemy.animator.SetBool(Settings.isIdle, true);
        enemy.animator.SetBool(Settings.isMoving, false);
    }

    private void SetAimWeaponAnimationParameters(AimDirection aimDirection){
        InitialiseAimAnimationParameters();

        switch (aimDirection){
            case AimDirection.Up:
                enemy.animator.SetBool(Settings.aimUp, true);
                break;
            case AimDirection.UpRight:
                enemy.animator.SetBool(Settings.aimUpRight, true);
                break;
            case AimDirection.UpLeft:
                enemy.animator.SetBool(Settings.aimUpLeft, true);
                break;
            case AimDirection.Right:
                enemy.animator.SetBool(Settings.aimRight, true);
                break;
            case AimDirection.Left:
                enemy.animator.SetBool(Settings.aimLeft, true);
                break;
            case AimDirection.Down:
                enemy.animator.SetBool(Settings.aimDown, true);
                break;
        }
    }

}
