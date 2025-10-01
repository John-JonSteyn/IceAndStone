using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Displays final results and auto-returns after timeout.
/// </summary>
[DisallowMultipleComponent]
public sealed class ResultsController : MonoBehaviour
{
    public event Action DoneRequested;
    public event Action TimedOut;

    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private float autoReturnSeconds = 30f;

    private Label headerLabel;
    private Label resultsLabel;
    private Button buttonDone;

    private float countdownSeconds;
    private bool isCountingDown;

    private TeamPair teams;
    private IReadOnlyList<int> teamAScores;
    private IReadOnlyList<int> teamBScores;

    /// <summary>Initializes UI and starts countdown.</summary>
    private void OnEnable()
    {
        var root = uiDocument != null
            ? uiDocument.rootVisualElement
            : GetComponent<UIDocument>()?.rootVisualElement;

        if (root == null) return;

        headerLabel = root.Q<Label>("Header");
        resultsLabel = root.Q<Label>("LblResults");
        buttonDone = root.Q<Button>("BtnNext");

        if (headerLabel != null)
            headerLabel.text = "Results";
        if (buttonDone != null)
            buttonDone.clicked += HandleDoneClicked;

        Redraw();

        countdownSeconds = Mathf.Max(1f, autoReturnSeconds);
        isCountingDown = true;
    }

    /// <summary>Stops countdown and unsubscribes events.</summary>
    private void OnDisable()
    {
        if (buttonDone != null) buttonDone.clicked -= HandleDoneClicked;
        isCountingDown = false;
    }

    /// <summary>Handles countdown for timeout.</summary>
    private void Update()
    {
        if (!isCountingDown) return;

        countdownSeconds -= Time.deltaTime;
        if (countdownSeconds <= 0f)
        {
            isCountingDown = false;
            TimedOut?.Invoke();
        }
    }

    /// <summary>Sets teams and scores for display.</summary>
    public void SetResults(TeamPair teamPair, IReadOnlyList<int> teamARoundScores, IReadOnlyList<int> teamBRoundScores)
    {
        teams = teamPair ?? new TeamPair();
        teamAScores = teamARoundScores ?? Array.Empty<int>();
        teamBScores = teamBRoundScores ?? Array.Empty<int>();
        Redraw();

        countdownSeconds = Mathf.Max(1f, autoReturnSeconds);
        isCountingDown = true;
    }

    /// <summary>Handles Done button click.</summary>
    private void HandleDoneClicked()
    {
        DoneRequested?.Invoke();
    }

    /// <summary>Updates results UI text.</summary>
    private void Redraw()
    {
        if (resultsLabel == null) return;

        string teamAName = teams?.TeamA?.Name ?? "Team A";
        string teamBName = teams?.TeamB?.Name ?? "Team B";

        var stringBuilder = new StringBuilder(256);

        stringBuilder.AppendLine(teamAName);
        if (teamAScores == null || teamAScores.Count == 0)
        {
            stringBuilder.AppendLine("—");
        }
        else
        {
            for (int roundIndex = 0; roundIndex < teamAScores.Count; roundIndex++)
                stringBuilder.AppendLine($"Round {roundIndex + 1}: {Mathf.Max(0, teamAScores[roundIndex])}");
        }
        stringBuilder.AppendLine();

        stringBuilder.AppendLine(teamBName);
        if (teamBScores == null || teamBScores.Count == 0)
        {
            stringBuilder.AppendLine("—");
        }
        else
        {
            for (int roundIndex = 0; roundIndex < teamBScores.Count; roundIndex++)
                stringBuilder.AppendLine($"Round {roundIndex + 1}: {Mathf.Max(0, teamBScores[roundIndex])}");
        }

        resultsLabel.text = stringBuilder.ToString();
    }
}
