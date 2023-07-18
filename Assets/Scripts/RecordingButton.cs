using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Diamond {
  public class RecordingButton: MonoBehaviour {
    [SerializeField] int _maxRecordingLength = 60;

    AudioSource _audioSource;
    Button _button;
    Image _image;
    bool _isRecording = false;
    AudioClip _microphoneAudioClip;
    Color _startColor;
    TMP_Text _text;

    void Awake() {
      _audioSource = GetComponent<AudioSource>();
      _button = GetComponent<Button>();
      _button.onClick.AddListener(OnClick);
      _image = GetComponent<Image>();
      _text = GetComponentInChildren<TMP_Text>();
      _startColor = _image.color;
    }

    public void OnClick() {
      if (_isRecording) {
        StopRecording();
      }
      else {
        StartRecording();
      }
    }

    void StartRecording() {
      _image.color = new Color(1f, 0.2f, 0.2f);
      _text.text = "Stop Recording";

      _audioSource.Stop();
      string microphoneName = Microphone.devices[0];
      _microphoneAudioClip = Microphone.Start(microphoneName, false, _maxRecordingLength, AudioSettings.outputSampleRate);

      _isRecording = true;
    }

    void StopRecording() {
      _image.color = _startColor;
      _text.text = "Start Recording";

      string microphoneName = Microphone.devices[0];
      Microphone.End(microphoneName);
      // _audioSource.clip = _microphoneAudioClip;
      // _audioSource.Play();

      var savePath = Path.Combine(Application.dataPath, System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".wav");
      SavWav.Save(savePath, _microphoneAudioClip);

      _isRecording = false;
    }
  }
}
