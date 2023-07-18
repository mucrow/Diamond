using UnityEngine;
using UnityEngine.Serialization;

namespace Diamond {
  public class ScaleWithLoudness: MonoBehaviour {
    [SerializeField] float _loudnessCoeff = 5f;
    [SerializeField] float _minScale = 0.25f;
    [SerializeField] float _releaseTime = 0.05f;
    [SerializeField] bool _useMicrophone;

    float _scale = 1f;

    AudioClip _microphoneAudioClip;
    AudioSource _audioSource;

    void Awake() {
      _audioSource = GetComponent<AudioSource>();
      if (_useMicrophone) {
        string microphoneName = Microphone.devices[0];
        _microphoneAudioClip = Microphone.Start(microphoneName, true, 5 * 60, AudioSettings.outputSampleRate);
        _audioSource.clip = _microphoneAudioClip;
        _audioSource.loop = true;
        _audioSource.Play();
      }
    }

    void Update() {
      float loudnessRange = _loudnessCoeff - _minScale;

      if (_scale > _minScale) {
        _scale -= loudnessRange * (Time.deltaTime / _releaseTime);
        _scale = Mathf.Max(_scale, _minScale);
      }

      float loudness = GetLoudness();
      var targetScale = loudness * _loudnessCoeff;
      _scale = Mathf.Max(targetScale, _scale, _minScale);
      transform.localScale = new Vector3(1f, 1f, 1f) * _scale;
    }

    float GetLoudness() {
      if (_useMicrophone) {
        return GetLoudnessFromMicrophone();
      }
      return GetLoudnessFromAudioClip(_audioSource.timeSamples, _audioSource.clip);
    }

    float GetLoudnessFromMicrophone() {
      return GetLoudnessFromAudioClip(Microphone.GetPosition(Microphone.devices[0]), _microphoneAudioClip);
    }

    static float GetLoudnessFromAudioClip(int clipPosition, AudioClip clip) {
      int sampleWindow = 60;
      int startPosition = clipPosition - sampleWindow;

      if (startPosition < 0) {
        return 0f;
      }

      float[] waveData = new float[sampleWindow];
      clip.GetData(waveData, startPosition);

      float totalLoudness = 0f;
      for (int i = 0; i < sampleWindow; ++i) {
        totalLoudness += Mathf.Abs(waveData[i]);
      }

      return totalLoudness / sampleWindow;
    }
  }
}
