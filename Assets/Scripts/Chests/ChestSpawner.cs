using System.Collections.Generic;
using UnityEngine;

public class ChestSpawner : MonoBehaviour
{
    #region Header CHEST PREFAB
    [Space(10)]
    [Header("CHEST PREFAB")]
    #endregion
    [SerializeField] private GameObject chestPrefab;

    #region Header CHEST SPAWN CHANCE
    [Space(10)]
    [Header("CHEST SPAWN CHANCE")]
    #endregion
    [SerializeField] [Range(0, 100)] private int chestSpawnChance;

    #region Header CHEST SPAWN DETAILS
    [Space(10)]
    [Header("CHEST SPAWN DETAILS")]
    #endregion
    [SerializeField] private ChestSpawnEvent chestSpawnEvent;
    [SerializeField] private ChestSpawnPosition chestSpawnPosition;
    [SerializeField] [Range(1, 2)] private int spawnType;

    #region Header CHEST CONTENT DETAILS
    [Space(10)]
    [Header("CHEST CONTENT DETAILS")]
    #endregion
    [SerializeField] private List<SpawnableObjectByLevel<WeaponDetailsSO>> weaponSpawnByLevelList;

    private bool chestSpawned = false;
    private Room chestRoom;

    private void OnEnable(){
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;

        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;
    }
    private void OnDisable(){
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;

        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs){
        if (chestRoom == null){
            chestRoom = GetComponentInParent<InstantiatedRoom>().room;
        }

        if (!chestSpawned && chestSpawnEvent == ChestSpawnEvent.onRoomEntry && chestRoom == roomChangedEventArgs.room){
            SpawnChest();
        }
    }

    private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedArgs roomEnemiesDefeatedArgs){
        if (chestRoom == null){
            chestRoom = GetComponentInParent<InstantiatedRoom>().room;
        }
        if (!chestSpawned && chestSpawnEvent == ChestSpawnEvent.onEnemiesDefeated && chestRoom == roomEnemiesDefeatedArgs.room){
            SpawnChest();
        }
    }

    private void SpawnChest(){
        chestSpawned = true;

        if (!RandomSpawnChest()) return;

        GetItemsToSpawn(out int ammoNum, out int healthNum, out int weaponNum);

        GameObject chestGameObject = Instantiate(chestPrefab, this.transform);

        if (chestSpawnPosition == ChestSpawnPosition.atSpawnerPosition){
            chestGameObject.transform.position = this.transform.position;
        }
        else if (chestSpawnPosition == ChestSpawnPosition.atPlayerPosition)
        {
            Vector3 spawnPosition = HelperUtilities.GetSpawnPositionNearestToPlayer(GameManager.Instance.GetPlayer().transform.position);

            Vector3 variation = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);

            chestGameObject.transform.position = spawnPosition + variation;
        }

        Chest chest = chestGameObject.GetComponent<Chest>();

        if (chestSpawnEvent == ChestSpawnEvent.onRoomEntry){
            chest.Initialize(false, GetHealthPercentToSpawn(healthNum), GetWeaponDetailsToSpawn(weaponNum), GetAmmoPercentToSpawn(ammoNum));
        } else {
            chest.Initialize(true, GetHealthPercentToSpawn(healthNum), GetWeaponDetailsToSpawn(weaponNum), GetAmmoPercentToSpawn(ammoNum));
        }
    }

    private bool RandomSpawnChest(){
        int randomPercent = Random.Range(1, 100 + 1);

        if (randomPercent <= chestSpawnChance) return true;
        return false;
    }

    private void GetItemsToSpawn(out int ammo, out int health, out int weapons){
        ammo = 0;
        health = 0;
        weapons = 0;

        if (spawnType == 1){
            weapons++;
            return;
        }
        else if (spawnType == 2){
            int choice;
            choice = Random.Range(0, 2);
            if (choice == 0){
                ammo++;
            }
            else if (choice == 1){
                health++;
            }
        }        
    }

    private int GetHealthPercentToSpawn(int healthNum){
        if (healthNum == 0) return 0;
        return 20; // default health
    }

    private WeaponDetailsSO GetWeaponDetailsToSpawn(int weaponNum){
        if (weaponNum == 0) return null;

        RandomSpawnableObject<WeaponDetailsSO> weaponRandom = new RandomSpawnableObject<WeaponDetailsSO>(weaponSpawnByLevelList);

        WeaponDetailsSO weaponDetails = weaponRandom.GetItem();
        while (GameManager.Instance.GetPlayer().IsWeaponHeldByPlayer(weaponDetails)){
            weaponDetails = weaponRandom.GetItem();
        }
        
        return weaponDetails;
    }

    private int GetAmmoPercentToSpawn(int ammoNum){
        if (ammoNum == 0) return 0;
        return 100; // default percent
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate(){
    
        HelperUtilities.ValidateCheckNullValue(this, nameof(chestPrefab), chestPrefab);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(chestSpawnChance), chestSpawnChance, true);

        HelperUtilities.ValidateCheckPositiveValue(this, nameof(spawnType), spawnType, false);

        if (weaponSpawnByLevelList != null && weaponSpawnByLevelList.Count > 0){
            foreach (SpawnableObjectByLevel<WeaponDetailsSO> weaponDetailsByLevel in weaponSpawnByLevelList){
                HelperUtilities.ValidateCheckNullValue(this, nameof(weaponDetailsByLevel.dungeonLevel), weaponDetailsByLevel.dungeonLevel);

                foreach (SpawnableObjectRatio<WeaponDetailsSO> weaponRatio in weaponDetailsByLevel.spawnableObjectRatioList){
                    HelperUtilities.ValidateCheckNullValue(this, nameof(weaponRatio.dungeonObject), weaponRatio.dungeonObject);
                    HelperUtilities.ValidateCheckPositiveValue(this, nameof(weaponRatio.ratio), weaponRatio.ratio, true);
                }
            }
        }
    }
#endif
    #endregion
}
