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
    private static int rownum = 0;

    public static void AddCSV(string filename, string linename, string tyepname, float startime, float endtime){
        var list = "";
        Debug.Log(filename);
        list = linename + "," + tyepname + "," + startime.ToString() + "," + endtime.ToString() + "\n";
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
        rownum = 0;
        while(file.Peek() != -1){
            string line = file.ReadLine();
            csvDatas.Add(line.Split(','));
            rownum = rownum + 1;
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

    public static int GetRowNum(){
        return rownum;
    }
}

// エディタに独自のウィンドウを作成する
public class EditMusicScore : EditorWindow
{
    private const float eachspace = 45f;
    private const float heightspace = 35f;
    private const int sp = 5;
    private const float basepos = 85f;
    private const float perbasepos = 47.2f;
    private Vector2 leftScrollPos = Vector2.zero;
    private Vector2 rightScrollPos = Vector2.zero;
    // private const int count = 18;
    private string filename;
    private static bool isPlay = false;
    private GameObject AudioObj;
    private AudioSource audioSource;
    private AudioClip audioClip;
    private int audiolength;
    private int lineCount;
    private float scale;
    private static float stopmusicpos = 0.0f;
    private float space = 180f;
    private float offset = 0.0f;
    private float time;
    private static List<string[]> CSVData = new List<string[]>();
    private static List<string[]> MusicData = new List<string[]>();
    private static string[] linename = new string[]{};
    private static int[] perlinecount = new int[]{};
    private const int linenum = 18;
    private Rect lineRect;
    private float bpm = 240f;

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
        var per_bpm = (float)bpm/(float)240;
        var prevColor = Handles.color;
        Handles.color = Color.red;
        var startLinePos = new Vector3(width*per_bpm,15.0f,0.0f);
        var endLinePos = new Vector3(width*per_bpm,height,0.0f);
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
            if(GUILayout.Button("Update")){
                UpdateCSV();
            }
            GUILayout.Label("BPM");
            bpm = EditorGUILayout.DelayedFloatField(bpm);
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
    //
        // RedLine();
        scale = EditorGUILayout.Slider(scale, 1, 10,GUILayout.Width(200f));
        var per_measure_text = "per_measure:"+((float)240/(float)bpm).ToString();
        var per_measure_text_16 = "per_measure_16:"+((float)240/(float)bpm/(float)16).ToString();
        GUILayout.Label(per_measure_text);
        GUILayout.Label(per_measure_text_16);
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
            float before_xpos_per = 0f;
            for (int i = 0; i <= lineCount; i++)
            {
                var spos = new Vector3(xpos, 15.0f);
                var vector = new Vector3(xpos, 2.5f, 0f);
                var beat_4_top = new Vector3(before_xpos_per, 15.0f, 0f);
                var beat_4_bottom = new Vector3(before_xpos_per, 1000.0f, 0f);
                var beat_8_top = new Vector3(before_xpos_per, 15.0f, 0f);
                var beat_8_bottom = new Vector3(before_xpos_per, 1000.0f, 0f);
                var beat_12_top = new Vector3(before_xpos_per, 15.0f, 0f);
                var beat_12_bottom = new Vector3(before_xpos_per, 1000.0f, 0f);
                var beat_16_top = new Vector3(before_xpos_per, 15.0f, 0f);
                var beat_16_bottom = new Vector3(before_xpos_per, 1000.0f, 0f);
                int beat_count_4 = 0;
                int beat_count_8 = 0;
                int beat_count_12 = 0;
                // baseLinePosは起点のPosition
                if (i % 5 == 0)
                {
                    vector.y = 0f;
                }
                Handles.DrawLine(spos, vector);
                if(xpos != 0.0f){
                    for(int beat = 0; beat < 16; beat++){
                        beat_16_top.x = (space * scale) / 16 + beat_16_top.x;
                        beat_16_bottom.x = beat_16_top.x;
                        var beatColor = Handles.color;
                        Handles.color = new Color(35f,35f,35f,0.1f);
                        Handles.DrawLine(beat_16_top, beat_16_bottom);
                        Handles.color = beatColor;
                        if(beat_count_4 < 4){
                            beat_4_top.x = (space * scale) / 4 + beat_4_top.x;
                            beat_4_bottom.x = beat_4_top.x;
                            beatColor = Handles.color;
                            Handles.color = new Color(35f,35f,35f,0.4f);
                            Handles.DrawLine(beat_4_top, beat_4_bottom);
                            Handles.color = beatColor;
                            beat_count_4++;
                        }
                        if(beat_count_8 < 8){
                            beat_8_top.x = (space * scale) / 8 + beat_8_top.x;
                            beat_8_bottom.x = beat_8_top.x;
                            beatColor = Handles.color;
                            Handles.color = new Color(35f,35f,35f,0.3f);
                            Handles.DrawLine(beat_8_top, beat_8_bottom);
                            Handles.color = beatColor;
                            beat_count_8++;
                        }
                        if(beat_count_12 < 12){
                            beat_12_top.x = (space * scale) / 8 + beat_12_top.x;
                            beat_12_bottom.x = beat_12_top.x;
                            beatColor = Handles.color;
                            Handles.color = new Color(35f,35f,35f,0.2f);
                            Handles.DrawLine(beat_12_top, beat_12_bottom);
                            Handles.color = beatColor;
                            beat_count_8++;
                        }
                        before_xpos_per = xpos;
                    }
                }
                xpos = xpos + space * scale;
            }
            Handles.DrawLine(startPos, endPos);
            Handles.color = prev;
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(60f);
            for (int i = 0; i < linenum; i++){
                // EditorGUI.DrawRect(new Rect(0f, basepos+perbasepos*i, 1*scale, 10), Color.green);
                EditorGUILayout.BeginHorizontal( GUI.skin.box );
                GUILayout.Box("", GUILayout.Height(heightspace),GUILayout.Width(((float)lineCount*space)*scale));
                EditorGUILayout.EndHorizontal();
                for(int a=0; a < CSV.GetRowNum(); a++){
                    if(CSVData[a][0] == linename[i]){
                        // Debug.Log("i:"+i+CSVData[a][0]);
                        if(CSVData[a][1] == "Tap"){
                            var time = float.Parse(CSVData[a][2]);
                            EditorGUI.DrawRect(new Rect(time*space*scale, basepos+perbasepos*i, 1*scale, 10), Color.green);
                        }else{
                            var starttime = float.Parse(CSVData[a][2]);
                            var endtime = float.Parse(CSVData[a][3]);
                            if(starttime == endtime){
                                EditorGUI.DrawRect(new Rect(starttime*space*scale, basepos+perbasepos*i, 1*scale, 10), Color.green);
                            }else{
                                EditorGUI.DrawRect(new Rect(starttime*space*scale, basepos+perbasepos*i, endtime*space*scale-starttime*space*scale, 10), Color.green);
                            }
                        }   
                    }
                }
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
        var per_bpm = (float)bpm/(float)240;

        if(isPlay){
            Debug.Log(per_bpm);
            Debug.Log(perspace*per_bpm);
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

    public static void UpdateCSV(){
        AssetDatabase.Refresh();
        CSVData = new List<string[]>();
        var filename = AddBlock.GetFileName();
        CSV.GetCSV(filename, CSVData);
        linename = AddBlock.GetLineName();
        Debug.Log(CSV.GetRowNum());
        Debug.Log(CSVData[CSV.GetRowNum()-1][0]);
    }

}

public class AddBlock: EditorWindow{
    private int _LineIndex;
    private int _TypeIndex;
    private float startime = 0.0f;
    private float endtime = 0.0f;
    private static bool canSet = true;
    private string filepath;
    private static string filename = "MusicData";
    private static List<string[]> CSVData = new List<string[]>();

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
            EditMusicScore.UpdateCSV();
            // Debug.Log(CSVData[1][0]==null);
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

    public static string GetFileName(){
        return filename;
    }

    public static string[] GetLineName(){
        return Line;
    }
}


