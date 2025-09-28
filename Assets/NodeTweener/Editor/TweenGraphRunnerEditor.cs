using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(TweenGraphRunner))]
public class TweenGraphRunnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TweenGraphRunner runner = (TweenGraphRunner)target;

        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Execution Controls", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Play"))
        {
            runner.PlaySequence();
        }

        if (GUILayout.Button("Stop"))
        {
            runner.StopSequence();
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Pause"))
        {
            runner.PauseSequence();
        }

        if (GUILayout.Button("Resume"))
        {
            runner.ResumeSequence();
        }

        GUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Status:", runner.IsPlaying() ? "Playing" : "Stopped");

        EditorGUILayout.Space();
        if (runner.tweenGraph != null && GUILayout.Button("Open in Graph Editor"))
        {
            NodeBasedTweenEditor.OpenWindow();
        }
    }
}
