using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GUILayoutWindow : EditorWindow
{
    string myString = "La La Land is a 2016 American musical romantic drama film" +
        "written and directed by Damien Chazelle, and starring Ryan Gosling and Emma Stone" +
        "as a musician and an aspiring actress who meet and fall in love in Los Angeles." +
        "The film's title refers both to the city of Los Angeles and to the idiom" +
        "for being out of touch with reality.";

    bool myBool = true;
    float myFloat = 1.23f;
    int toolbarIndex = 0;
    string[] toolbars = { "toolbar1", "toolbar2", "toolbar3" };
    public float sliderValue = 1.0F;

    [MenuItem("Window/GUI Layout Window")]
    static void Init()
    {
        GUILayoutWindow window =
            (GUILayoutWindow)EditorWindow.GetWindow(typeof(GUILayoutWindow));

        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0, 0, 400, 400));

        GUILayout.BeginHorizontal();
        if (GUILayout.RepeatButton("Repeat\nButton")) 
            Debug.Log("Repeat Button") ;

        if (GUILayout.Button("Button"))
            Debug.Log("Button");

        GUILayout.FlexibleSpace();

        GUILayout.BeginVertical();
        GUILayout.Box("Value:" + Mathf.Round(sliderValue));
        sliderValue = GUILayout.HorizontalSlider(sliderValue, 0.0f, 10);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        GUILayout.Label("Label");
        GUILayout.Space(30);

        myBool = GUILayout.Toggle(myBool, "Toggle");
        toolbarIndex = GUILayout.Toolbar(toolbarIndex, toolbars);
        myString = GUILayout.TextArea(myString);
        myString = GUILayout.TextField(myString);
        myString = GUILayout.PasswordField(myString, '#');

        myFloat = GUILayout.HorizontalSlider(myFloat, 0f, 5f);
        myFloat = GUILayout.VerticalSlider(myFloat, 0f, 5f);
        GUILayout.EndArea();





    }

}
