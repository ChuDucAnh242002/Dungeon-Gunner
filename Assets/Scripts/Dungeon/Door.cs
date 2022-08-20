using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class Door : MonoBehaviour
{
    #region Header OBJECT REFERENCES
    [Space(10)]
    [Header("OBJECT REFERENCES")]
    #endregion

    #region Tooltip
    [Tooltip("BoxCollider2D for DoorCollider gameobject")]
    #endregion
    [SerializeField] private BoxCollider2D doorCollider;

    [HideInInspector] public bool isBossRoomDoor = false;
    private BoxCollider2D doorTrigger;
    private bool isOpen = false;
    private bool previouslyOpened = false;
    private Animator animator;

    private void Awake(){
        doorCollider.enabled = false;
        animator = GetComponent<Animator>();
        doorTrigger = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.tag == Settings.playerTag || collision.tag == Settings.playerWeapon){
            OpenDoor();
        }
    }

    private void OnEnable(){

        // When the player moves far enough away from the room, the animator state get reset
        animator.SetBool(Settings.open, isOpen);
    }



    public void OpenDoor(){
        if(!isOpen){
            isOpen = true;
            previouslyOpened = true;
            doorCollider.enabled = false;
            doorTrigger.enabled = false;

            animator.SetBool(Settings.open, true);
        }
    }

    public void LockDoor(){
        isOpen = false;
        doorCollider.enabled = true;
        doorTrigger.enabled = false;

        animator.SetBool(Settings.open, false);
    }

    public void UnlockDoor(){
        doorCollider.enabled = false;
        doorTrigger.enabled = true;

        if (previouslyOpened){
            isOpen = false;
            OpenDoor();
        }

    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate(){
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorCollider), doorCollider);
    }

#endif
    #endregion
}
