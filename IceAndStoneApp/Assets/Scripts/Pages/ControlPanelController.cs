using UnityEngine;
using UnityEngine.UIElements;

public class ControlPanelController : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;

    #region Properties
    private VisualElement _container;
    private TextField _teamAScoreText;
    private TextField _teamBScoreText;
    private IntegerField _teamAScoreInt;
    private IntegerField _teamBScoreInt;

    private Button _btnAddScoreA;
    private Button _btnAddScoreB;
    private Button _btnEndRound;
    private Button _btnEndGame;
    #endregion

    #region Methods
    /// <summary>Binds UI and subscribes to events.</summary>
    private void OnEnable()
    {
        var root = uiDocument != null ? uiDocument.rootVisualElement : GetComponent<UIDocument>()?.rootVisualElement;
        if (root == null) return;

        _container = root.Q<VisualElement>("Container");

        _teamAScoreText = root.Q<TextField>("TeamAScore");
        _teamBScoreText = root.Q<TextField>("TeamBScore");
        _teamAScoreInt = root.Q<IntegerField>("TeamAScore");
        _teamBScoreInt = root.Q<IntegerField>("TeamBScore");

        _btnAddScoreA = root.Q<Button>("BtnAddScoreA");
        _btnAddScoreB = root.Q<Button>("BtnAddScoreB");
        _btnEndRound = root.Q<Button>("BtnEndRound");
        _btnEndGame = root.Q<Button>("BtnEndGame");

        if (_btnAddScoreA != null) _btnAddScoreA.clicked += HandleAddScoreA;
        if (_btnAddScoreB != null) _btnAddScoreB.clicked += HandleAddScoreB;
        if (_btnEndRound != null) _btnEndRound.clicked += HandleEndRound;
        if (_btnEndGame != null) _btnEndGame.clicked += HandleEndGame;

        Hide();
    }

    /// <summary>Handles hotkey toggle.</summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            Toggle();
    }

    /// <summary>Unsubscribes from events.</summary>
    private void OnDisable()
    {
        if (_btnAddScoreA != null) _btnAddScoreA.clicked -= HandleAddScoreA;
        if (_btnAddScoreB != null) _btnAddScoreB.clicked -= HandleAddScoreB;
        if (_btnEndRound != null) _btnEndRound.clicked -= HandleEndRound;
        if (_btnEndGame != null) _btnEndGame.clicked -= HandleEndGame;
    }

    /// <summary>Shows the control panel.</summary>
    public void Show()
    {
        if (_container != null)
            _container.style.display = DisplayStyle.Flex;
    }

    /// <summary>Hides the control panel.</summary>
    public void Hide()
    {
        if (_container != null)
            _container.style.display = DisplayStyle.None;
    }

    /// <summary>Toggles control panel visibility.</summary>
    public void Toggle()
    {
        if (_container == null)
            return;
        _container.style.display = _container.style.display == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None;
    }

    /// <summary>Handles Add Score A button click.</summary>
    private void HandleAddScoreA()
    {
        GameEvents.RaiseAddScoreA(ReadScore(_teamAScoreText, _teamAScoreInt));
    }

    /// <summary>Handles Add Score B button click.</summary>
    private void HandleAddScoreB()
    {
        GameEvents.RaiseAddScoreB(ReadScore(_teamBScoreText, _teamBScoreInt));
    }

    /// <summary>Handles End Round button click.</summary>
    private void HandleEndRound()
    {
        GameEvents.RaiseEndRound();
    }

    /// <summary>Handles End Game button click.</summary>
    private void HandleEndGame()
    {
        GameEvents.RaiseEndGame();
    }

    /// <summary>Parses score from integer or text field.</summary>
    private static int ReadScore(TextField textField, IntegerField intField)
    {
        if (intField != null)
            return Mathf.Max(0, intField.value);
        if (textField != null && int.TryParse(textField.value, out int parsed))
            return Mathf.Max(0, parsed);
        return 0;
    }
    #endregion
}
