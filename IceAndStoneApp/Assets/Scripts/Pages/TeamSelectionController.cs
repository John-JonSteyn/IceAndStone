using System;
using UnityEngine;
using UnityEngine.UIElements;

public class TeamSelectionController : MonoBehaviour
{
    public event Action BackRequested;
    public event Action<TeamPair> NextRequested;

    [SerializeField] private UIDocument uiDocument;

    private TextField teamANameField;
    private TextField teamBNameField;
    private RadioButtonGroup teamAColourGroup;
    private RadioButtonGroup teamBColourGroup;
    private Button backButton;
    private Button nextButton;

    private readonly TeamPair working = new();

    /// <summary>Initialises UI references and hooks button events.</summary>
    private void OnEnable()
    {
        var root = uiDocument != null ? uiDocument.rootVisualElement : GetComponent<UIDocument>()?.rootVisualElement;
        if (root == null)
        {
            Debug.LogWarning("[TeamSelectionController] Missing UIDocument or rootVisualElement.");
            return;
        }

        teamANameField = root.Q<TextField>("TeamAName");
        teamBNameField = root.Q<TextField>("TeamBName");
        teamAColourGroup = root.Q<RadioButtonGroup>("TeamAColourGroup");
        teamBColourGroup = root.Q<RadioButtonGroup>("TeamBColourGroup");
        backButton = root.Q<Button>("BtnBack");
        nextButton = root.Q<Button>("BtnNext");

        if (backButton != null) backButton.clicked += HandleBackClicked;
        if (nextButton != null) nextButton.clicked += HandleNextClicked;
    }

    /// <summary>Unhooks button events.</summary>
    private void OnDisable()
    {
        if (backButton != null) backButton.clicked -= HandleBackClicked;
        if (nextButton != null) nextButton.clicked -= HandleNextClicked;
    }

    /// <summary>Sets team names and colours from initial values.</summary>
    public void SetInitialValues(TeamPair initial)
    {
        if (initial == null) return;

        CopyTeam(initial.TeamA, working.TeamA);
        CopyTeam(initial.TeamB, working.TeamB);

        if (teamANameField != null) teamANameField.value = working.TeamA.Name;
        if (teamBNameField != null) teamBNameField.value = working.TeamB.Name;
        if (teamAColourGroup != null) teamAColourGroup.value = IndexFor(working.TeamA.Colour);
        if (teamBColourGroup != null) teamBColourGroup.value = IndexFor(working.TeamB.Colour);
    }

    /// <summary>Invokes BackRequested when back button is clicked.</summary>
    private void HandleBackClicked() => BackRequested?.Invoke();

    /// <summary>Validates and applies values, then invokes NextRequested.</summary>
    private void HandleNextClicked()
    {
        working.TeamA.Name = teamANameField != null ? (teamANameField.value ?? "").Trim() : "";
        working.TeamB.Name = teamBNameField != null ? (teamBNameField.value ?? "").Trim() : "";
        working.TeamA.Colour = ColourFrom(teamAColourGroup);
        working.TeamB.Colour = ColourFrom(teamBColourGroup);

        if (string.IsNullOrWhiteSpace(working.TeamA.Name) || string.IsNullOrWhiteSpace(working.TeamB.Name))
        {
            Debug.LogWarning("[TeamSelectionController] Team names cannot be empty.");
            return;
        }

        NextRequested?.Invoke(working);
    }

    /// <summary>Copies values from one team model to another.</summary>
    private static void CopyTeam(TeamModel source, TeamModel destination)
    {
        destination.Name = source.Name;
        destination.Colour = source.Colour;
        destination.HasFirstRound = source.HasFirstRound;
        destination.Players.Clear();
        destination.Players.AddRange(source.Players);
    }

    /// <summary>Gets colour from a radio button group.</summary>
    private static TeamColour ColourFrom(RadioButtonGroup group)
    {
        int index = group != null ? group.value : 0;
        return index switch
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

    /// <summary>Gets index for a given team colour.</summary>
    private static int IndexFor(TeamColour colour) => colour switch
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
