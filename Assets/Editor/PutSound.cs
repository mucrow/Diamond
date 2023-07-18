using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class PutSound: EditorWindow {
  bool _isRecording = false;
  int _maxRecordingLength = 60;
  AudioClip _microphoneAudioClip;

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
  }
}
