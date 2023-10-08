using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{

    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";

    public static OptionsUI Instance { get; private set; }

    [SerializeField] private Button soundEffectsButton;
    [SerializeField] private Scrollbar musicScrollBar;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button MoveUpButton;
    [SerializeField] private Button MoveDownButton;
    [SerializeField] private Button MoveLeftButton;
    [SerializeField] private Button MoveRightButton;
    [SerializeField] private Button InteractButton;
    [SerializeField] private Button InteractAlternateButton;
    [SerializeField] private Button PauseButton;

    [SerializeField] private TextMeshProUGUI soundEffectsText;
    [SerializeField] private TextMeshProUGUI musicScrollBarText;
    [SerializeField] private TextMeshProUGUI MoveUpText;
    [SerializeField] private TextMeshProUGUI MoveDownText;
    [SerializeField] private TextMeshProUGUI MoveLeftText;
    [SerializeField] private TextMeshProUGUI MoveRightText;
    [SerializeField] private TextMeshProUGUI InteractText;
    [SerializeField] private TextMeshProUGUI InteractAlterantText;
    [SerializeField] private TextMeshProUGUI PauseText;
    [SerializeField] private Transform pressToRebindTransform;


    


    private void Awake() {
        Instance = this;

        soundEffectsButton.onClick.AddListener(() => {
            SoundManager.Instance.ChangeVolume();
            UpdateVisual();
        });
        closeButton.onClick.AddListener(() => {
            Hide();
        });
        musicScrollBar.onValueChanged.AddListener((value) => {
            // musicScrollBarText.text = "Music: " + Mathf.Round(value * 10f).ToString();
            MusicManager.Instance.SetVolume(value);
            UpdateVisual();
        });
        MoveUpButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.MoveUp);});
        MoveDownButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.MoveDown);});
        MoveLeftButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.MoveLeft);});
        MoveRightButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.MoveRight);});
        InteractButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Interact);});
        InteractAlternateButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.InteractAlternate);});
        PauseButton.onClick.AddListener(() => { RebindBinding(GameInput.Binding.Pause);});
    }

    private void Start() {
        KitchenGameManager.Instance.OnGameUnpaused += KitchenGameManager_OnGameUnpaused;

        float savedVolume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, .3f);
        musicScrollBar.value = savedVolume;

        UpdateVisual();

        HidePressToRebindKey();
        Hide();
    }

    private void KitchenGameManager_OnGameUnpaused(object sender, System.EventArgs e) {
        Hide();
    }

    private void UpdateVisual() {
        soundEffectsText.text = "Sound Effects: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10f);
        musicScrollBarText.text = "Music: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10f);

        MoveUpText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveUp);
        MoveDownText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveDown);
        MoveLeftText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveLeft);
        MoveRightText.text = GameInput.Instance.GetBindingText(GameInput.Binding.MoveRight);
        InteractText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        InteractAlterantText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlternate);
        PauseText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);

    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ShowPressToRebindKey() {
        pressToRebindTransform.gameObject.SetActive(true);
    }

    private void HidePressToRebindKey() {
        pressToRebindTransform.gameObject.SetActive(false);
    }

    private void RebindBinding(GameInput.Binding binding) {
        ShowPressToRebindKey();
        GameInput.Instance.RebindBinding(binding, () => {
            HidePressToRebindKey();
            UpdateVisual();
        });
    }
}
