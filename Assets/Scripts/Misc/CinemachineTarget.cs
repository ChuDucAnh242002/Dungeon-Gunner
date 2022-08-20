using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemachineTarget : MonoBehaviour
{
    private CinemachineTargetGroup cinemachineTargetGroup;

    [SerializeField] private Transform cursorTarget;

    private void Awake(){
        cinemachineTargetGroup = GetComponent<CinemachineTargetGroup>();
    }

    void Start(){
        SetCinemachineTargetGroup();
    }

    private void SetCinemachineTargetGroup(){
        CinemachineTargetGroup.Target cinemachineGroupTarget_player = new CinemachineTargetGroup.Target {
            weight = 1f,
            radius = 2.5f,
            target = GameManager.Instance.GetPlayer().transform
        };

        CinemachineTargetGroup.Target cinemachineGroupTarget_cursor = new CinemachineTargetGroup.Target {
            weight = 0.3f,
            radius = 1f,
            target = cursorTarget
        };

        CinemachineTargetGroup.Target[] cinemachineTargetArray = new CinemachineTargetGroup.Target[]{
            cinemachineGroupTarget_player,
            cinemachineGroupTarget_cursor
        };

        cinemachineTargetGroup.m_Targets = cinemachineTargetArray;
    }

    private void Update(){
        cursorTarget.position = HelperUtilities.GetMouseWorldPosition();

    }
}
