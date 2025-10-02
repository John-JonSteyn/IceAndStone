using IceAndStone.App.Models;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>Handles player entry and writes teams directly into StateMachine.</summary>
public class PlayerSetupController : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;

    #region Properties
    private Label _teamAHeader;
    private Label _teamBHeader;

    private TextField _teamALeadField;
    private TextField _teamASecondField;
    private TextField _teamAThirdField;
    private TextField _teamASkipField;

    private TextField _teamBLeadField;
    private TextField _teamBSecondField;
    private TextField _teamBThirdField;
    private TextField _teamBSkipField;

    private Button _backButton;
    private Button _nextButton;

    private readonly TeamPair _working = new();
    #endregion

    #region Methods
    private void OnEnable()
    {
        var root = _uiDocument != null ? _uiDocument.rootVisualElement : GetComponent<UIDocument>().rootVisualElement;

        _teamAHeader = root.Q<Label>("TeamAHeader");
        _teamALeadField = root.Q<TextField>("InFieldTALead");
        _teamASecondField = root.Q<TextField>("InFieldTASecond");
        _teamAThirdField = root.Q<TextField>("InFieldTAThird");
        _teamASkipField = root.Q<TextField>("InFieldTASkip");

        _teamBHeader = root.Q<Label>("TeamBHeader");
        _teamBLeadField = root.Q<TextField>("InFieldTBLead");
        _teamBSecondField = root.Q<TextField>("InFieldTBSecond");
        _teamBThirdField = root.Q<TextField>("InFieldTBThird");
        _teamBSkipField = root.Q<TextField>("InFieldTBSkip");

        _backButton = root.Q<Button>("BtnBack");
        _nextButton = root.Q<Button>("BtnNext");

        _backButton.clicked += HandleBackClicked;
        _nextButton.clicked += HandleNextClicked;

        var fromState = StateMachine.Instance.Teams;
        CopyIntoWorking(fromState);
        ApplyHeaders();
        PrefillInputsFromPlayers();
    }

    private void OnDisable()
    {
        _backButton.clicked -= HandleBackClicked;
        _nextButton.clicked -= HandleNextClicked;
    }

    /// <summary>Navigates back to team setup.</summary>
    private void HandleBackClicked()
    {
        StateMachine.Instance.GoToState(StateMachine.UiState.TeamSetup);
    }

    /// <summary>Validates players, writes into StateMachine, and goes to game HUD.</summary>
    private void HandleNextClicked()
    {
        _working.TeamA.Players = CollectNonEmpty(_teamALeadField, _teamASecondField, _teamAThirdField, _teamASkipField);
        _working.TeamB.Players = CollectNonEmpty(_teamBLeadField, _teamBSecondField, _teamBThirdField, _teamBSkipField);

        if (_working.TeamA.Players.Count < 2 || _working.TeamB.Players.Count < 2)
        {
            Debug.LogWarning("Each team should have at least two players.");
            return;
        }

        StateMachine.Instance.SetTeam(_working);
        StateMachine.Instance.GoToState(StateMachine.UiState.GameHud);
    }

    /// <summary>Sets the team name labels.</summary>
    private void ApplyHeaders()
    {
        _teamAHeader.text = _working.TeamA.Name ?? "";
        _teamBHeader.text = _working.TeamB.Name ?? "";
    }

    /// <summary>Prefills all text fields from team player lists.</summary>
    private void PrefillInputsFromPlayers()
    {
        SetText(_teamALeadField, GetPlayerOrEmpty(_working.TeamA.Players, 0));
        SetText(_teamASecondField, GetPlayerOrEmpty(_working.TeamA.Players, 1));
        SetText(_teamAThirdField, GetPlayerOrEmpty(_working.TeamA.Players, 2));
        SetText(_teamASkipField, GetPlayerOrEmpty(_working.TeamA.Players, 3));

        SetText(_teamBLeadField, GetPlayerOrEmpty(_working.TeamB.Players, 0));
        SetText(_teamBSecondField, GetPlayerOrEmpty(_working.TeamB.Players, 1));
        SetText(_teamBThirdField, GetPlayerOrEmpty(_working.TeamB.Players, 2));
        SetText(_teamBSkipField, GetPlayerOrEmpty(_working.TeamB.Players, 3));
    }

    /// <summary>Copies a TeamPair into the working buffer.</summary>
    private void CopyIntoWorking(TeamPair source)
    {
        CopyTeam(source.TeamA, _working.TeamA);
        CopyTeam(source.TeamB, _working.TeamB);
    }

    /// <summary>Copies one team model into another.</summary>
    private static void CopyTeam(TeamModel source, TeamModel destination)
    {
        destination.Name = source.Name;
        destination.Colour = source.Colour;
        destination.HasFirstRound = source.HasFirstRound;
        destination.Players.Clear();
        if (source.Players != null)
            destination.Players.AddRange(source.Players);
    }

    /// <summary>Returns a player name or empty string for an index.</summary>
    private static string GetPlayerOrEmpty(List<string> players, int index)
    {
        return (players != null && index >= 0 && index < players.Count) ? players[index] : "";
    }

    /// <summary>Sets a text field’s value.</summary>
    private static void SetText(TextField field, string value)
    {
        field.value = value ?? "";
    }

    /// <summary>Collects non-empty names from text fields.</summary>
    private static List<string> CollectNonEmpty(params TextField[] fields)
    {
        var list = new List<string>(4);
        foreach (var field in fields)
        {
            var value = field.value?.Trim() ?? "";
            if (!string.IsNullOrWhiteSpace(value)) list.Add(value);
        }
        return list;
    }
    #endregion
}
