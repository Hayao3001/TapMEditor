using UnityEditor; // エディタ拡張関連はUnityEditor名前空間に定義されているのでusingしておく。
using System.IO;
using System.Media;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.EditorCoroutines.Editor;


//Todo
//Resourves.Loadでできるようにする
public class CSV : MonoBehaviour
{
    public static void AddCSV(string filename, string linename, string tyepname, float startime, float endtime){
        var list = "";
        Debug.Log(filename);
        if(!HasData(Path.GetFileNameWithoutExtension(filename))){
            list = linename + "," + tyepname + "," + startime.ToString() + "," + endtime.ToString() + "\n";
        }else{
            list = linename + "," + tyepname + "," + startime.ToString() + "," + endtime.ToString() + "\n";
        }
        if(filename == null){
            return;
        }
        using (var file = new StreamWriter(filename, true, System.Text.Encoding.GetEncoding("UTF-8"))){
            file.Write(list);
        }
    }

    public static void GetCSV(string filename, List<string[]> csvDatas){
        var csvfile = Resources.Load(filename) as TextAsset;
        StringReader file = new StringReader(csvfile.text);
        while(file.Peek() != -1){
            string line = file.ReadLine();
            csvDatas.Add(line.Split(','));
        }
    }

    public static bool HasData(string filename){
        int count = 0;
        var csvfile = Resources.Load(filename) as TextAsset;
        StringReader file = new StringReader(csvfile.text);
        while(file.Peek() != -1){
            string line = file.ReadLine();
            count++;
        }
        if(count == 0){
            return false;
        }else{
            Debug.Log(count);
            return true;
        }
    }
}

// エディタに独自のウィンドウを作成する
public class EditMusicScore : EditorWindow
{
    private const float eachspace = 45f;
    private const float heightspace = 35f;
    private const int sp = 5;
    private Vector2 leftScrollPos = Vector2.zero;
    private Vector2 rightScrollPos = Vector2.zero;
    private const int count = 18;
    private string filename;
    private static bool isPlay = false;
    private GameObject AudioObj;
    private AudioSource audioSource;
    private AudioClip audioClip;
    private int audiolength;
    private int lineCount;
    private float scale;
    private static float stopmusicpos = 0.0f;
    private float space = 13.5f;
    private float offset = 0.0f;
    private float time;

    // メニューのWindowにEditorExという項目を追加。
    [MenuItem("Window/EditMusicScore")]
    static void Open()
    {
        EditorWindow.GetWindow<EditMusicScore>( "Edit_music_score" ); 
    }

