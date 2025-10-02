using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>Displays live game info by reading StateMachine.</summary>
public class GameHudController : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;

    private Label _headerLabel;
    private Label _teamAHeader;
    private Label _teamAScores;
    private Label _teamBHeader;
    private Label _teamBScores;

    private void OnEnable()
    {
        var root = _uiDocument != null ? _uiDocument.rootVisualElement : GetComponent<UIDocument>().rootVisualElement;

        _headerLabel = root.Q<Label>("Header");

        var teamAElement = root.Q<VisualElement>("TeamA");
        _teamAHeader = teamAElement.Q<Label>("TeamAHeader");
        _teamAScores = teamAElement.Q<Label>("TeamAScores");

        var teamBElement = root.Q<VisualElement>("TeamB");
        _teamBHeader = teamBElement.Q<Label>("TeamBHeader");
        _teamBScores = teamBElement.Q<Label>("TeamBScores");

        RefreshFromState();
    }

    /// <summary>Refreshes header, team names, and scores from StateMachine.</summary>
    public void RefreshFromState()
    {
        var state = StateMachine.Instance;
        if (state == null) return;

        var sessionNumber = state.SessionNumber;
        var gameNumber = state.GameNumber;
        var teams = state.Teams;
        var aScores = state.GetTeamARoundScores();
        var bScores = state.GetTeamBRoundScores();

        _headerLabel.text = $"Session {sessionNumber} - Game {gameNumber}";
        _teamAHeader.text = teams.TeamA?.Name ?? "Team A";
        _teamBHeader.text = teams.TeamB?.Name ?? "Team B";
        _teamAScores.text = BuildPerRoundBlock(aScores);
        _teamBScores.text = BuildPerRoundBlock(bScores);
    }

    /// <summary>Builds a multi-line block of per-round results.</summary>
    private static string BuildPerRoundBlock(IReadOnlyList<int> perRound)
    {
        if (perRound == null || perRound.Count == 0) return "-";

        var builder = new StringBuilder(perRound.Count * 6);
        for (var roundIndex = 0; roundIndex < perRound.Count; roundIndex++)
        {
            var roundNumber = roundIndex + 1;
            builder.Append($"R{roundNumber}: {perRound[roundIndex]}");
            if (roundIndex < perRound.Count - 1) builder.Append('\n');
        }
        return builder.ToString();
    }
}
