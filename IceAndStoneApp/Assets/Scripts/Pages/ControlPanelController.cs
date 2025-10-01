using UnityEngine;
using UnityEngine.UIElements;

public class ControlPanelController : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;

    private VisualElement container;
    private TextField teamAScoreText;
    private TextField teamBScoreText;
    private IntegerField teamAScoreInt;
    private IntegerField teamBScoreInt;

    private Button btnAddScoreA;
    private Button btnAddScoreB;
    private Button btnEndRound;
    private Button btnEndGame;

    /// <summary>Binds UI and subscribes to events.</summary>
    private void OnEnable()
    {
        var root = uiDocument != null ? uiDocument.rootVisualElement : GetComponent<UIDocument>()?.rootVisualElement;
        if (root == null) return;

        container = root.Q<VisualElement>("Container");

        teamAScoreText = root.Q<TextField>("TeamAScore");
        teamBScoreText = root.Q<TextField>("TeamBScore");
        teamAScoreInt = root.Q<IntegerField>("TeamAScore");
        teamBScoreInt = root.Q<IntegerField>("TeamBScore");

        btnAddScoreA = root.Q<Button>("BtnAddScoreA");
        btnAddScoreB = root.Q<Button>("BtnAddScoreB");
        btnEndRound = root.Q<Button>("BtnEndRound");
        btnEndGame = root.Q<Button>("BtnEndGame");

        if (btnAddScoreA != null) btnAddScoreA.clicked += HandleAddScoreA;
        if (btnAddScoreB != null) btnAddScoreB.clicked += HandleAddScoreB;
        if (btnEndRound != null) btnEndRound.clicked += HandleEndRound;
        if (btnEndGame != null) btnEndGame.clicked += HandleEndGame;

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
        if (btnAddScoreA != null) btnAddScoreA.clicked -= HandleAddScoreA;
        if (btnAddScoreB != null) btnAddScoreB.clicked -= HandleAddScoreB;
        if (btnEndRound != null) btnEndRound.clicked -= HandleEndRound;
        if (btnEndGame != null) btnEndGame.clicked -= HandleEndGame;
    }

    /// <summary>Shows the control panel.</summary>
    public void Show()
    {
        if (container != null) container.style.display = DisplayStyle.Flex;
    }

    /// <summary>Hides the control panel.</summary>
    public void Hide()
    {
        if (container != null) container.style.display = DisplayStyle.None;
    }

    /// <summary>Toggles control panel visibility.</summary>
    public void Toggle()
    {
        if (container == null) return;
        container.style.display = container.style.display == DisplayStyle.None ? DisplayStyle.Flex : DisplayStyle.None;
    }

    /// <summary>Handles Add Score A button click.</summary>
    private void HandleAddScoreA()
    {
        GameEvents.RaiseAddScoreA(ReadScore(teamAScoreText, teamAScoreInt));
    }

    /// <summary>Handles Add Score B button click.</summary>
    private void HandleAddScoreB()
    {
        GameEvents.RaiseAddScoreB(ReadScore(teamBScoreText, teamBScoreInt));
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
        if (intField != null) return Mathf.Max(0, intField.value);
        if (textField != null && int.TryParse(textField.value, out int parsed)) return Mathf.Max(0, parsed);
        return 0;
    }
}