    /// <summary>
    /// サマリー
    /// </summary>
    /// <param name="musicpos"></param>
    /// <returns></returns>
    IEnumerator LoadToAudioClipAndPlay(float musicpos){
        if(audioSource == null || !(audioSource.isPlaying)){
            AudioObj = new GameObject("Audio");

            AudioObj.hideFlags = HideFlags.HideAndDontSave;
            audioSource = AudioObj.AddComponent<AudioSource>();
            if (audioSource == null || string.IsNullOrEmpty(filename) || audioSource.isPlaying){
                yield break;
            }
            using(WWW www = new WWW(filename)){
                while (!www.isDone)
                    yield return null;
                audioClip = www.GetAudioClip(false, true);
                audioSource.clip = audioClip;
                if(musicpos == time){
                    audioSource.time = musicpos;
                }else{
                    audioSource.time = time;
                }
                audioSource.Play();
                isPlay = true;
                lineCount = (int)audioClip.length + 1;
            } 
        }
    }
    void PaintRedLine(float height, float width){
        var prevColor = Handles.color;
        Handles.color = Color.red;
        var startLinePos = new Vector3(width,15.0f,0.0f);
        var endLinePos = new Vector3(width,height,0.0f);
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
                    AddBlock.ChangeStateSet();
                    audioSource.Pause();
                    time = audioSource.time;
                    isPlay = false;
                }
            }
            if(GUILayout.Button("停止")){
                if(audioClip != null){
                    stopmusicpos=0.0f;
                    audioSource.Stop();
                    AddBlock.ChangeStateSet();
                    isPlay = false;
                    offset = 0.0f;
                    time = 0.0f;
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
        time = EditorGUILayout.DelayedFloatField(time);
        rightScrollPos = EditorGUILayout.BeginScrollView( rightScrollPos,GUI.skin.box );
        {
            var perspace = (space*scale);
            if(audioSource == null){
                offset = 0.0f;
            }else if(audioSource.time == time){
                offset = (time)*perspace;
            }else{
                offset = (audioSource.time)*perspace;
            }
            PaintRedLine(1000f, offset);
            EditorGUILayout.BeginHorizontal( GUI.skin.box );
            float startpoint = 0.0f;
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
            GUILayout.Space(35f);
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
        var perspace = (space*scale);
        if(isPlay){
            offset = (audioSource.time)*perspace;
        }
        Repaint();
    }


    public static float GetMusicTime(){
        return stopmusicpos;
    }

    public static bool GetisPlay(){
        return isPlay;
    }
}

public class AddBlock: EditorWindow{
    private int _LineIndex;
    private int _TypeIndex;
    private float startime = 0.0f;
    private float endtime = 0.0f;
    private static bool canSet = true;
    private string filepath;
    private string filename = "MusicData";
    public static List<string[]> CSVData = new List<string[]>();

    private static readonly string[] Line = new string[]
    {
        "bottomline1","bottomline2","bottomline3","bottomline4", "rightline1","rightline2","rightline3", 
        "topline1","topline2","topline3","topline4", "leftline1","leftline2","leftline3",
        "diagonal1","diagonal2","diagona3","diagonal4"
    };

    private static readonly string[] BlockType = new string[]
    {
        "Tap", "Long"
    };
    [MenuItem("Window/AddBlock")]
    static void Open()
    {
        EditorWindow.GetWindow<AddBlock>( "addblock" ); 
    }

    void OnGUI(){
        if(!EditMusicScore.GetisPlay() && !canSet){
            startime = EditMusicScore.GetMusicTime();
            endtime = EditMusicScore.GetMusicTime();
            canSet = !canSet;
        }
        if(GUILayout.Button("Add File")){
            filepath = EditorUtility.OpenFilePanel("Open csv", "", "CSV");
            if(filepath == null){
                return;
            }else{
                filename = Path.GetFileNameWithoutExtension(filepath);
            }
        }
        filename = EditorGUILayout.DelayedTextField("filename", filename);
        GUILayout.Space(15f);
        GUILayout.Label("Now Time");
        startime = EditorGUILayout.DelayedFloatField(startime);
        GUILayout.Label("Select line");
        _LineIndex = GUILayout.SelectionGrid(_LineIndex, Line, 3);
        GUILayout.Label("Select Block Type");
        _TypeIndex = GUILayout.SelectionGrid(_TypeIndex, BlockType, 3);
        if(_TypeIndex == 1){
            GUILayout.Label("Input Last time");
            endtime = EditorGUILayout.DelayedFloatField(endtime);
        }
        GUILayout.Space(35f);
        if(GUILayout.Button("Add")){
            if(_TypeIndex == 1){
                CSV.AddCSV(filepath, Line[_LineIndex], BlockType[_TypeIndex], startime, endtime);
            }else{
                CSV.AddCSV(filepath, Line[_LineIndex], BlockType[_TypeIndex], startime, 0.0f);
            }
            CSV.GetCSV(filename, CSVData);
            Debug.Log(CSVData[1][0]==null);
        }
    }

    void Update(){
        Repaint();
        // CSV.GetCSV(filename, CSVData);
        // CSV.HasData(filename);
        // Debug.Log("csvDatas:"+CSVData[1][2]);
    }

    public static void ChangeStateSet(){
        canSet = !canSet;
    }

    public static List<string[]> GetMusicData(){
        return CSVData;
    }
}


