using System.Collections;
using TMPro;
using UnityEngine;

public class Quest : MonoBehaviour
{
    private int _nextQuestIndex = 0;

    private bool _isInProgress = true;
    [SerializeField] private AudioClip[] _questSounds;
    [SerializeField] private string[] _questDescriptions;

    void Start()
    {
        TextMeshProUGUI objectiveText = GetComponent<TextMeshProUGUI>();
        StartCoroutine(FadeText(objectiveText));
    }
    public void ActivateNextQuest()
    {

        if (_nextQuestIndex < _questSounds.Length && _nextQuestIndex < _questDescriptions.Length)
        {
            _isInProgress = true;
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().PlayOneShot(_questSounds[_nextQuestIndex]);
            TextMeshProUGUI objectiveText = GetComponent<TextMeshProUGUI>();
            objectiveText.text = _questDescriptions[_nextQuestIndex];
            objectiveText.alpha = 1; // Reset alpha to fully visible
            StopAllCoroutines();
            StartCoroutine(FadeText(objectiveText));
            _nextQuestIndex++;
        }
        else
        {
            Debug.Log("No more quests available.");
        }
    }

    private IEnumerator FadeText(TextMeshProUGUI textToFade, float duration = 5f)
    {
        float elapsed = 0f;
        while (elapsed <= duration)
        {
            elapsed += Time.deltaTime;
            textToFade.alpha = Mathf.Lerp(1, 0, elapsed / duration);
            yield return null;
        }
        textToFade.alpha = 0;
        _isInProgress = false;

    }

    public bool IsInProgress()
    {
        return _isInProgress;
    }
    
    public bool IsFinished()
    {
        return _nextQuestIndex >= _questSounds.Length;
    }
}
