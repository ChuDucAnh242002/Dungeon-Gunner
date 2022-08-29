using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#region REQUIRE COMPONENTS

[RequireComponent(typeof(EnemyMovementAI))]
[RequireComponent(typeof(MovementToPositionEvent))]
[RequireComponent(typeof(MovementToPosition))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(Idle))]

[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]

#endregion REQUIRE COMPONENTS
[DisallowMultipleComponent]
public class Enemy : MonoBehaviour
{
    public EnemyDetailsSO enemyDetails;
    private EnemyMovementAI enemyMovementAI;
    [HideInInspector] public MovementToPositionEvent movementToPositionEvent;
    [HideInInspector] public IdleEvent idleEvent;
    [HideInInspector] public SpriteRenderer[] spriteRendererArray;
    [HideInInspector] public Animator animator;

    private CircleCollider2D circleCollider2D;
    private PolygonCollider2D polygonCollider2D;

    private void Awake(){
        enemyMovementAI = GetComponent<EnemyMovementAI>();
        movementToPositionEvent = GetComponent<MovementToPositionEvent>();
        idleEvent = GetComponent<IdleEvent>();
        
        circleCollider2D = GetComponent<CircleCollider2D>();
        polygonCollider2D = GetComponent<PolygonCollider2D>();
        spriteRendererArray = GetComponentsInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
}
