using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
public class Uihandler : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI distanceTravelledText;

    [SerializeField]
    TextMeshProUGUI gameOverText;
    [SerializeField]
    CanvasGroup gameOverCanvasGroup;


    Carhandler playerCarhandler;
    void Awake()
    {
        playerCarhandler = GameObject.FindGameObjectWithTag("Player").GetComponent<Carhandler>();
        playerCarhandler.OnPlayerCrashed += PlayerCarHandler_OnPlayerCrashed;
    }

    void Start()
    {
        gameOverCanvasGroup.interactable = false;
        gameOverCanvasGroup.alpha = 0;
    }
    void Update()
    {
        distanceTravelledText.text = playerCarhandler.DistanceTravelled.ToString("000000");
    }
    IEnumerator StartGameOverAnimationCO()
    {
        yield return new WaitForSecondsRealtime(3.0f);
        gameOverCanvasGroup.interactable = true;
        while (gameOverCanvasGroup.alpha < 0.8f)
        {
            gameOverCanvasGroup.alpha = Mathf.MoveTowards(gameOverCanvasGroup.alpha, 1.0f, Time.deltaTime * 2);
            yield return null;
        }
    }
    void PlayerCarHandler_OnPlayerCrashed(Carhandler obj)
    {
        gameOverText.text = $"DISTANCE {distanceTravelledText.text}";
        StartCoroutine(StartGameOverAnimationCO());
    }
    public void OnRestartClicked()
    {
        Time.timeScale = 1.0f;
        Carhandler car = FindObjectOfType<Carhandler>();
        if (car != null)
        {
            car.StopExplosionSound();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }



}

