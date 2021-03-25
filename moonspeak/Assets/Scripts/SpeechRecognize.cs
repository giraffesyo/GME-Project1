using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections;
using Microsoft.CognitiveServices.Speech.Audio;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CognitiveServices.Speech.Translation;
using TMPro;
using Button = UnityEngine.UI.Button;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif
#if PLATFORM_IOS
using UnityEngine.iOS;
#endif

public class SpeechRecognize : MonoBehaviour
{
    [SerializeField] private string SubscriptionKey;

    [SerializeField] private string SubscriptionRegion;

    private bool _micPermissionGranted = false;
    private bool _processingAnswer = false;
    private bool _processingService = false;
    public TextMeshProUGUI outputText;
    private string _answer;
    public Button recoButton;
    private TranslationRecognizer _translationRecognizer;
    private SpeechRecognizer _speechRecognizer;
    private KeywordRecognizer _keywordRecognizer;
    private SpeechTranslationConfig _translationConfig;
    private SpeechConfig _speechConfig;
    private AudioConfig _audioInput;
    private PushAudioInputStream _pushStream;
    private ReviewManager _reviewManager;

    private readonly object _threadLocker = new object();
    private bool _recognitionStarted = false;
    private string _transcription;
    private string _unfinishedTranscription;
    private int _lastSample = 0;
    private AudioSource _audioSource;

#if PLATFORM_ANDROID || PLATFORM_IOS
    // Required to manifest microphone permission, cf.
    // https://docs.unity3d.com/Manual/android-manifest.html
    private Microphone mic;
#endif

    private byte[] ConvertAudioClipDataToInt16ByteArray(float[] data)
    {
        MemoryStream dataStream = new MemoryStream();
        int x = sizeof(Int16);
        Int16 maxValue = Int16.MaxValue;
        int i = 0;
        while (i < data.Length)
        {
            dataStream.Write(BitConverter.GetBytes(Convert.ToInt16(data[i] * maxValue)), 0, x);
            ++i;
        }
        byte[] bytes = dataStream.ToArray();
        dataStream.Dispose();
        return bytes;
    }

    private void SpeechRecognizingHandler(object sender, SpeechRecognitionEventArgs e)
    {
        lock (_threadLocker)
        {
            _transcription = null;
            _answer = null;
            _unfinishedTranscription = Regex.Replace(e.Result.Text.Trim(), "\\p{P}+", "").ToLower();
        }
    }

    private void SpeechRecognizedHandler(object sender, SpeechRecognitionEventArgs e)
    {
        lock (_threadLocker)
        {
            _unfinishedTranscription = null;
            _transcription = Regex.Replace(e.Result.Best().FirstOrDefault()?.LexicalForm.Trim() ?? "", "\\p{P}+", "").ToLower();
            _answer = _transcription;
        }
        
    }

    private void SpeechCanceledHandler(object sender, SpeechRecognitionCanceledEventArgs e)
    {
        lock (_threadLocker)
        {
            _unfinishedTranscription = null;
            _transcription = e.ErrorDetails.ToString();
        }
    }

    public void OnClick()
    {
        StartCoroutine(ToggleService());
        if (!string.IsNullOrWhiteSpace(_answer))
            StartCoroutine(SubmitAnswer());
    }

    IEnumerator ToggleService()
    {
        _processingService = true;
        if (!_recognitionStarted)
        {
            StartStream();
        }
        else
        {
            StopStream();
        }
        yield return new WaitForSeconds(0.3f);
        _processingService = false;
    }

    async void StartStream()
    {
        if (!Microphone.IsRecording(Microphone.devices[0]))
        {
            _audioSource.clip = Microphone.Start(Microphone.devices[0], true, 200, 16000);
        }
        await _speechRecognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);
            
