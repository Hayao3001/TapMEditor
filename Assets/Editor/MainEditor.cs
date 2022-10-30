using UnityEngine;
using UnityEditor; // エディタ拡張関連はUnityEditor名前空間に定義されているのでusingしておく。
using System.Collections;
using System.IO;
using System.Media;

// エディタに独自のウィンドウを作成する
public class EditorExWindow : EditorWindow
{
    private float eachspace = 45f;
    private float heightspace = 35f;
    private int sp = 5;
    private Vector2 leftScrollPos = Vector2.zero;
    private Vector2 rightScrollPos = Vector2.zero;
    private int count = 19;
    private string filename;
    // メニューのWindowにEditorExという項目を追加。
    [MenuItem("Window/EditorEx")]
    static void Open()
    {
        // メニューのWindow/EditorExを選択するとOpen()が呼ばれる。
        // 表示させたいウィンドウは基本的にGetWindow()で表示＆取得する。
        EditorWindow.GetWindow<EditorExWindow>( "EditorEx" ); // タイトル名を"EditorEx"に指定（後からでも変えられるけど）
    }
 
    // public static void PlayClip( AudioClip clip )
    // {
    //     var unityEditorAssembly = typeof( AudioImporter ).Assembly;
    //     var audioUtilClass = unityEditorAssembly.GetType( "UnityEditor.AudioUtil" );

    //     var method = audioUtilClass.GetMethod
    //     (
    //         "PlayClip",
    //         BindingFlags.Static | BindingFlags.Public,
    //         null,
    //         new Type[] { typeof(AudioClip) },
    //         null
    //     );

    //     method.Invoke( null, new object[] { clip } );
    // }
    // Windowのクライアント領域のGUI処理を記述
    void OnGUI()
    {
        // 試しにラベルを表示
        EditorGUILayout.BeginHorizontal( GUI.skin.box );
        EditorGUILayout.BeginVertical( GUI.skin.box,GUILayout.Width ( 120 ) );
        leftScrollPos = EditorGUILayout.BeginScrollView( leftScrollPos,GUI.skin.box );
        {
            if(GUILayout.Button("再生")){
                if(filename != null){
                    var player = new SoundPlayer( filename );
                    player.Play();   
                }
            }
            if(GUILayout.Button("add MP3")){
                Debug.Log("Click Button");
                filename = EditorUtility.OpenFilePanel("Open wav", "", "WAV");
                if (string.IsNullOrEmpty(filename))
                    return;
                Debug.Log(filename);
            }
            GUILayout.Box("Music", GUILayout.Height(heightspace-4f),GUILayout.Width(120f));
            for(int i = 0; i <= 3; i++){
                int a = i + 1;
                GUILayout.Space(sp+3);
                GUILayout.Box("bottomline"+a.ToString(), GUILayout.Height(heightspace),GUILayout.Width(120f));
            }
            for(int i = 0; i <= 2; i++){
                int a = i + 1;
                GUILayout.Space(sp+3);
                GUILayout.Box("rightline"+a.ToString(), GUILayout.Height(heightspace),GUILayout.Width(120f));
            }
            for(int i = 0; i <= 3; i++){
                int a = i + 1;
                GUILayout.Space(sp+3);
                GUILayout.Box("topline"+a.ToString(), GUILayout.Height(heightspace),GUILayout.Width(120f));
            }
            for(int i = 0; i <= 2; i++){
                int a = i + 1;
                GUILayout.Space(sp+3);
                GUILayout.Box("leftline"+a.ToString(), GUILayout.Height(heightspace),GUILayout.Width(120f));
            }
            for(int i = 0; i <= 3; i++){
                int a = i + 1;
                GUILayout.Space(sp+3);
                GUILayout.Box("diagonal"+a.ToString(), GUILayout.Height(heightspace),GUILayout.Width(120f));
            }
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical( GUI.skin.box);

        rightScrollPos = EditorGUILayout.BeginScrollView( rightScrollPos,GUI.skin.box );
        {
            EditorGUILayout.BeginHorizontal( GUI.skin.box );
            int lineCount = 190;
            float startpoint = 5.0f;
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
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(25f);
            for (int i = 0; i < count; i++){
                EditorGUILayout.BeginHorizontal( GUI.skin.box );
                GUILayout.Box("", GUILayout.Height(heightspace),GUILayout.Width((float)lineCount*space));
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(sp-5.0f);
            }
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }
}