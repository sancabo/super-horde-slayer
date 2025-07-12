using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    [SerializeField] private List<Bonfire> _bonfires = new();
    [SerializeField] private TextMeshProUGUI _bonfiresText;

    [SerializeField] private Player _player;
    [SerializeField] private GameObject _gameOverText;
    [SerializeField] private GameObject _winText;

    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private CharacterInventory _characterInventory;
    [SerializeField] private GameObject _fogOfWar;

    [SerializeField] private GameObject _signatureSpellMenu;

    [SerializeField] private GameObject _signatureSpellMenuAdvanced;

    [SerializeField] private GameObject _signatureButton;

    [SerializeField] private GameDirector _gameDirector;

    [SerializeField] private Quest _quest;

    [SerializeField] private AudioClip _gameOverSound;

    [SerializeField] private AudioClip _winSound;
    [SerializeField] private string _winScreenScene;
    [SerializeField] private GameObject _confirmExitButton;
    [SerializeField] private GameObject _gameStats;
    private double _enemiesKilled = 0;
    private int _litBonfires = 0;

    private bool _pickedWeapon = false;

    private bool _playerLeveled = false;

    private bool _playerUpgraded = false;

    private bool _fiveEnemiesKilled = false;

    private bool _paused = false;

    private bool _signatureSelected = false;
    private bool _signatureAdvancedSelected = false;
    private bool _waitingForSignature = false;

    private bool _playerWon = false;

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_paused) ClosePauseMenu();
            else OpenPauseMenu();

        }
    }

    public void ClosePauseMenu()
    {
        _characterInventory.Close();
        _pauseMenu.SetActive(false);
        _paused = false;
        Time.timeScale = 1;
    }

    public void OpenPauseMenu()
    {
        if (!_waitingForSignature)
        {
            _pauseMenu.SetActive(true);
            _characterInventory.gameObject.SetActive(true);
            _paused = true;
            Time.timeScale = 0;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(_gameStats);
        _signatureSpellMenu.SetActive(false);
        ClosePauseMenu();
    }

    void FixedUpdate()
    {
        int litBonfires = _bonfires.Where(b => b.IsLit()).Count();
        //Player lights one bonfire
        if (_litBonfires == 0 && litBonfires == 1)
        {
            AdvanceQuestImmediate();
            StartCoroutine(NextQuest());
        }
        _litBonfires = litBonfires;
        if (litBonfires == 8 /*_bonfires.Count()*/)
        {
            if (!_playerWon)
            {
                _playerWon = true;
                _player.DisablePlayer();
                PlayerWon();
            }

        }

        //_bonfiresText.text = $"Bonfires: {litBonfires}/ {_bonfires.Count()}";
        _bonfiresText.text = $"Bonfires: {litBonfires}/ 8";

        if (_player.GetLvl() >= 8 && !_signatureSelected)
        {
            _signatureButton.SetActive(true);
        }

        if (_player.GetLvl() >= 16 && !_signatureAdvancedSelected)
        {
            _signatureButton.SetActive(true);
        }
    }

    private IEnumerator WaitForSignature()
    {
        while (!_player.HasSignature())
        {
            yield return null;
        }
        _signatureSpellMenu.SetActive(false);
        Time.timeScale = 1;
        _waitingForSignature = false;
    }

    private IEnumerator WaitForSignatureAdvanced()
    {
        while (!_player.HasSignatureAdvanccced())
        {
            yield return null;
        }
        _signatureSpellMenuAdvanced.SetActive(false);
        Time.timeScale = 1;
        _waitingForSignature = false;
    }

    private IEnumerator NextQuest(float delay = 1f)
    {
        yield return new WaitForSeconds(6f);
        AdvanceQuestImmediate();
        yield return new WaitForSeconds(delay);
        AdvanceQuestImmediate();
        _gameDirector.InstantiateOnDemand(0);
        _gameDirector.InstantiateOnDemand(0);
        _gameDirector.InstantiateOnDemand(1);
        yield return new WaitForSeconds(6f);
        _gameDirector.InstantiateOnDemand(0);
        _gameDirector.InstantiateOnDemand(0);

    }

    public void nextSignature()
    {
        if (!_signatureSelected)
        {
            _waitingForSignature = true;
            Time.timeScale = 0;
            _signatureSpellMenu.SetActive(true);
            _signatureSelected = true;
            StartCoroutine(WaitForSignature());
            _signatureButton.SetActive(false);
        }
        else if (!_signatureAdvancedSelected)
        {
            _signatureAdvancedSelected = true;
            Time.timeScale = 0;
            _signatureSpellMenuAdvanced.SetActive(true);
            _signatureAdvancedSelected = true;
            StartCoroutine(WaitForSignatureAdvanced());
            _signatureButton.SetActive(false);

        }
    }
    //Player picks a weapon
    public void PickedWeapon()
    {
        if (!_pickedWeapon)
        {
            _pickedWeapon = true;
            AdvanceQuestImmediate();
        }

    }

    //Player levels an attribute and leaves pause menu
    public void PlayerLeveled()
    {
        if (!_playerLeveled)
        {
            _playerLeveled = true;
            AdvanceQuestImmediate();
        }
    }

    public void PlayerUpgradedAttribute()
    {
        if (!_playerUpgraded)
        {
            _playerUpgraded = true;
            _player.AwardEssence(100);
            AdvanceQuestImmediate();
        }
    }

    //Player kills five enemies
    public void EnemyKilled()
    {
        _enemiesKilled++;
        if (_enemiesKilled == 5)
        {
            if (!_fiveEnemiesKilled)
            {
                _fiveEnemiesKilled = true;
                AdvanceQuestImmediate();
            }

        }
    }

    public void AdvanceQuestImmediate()
    {
        if (_quest.IsFinished())
        {
            if (!_gameDirector.IsEnabled())
            {
                _fogOfWar.GetComponent<Animator>().SetTrigger("fade");
                _gameDirector.SetEnabled(true);
            }

        }
        else _quest.ActivateNextQuest();
    }

    public void ToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayerWon()
    {
        StartCoroutine(FadeAudio(_winSound, 2f));
        StartCoroutine(FadeBackGround(_winText, "WinScreen", 2f));
    }

    public void PlayerDied()
    {
        StartCoroutine(FadeAudio(_gameOverSound));
        StartCoroutine(FadeBackGround(_gameOverText, "GameOver"));
    }

    IEnumerator FadeAudio(AudioClip sound, float fadeDuration = 1f)
    {
        // Find all active AudioSources in the scene
        var thisAudioSource = GetComponent<AudioSource>();
        var audioSources = FindObjectsOfType<AudioSource>().Where(a => a.GetInstanceID() != thisAudioSource.GetInstanceID()).ToArray();
        float timer = 0f;

        // Store original volumes
        thisAudioSource.PlayOneShot(sound);
        var originalVolumes = audioSources.Select(a => a.volume).ToArray();



        // Fade out
        while (timer < fadeDuration)
        {
            float remainingPercentage = 1 - (timer / fadeDuration);
            for (int i = 0; i < audioSources.Length; i++)
            {
                audioSources[i].volume = originalVolumes[i] * remainingPercentage;
            }
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        // Ensure all are silent and stop them
        foreach (var a in audioSources)
        {
            a.volume = 0f;
            a.Stop();
        }

    }

    IEnumerator FadeBackGround(GameObject fader, string scene, float fadeDuration = 1f, float waitTime = 0.5f)
    {
        // Find all active AudioSources in the scene

        float timer = 0f;

        // Store original volumes
        fader.SetActive(true);
        var canvasGroup = fader.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = fader.AddComponent<CanvasGroup>();
        }

        // Fade out
        while (timer < fadeDuration)
        {

            canvasGroup.alpha = timer / fadeDuration;
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(waitTime);
        if (scene == "GameOver")
        {
            SceneManager.LoadScene(scene);
        }
        else
        {
            StartCoroutine(LoadWinScreen());
        }


    }

    IEnumerator LoadWinScreen()
    {
        // Set the current Scene to be able to unload it later
        Scene currentScene = SceneManager.GetActiveScene();

        // The Application loads the Scene in the background at the same time as the current Scene.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_winScreenScene, LoadSceneMode.Additive);

        // Wait until the last operation fully loads to return anything
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Move the GameObject (you attach this in the Inspector) to the newly loaded Scene
        SceneManager.MoveGameObjectToScene(_gameStats, SceneManager.GetSceneByName(_winScreenScene));
        // Unload the previous Scene
        SceneManager.UnloadSceneAsync(currentScene);
    }

    public void OpenConfirmExitPopup()
    {
        _confirmExitButton.SetActive(true);
    }

    public void CloseConfirmExitPopup()
    {
        _confirmExitButton.SetActive(false);
    }

}
