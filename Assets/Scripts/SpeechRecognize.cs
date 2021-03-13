//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
// I totally wrote all this code! - Richard
//

using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections;
using Microsoft.CognitiveServices.Speech.Audio;
using System.IO;
using Microsoft.CognitiveServices.Speech.Translation;
using TMPro;

#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif
#if PLATFORM_IOS
using UnityEngine.iOS;
using System.Collections;
#endif

public class SpeechRecognize : MonoBehaviour
{
    [SerializeField] private string SubscriptionKey;

    [SerializeField] private string SubscriptionRegion;

    private bool _micPermissionGranted = false;
    public Text outputText;
    public Button recoButton;
    TranslationRecognizer _recognizer;
    SpeechTranslationConfig _config;
    AudioConfig _audioInput;
    PushAudioInputStream _pushStream;

    private readonly object _threadLocker = new object();
    private bool _recognitionStarted = false;
    private string _message;
    int _lastSample = 0;
    AudioSource _audioSource;

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

    private void RecognizingHandler(object sender, TranslationRecognitionEventArgs e)
    {
        lock (_threadLocker)
        {
            _message = $"{e.Result.Text} \n {e.Result.Translations["en"]}";
            Debug.Log("RecognizingHandler: " + _message);
        }
    }

    private void RecognizedHandler(object sender, TranslationRecognitionEventArgs e)
    {
        lock (_threadLocker)
        {
            _message = $"{e.Result.Text} \n {e.Result.Translations["en"]}";
            Debug.Log("RecognizedHandler: " + _message);
        }
    }

    private void CanceledHandler(object sender, TranslationRecognitionCanceledEventArgs e)
    {
        lock (_threadLocker)
        {
            _message = e.ErrorDetails.ToString();
            Debug.Log("CanceledHandler: " + _message);
        }
    }

    public async void OnPointerDown()
    {
        if (!Microphone.IsRecording(Microphone.devices[0]))
        {
            Debug.Log("Microphone.Start: " + Microphone.devices[0]);
            _audioSource.clip = Microphone.Start(Microphone.devices[0], true, 200, 16000);
            Debug.Log("audioSource.clip channels: " + _audioSource.clip.channels);
            Debug.Log("audioSource.clip frequency: " + _audioSource.clip.frequency);
        }

        await _recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);
        lock (_threadLocker)
        {
            _recognitionStarted = true;
            Debug.Log("RecognitionStarted: " + _recognitionStarted.ToString());
        }
    }

    public async void OnPointerUp()
    {
        if (_recognitionStarted)
        {
            await _recognizer.StopContinuousRecognitionAsync().ConfigureAwait(true);

            if (Microphone.IsRecording(Microphone.devices[0]))
            {
                Debug.Log("Microphone.End: " + Microphone.devices[0]);
                Microphone.End(null);
                _lastSample = 0;
            }

            lock (_threadLocker)
            {
                _recognitionStarted = false;
                Debug.Log("RecognitionStarted: " + _recognitionStarted.ToString());
            }
        }
    }

    void Start()
    {
        if (outputText == null)
        {
            UnityEngine.Debug.LogError("outputText property is null! Assign a UI Text element to it.");
        }
        else if (recoButton == null)
        {
            _message = "recoButton property is null! Assign a UI Button to it.";
            UnityEngine.Debug.LogError(_message);
        }
        else
        {
            // Continue with normal initialization, Text and Button objects are present.
#if PLATFORM_ANDROID
            // Request to use the microphone, cf.
            // https://docs.unity3d.com/Manual/android-RequestingPermissions.html
            message = "Waiting for mic permission";
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
            _message = "Click button to recognize speech";
#endif
            _config = SpeechTranslationConfig.FromSubscription(SubscriptionKey, SubscriptionRegion);
            _config.SpeechRecognitionLanguage = "es-US";
            _config.AddTargetLanguage("en-US");
            _pushStream = AudioInputStream.CreatePushStream();
            _audioInput = AudioConfig.FromStreamInput(_pushStream);
            _recognizer = new TranslationRecognizer(_config, _audioInput);
            _recognizer.Recognizing += RecognizingHandler;
            _recognizer.Recognized += RecognizedHandler;
            _recognizer.Canceled += CanceledHandler;

            foreach (var device in Microphone.devices)
            {
                Debug.Log("DeviceName: " + device);
            }
            _audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
        }
    }

    void Disable()
    {
        _recognizer.Recognizing -= RecognizingHandler;
        _recognizer.Recognized -= RecognizedHandler;
        _recognizer.Canceled -= CanceledHandler;
        _pushStream.Close();
        _recognizer.Dispose();
    }

    void FixedUpdate()
    {
#if PLATFORM_ANDROID
        if (!micPermissionGranted && Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            micPermissionGranted = true;
            message = "Click button to recognize speech";
        }
#elif PLATFORM_IOS
        if (!micPermissionGranted && Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            micPermissionGranted = true;
            message = "Click button to recognize speech";
        }
#endif
        lock (_threadLocker)
        {
            if (recoButton != null)
            {
                recoButton.interactable = _micPermissionGranted;
            }
            if (outputText != null)
            {
                outputText.text = _message;
            }
        }

        if (Microphone.IsRecording(Microphone.devices[0]) && _recognitionStarted == true)
        {
            GameObject.Find("Option1").GetComponentInChildren<TextMeshProUGUI>().text = "Stop";
            int pos = Microphone.GetPosition(Microphone.devices[0]);
            int diff = pos - _lastSample;

            if (diff > 0)
            {
                float[] samples = new float[diff * _audioSource.clip.channels];
                _audioSource.clip.GetData(samples, _lastSample);
                byte[] ba = ConvertAudioClipDataToInt16ByteArray(samples);
                if (ba.Length != 0)
                {
                    Debug.Log("pushStream.Write pos:" + Microphone.GetPosition(Microphone.devices[0]).ToString() + " length: " + ba.Length.ToString());
                    _pushStream.Write(ba);
                }
            }
            _lastSample = pos;
        }
        else if (!Microphone.IsRecording(Microphone.devices[0]) && _recognitionStarted == false)
        {
            GameObject.Find("Option1").GetComponentInChildren<TextMeshProUGUI>().text = "Start";
        }
    }
}