        lock (_threadLocker)
        {
            _recognitionStarted = true;
        }
    }

    async void StopStream()
    {
        _unfinishedTranscription = null;
        await _speechRecognizer.StopContinuousRecognitionAsync().ConfigureAwait(true);

        if (Microphone.IsRecording(Microphone.devices[0]))
        {
            Microphone.End(null);
            _lastSample = 0;
        }

        lock (_threadLocker)
        {
            _recognitionStarted = false;
        }

    }

    IEnumerator SubmitAnswer()
    {
        // This is really just so users can see what's being submitted for their answer
        _processingAnswer = true;
        yield return new WaitForSeconds(0.75f);
        FindObjectOfType<ReviewManager>().CheckAnswer(_answer);
        _answer = null;
        _processingAnswer = false;
    }

    void Start()
    {
        if (outputText == null)
        {
            UnityEngine.Debug.LogError("outputText property is null! Assign a UI Text element to it.");
        }
        else if (recoButton == null)
        {
            _unfinishedTranscription = "recoButton property is null! Assign a UI Button to it.";
        }
        else
        {
            // Continue with normal initialization, Text and Button objects are present.
            ConfigureMicrophoneAccess();
            ConfigureSpeechRecognizer();
        }
    }

    void ConfigureMicrophoneAccess()
    {
#if PLATFORM_ANDROID
            // Request to use the microphone, cf.
            // https://docs.unity3d.com/Manual/android-RequestingPermissions.html
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
            }
#elif PLATFORM_IOS
        if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            Application.RequestUserAuthorization(UserAuthorization.Microphone);
        }
#else
            _micPermissionGranted = true;
#endif

    }

    void ConfigureSpeechRecognizer()
    {
        _speechConfig = SpeechConfig.FromSubscription(SubscriptionKey, SubscriptionRegion);
        _speechConfig.SpeechRecognitionLanguage = "es-US";
        _speechConfig.OutputFormat = OutputFormat.Detailed;
        _pushStream = AudioInputStream.CreatePushStream();
        _audioInput = AudioConfig.FromStreamInput(_pushStream);
        _speechRecognizer = new SpeechRecognizer(_speechConfig, _audioInput);
        _speechRecognizer.Recognizing += SpeechRecognizingHandler;
        _speechRecognizer.Recognized += SpeechRecognizedHandler;
        _speechRecognizer.Canceled += SpeechCanceledHandler;
        _audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
        _audioSource.loop = false;
        _audioSource.playOnAwake = false;
    }

    void DisableSpeechRecognizer()
    {
        _speechRecognizer.Recognizing -= SpeechRecognizingHandler;
        _speechRecognizer.Recognized -= SpeechRecognizedHandler;
        _speechRecognizer.Canceled -= SpeechCanceledHandler;
        _pushStream.Close();
        _speechRecognizer.Dispose();
    }

    void Dispose()
    {
        DisableSpeechRecognizer();
    }

    void FixedUpdate()
    {
#if PLATFORM_ANDROID
        if (!_micPermissionGranted && Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            _micPermissionGranted = true;
        }
#elif PLATFORM_IOS
        if (!_micPermissionGranted && Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            _micPermissionGranted = true;
        }
#endif
        lock (_threadLocker)
        {
            if (recoButton != null)
            {
                recoButton.interactable = _micPermissionGranted && !_processingService && !_processingAnswer;
            }
            if (outputText != null && _recognitionStarted)
            {
                if (string.IsNullOrWhiteSpace(_transcription) && string.IsNullOrWhiteSpace(_answer))
                {
                    outputText.text = _unfinishedTranscription;
                    outputText.color = Color.red;
                    recoButton.image.color = Color.red;
                    recoButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                    recoButton.GetComponentInChildren<TextMeshProUGUI>().text = "Listening...";
                }
                else if (!string.IsNullOrWhiteSpace(_transcription))
                {
                    _transcription = null;
                    recoButton.onClick.Invoke();
                }
                else
                {
                    outputText.text = _answer;
                    outputText.color = Color.white;
                }
            }
        }

        if (Microphone.IsRecording(Microphone.devices[0]) && _recognitionStarted)
        {
            int pos = Microphone.GetPosition(Microphone.devices[0]);
            int diff = pos - _lastSample;

            if (diff > 0)
            {
                float[] samples = new float[diff * _audioSource.clip.channels];
                _audioSource.clip.GetData(samples, _lastSample);
                byte[] ba = ConvertAudioClipDataToInt16ByteArray(samples);
                if (ba.Length != 0)
                {
                    _pushStream.Write(ba);
                }
            }
            _lastSample = pos;
        }
        else if (!Microphone.IsRecording(Microphone.devices[0]) && !_recognitionStarted)
        {
            outputText.text = _processingAnswer ? _answer : "";
            outputText.color = Color.white;
            recoButton.image.color = _processingAnswer ? Color.red : Color.white;
            recoButton.GetComponentInChildren<TextMeshProUGUI>().color = _processingAnswer ? Color.white : Color.black;
            recoButton.GetComponentInChildren<TextMeshProUGUI>().text = _processingAnswer ? "Checking..." : "Press and say the word";
        }
    }
}
