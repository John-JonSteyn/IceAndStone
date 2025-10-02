using IceAndStone.App.Config;
using IceAndStone.App.Net;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>Shows final results and returns to Welcome after a timeout.</summary>
[DisallowMultipleComponent]
public sealed class ResultsController : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;

    #region Properties
    private Label _headerLabel;
    private Label _resultsLabelA;
    private Label _resultsLabelB;
    private Button _btnNewGame;
    private Button _btnEnd;
    #endregion

    #region Methods
    private void OnEnable()
    {
        var root = _uiDocument != null
            ? _uiDocument.rootVisualElement
            : GetComponent<UIDocument>()?.rootVisualElement;

        _headerLabel = root.Q<Label>("Header");
        _resultsLabelA = root.Q<Label>("LblResultsA");
        _resultsLabelB = root.Q<Label>("LblResultsB");
        _btnNewGame = root.Q<Button>("BtnNewGame");
        _btnEnd = root.Q<Button>("BtnEnd");

        _btnNewGame.clicked += HandleNewGameClicked;
        _btnEnd.clicked += HandleEndClicked;

        Redraw();
    }

    private void OnDisable()
    {
        _btnNewGame.clicked -= HandleNewGameClicked;
        _btnEnd.clicked -= HandleEndClicked;
    }

    private void HandleNewGameClicked()
    {
        StateMachine.Instance.GoToState(StateMachine.UiState.GameHud);
    }

    private void HandleEndClicked()
    {
        StateMachine.Instance.EndSession();
        StateMachine.Instance.GoToWelcome();
    }

    private void Redraw()
    {
        var state = StateMachine.Instance;
        var teams = state.Teams;
        _headerLabel.text = $"Results Game {StateMachine.Instance.GameNumber}";
        var scoresA = state.GetTeamARoundScores();
        var scoresB = state.GetTeamBRoundScores();

        var teamAName = teams?.TeamA?.Name ?? "Team A";
        var teamBName = teams?.TeamB?.Name ?? "Team B";

        _resultsLabelA.text = BuildTeamBlock(teamAName, scoresA);
        _resultsLabelB.text = BuildTeamBlock(teamBName, scoresB);
    }

    private static string BuildTeamBlock(string teamName, IReadOnlyList<int> scores)
    {
        var stringBuilder = new StringBuilder(256);

        stringBuilder.AppendLine(teamName);

        if (scores == null || scores.Count == 0)
        {
            stringBuilder.AppendLine("—");
            return stringBuilder.ToString();
        }

        for (var roundIndex = 0; roundIndex < scores.Count; roundIndex++)
        {
            var roundNumber = roundIndex + 1;
            stringBuilder.AppendLine($"Round {roundNumber}: {Mathf.Max(0, scores[roundIndex])}");
        }

        return stringBuilder.ToString();
    }
    #endregion
}
