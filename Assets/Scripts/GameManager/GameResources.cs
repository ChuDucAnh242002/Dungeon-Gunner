using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Audio;

public class GameResources : MonoBehaviour
{
    public static GameResources instance;
    
    public static GameResources Instance {
        get {
            if (instance == null){
                instance = Resources.Load<GameResources>("GameResources");
            }
            return instance;
        }
    }

    #region Header DUNGEON
    [Space(10)]
    [Header("DUNGEON")]
    #endregion
    #region Tooltip
    [Tooltip("Populate with the dungeon RoomNodeTypeListSO")]
    #endregion

    public RoomNodeTypeListSO roomNodeTypeList;
    public GameObject playerSelectionPrefab;

    #region Header PLAYER
    [Space(10)]
    [Header("PLAYER")]
    #endregion
    #region Tooltip
    [Tooltip("The current player SO, used to reference the current player between scenes")]
    #endregion
    public CurrentPlayerSO currentPlayerSO;
    public List<PlayerDetailsSO> playerDetailsList;


    #region Header MUSIC
    [Space(10)]
    [Header("MUSIC")]
    #endregion
    public AudioMixerGroup musicMasterMixerGroup;
    public MusicTrackSO mainMenuMusic;
    public AudioMixerSnapshot musicOnFullSnapshot;
    public AudioMixerSnapshot musicLowSnapshot;
    public AudioMixerSnapshot musicOffSnapshot;

    #region Header SOUNDS
    [Space(10)]
    [Header("SOUNDS")]
    #endregion
    public AudioMixerGroup soundsMasterMixerGroup;
    public SoundEffectSO doorOpenCloseSoundEffect;
    public SoundEffectSO tableFlip;
    public SoundEffectSO chestOpen;
    public SoundEffectSO healthPickUp;
    public SoundEffectSO weaponPickup;
    public SoundEffectSO ammoPickup;

    #region Header MATERIALS
    [Space(10)]
    [Header("MATERIALS")]
    #endregion
    public Material dimmedMaterial;

    #region Tooltip
    [Tooltip("Sprite-Lit_Default Material")]
    #endregion
    public Material litMaterial;

    #region Tooltip
    [Tooltip("Populate with the Variable Lit Shader")]
    #endregion
    public Shader variableLitShader;
    public Shader materializeShader;

    #region Header SPECIAL TILEMAP TILES
    [Space(10)]
    [Header("SPECIAL TILEMAP TILES")]
    #endregion
    public TileBase[] enemyUnwalkableCollisionTilesArray;
    public TileBase preferredEnemyPathTile;

    #region Header UI
    [Space(10)]
    [Header("UI")]
    #endregion
    public GameObject ammoIconPrefab;
    public GameObject heartPrefab;
    public GameObject chestItemPrefab;
    public GameObject scorePrefab;
    public Sprite heartIcon;
    public Sprite bulletIcon;


    #region Header MINIMAP
    [Space(10)]
    [Header("MINIMAP")]
    #endregion
    public GameObject minimapSkullPrefab;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate(){
        HelperUtilities.ValidateCheckNullValue(this, nameof(roomNodeTypeList), roomNodeTypeList);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerSelectionPrefab), playerSelectionPrefab);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(playerDetailsList), playerDetailsList);

        HelperUtilities.ValidateCheckNullValue(this, nameof(currentPlayerSO), currentPlayerSO);
        HelperUtilities.ValidateCheckNullValue(this, nameof(mainMenuMusic), mainMenuMusic);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundsMasterMixerGroup), soundsMasterMixerGroup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorOpenCloseSoundEffect), doorOpenCloseSoundEffect);
        HelperUtilities.ValidateCheckNullValue(this, nameof(tableFlip), tableFlip);
        HelperUtilities.ValidateCheckNullValue(this, nameof(chestOpen), chestOpen);
        HelperUtilities.ValidateCheckNullValue(this, nameof(healthPickUp), healthPickUp);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoPickup), ammoPickup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponPickup), weaponPickup);

        HelperUtilities.ValidateCheckNullValue(this, nameof(dimmedMaterial), dimmedMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(litMaterial), litMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(variableLitShader), variableLitShader);
        HelperUtilities.ValidateCheckNullValue(this, nameof(materializeShader), materializeShader);

        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemyUnwalkableCollisionTilesArray), enemyUnwalkableCollisionTilesArray);
        HelperUtilities.ValidateCheckNullValue(this, nameof(preferredEnemyPathTile), preferredEnemyPathTile);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicMasterMixerGroup), musicMasterMixerGroup);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicOnFullSnapshot), musicOnFullSnapshot);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicLowSnapshot), musicLowSnapshot);
        HelperUtilities.ValidateCheckNullValue(this, nameof(musicOffSnapshot), musicOffSnapshot);

        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoIconPrefab), ammoIconPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(heartPrefab), heartPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(chestItemPrefab), chestItemPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(scorePrefab), scorePrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(heartIcon), heartIcon);
        HelperUtilities.ValidateCheckNullValue(this, nameof(bulletIcon), bulletIcon);
        HelperUtilities.ValidateCheckNullValue(this, nameof(minimapSkullPrefab), minimapSkullPrefab);

    }

#endif
    #endregion

}
