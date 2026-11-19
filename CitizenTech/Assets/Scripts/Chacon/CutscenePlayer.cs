using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

public class CutscenePlayer : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Painel da cutscene (RawImage + botão Skip). Começa DESATIVADO.")]
    [SerializeField] private GameObject root;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private RawImage videoImage;
    [SerializeField] private Button skipButton;

    [Header("Vídeo")]
    [SerializeField] private VideoClip clip;

    [Header("Pular")]
    [SerializeField] private KeyCode skipKey = KeyCode.Escape;

    [Tooltip("Use pra parar a música do menu, por exemplo.")]
    public UnityEvent onStarted;

    private Action onFinished;
    private RenderTexture renderTexture;
    private bool playing;

    private void Start()
    {
        skipButton.onClick.AddListener(Finish);
        if (root != null)
            root.SetActive(false);
    }

    private void Update()
    {
        if (playing && Input.GetKeyDown(skipKey))
            Finish();
    }

    public void Play(Action finishedCallback)
    {
        if (playing)
            return;

        if (clip == null || videoPlayer == null)
        {
            Debug.LogWarning("[CutscenePlayer] Sem clip ou VideoPlayer, pulando a cutscene.", this);
            finishedCallback?.Invoke();
            return;
        }

        playing = true;
        onFinished = finishedCallback;

        root.SetActive(true);
        foreach (Transform child in root.transform)
            child.gameObject.SetActive(true);
        videoPlayer.gameObject.SetActive(true);
        onStarted?.Invoke();

        renderTexture = new RenderTexture((int)clip.width, (int)clip.height, 0);
        videoImage.texture = renderTexture;

        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.clip = clip;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;

        // Áudio pelo AudioSource (o modo Direct não funciona no Windows).
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
            audioSource.volume = PlayerPrefs.GetFloat("MasterVolume", 1f);

            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.controlledAudioTrackCount = 1;
            videoPlayer.EnableAudioTrack(0, true);
            videoPlayer.SetTargetAudioSource(0, audioSource);
        }

        videoPlayer.prepareCompleted += OnPrepared;
        videoPlayer.loopPointReached += OnEnded;
        videoPlayer.errorReceived += OnError;
        videoPlayer.Stop();
        videoPlayer.time = 0;
        videoPlayer.frame = 0;
        videoPlayer.skipOnDrop = false;
        videoPlayer.waitForFirstFrame = true;
        videoPlayer.Prepare();
    }

    private void OnPrepared(VideoPlayer vp)
    {
        vp.time = 0;
        Debug.Log($"[CutscenePlayer] Vídeo pronto: {vp.width}x{vp.height}, {vp.length:F1}s");
        vp.Play();
    }

    private void OnEnded(VideoPlayer vp) => Finish();

    private void OnError(VideoPlayer vp, string message)
    {
        Debug.LogError($"[CutscenePlayer] Erro no vídeo: {message}", this);
        Finish(); // não deixa o jogador preso na cutscene
    }

    // Chamado pelo fim do vídeo, pelo botão Skip e pela tecla.
    private void Finish()
    {
        if (!playing)
            return;
        playing = false;

        videoPlayer.prepareCompleted -= OnPrepared;
        videoPlayer.loopPointReached -= OnEnded;
        videoPlayer.errorReceived -= OnError;
        videoPlayer.Stop();

        // O painel fica ligado de propósito: o fundo preto cobre a tela até o loading assumir.
        Action callback = onFinished;
        onFinished = null;
        callback?.Invoke();
    }

    private void OnDestroy()
    {
        if (renderTexture != null)
            renderTexture.Release();
    }
}