/*
This is a custom Editor script that lets you add comments/notes to components in the Unity Editor.

Place this script in Assets/Editor

Place EditorCommentComponent with your other normal scripts.

To comment an Object, simply attach an EditorCommentComponent to the object, and type away in the text box.

If you're a super good person, you could even create an empty game object called "_README" in your scene, and then put a comment on that sucker.
Comments for the scene!
*/

@CustomEditor(EditorCommentComponent)
class PluginEditorComment extends Editor {

 var scroll : Vector2;

    function OnInspectorGUI () {
        //target.comment = "no comment";
        ProgressBar (target.comment);

        //var allowSceneObjects : boolean = !EditorUtility.IsPersistent (target);
        //target.gun = EditorGUILayout.ObjectField ("Gun Object", target.gun, GameObject, allowSceneObjects);
    }

    // Custom GUILayout progress bar.
    function ProgressBar ( label : String) {
       
        var guist : GUIStyle = new GUIStyle();
        guist.fontSize = 24;
        guist.fontStyle = FontStyle.Bold;
        //guist.richText = true;
        guist.alignment = TextAnchor.MiddleCenter;
        guist.normal.textColor = Color.white;
        
         GUILayout.Label("NOTES", guist);
        
        //scroll = EditorGUILayout.BeginScrollView(scroll);     
        //GUILayout.ExpandHeight(true)  
        var fieldStyle = new GUIStyle("textfield");
        
        fieldStyle.wordWrap = true;
        fieldStyle.fontSize = 14;
        var ro : RectOffset = new RectOffset();
        ro.left = 25;
        ro.right = 25;
        ro.top = 5;
        ro.bottom = 15;
        fieldStyle.margin = ro;
        
        ro.left = 5;
        ro.right = 5;
        ro.top = 5;
        ro.bottom = 5;
        fieldStyle.padding = ro;
        
        //guist.fontSize = 14;
        target.comment = EditorGUILayout.TextArea(target.comment, fieldStyle);
        //target.comment = EditorGUI.TextArea(target.comment);
        //EditorGUILayout.EndScrollView();
        
    }
}