using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class GameOverMenuController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _gameOverUI;
    [SerializeField] private SpriteRenderer _fader;

    [SerializeField] private AudioClip _hoverSound;
    [SerializeField] private AudioClip _clickSound;

    [SerializeField] private bool _rainEnabled;

    [SerializeField] private GameObject _rainPrefab;

    [SerializeField] private Transform _rainAnchor;

    [SerializeField] private TextMeshProUGUI _menuText;
    [SerializeField] private Color _textColorBase;
    [SerializeField] private Color _textColorAlternate;

    private AudioSource _audioSource;
    void Start()
    {
        _audioSource = transform.Find("ButtonsSounds").GetComponent<AudioSource>();
        Assert.IsNotNull(_audioSource, "AudioSource component is missing on MenuController.");
        Color faderColor = _fader.color;
        faderColor.a = 1;
        _fader.color = faderColor;
        _gameOverUI.alpha = 0f;
        StartCoroutine(WaitAndPlayMusic());
    }

    private IEnumerator WaitAndPlayMusic(float waitTime = 0.5f, float fadeTime = 1f)
    {
        Debug.Log("Waiting for " + waitTime + " seconds before playing music.");
        yield return new WaitForSeconds(waitTime);
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        audioSource.volume = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = elapsedTime / fadeTime;
            _gameOverUI.alpha = elapsedTime / fadeTime;
            yield return null;
        }

        audioSource.volume = 1f;
        _gameOverUI.alpha = 1f;

        StartCoroutine(FadeBackground(fadeTime));


    }

    void FixedUpdate()
    {
        if (_rainEnabled)
        {
            Instantiate(_rainPrefab, new Vector3(Random.Range(_rainAnchor.position.x - 100, _rainAnchor.position.x + 100), _rainAnchor.position.y, _rainAnchor.position.z), Quaternion.identity);
            float t = Mathf.PingPong(Time.time, 1f);
            _menuText.color = Color.Lerp(_textColorBase, _textColorAlternate, t / 1f);

        }
    }

    private IEnumerator FadeBackground(float fadeTime)
    {
        Color color = _fader.color;
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            color.a = 1f - elapsedTime / fadeTime;
            _fader.color = color;
            yield return null;
        }
        color.a = 0f;
        _fader.color = color;
    }

    public void PlayHoverSound()
    {
        _audioSource.PlayOneShot(_clickSound);
    }

    public void PlayClickSound()
    {
        _audioSource.PlayOneShot(_hoverSound);
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ToMainLevel()
    {
        SceneManager.LoadScene("MainLevel");
    }
    
   
}
