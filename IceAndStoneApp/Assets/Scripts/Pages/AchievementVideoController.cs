using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

[DisallowMultipleComponent]
public sealed class AchievementVideoController : MonoBehaviour
{
    [Header("UI Binding")]
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private string videoElementName = "VideoContainer";

    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RenderTexture renderTexture;

    [Header("Achievement Clips")]
    [SerializeField] private VideoClip houseCarlClip;
    [SerializeField] private VideoClip icebreakerClip;
    [SerializeField] private VideoClip jarlClip;
    [SerializeField] private VideoClip kingClip;
    [SerializeField] private VideoClip lokisLuckClip;
    [SerializeField] private VideoClip lookingForLeifClip;
    [SerializeField] private VideoClip mightyMjolnirClip;
    [SerializeField] private VideoClip nilfheimsTouchClip;
    [SerializeField] private VideoClip odinsOffspringClip;
    [SerializeField] private VideoClip thaneClip;

    private VisualElement videoElement;
    private Texture2D bridgeTexture;

    private void OnEnable()
    {
        var root = uiDocument != null
            ? uiDocument.rootVisualElement
            : GetComponent<UIDocument>()?.rootVisualElement;

        if (root == null) return;

        videoElement = root.Q<VisualElement>(videoElementName);

        Hide();
    }

    private void Update()
    {
        CopyRenderTextureToTexture2D();
    }

    public void Hide()
    {
        if (videoElement != null)
            videoElement.style.display = DisplayStyle.None;
    }

    private void CopyRenderTextureToTexture2D()
    {
        if (renderTexture == null || videoElement == null) return;

        if (bridgeTexture == null ||
            bridgeTexture.width != renderTexture.width ||
            bridgeTexture.height != renderTexture.height)
        {
            bridgeTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        }

        RenderTexture.active = renderTexture;
        bridgeTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        bridgeTexture.Apply();
        RenderTexture.active = null;

        videoElement.style.backgroundImage = new StyleBackground(bridgeTexture);
    }
}
