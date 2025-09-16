using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using IceAndStone.App.Config;
using IceAndStone.App.Net;

[DefaultExecutionOrder(-1000)]
[DisallowMultipleComponent]
public class Bootstrapper : MonoBehaviour
{
    private static Bootstrapper _instance;
    private Label _hudLabel;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureBootstrapper()
    {
        var existing = FindFirstObjectByType<Bootstrapper>();
        if (existing != null)
            return;

        var prefab = Resources.Load<GameObject>("Bootstrapper");
        if (prefab != null)
        {
            var spawned = Instantiate(prefab);
            spawned.name = "Bootstrapper";
            return;
        }

        new GameObject("Bootstrapper").AddComponent<Bootstrapper>();
    }

    private async void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        var config = await ConfigManager.StartupAsync();
        Debug.Log($"[Bootstrapper] Config OK (ApiBaseUrl={config.ApiBaseUrl}, VenueId={config.VenueId}, LaneId={config.LaneId}, Version={config.AppVersion})");

        ApiInterface.Initialize(config.ApiBaseUrl);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (TryGetComponent<UIDocument>(out var uiDocument))
            BindUiWhenReady(uiDocument, config.AppVersion);
#endif
        _ = CheckApiHealthAsync();
    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    private void BindUiWhenReady(UIDocument uiDocument, string appVersion)
    {
        var root = uiDocument.rootVisualElement;
        if (root.panel == null)
        {
            root.RegisterCallback<AttachToPanelEvent>(_ => ApplyUiBindings(root, appVersion));
            return;
        }

        ApplyUiBindings(root, appVersion);
    }

    private void ApplyUiBindings(VisualElement root, string appVersion)
    {
        _hudLabel = root.Q<Label>("hud");
        if (_hudLabel != null)
        {
            var text = string.IsNullOrWhiteSpace(appVersion) ? "dev" : appVersion;
            _hudLabel.text =
                $"API URL: {ConfigManager.Current.ApiBaseUrl}\n" +
                $"Version: {text}\n" +
                $"Venue ID {ConfigManager.Current.VenueId}\n" +
                $"Lane ID {ConfigManager.Current.LaneId}";
        }
    }
#endif

    private async Task CheckApiHealthAsync()
    {
        var apiOk = false;
        try
        {
            using var cancellationToken = new CancellationTokenSource(2500);
            var response = await ApiInterface.HealthCheckAsync(cancellationToken.Token);
            apiOk = response.IsSuccessStatusCode;
        }
        catch
        {
            apiOk = false;
        }

        if (apiOk)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (_hudLabel != null)
                _hudLabel.text += "\nAPI: OK";
#endif
        }
        else
        {
            Debug.LogWarning("[Bootstrapper] API UNREACHABLE");
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (_hudLabel != null)
                _hudLabel.text += "\nAPI: UNREACHABLE";
#endif
        }
    }
}
