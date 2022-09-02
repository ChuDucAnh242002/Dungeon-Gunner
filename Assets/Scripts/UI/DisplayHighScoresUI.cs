using UnityEngine;

public class DisplayHighScoresUI : MonoBehaviour
{
    #region Header OBJECT REFERENCES
    [Space(10)]
    [Header("OBJECT REFERENCES")]
    #endregion
    [SerializeField] private Transform contentAnchorTransform;

    private void Start(){
        DisplayScores();
    }

    private void DisplayScores(){
        HighScores highScores = HighScoreManager.Instance.GetHighScores();
        GameObject scoreGameobject;

        int rank = 0;
        foreach (Score score in highScores.scoreList){
            rank++;

            scoreGameobject = Instantiate(GameResources.Instance.scorePrefab, contentAnchorTransform);

            ScorePrefab scorePrefab = scoreGameobject.GetComponent<ScorePrefab>();

            scorePrefab.rankTMP.text = rank.ToString();
            scorePrefab.nameTMP.text = score.playerName;
            scorePrefab.levelTMP.text = score.levelDescription;
            scorePrefab.scoreTMP.text = score.playerScore.ToString("###,###0");
        }

        scoreGameobject = Instantiate(GameResources.Instance.scorePrefab, contentAnchorTransform);
    }
}
