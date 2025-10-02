using UnityEngine;
using UnityEngine.UIElements;

public class TeamSelectionController : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;

    #region Properties
    private TextField _teamANameField;
    private TextField _teamBNameField;
    private RadioButtonGroup _teamAColourGroup;
    private RadioButtonGroup _teamBColourGroup;
    private Button _backButton;
    private Button _nextButton;

    private readonly TeamPair _working = new();
    #endregion

    #region Methods
    private void OnEnable()
    {
        var root = _uiDocument != null ? _uiDocument.rootVisualElement : GetComponent<UIDocument>().rootVisualElement;

        _teamANameField = root.Q<TextField>("TeamAName");
        _teamBNameField = root.Q<TextField>("TeamBName");
        _teamAColourGroup = root.Q<RadioButtonGroup>("TeamAColourGroup");
        _teamBColourGroup = root.Q<RadioButtonGroup>("TeamBColourGroup");
        _backButton = root.Q<Button>("BtnBack");
        _nextButton = root.Q<Button>("BtnNext");

        _backButton.clicked += HandleBackClicked;
        _nextButton.clicked += HandleNextClicked;

        SetInitialValues(StateMachine.Instance.Teams);
    }

    private void OnDisable()
    {
        _backButton.clicked -= HandleBackClicked;
        _nextButton.clicked -= HandleNextClicked;
    }

    private void SetInitialValues(TeamPair initial)
    {
        CopyTeam(initial.TeamA, _working.TeamA);
        CopyTeam(initial.TeamB, _working.TeamB);

        _teamANameField.value = _working.TeamA.Name;
        _teamBNameField.value = _working.TeamB.Name;
        _teamAColourGroup.value = IndexFor(_working.TeamA.Colour);
        _teamBColourGroup.value = IndexFor(_working.TeamB.Colour);
    }

    private void HandleBackClicked()
    {
        StateMachine.Instance.GoToState(StateMachine.UiState.Welcome);
    }

    private void HandleNextClicked()
    {
        _working.TeamA.Name = (_teamANameField.value ?? "").Trim();
        _working.TeamB.Name = (_teamBNameField.value ?? "").Trim();
        _working.TeamA.Colour = ColourFrom(_teamAColourGroup);
        _working.TeamB.Colour = ColourFrom(_teamBColourGroup);

        if (string.IsNullOrWhiteSpace(_working.TeamA.Name) || string.IsNullOrWhiteSpace(_working.TeamB.Name))
        {
            Debug.LogWarning("[TeamSelectionController] Team names cannot be empty.");
            return;
        }

        StateMachine.Instance.SetTeam(_working);
        StateMachine.Instance.GoToState(StateMachine.UiState.PlayerEntry);
    }

    private static void CopyTeam(TeamModel source, TeamModel destination)
    {
        destination.Name = source.Name;
        destination.Colour = source.Colour;
        destination.HasFirstRound = source.HasFirstRound;
        destination.Players.Clear();
        destination.Players.AddRange(source.Players);
    }

    private static TeamColour ColourFrom(RadioButtonGroup group)
    {
        return group.value switch
        {
            0 => TeamColour.Red,
            1 => TeamColour.Orange,
            2 => TeamColour.Yellow,
            3 => TeamColour.Green,
            4 => TeamColour.Blue,
            5 => TeamColour.Purple,
            _ => TeamColour.Red
        };
    }

    private static int IndexFor(TeamColour colour)
    {
        return colour switch
        {
            TeamColour.Red => 0,
            TeamColour.Orange => 1,
            TeamColour.Yellow => 2,
            TeamColour.Green => 3,
            TeamColour.Blue => 4,
            TeamColour.Purple => 5,
            _ => 0
        };
    }
    #endregion
}
