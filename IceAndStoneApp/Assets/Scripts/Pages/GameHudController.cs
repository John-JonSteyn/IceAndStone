using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

public class GameHudController : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;

    private Label headerLabel;
    private Label teamAHeader;
    private Label teamAScores;
    private Label teamBHeader;
    private Label teamBScores;

    private readonly List<int> teamARoundScores = new List<int>();
    private readonly List<int> teamBRoundScores = new List<int>();
    private int pendingScoreA;
    private int pendingScoreB;

    private TeamPair teams = new TeamPair();
    private int sessionNumber;
    private int gameNumber;

    /// <summary>Binds UI and subscribes to events.</summary>
    private void OnEnable()
    {
        var root = uiDocument != null
            ? uiDocument.rootVisualElement
            : GetComponent<UIDocument>()?.rootVisualElement;

        if (root == null) return;

        headerLabel = root.Q<Label>("Header");

        var teamAElement = root.Q<VisualElement>("TeamA");
        teamAHeader = teamAElement?.Q<Label>("TeamAHeader");
        teamAScores = teamAElement?.Q<Label>("TeamAScores");

        var teamBElement = root.Q<VisualElement>("TeamB");
        teamBHeader = teamBElement?.Q<Label>("TeamBHeader");
        teamBScores = teamBElement?.Q<Label>("TeamBScores");

        GameEvents.AddScoreARequested += HandleAddScoreARequested;
        GameEvents.AddScoreBRequested += HandleAddScoreBRequested;
        GameEvents.EndRoundRequested += HandleEndRoundRequested;
        GameEvents.EndGameRequested += HandleEndGameRequested;

        RedrawAll();
    }

    /// <summary>Unsubscribes from events.</summary>
    private void OnDisable()
    {
        GameEvents.AddScoreARequested -= HandleAddScoreARequested;
        GameEvents.AddScoreBRequested -= HandleAddScoreBRequested;
        GameEvents.EndRoundRequested -= HandleEndRoundRequested;
        GameEvents.EndGameRequested -= HandleEndGameRequested;
    }

    /// <summary>Sets session and game numbers.</summary>
    public void SetSessionAndGame(int newSessionNumber, int newGameNumber)
    {
        sessionNumber = newSessionNumber;
        gameNumber = newGameNumber;
        RedrawHeader();
    }

    /// <summary>Sets active team names and info.</summary>
    public void SetTeams(TeamPair teamPair)
    {
        teams = teamPair ?? new TeamPair();
        RedrawTeamHeaders();
    }

    /// <summary>Clears round scores and pending values.</summary>
    public void ResetScores()
    {
        teamARoundScores.Clear();
        teamBRoundScores.Clear();
        pendingScoreA = 0;
        pendingScoreB = 0;
        RedrawScores();
    }

    /// <summary>Returns copy of Team A scores.</summary>
    public IReadOnlyList<int> GetTeamARoundScores() => teamARoundScores.ToArray();

    /// <summary>Returns copy of Team B scores.</summary>
    public IReadOnlyList<int> GetTeamBRoundScores() => teamBRoundScores.ToArray();

    /// <summary>Handles add score event for Team A.</summary>
    private void HandleAddScoreARequested(int score)
    {
        pendingScoreA = Mathf.Max(0, score);
    }

    /// <summary>Handles add score event for Team B.</summary>
    private void HandleAddScoreBRequested(int score)
    {
        pendingScoreB = Mathf.Max(0, score);
    }

    /// <summary>Handles end round event.</summary>
    private void HandleEndRoundRequested()
    {
        teamARoundScores.Add(pendingScoreA);
        teamBRoundScores.Add(pendingScoreB);

        pendingScoreA = 0;
        pendingScoreB = 0;

        RedrawScores();
    }

    /// <summary>Handles end game event.</summary>
    private void HandleEndGameRequested()
    {
        Debug.Log("[GameHudController] End Game requested.");
    }

    /// <summary>Redraws all UI elements.</summary>
    private void RedrawAll()
    {
        RedrawHeader();
        RedrawTeamHeaders();
        RedrawScores();
    }

    /// <summary>Updates header text.</summary>
    private void RedrawHeader()
    {
        if (headerLabel != null)
            headerLabel.text = $"Session {sessionNumber} - Game {gameNumber}";
    }

    /// <summary>Updates team name labels.</summary>
    private void RedrawTeamHeaders()
    {
        if (teamAHeader != null) teamAHeader.text = teams.TeamA?.Name ?? "Team A";
        if (teamBHeader != null) teamBHeader.text = teams.TeamB?.Name ?? "Team B";
    }

    /// <summary>Updates round scores display.</summary>
    private void RedrawScores()
    {
        if (teamAScores != null) teamAScores.text = BuildPerRoundBlock(teamARoundScores);
        if (teamBScores != null) teamBScores.text = BuildPerRoundBlock(teamBRoundScores);
    }

    /// <summary>Builds text block for per-round results.</summary>
    private static string BuildPerRoundBlock(List<int> perRound)
    {
        if (perRound == null || perRound.Count == 0) return "-";

        var stringBuilder = new StringBuilder(perRound.Count * 6);
        for (int i = 0; i < perRound.Count; i++)
        {
            int roundNumber = i + 1;
            stringBuilder.Append($"R{roundNumber}: {perRound[i]}");
            if (i < perRound.Count - 1) stringBuilder.Append('\n');
        }
        return stringBuilder.ToString();
    }
}
