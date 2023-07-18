using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class PutSound: EditorWindow {
  bool _isRecording = false;
  int _maxRecordingLength = 60;
  AudioClip _microphoneAudioClip;
  Texture2D _microphoneAudioClipVisual;

  [MenuItem("Window/Audio/PutSound")]
  public static void ShowWindow() {
    EditorWindow.GetWindow(typeof(PutSound));
  }

  void OnGUI() {
    EditorGUI.BeginDisabledGroup(_isRecording);
    _maxRecordingLength = EditorGUILayout.IntField("Max Recording Length (seconds)", _maxRecordingLength);
    EditorGUI.EndDisabledGroup();

    if (_isRecording) {
      if (GUILayout.Button("Stop Recording")) {
        string microphoneName = Microphone.devices[0];
        Microphone.End(microphoneName);
        var savePath = Path.Combine(Application.dataPath, DateTime.Now.ToString("yyyyMMddHHmmss") + ".wav");
        SavWav.Save(savePath, _microphoneAudioClip);
        UpdateMicrophoneAudioClipVisual();
        AssetDatabase.Refresh();
        _isRecording = false;
      }
    }
    else {
      if (GUILayout.Button("Start Recording")) {
        string microphoneName = Microphone.devices[0];
        _microphoneAudioClip = Microphone.Start(microphoneName, false, _maxRecordingLength, AudioSettings.outputSampleRate);
        _isRecording = true;
      }
    }

    if (_microphoneAudioClipVisual) {
      GUILayout.Box(_microphoneAudioClipVisual);
      // EditorGUILayout.
      // GUILayout.
      // EditorGUI.DrawPreviewTexture(new Rect(10, 10, 200, 100), _microphoneAudioClipVisual);
    }
  }

  void UpdateMicrophoneAudioClipVisual() {
    int leftPadding = 8;
    int rightPadding = 4;
    int w = Mathf.FloorToInt(EditorGUIUtility.currentViewWidth - (leftPadding + rightPadding));
    int h = 100;
    _microphoneAudioClipVisual = new Texture2D(w, h);
    for (int y = 0; y < h; ++y) {
      for (int x = 0; x < w; ++x) {
        _microphoneAudioClipVisual.SetPixel(x, y, Color.black);
      }
    }
    var data = new float[w];
    _microphoneAudioClip.GetData(data, 0);
    for (int x = 0; x < data.Length; ++x) {
      int y = Mathf.FloorToInt((data[x] / 2f + 0.5f) * h);
      _microphoneAudioClipVisual.SetPixel(x, y, Color.yellow);
    }
    _microphoneAudioClipVisual.Apply();
  }
}
