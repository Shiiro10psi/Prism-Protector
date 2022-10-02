using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameStateUI : MonoBehaviour
{
    TMP_Text healthText, scoreText, laserCountText, laserStrengthText, titleText,playButtonText;
    CanvasGroup titleScreen;

    GameState gs;
    FireController fc;
    AudioSource sound;

    [SerializeField] AudioClip PlaySound, PauseSound, ButtonSound;

    private void Awake()
    {
        sound = GetComponent<AudioSource>();
        Time.timeScale = 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        gs = FindObjectOfType<GameState>();
        fc = FindObjectOfType<FireController>();

        var texts = GetComponentsInChildren<TMP_Text>();
        foreach (TMP_Text t in texts)
        {
            if (t.name == "HealthText") healthText = t;
            if (t.name == "ScoreText") scoreText = t;
            if (t.name == "LaserCountText") laserCountText = t;
            if (t.name == "LaserStrengthText") laserStrengthText = t;
            if (t.name == "TitleText") titleText = t;
            if (t.name == "PlayButtonText") playButtonText = t;
        }
        var cg = GetComponentsInChildren<CanvasGroup>();
        foreach(CanvasGroup c in cg)
        {
            if (c.name == "TitleCreditsPanel") titleScreen = c;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = gs.GetHealth().ToString();
        scoreText.text = gs.GetScore().ToString();
        laserCountText.text = gs.GetLasers().ToString();
        laserStrengthText.text = fc.GetStrength().ToString();
    }

    public void GameOver()
    {
        sound.Play();
        StartCoroutine(wait());
        
    }

    public void PlayPauseButton()
    {
        if (Time.timeScale == 0 && gs.GetHealth() != 0)
        {
            
            sound.PlayOneShot(PlaySound);
            playButtonText.text = "Pause";
            titleScreen.alpha = 0;
            titleScreen.interactable = false;
            titleScreen.blocksRaycasts = false;
            Time.timeScale = 1;
            return;
        }

        if (Time.timeScale == 1)
        {
            
            sound.PlayOneShot(PauseSound);
            playButtonText.text = "Play";
            Time.timeScale = 0;
            titleScreen.alpha = 1;
            titleScreen.interactable = true;
            titleScreen.blocksRaycasts = true;
            return;
        }

        sound.PlayOneShot(ButtonSound);
    }

    public void MusicButton()
    {
        sound.PlayOneShot(ButtonSound);
        FindObjectOfType<SoundController>().MusicButton();
    }

    public void SoundButton()
    {
        sound.PlayOneShot(ButtonSound);
        FindObjectOfType<SoundController>().SoundButton();
    }

    public void RestartButton()
    {
        if (Time.timeScale == 1)
        {
            
            playButtonText.text = "Play";
            Time.timeScale = 0;
            titleScreen.alpha = 1;
            titleScreen.interactable = true;
            titleScreen.blocksRaycasts = true;
        }
        sound.PlayOneShot(ButtonSound);
        FindObjectOfType<SceneLoader>().ReloadScene();
    }

    private IEnumerator wait()
    {
        
        
        yield return new WaitForSeconds(1);
        Time.timeScale = 0;
        playButtonText.text = "Game Over";
        titleScreen.alpha = 1;
        titleScreen.interactable = true;
        titleScreen.blocksRaycasts = true;

    }
}
