using TMPro;
using UnityEngine;

public class Bonfire : MonoBehaviour
{
    public GameObject _promptNotEnoughEssence;
    public GameObject _promptEnoughEssence;
    public GameObject _buyPanel;
    public StatusBar _StatusBar;
    public Player _player;
    public int _upgradeCost = 500;
    private bool _firstActivation = true;
    private bool _playerCloseToBonfire = false;


    //Indicates wheter to ignore the holding of key E until it's pressed again.
    private bool _ignorePressedKey = false;

    //Holds the prompt that will show up when player is close.
    private GameObject _prompt;

    private bool _lit = false;

    void Update()
    {
        _prompt = (_player._essence < _upgradeCost) ? _promptNotEnoughEssence : _promptEnoughEssence;

        if (_firstActivation)
        {
            _promptEnoughEssence.GetComponent<TextMeshProUGUI>().text = $"[E] Light ({_upgradeCost} essence)";
            _promptNotEnoughEssence.GetComponent<TextMeshProUGUI>().text = $"Need {_upgradeCost} essence!";
            transform.GetComponentInChildren<ParticleSystem>().Stop();
            _prompt.SetActive(false);
            _buyPanel.SetActive(false);
            _firstActivation = false;
        }

        if (!_lit)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                _ignorePressedKey = false;
            }

            if (Input.GetKeyUp(KeyCode.E))
            {
                CloseUpgradePanel();
            }

            if (Input.GetKey(KeyCode.E) && !_ignorePressedKey && _player._essence >= _upgradeCost)
            {
                if (_playerCloseToBonfire) OpenUpgradePanel();

            }
        }
    }

    public void OnClickSideEffect(Player.UpgradeType upgradeType)
    {
        Debug.Log($"BUY PANEL Selected: {upgradeType}");
        _player.AwardEssence(-_upgradeCost);
        _ignorePressedKey = true;
        CloseUpgradePanel();
        Light();
        _StatusBar.AddInstance(upgradeType);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !_lit)
        {
            _playerCloseToBonfire = true;
            if (!_lit) _prompt.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _playerCloseToBonfire = false;
            if(_prompt != null) _prompt.SetActive(false);
        }
    }
    private void OpenUpgradePanel()
    {
        _prompt.SetActive(false);
        _buyPanel.SetActive(true);
        _player.SetControlsEnabled(false);
        _buyPanel.GetComponent<BuyItemButton>().activated = true;
        _buyPanel.GetComponent<BuyItemButton>().SetBonfire(this);
        Time.timeScale = 0;
    }

    private void CloseUpgradePanel()
    {
        _prompt.SetActive(true);
        _buyPanel.SetActive(false);
        _player.SetControlsEnabled(true);
        _buyPanel.GetComponent<BuyItemButton>().activated = false;
        Time.timeScale = 1;
    }

    //Lights the baonfire, advancing the objective and preventing it from being used again.
    void Light()
    {
        _lit = true;
        _prompt.SetActive(false);
        _promptEnoughEssence.SetActive(false);
        _promptNotEnoughEssence.SetActive(false);
        GetComponent<AudioSource>().Play();
        transform.GetComponentInChildren<ParticleSystem>().Play();
    }

    public bool IsLit()
    {
        return _lit;
    }
}
