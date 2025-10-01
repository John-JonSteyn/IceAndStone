using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Handles player entry UI and raises navigation events.
/// </summary>
public class PlayerSetupController : MonoBehaviour
{
    public event Action BackRequested;
    public event Action<TeamPair> NextRequested;

    [SerializeField] private UIDocument uiDocument;

    private Label teamAHeader;
    private Label teamBHeader;

    private TextField teamALeadField;
    private TextField teamASecondField;
    private TextField teamAThirdField;
    private TextField teamASkipField;

    private TextField teamBLeadField;
    private TextField teamBSecondField;
    private TextField teamBThirdField;
    private TextField teamBSkipField;

    private Button backButton;
    private Button nextButton;

    private readonly TeamPair working = new();

    private TeamPair pendingTeamInfo;
    private bool uiBound;

    /// <summary>Assigns incoming team info and defers if UI not bound yet.</summary>
    public void SetTeamInfo(TeamPair selection)
    {
        if (selection == null) return;

        if (!uiBound)
        {
            pendingTeamInfo = selection;
            return;
        }

        CopyIntoWorking(selection);
        ApplyHeaders();
        PrefillInputsFromPlayers();
    }

    /// <summary>Binds UI elements and applies any pending data.</summary>
    private void OnEnable()
    {
        var root = uiDocument != null ? uiDocument.rootVisualElement : GetComponent<UIDocument>()?.rootVisualElement;
        if (root == null) return;

        teamAHeader = root.Q<Label>("TeamAHeader");
        teamALeadField = root.Q<TextField>("InFieldTALead");
        teamASecondField = root.Q<TextField>("InFieldTASecond");
        teamAThirdField = root.Q<TextField>("InFieldTAThird");
        teamASkipField = root.Q<TextField>("InFieldTASkip");

        teamBHeader = root.Q<Label>("TeamBHeader");
        teamBLeadField = root.Q<TextField>("InFieldTBLead");
        teamBSecondField = root.Q<TextField>("InFieldTBSecond");
        teamBThirdField = root.Q<TextField>("InFieldTBThird");
        teamBSkipField = root.Q<TextField>("InFieldTBSkip");

        backButton = root.Q<Button>("BtnBack");
        nextButton = root.Q<Button>("BtnNext");

        if (backButton != null) backButton.clicked += HandleBackClicked;
        if (nextButton != null) nextButton.clicked += HandleNextClicked;

        uiBound = true;

        if (pendingTeamInfo != null)
        {
            CopyIntoWorking(pendingTeamInfo);
            pendingTeamInfo = null;
        }

        ApplyHeaders();
        PrefillInputsFromPlayers();
    }

    /// <summary>Unbinds events and marks UI as not ready.</summary>
    private void OnDisable()
    {
        uiBound = false;

        if (backButton != null) backButton.clicked -= HandleBackClicked;
        if (nextButton != null) nextButton.clicked -= HandleNextClicked;
    }

    /// <summary>Raises back navigation event.</summary>
    private void HandleBackClicked() => BackRequested?.Invoke();

    /// <summary>Validates players and raises next navigation event.</summary>
    private void HandleNextClicked()
    {
        working.TeamA.Players = CollectNonEmpty(teamALeadField, teamASecondField, teamAThirdField, teamASkipField);
        working.TeamB.Players = CollectNonEmpty(teamBLeadField, teamBSecondField, teamBThirdField, teamBSkipField);

        if (working.TeamA.Players.Count < 2 || working.TeamB.Players.Count < 2)
        {
            Debug.LogWarning("Each team should have at least two players.");
            return;
        }

        NextRequested?.Invoke(working);
    }

    /// <summary>Applies team names to header labels.</summary>
    private void ApplyHeaders()
    {
        if (teamAHeader != null) teamAHeader.text = working.TeamA.Name ?? "";
        if (teamBHeader != null) teamBHeader.text = working.TeamB.Name ?? "";
    }

    /// <summary>Prefills text fields from existing player data.</summary>
    private void PrefillInputsFromPlayers()
    {
        SetText(teamALeadField, GetPlayerOrEmpty(working.TeamA.Players, 0));
        SetText(teamASecondField, GetPlayerOrEmpty(working.TeamA.Players, 1));
        SetText(teamAThirdField, GetPlayerOrEmpty(working.TeamA.Players, 2));
        SetText(teamASkipField, GetPlayerOrEmpty(working.TeamA.Players, 3));

        SetText(teamBLeadField, GetPlayerOrEmpty(working.TeamB.Players, 0));
        SetText(teamBSecondField, GetPlayerOrEmpty(working.TeamB.Players, 1));
        SetText(teamBThirdField, GetPlayerOrEmpty(working.TeamB.Players, 2));
        SetText(teamBSkipField, GetPlayerOrEmpty(working.TeamB.Players, 3));
    }

    /// <summary>Returns a player name or empty string for index.</summary>
    private static string GetPlayerOrEmpty(List<string> players, int index)
    {
        return (players != null && index >= 0 && index < players.Count) ? players[index] : "";
    }

    /// <summary>Copies data from one TeamPair into working instance.</summary>
    private void CopyIntoWorking(TeamPair source)
    {
        CopyTeam(source.TeamA, working.TeamA);
        CopyTeam(source.TeamB, working.TeamB);
    }

    /// <summary>Copies one team model into another.</summary>
    private static void CopyTeam(TeamModel source, TeamModel destination)
    {
        destination.Name = source.Name;
        destination.Colour = source.Colour;
        destination.HasFirstRound = source.HasFirstRound;
        destination.Players.Clear();
        if (source.Players != null) destination.Players.AddRange(source.Players);
    }

    /// <summary>Sets text for a UI text field.</summary>
    private static void SetText(TextField field, string value)
    {
        if (field != null) field.value = value ?? "";
    }

    /// <summary>Collects non-empty player names from fields.</summary>
    private static List<string> CollectNonEmpty(params TextField[] fields)
    {
        var list = new List<string>(4);
        foreach (var field in fields)
        {
            string value = field?.value?.Trim() ?? "";
            if (!string.IsNullOrWhiteSpace(value)) list.Add(value);
        }
        return list;
    }
}
