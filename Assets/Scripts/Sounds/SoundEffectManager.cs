using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DisallowMultipleComponent]
public class SoundEffectManager : SingletonMonobehaviour<SoundEffectManager>
{
    public int soundsVolume = 10;

    private void Start(){
        if (PlayerPrefs.HasKey("soundsVolume")){
            soundsVolume = PlayerPrefs.GetInt("soundsVolume");
        }

        SetSoundsVolume(soundsVolume);
    }

    private void OnDisable(){
        PlayerPrefs.SetInt("soundsVolume", soundsVolume);
    }

    public void PlaySoundEffect(SoundEffectSO soundEffect){
        SoundEffect sound = (SoundEffect) PoolManager.Instance.ReuseComponent(soundEffect.soundPrefab, Vector3.zero, Quaternion.identity);
        sound.SetSound(soundEffect);
        sound.gameObject.SetActive(true);
        StartCoroutine(DisableSound(sound, soundEffect.soundEffectClip.length));
    }

    private IEnumerator DisableSound(SoundEffect sound, float soundDuration){
        yield return new WaitForSeconds(soundDuration);
        sound.gameObject.SetActive(false);
    }

    public void IncreaseSoundVolume(){
        int maxSoundVolume = 20;

        if (soundsVolume >= maxSoundVolume) return;

        soundsVolume += 1;

        SetSoundsVolume(soundsVolume);
    }

    public void DecreaseSoundVolume(){
        int minSoundVolume = 0;

        if (soundsVolume <= minSoundVolume) return;

        soundsVolume -= 1;

        SetSoundsVolume(soundsVolume);
    }

    private void SetSoundsVolume(int soundsVolume){
        float muteDecibels = -80f;
        if (soundsVolume == 0){
            GameResources.Instance.soundsMasterMixerGroup.audioMixer.SetFloat("soundsVolume",
                muteDecibels);
        } else {
            GameResources.Instance.soundsMasterMixerGroup.audioMixer.SetFloat("soundsVolume", 
                HelperUtilities.LinearToDecibels(soundsVolume));
        }
    }
}
