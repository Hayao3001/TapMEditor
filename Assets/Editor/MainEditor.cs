using UnityEngine;
using UnityEditor; // エディタ拡張関連はUnityEditor名前空間に定義されているのでusingしておく。
using System.Collections;
 
// エディタに独自のウィンドウを作成する
public class EditorExWindow : EditorWindow
{
    private float eachspace = 45f;
    private float heightspace = 35f;
    private int sp = 10;
    // メニューのWindowにEditorExという項目を追加。
    [MenuItem("Window/EditorEx")]
    static void Open()
    {
        // メニューのWindow/EditorExを選択するとOpen()が呼ばれる。
        // 表示させたいウィンドウは基本的にGetWindow()で表示＆取得する。
        EditorWindow.GetWindow<EditorExWindow>( "EditorEx" ); // タイトル名を"EditorEx"に指定（後からでも変えられるけど）
    }
 
    // Windowのクライアント領域のGUI処理を記述
    void OnGUI()
    {
        // 試しにラベルを表示
        using (new EditorGUILayout.HorizontalScope())
        {
            using (var right = new EditorGUILayout.VerticalScope(GUILayout.Width(120f)))
            {
                
                GUILayout.Box("I'm inside the button");
                if(GUILayout.Button("add MP3")){
                    Debug.Log("Click Button");
                }
                GUILayout.Box("Music", GUILayout.Height(heightspace-4f),GUILayout.Width(120f));
                for(int i = 0; i <= 3; i++){
                    int a = i + 1;
                    GUILayout.Space(sp);
                    GUILayout.Box("bottomline"+a.ToString(), GUILayout.Height(heightspace),GUILayout.Width(120f));
                }
                for(int i = 0; i <= 2; i++){
                    int a = i + 1;
                    GUILayout.Space(sp);
                    GUILayout.Box("rightline"+a.ToString(), GUILayout.Height(heightspace),GUILayout.Width(120f));
                }
                for(int i = 0; i <= 3; i++){
                    int a = i + 1;
                    GUILayout.Space(sp);
                    GUILayout.Box("topline"+a.ToString(), GUILayout.Height(heightspace),GUILayout.Width(120f));
                }
                for(int i = 0; i <= 2; i++){
                    int a = i + 1;
                    GUILayout.Space(sp);
                    GUILayout.Box("leftline"+a.ToString(), GUILayout.Height(heightspace),GUILayout.Width(120f));
                }
                for(int i = 0; i <= 3; i++){
                    int a = i + 1;
                    GUILayout.Space(sp);
                    GUILayout.Box("diagonal"+a.ToString(), GUILayout.Height(heightspace),GUILayout.Width(120f));
                }
            }

            using (new EditorGUILayout.VerticalScope(GUILayout.Width(1800f)))
            {
                int lineCount = 130;
                float startpoint = 130.0f;
                var space = 13.5f;
                var startPos = new Vector3(startpoint,15.0f,0.0f);
                var endPos = new Vector3((float)lineCount*space+startpoint,15.0f,0.0f);
                var baseLinePos = new Vector3(200.0f,0.0f,0.0f);
                var prev = Handles.color;
                Handles.color = Color.white;
                var xpos = startpoint;
                for (int i = 0; i <= lineCount; i++)
                {
                    var spos = new Vector3(xpos, 15.0f);
                    var vector = new Vector3(xpos, 10.0f, 0f);
                    // baseLinePosは起点のPosition
                    if (i % 5 == 0)
                    {
                        vector.y = 0f;
                    }
                    Handles.DrawLine(spos, vector);
                    xpos = xpos + space;
                }
                Handles.DrawLine(startPos, endPos);
                Handles.color = prev;
                GUILayout.Space(45);
                //musicline
                using(new EditorGUILayout.VerticalScope(GUILayout.Height(eachspace))){
                    GUILayout.Box("", GUILayout.Height(heightspace),GUILayout.Width((float)lineCount*space));
                }
                //bottomline1
                using(new EditorGUILayout.VerticalScope(GUILayout.Height(eachspace))){
                    GUILayout.Box("", GUILayout.Height(heightspace),GUILayout.Width((float)lineCount*space));
                }
                //bottomline2
                using(new EditorGUILayout.VerticalScope(GUILayout.Height(eachspace))){
                    GUILayout.Box("", GUILayout.Height(heightspace),GUILayout.Width((float)lineCount*space));
                }
                //bottomline3
                using(new EditorGUILayout.VerticalScope(GUILayout.Height(eachspace))){
                    GUILayout.Box("", GUILayout.Height(heightspace),GUILayout.Width((float)lineCount*space));
                }
                //bottomline4
                using(new EditorGUILayout.VerticalScope(GUILayout.Height(eachspace))){
                    GUILayout.Box("", GUILayout.Height(heightspace),GUILayout.Width((float)lineCount*space));
                }
                //rightline1
                using(new EditorGUILayout.VerticalScope(GUILayout.Height(eachspace))){
                    GUILayout.Box("", GUILayout.Height(heightspace),GUILayout.Width((float)lineCount*space));
                }
                //rightline2
                using(new EditorGUILayout.VerticalScope(GUILayout.Height(eachspace))){
                    GUILayout.Box("", GUILayout.Height(heightspace),GUILayout.Width((float)lineCount*space));
                }
                //rightline3
                using(new EditorGUILayout.VerticalScope(GUILayout.Height(eachspace))){
                    GUILayout.Box("", GUILayout.Height(heightspace),GUILayout.Width((float)lineCount*space));
                }
                //topline1
                using(new EditorGUILayout.VerticalScope(GUILayout.Height(eachspace))){
                    GUILayout.Box("", GUILayout.Height(heightspace),GUILayout.Width((float)lineCount*space));
                }
                //topline2
                using(new EditorGUILayout.VerticalScope(GUILayout.Height(eachspace))){
                    GUILayout.Box("", GUILayout.Height(heightspace),GUILayout.Width((float)lineCount*space));
                }
                //topline3
                using(new EditorGUILayout.VerticalScope(GUILayout.Height(eachspace))){
                    GUILayout.Box("", GUILayout.Height(heightspace),GUILayout.Width((float)lineCount*space));
                }
                //topline4
                using(new EditorGUILayout.VerticalScope(GUILayout.Height(eachspace))){
                    GUILayout.Box("", GUILayout.Height(heightspace),GUILayout.Width((float)lineCount*space));
                }
                //leftline1
                using(new EditorGUILayout.VerticalScope(GUILayout.Height(eachspace))){
                    GUILayout.Box("", GUILayout.Height(heightspace),GUILayout.Width((float)lineCount*space));
                }
                //leftline2
                using(new EditorGUILayout.VerticalScope(GUILayout.Height(eachspace))){
                    GUILayout.Box("", GUILayout.Height(heightspace),GUILayout.Width((float)lineCount*space));
                }
                //leftline3
                using(new EditorGUILayout.VerticalScope(GUILayout.Height(eachspace))){
                    GUILayout.Box("", GUILayout.Height(heightspace),GUILayout.Width((float)lineCount*space));
                }
                //diagonal1
                using(new EditorGUILayout.VerticalScope(GUILayout.Height(eachspace))){
                    GUILayout.Box("", GUILayout.Height(heightspace),GUILayout.Width((float)lineCount*space));
                }
                //diagonal2
                using(new EditorGUILayout.VerticalScope(GUILayout.Height(eachspace))){
                    GUILayout.Box("", GUILayout.Height(heightspace),GUILayout.Width((float)lineCount*space));
                }
                //diagonal3
                using(new EditorGUILayout.VerticalScope(GUILayout.Height(eachspace))){
                    GUILayout.Box("", GUILayout.Height(heightspace),GUILayout.Width((float)lineCount*space));
                }
                //diagonal4
                using(new EditorGUILayout.VerticalScope(GUILayout.Height(eachspace))){
                    GUILayout.Box("", GUILayout.Height(heightspace),GUILayout.Width((float)lineCount*space));
                }
            }
        }
    }
}