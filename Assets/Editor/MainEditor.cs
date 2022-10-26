using UnityEngine;
using UnityEditor; // エディタ拡張関連はUnityEditor名前空間に定義されているのでusingしておく。
using System.Collections;
 
// エディタに独自のウィンドウを作成する
public class EditorExWindow : EditorWindow
{
    private Vector3 startPos = new Vector3(200.0f,15.0f,0.0f);
    private Vector3 endPos = new Vector3(2000.0f,15.0f,0.0f);
    private int lineCount = 200;
    private Vector3 baseLinePos = new Vector3(200.0f,0.0f,0.0f);
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
            using (var right = new EditorGUILayout.VerticalScope(GUILayout.Width(200f)))
            {
                GUILayout.Box("I'm inside the button");
                if(GUILayout.Button("add MP3")){
                    Debug.Log("Click Button");
                }
            }

            using (new EditorGUILayout.VerticalScope(GUILayout.Width(1800f)))
            {
                var prev = Handles.color;
                Handles.color = Color.white;
                var xpos = 200.0f;
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
                    xpos = xpos + 10.0f;
                }
                Handles.DrawLine(startPos, endPos);
                Handles.color = prev;
            }
        }
    }
}