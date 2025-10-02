using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

[DisallowMultipleComponent]
public sealed class AchievementVideoController : MonoBehaviour
{
    #region Editor Fields
    [Header("UI Binding")]
    [SerializeField] private UIDocument _uiDoc;

    [Header("Video")]
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private RenderTexture _renderTexture;

    [Header("Achievement Clips")]
    [SerializeField] private VideoClip _vidClipHouseCarl;
    [SerializeField] private VideoClip _vidClipIceBreaker;
    [SerializeField] private VideoClip _vidClipJarl;
    [SerializeField] private VideoClip _vidClipKing;
    [SerializeField] private VideoClip _vidClipLoki;
    [SerializeField] private VideoClip _vidClipLeif;
    [SerializeField] private VideoClip _vidClipMjolnir;
    [SerializeField] private VideoClip _vidClipNilfheim;
    [SerializeField] private VideoClip _vidClipOdin;
    [SerializeField] private VideoClip _vidClipThane;
    #endregion

    #region Properties
    private VisualElement videoElement;
    private Texture2D bridgeTexture;
    #endregion

    #region Methods
    private void OnEnable()
    {
        var root = _uiDoc != null
            ? _uiDoc.rootVisualElement
            : GetComponent<UIDocument>()?.rootVisualElement;

        if (root == null) return;

        videoElement = root.Q<VisualElement>("VideoContainer");

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
        if (_renderTexture == null || videoElement == null) return;

        if (bridgeTexture == null ||
            bridgeTexture.width != _renderTexture.width ||
            bridgeTexture.height != _renderTexture.height)
        {
            bridgeTexture = new Texture2D(_renderTexture.width, _renderTexture.height, TextureFormat.RGBA32, false);
        }

        RenderTexture.active = _renderTexture;
        bridgeTexture.ReadPixels(new Rect(0, 0, _renderTexture.width, _renderTexture.height), 0, 0);
        bridgeTexture.Apply();
        RenderTexture.active = null;

        videoElement.style.backgroundImage = new StyleBackground(bridgeTexture);
    }
    #endregion
}
