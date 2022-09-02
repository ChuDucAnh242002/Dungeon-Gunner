using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{

    #region Header OBJECT REFERENCES
    [Space(10)]
    [Header("OBJECT REFERENCES")]
    #endregion
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject highScoresButton;
    [SerializeField] private GameObject returnToMainMenuButton;
    private bool isHighScoresSceneLoaded = false;

    private void Start()
    {
        MusicManager.Instance.PlayMusic(GameResources.Instance.mainMenuMusic, 0f, 2f);

        SceneManager.LoadScene("CharacterSelectorScene", LoadSceneMode.Additive);

        returnToMainMenuButton.SetActive(false);
    }

    public void PlayGame(){
        SceneManager.LoadScene("MainGameScene");
    }

    public void LoadHighScores(){
        playButton.SetActive(false);
        highScoresButton.SetActive(false);
        isHighScoresSceneLoaded = true;

        SceneManager.UnloadSceneAsync("CharacterSelectorScene");

        returnToMainMenuButton.SetActive(true);

        SceneManager.LoadScene("HighScoreScene", LoadSceneMode.Additive);
    }

    public void LoadCharacterSelector(){
        returnToMainMenuButton.SetActive(false);

        if (isHighScoresSceneLoaded){
            SceneManager.UnloadSceneAsync("HighScoreScene");
            isHighScoresSceneLoaded = false;
        }

        playButton.SetActive(true);
        highScoresButton.SetActive(true);

        SceneManager.LoadScene("CharacterSelectorScene", LoadSceneMode.Additive);
    }
}
