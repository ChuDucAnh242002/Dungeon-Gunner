using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponStatusUI : MonoBehaviour
{
    #region Header OBJECT REFERNECES
    [Space(10)]
    [Header("OBJECT REFERENCES")]
    #endregion Header OBJECT REFERNECES
    [SerializeField] private Image weaponImage;
    [SerializeField] private Transform ammoHolderTransform;
    [SerializeField] private TextMeshProUGUI reloadText;
    [SerializeField] private TextMeshProUGUI ammoRemainingText;
    [SerializeField] private Transform ammoRemainingTransform;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private Transform reloadBar;
    [SerializeField] private Image barImage;
    private Player player;
    private List<GameObject> ammoIconList = new List<GameObject>();
    private Coroutine reloadWeaponCoroutine;
    private Coroutine blinkingReloadTextCoroutine;

    private void Awake(){
        player = GameManager.Instance.GetPlayer();
    }

    private void OnEnable(){
        player.setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;
        player.weaponFiredEvent.OnWeaponFired += WeaponFiredEvent_OnWeaponFired;
        player.reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnReloadWeapon;
        player.weaponReloadedEvent.OnWeaponReloaded += WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void OnDisable(){
        player.setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;
        player.weaponFiredEvent.OnWeaponFired -= WeaponFiredEvent_OnWeaponFired;
        player.reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnReloadWeapon;
        player.weaponReloadedEvent.OnWeaponReloaded -= WeaponReloadedEvent_OnWeaponReloaded;
    }   

    private void Start(){
        Weapon currentWeapon = player.activeWeapon.GetCurrentWeapon();
        SetActiveWeapon(currentWeapon);
    }

    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs){
        SetActiveWeapon(setActiveWeaponEventArgs.weapon);
    }

    private void WeaponFiredEvent_OnWeaponFired(WeaponFiredEvent weaponFiredEvent, WeaponFiredEventArgs weaponFiredEventArgs){
        WeaponFired(weaponFiredEventArgs.weapon);
    }

    private void WeaponFired(Weapon weapon){
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);
        UpdateReloadText(weapon);
    }
    
    private void ReloadWeaponEvent_OnReloadWeapon(ReloadWeaponEvent reloadWeaponEvent, ReloadWeaponEventArgs reloadWeaponEventArgs){
        UpdateWeaponReloadBar(reloadWeaponEventArgs.weapon);
    }

    private void WeaponReloadedEvent_OnWeaponReloaded(WeaponReloadedEvent weaponReloadedEvent, WeaponReloadedEventArgs weaponReloadedEventArgs){
        WeaponReloaded(weaponReloadedEventArgs.weapon);
    }

    private void WeaponReloaded(Weapon weapon){
        Weapon currentWeapon = player.activeWeapon.GetCurrentWeapon();
        if(currentWeapon == weapon){
            UpdateReloadText(weapon);
            UpdateAmmoText(weapon);
            UpdateAmmoLoadedIcons(weapon);
            ResetWeaponReloadBar();
        }
    }

    private void SetActiveWeapon(Weapon weapon){
        UpdateActiveWeaponImage(weapon.weaponDetails);
        UpdateActiveWeaponName(weapon);
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);

        if (weapon.isWeaponReloading){
            UpdateWeaponReloadBar(weapon);
        } else {
            ResetWeaponReloadBar();
        }

        UpdateReloadText(weapon);
    }

    private void UpdateActiveWeaponImage(WeaponDetailsSO weaponDetails){
        weaponImage.sprite = weaponDetails.weaponSprite;
    }

    private void UpdateActiveWeaponName(Weapon weapon){
        string weaponName = weapon.weaponDetails.weaponName.ToUpper();
        weaponNameText.text = "(" + weapon.weaponListPosition + ")" + weaponName;
    }

    private void UpdateAmmoText(Weapon weapon){
        bool hasInfiniteAmmo = weapon.weaponDetails.hasInfiniteAmmo;
        string weaponRemainingAmmoToString = weapon.weaponClipRemainingAmmo.ToString();
        string weaponAmmoCapacityToString = weapon.weaponDetails.weaponAmmoCapacity.ToString();
        if(hasInfiniteAmmo){
            ammoRemainingTransform.localPosition = new Vector3(0, 30, 0);
            ammoRemainingText.fontSize = 17;
            ammoRemainingText.text = "∞";
        } else {
            ammoRemainingText.text = weaponRemainingAmmoToString + "/" + weaponAmmoCapacityToString;
        }

    }

    private void UpdateAmmoLoadedIcons(Weapon weapon){
        ClearAmmoLoadedIcons();

        for (int i = 0; i < weapon.weaponClipRemainingAmmo; i++){
            GameObject ammoIcon = Instantiate(GameResources.Instance.ammoIconPrefab, ammoHolderTransform);

            ammoIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, Settings.uiAmmoIconSpacing * i);
            ammoIconList.Add(ammoIcon);
        }

        
    }

    private void ClearAmmoLoadedIcons(){
        foreach(GameObject ammoIcon in ammoIconList){
            Destroy(ammoIcon);
        }

        ammoIconList.Clear();
    }

    private void UpdateWeaponReloadBar(Weapon weapon){
        bool hasInfiniteClipCapacity = weapon.weaponDetails.hasInfiniteClipCapacity;
        if (hasInfiniteClipCapacity) return;

        StopReloadWeaponCoroutine();
        UpdateReloadText(weapon);

        reloadWeaponCoroutine = StartCoroutine(UpdateWeaponReloadBarRoutine(weapon));
    }

    private IEnumerator UpdateWeaponReloadBarRoutine(Weapon weapon){
        barImage.color = Color.red;
        float weaponReloadTime = weapon.weaponDetails.weaponReloadTime;

        while (weapon.isWeaponReloading){
            float barFill = weapon.weaponReloadTimer / weaponReloadTime;
            reloadBar.transform.localScale = new Vector3(barFill, 1f, 1f);
            yield return null;
        }
    }

    private void ResetWeaponReloadBar(){
        StopReloadWeaponCoroutine();
        barImage.color = Color.green;
        reloadBar.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void StopReloadWeaponCoroutine(){
        if(reloadWeaponCoroutine != null){
            StopCoroutine(reloadWeaponCoroutine);
        }
    }

    private void UpdateReloadText(Weapon weapon){
        bool hasInfiniteClipCapacity = weapon.weaponDetails.hasInfiniteClipCapacity;
        if ((!hasInfiniteClipCapacity) && (weapon.weaponClipRemainingAmmo <= 0 || weapon.isWeaponReloading)){
            barImage.color = Color.red;

            StopBlinkingReloadTextCoroutine();

            blinkingReloadTextCoroutine = StartCoroutine(StartBlinkingReloadTextRoutine());

        } else {
            StopBlinkingReloadText();
        }
    }
    
    private IEnumerator StartBlinkingReloadTextRoutine(){
        while (true){
            reloadText.text = "RELOAD";
            yield return new WaitForSeconds(0.3f);
            reloadText.text = "";
            yield return new WaitForSeconds(0.3f);
        }
    }

    private void StopBlinkingReloadText(){
        StopBlinkingReloadTextCoroutine();
        reloadText.text = "";
    }

    private void StopBlinkingReloadTextCoroutine(){
        if(blinkingReloadTextCoroutine != null){
            StopCoroutine(blinkingReloadTextCoroutine);
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate(){
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponImage), weaponImage );
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoHolderTransform), ammoHolderTransform);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadText), reloadText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoRemainingText), ammoRemainingText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponNameText), weaponNameText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadBar), reloadBar);
        HelperUtilities.ValidateCheckNullValue(this, nameof(barImage), barImage);
    }
#endif
    #endregion

    
}
