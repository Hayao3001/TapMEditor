using UnityEditor; // エディタ拡張関連はUnityEditor名前空間に定義されているのでusingしておく。
using System.IO;
using System.Media;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.EditorCoroutines.Editor;

// エディタに独自のウィンドウを作成する
public class EditorExWindow : EditorWindow
{
    private float eachspace = 45f;
    private float heightspace = 35f;
    private int sp = 5;
    private Vector2 leftScrollPos = Vector2.zero;
    private Vector2 rightScrollPos = Vector2.zero;
    private int count = 18;
    private string filename;
    private bool isPlay = false;
    private GameObject AudioObj;
    private AudioSource audioSource;
    private AudioClip audioClip;
    private int audiolength;
    private int lineCount;
    private float scale;
    private float stopmusicpos = 0.0f;
    private float space = 13.5f;
    private float offset = 5.0f;
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

    IEnumerator LoadToAudioClipAndPlay(float musicpos){
        if(audioSource == null){
            AudioObj = new GameObject("Audio");

            AudioObj.hideFlags = HideFlags.HideAndDontSave;
            audioSource = AudioObj.AddComponent<AudioSource>();
            if (audioSource == null || string.IsNullOrEmpty(filename)){
                Debug.Log("null break");
                yield break;
            }
            using(WWW www = new WWW(filename)){
                while (!www.isDone)
                    yield return null;
                audioClip = www.GetAudioClip(false, true);
                audioSource.clip = audioClip;
                audioSource.time = musicpos;
                audioSource.Play();
                isPlay = true;
                lineCount = (int)audioClip.length + 1;
            } 
        }
        if(!(audioSource.isPlaying)){
            AudioObj = new GameObject("Audio");

            AudioObj.hideFlags = HideFlags.HideAndDontSave;
            audioSource = AudioObj.AddComponent<AudioSource>();
            if (audioSource == null || string.IsNullOrEmpty(filename)){
                Debug.Log("null break");
                yield break;
            }
            using(WWW www = new WWW(filename)){
                while (!www.isDone)
                    yield return null;
                audioClip = www.GetAudioClip(false, true);
                audioSource.clip = audioClip;
                audioSource.time = musicpos;
                audioSource.Play();
                isPlay = true;
                lineCount = (int)audioClip.length + 1;
            }
        }
    }
    void RedLine(float height, float offset){
        var prevColor = Handles.color;
        Handles.color = Color.red;
        var startLinePos = new Vector3(offset,15.0f,0.0f);
        var endLinePos = new Vector3(offset,height,0.0f);
        Handles.DrawLine(startLinePos,endLinePos);
        Handles.color = prevColor;
    }
    // Windowのクライアント領域のGUI処理を記述
    void OnGUI()
    {
        // 試しにラベルを表示
        EditorGUILayout.BeginHorizontal( GUI.skin.box );
        EditorGUILayout.BeginVertical( GUI.skin.box,GUILayout.Width ( 150 ) );
        leftScrollPos = EditorGUILayout.BeginScrollView( leftScrollPos,GUI.skin.box );
        {
            if(GUILayout.Button("再生")){
                EditorCoroutineUtility.StartCoroutine(LoadToAudioClipAndPlay(stopmusicpos), this);
            }
            if(GUILayout.Button("一時停止")){
                if(audioClip != null){
                    stopmusicpos=audioSource.time;
                    Debug.Log(audioSource.time);
                    audioSource.Pause();
                    isPlay = false;
                }
            }
            if(GUILayout.Button("停止")){
                if(audioClip != null){
                    stopmusicpos=0.0f;
                    audioSource.Stop();
                    isPlay = false;
                    offset = 5.0f;
                }
            }
            if(GUILayout.Button("add MP3")){
                Debug.Log("Click Button");
                filename = EditorUtility.OpenFilePanel("Open wav", "", "WAV");
                if (string.IsNullOrEmpty(filename))
                    return;
                Debug.Log(filename);
            }
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

        // RedLine();
        scale = EditorGUILayout.Slider(scale, 1, 10,GUILayout.Width(200f));
        rightScrollPos = EditorGUILayout.BeginScrollView( rightScrollPos,GUI.skin.box );
        {
            RedLine(position.height, offset);
            EditorGUILayout.BeginHorizontal( GUI.skin.box );
            float startpoint = 5.0f;
            var startPos = new Vector3(startpoint,15.0f,0.0f);
            var endPos = new Vector3(((float)lineCount*space+startpoint)*scale,15.0f,0.0f);
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
                xpos = xpos + space * scale;
            }
            Handles.DrawLine(startPos, endPos);
            Handles.color = prev;
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(55f);
            for (int i = 0; i < count; i++){
                EditorGUILayout.BeginHorizontal( GUI.skin.box );
                GUILayout.Box("", GUILayout.Height(heightspace),GUILayout.Width(((float)lineCount*space)*scale));
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(sp-5.0f);
            }
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }
    void Update()
    {
        if(isPlay){
            offset = offset + 0.00406f*(space*scale);
        }
        //((Time.deltaTime)/50)
    }
}