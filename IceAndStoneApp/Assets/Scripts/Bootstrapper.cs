using UnityEngine;
using UnityEngine.UIElements;
using IceAndStone.App.Config;

[DefaultExecutionOrder(-1000)]
[DisallowMultipleComponent]
public class Bootstrapper : MonoBehaviour
{
    private static Bootstrapper _instance;
    private Label _versionLabel;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureBootstrapper()
    {
        var existing = FindFirstObjectByType<Bootstrapper>();
        if (existing != null) return;

        var prefab = Resources.Load<GameObject>("Bootstrapper");
        if (prefab != null)
        {
            var spawned = Instantiate(prefab);
            spawned.name = "Bootstrapper";
            return;
        }

        var go = new GameObject("Bootstrapper")
        {
            hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontUnloadUnusedAsset
        };
        go.AddComponent<Bootstrapper>();
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        ConfigManager.Startup();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (TryGetComponent<UIDocument>(out var uiDocument))
            BindVersionWhenReady(uiDocument);
#endif
    }

    private void BindVersionWhenReady(UIDocument uiDocument)
    {
        var root = uiDocument.rootVisualElement;
        if (root == null)
            return;

        if (root.panel == null)
        {
            root.RegisterCallback<AttachToPanelEvent>(_ => ApplyVersionToLabel(root));
            return;
        }

        ApplyVersionToLabel(root);
    }

    private void ApplyVersionToLabel(VisualElement root)
    {
        _versionLabel = root.Q<Label>("versionNumber");
        if (_versionLabel == null) return;

        var text = string.IsNullOrWhiteSpace(ConfigManager.Current.AppVersion)
            ? "dev"
            : ConfigManager.Current.AppVersion;

        _versionLabel.text = $"v{text}";
    }
}
