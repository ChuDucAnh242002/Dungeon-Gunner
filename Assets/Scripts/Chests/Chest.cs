using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(MaterializeEffect))]
public class Chest : MonoBehaviour, IUseable
{

    [ColorUsage(false, true)]
    [SerializeField] private Color materializeColor;
    [SerializeField] private float materializeTime = 3f;
    [SerializeField] private Transform itemSpawnPoint;
    private int healthPercent;
    private WeaponDetailsSO weaponDetails;
    private int ammoPercent;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private MaterializeEffect materializeEffect;
    private bool isEnabled = false;
    private ChestState chestState = ChestState.closed;
    private GameObject chestItemGameObject;
    private ChestItem chestItem;
    private TextMeshPro messageTextTMP;

    private void Awake(){
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        materializeEffect = GetComponent<MaterializeEffect>();
        messageTextTMP = GetComponentInChildren<TextMeshPro>();
    }

    public void Initialize(bool shouldMaterialize, int healthPercent, WeaponDetailsSO weaponDetails, int ammoPercent){
        this.healthPercent = healthPercent;
        this.weaponDetails = weaponDetails;
        this.ammoPercent = ammoPercent;
        if (shouldMaterialize){
            StartCoroutine(MaterializeChest());
        } else {
            EnableChest();
        }
    }

    private IEnumerator MaterializeChest(){
        SpriteRenderer[] spriteRendererArray = new SpriteRenderer[] { spriteRenderer };

        yield return StartCoroutine(materializeEffect.MaterializeRoutine(GameResources.Instance.materializeShader,
            materializeColor, materializeTime, spriteRendererArray, GameResources.Instance.litMaterial));

        EnableChest();
    }

    private void EnableChest(){
        isEnabled = true;
    }

    public void UseItem(){
        if (!isEnabled) return;

        switch (chestState){
            case ChestState.closed:
                OpenChest();
                break;
            case ChestState.healthItem:
                CollectHealthItem();
                break;
            case ChestState.ammoItem:
                CollectAmmoItem();
                break;
            case ChestState.weaponItem:
                CollectWeaponItem();
                break;
            case ChestState.emtpy:
                return;
            default: 
                return;
        }
    }

    private void OpenChest(){
        animator.SetBool(Settings.use, true);

        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.chestOpen);

        if (weaponDetails != null){
            if (GameManager.Instance.GetPlayer().IsWeaponHeldByPlayer(weaponDetails)){
                weaponDetails = null;
            }
        }

        UpdateChestState();
    }

    private void UpdateChestState(){
        if (healthPercent != 0){
            chestState = ChestState.healthItem;
            InstantiateHealthItem();
        }
        else if (ammoPercent != 0){
            chestState = ChestState.ammoItem;
            InstatiateAmmoItem();
        }
        else if (weaponDetails != null){
            chestState = ChestState.weaponItem;
            InstatiateWeaponItem();
        }
        else {
            chestState = ChestState.emtpy;
        }
    }

    private void InstantiateItem(){
        chestItemGameObject = Instantiate(GameResources.Instance.chestItemPrefab, this.transform);

        chestItem = chestItemGameObject.GetComponent<ChestItem>();
    }

    private void InstantiateHealthItem(){
        InstantiateItem();

        chestItem.Initialize(GameResources.Instance.heartIcon, healthPercent.ToString() + "%", itemSpawnPoint.position, materializeColor);
    }

    private void InstatiateAmmoItem(){
        InstantiateItem();

        chestItem.Initialize(GameResources.Instance.bulletIcon, ammoPercent.ToString() + "%", itemSpawnPoint.position, materializeColor);
    }

    private void InstatiateWeaponItem(){
        InstantiateItem();

        chestItemGameObject.GetComponent<ChestItem>().Initialize(weaponDetails.weaponSprite, weaponDetails.weaponName, itemSpawnPoint.position, materializeColor);
    }

    private void CollectHealthItem(){
        if (chestItem == null || !chestItem.isItemMaterialized) return;

        GameManager.Instance.GetPlayer().health.AddHealth(healthPercent);

        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.healthPickUp);

        healthPercent = 0;

        Destroy(chestItemGameObject);

        UpdateChestState();
    }

    private void CollectAmmoItem(){
        if (chestItem == null || !chestItem.isItemMaterialized) return;

        Player player = GameManager.Instance.GetPlayer();

        player.reloadWeaponEvent.CallReloadWeaponEvent(player.activeWeapon.GetCurrentWeapon(), ammoPercent);

        SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.ammoPickup);

        ammoPercent = 0;

        Destroy(chestItemGameObject);

        UpdateChestState();
    }

    private void CollectWeaponItem(){
        if (chestItem == null || !chestItem.isItemMaterialized) return;

        if (!GameManager.Instance.GetPlayer().IsWeaponHeldByPlayer(weaponDetails)){

            GameManager.Instance.GetPlayer().AddWeaponToPlayer(weaponDetails);

            SoundEffectManager.Instance.PlaySoundEffect(GameResources.Instance.weaponPickup);
        } 
        else {
            StartCoroutine(DisplayMessage("WEAPON\nALREAYD\nEQUIPPED", 5f));
        }
        weaponDetails = null;

        Destroy(chestItemGameObject);

        UpdateChestState();
    }

    private IEnumerator DisplayMessage(string messageText, float messageDisplayTime){
        messageTextTMP.text = messageText;

        yield return new WaitForSeconds(messageDisplayTime);

        messageTextTMP.text = "";
    }
}
