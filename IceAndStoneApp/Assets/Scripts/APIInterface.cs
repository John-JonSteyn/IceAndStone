using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IceAndStone.App.Net
{
    public static class ApiInterface
    {
        private static readonly HttpClient _httpClient = new();
        private static string _baseUrl = "http://localhost:8080";

        #region Startup

        /// <summary>Sets the base API URL.</summary>
        public static void Initialize(string baseUrl)
        {
            _baseUrl = string.IsNullOrWhiteSpace(baseUrl) ? _baseUrl : baseUrl.TrimEnd('/');
        }

        /// <summary>Checks if API is running.</summary>
        public static Task<HttpResponseMessage> HealthCheckAsync(CancellationToken cancellationToken = default)
        {
            return _httpClient.GetAsync($"{_baseUrl}/health", cancellationToken);
        }

        #endregion

        #region Sessions

        /// <summary>Starts a new session on a specific lane.</summary>
        public static Task<HttpResponseMessage> StartSessionAsync(object request, CancellationToken cancellationToken = default)
            => PostAsync("api/sessions/start", request, cancellationToken);

        /// <summary>Ends the current session and closes any active games.</summary>
        public static Task<HttpResponseMessage> EndSessionAsync(object request, CancellationToken cancellationToken = default)
            => PostAsync("api/sessions/end", request, cancellationToken);

        #endregion

        #region Games

        /// <summary>Starts a new game within a session.</summary>
        public static Task<HttpResponseMessage> StartGameAsync(object request, CancellationToken cancellationToken = default)
            => PostAsync("api/games/start", request, cancellationToken);

        /// <summary>Ends the current game and finalises results.</summary>
        public static Task<HttpResponseMessage> EndGameAsync(object request, CancellationToken cancellationToken = default)
            => PostAsync("api/games/end", request, cancellationToken);

        #endregion

        #region Rounds

        /// <summary>Starts a new round in an active game.</summary>
        public static Task<HttpResponseMessage> StartRoundAsync(object request, CancellationToken cancellationToken = default)
            => PostAsync("api/rounds/start", request, cancellationToken);

        /// <summary>Ends the current round and locks its scores.</summary>
        public static Task<HttpResponseMessage> EndRoundAsync(object request, CancellationToken cancellationToken = default)
            => PostAsync("api/rounds/end", request, cancellationToken);

        #endregion

        #region Scores

        /// <summary>Records a team’s score for the current round.</summary>
        public static Task<HttpResponseMessage> PostTeamScoreAsync(object request, CancellationToken cancellationToken = default)
            => PostAsync("api/scores", request, cancellationToken);

        #endregion

        #region Teams

        /// <summary>Creates the two opposing teams for a game.</summary>
        public static Task<HttpResponseMessage> CreateTeamsPairAsync(object request, CancellationToken cancellationToken = default)
            => PostAsync("api/teams/create-pair", request, cancellationToken);

        /// <summary>Adds players to an existing team roster.</summary>
        public static Task<HttpResponseMessage> AddPlayersAsync(object request, CancellationToken cancellationToken = default)
            => PostAsync("api/teams/add-players", request, cancellationToken);

        #endregion

        #region Utility

        /// <summary>Performs a GET and returns the raw response.</summary>
        public static Task<HttpResponseMessage> GetAsync(string path, CancellationToken cancellationToken = default)
        {
            return _httpClient.GetAsync($"{_baseUrl}/{path.TrimStart('/')}", cancellationToken);
        }

        /// <summary>Performs a POST with a JSON body and returns the raw response.</summary>
        public static Task<HttpResponseMessage> PostAsync<T>(string path, T body, CancellationToken cancellationToken = default)
        {
            var json = JsonConvert.SerializeObject(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return _httpClient.PostAsync($"{_baseUrl}/{path.TrimStart('/')}", content, cancellationToken);
        }

        /// <summary>Deserializes a JSON HttpContent payload into T (no error handling).</summary>
        public static async Task<T?> ReadJsonAsync<T>(HttpContent content)
        {
            var text = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(text);
        }

        #endregion
    }
}
