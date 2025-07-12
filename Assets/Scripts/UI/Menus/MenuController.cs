using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{

    [SerializeField] private GameObject _helpPanel;

    [SerializeField] private AudioClip _hoverSound;
    [SerializeField] private AudioClip _clickSound;

    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        Assert.IsNotNull(_audioSource, "AudioSource component is missing on MenuController.");
        Assert.IsNotNull(_hoverSound, "Hover sound clip is missing on MenuController.");
        Assert.IsNotNull(_clickSound, "Click sound clip is missing on MenuController.");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGame()
    {
        SceneManager.LoadScene("MainLevel");
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ShowHelpPanel()
    {
        _helpPanel.SetActive(true);
    }

    public void HideHelpPanel()
    {
        _helpPanel.SetActive(false);
    }

    public void PlayHoverSound()
    {
            _audioSource.PlayOneShot(_clickSound);
    }

    public void PlayClickSound()
    {
            _audioSource.PlayOneShot(_hoverSound);
    }
}
