using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using AT.ExcelData;
using Newtonsoft.Json;

public class GameDataConvertEditor : EditorWindow
{
    #region Editor Draw Main
    private static GameDataConvertEditor window;
    private GUIStyle box;
    private GUIStyle headerStyle;
    private GUIStyle boldStyle;
    protected bool isGUISkinInitialized = false;

    [MenuItem("PROJECT_AT/System/Game Data Convert Editor")]
    public static void InitEditor()
    {
        window = GetWindow(typeof(GameDataConvertEditor)) as GameDataConvertEditor;
        window.titleContent = new GUIContent("Game Data Convert");
        window.minSize = new Vector2(450, 700);
        window.Show();
    }

    public static GUIStyle MakeHeaderStyleLabel()
    {
        GUIStyle headerStyle = new GUIStyle(GUI.skin.label);
        headerStyle.fontSize = 12;
        headerStyle.fontStyle = FontStyle.Bold;

        return headerStyle;
    }

    public static GUIStyle MakeBoldStyleLabel()
    {
        GUIStyle Style = new GUIStyle(GUI.skin.label);
        Style.fontSize = 11;
        Style.fontStyle = FontStyle.Bold;

        return Style;
    }

    protected void InitGUIStyle()
    {
        box = new GUIStyle("box");
        box.normal.background = Resources.Load(EditorGUIUtility.isProSkin ? "brown" : "lightSkinBox", typeof(Texture2D)) as Texture2D;
        box.border = new RectOffset(4, 4, 4, 4);
        box.margin = new RectOffset(3, 3, 3, 3);
        box.padding = new RectOffset(4, 4, 4, 4);
    }

    private void OnGUI()
    {
        if (isGUISkinInitialized == false)
        {
            InitGUIStyle();
            headerStyle = MakeHeaderStyleLabel();
            boldStyle = MakeBoldStyleLabel();
            isGUISkinInitialized = true;
        }
        
        DrawEditor();
    }
    #endregion

    private const string EditorPath = "Assets/GameDataOriginal/";
    private const string EditorResPath = "Assets/DownloadBundleAsset/GameData/";
    private const string EditorScenePath = "/UnityStudy/Editor/GameDataConvertTool/GameDataEditor.unity";
    private const string EditorSceneName = "GameDataEditor";

    private ExcelQuery excel;
    private Vector2 viewScrollPos;
    private Vector2 sheetListScrollPos;
    private Vector2 attributeScrollPos;

    // Pold Flags
    private bool isExcelFileFound               = true;
    private bool isExcelInfomationExpanded      = true;
    private bool isUnknownListExpanded          = true;
    private bool isSerializeFailedListExpanded  = true;
    private bool isGameDataAttributeExpanded    = true;
    private bool isExportExpanded               = true;

    // Empty Space Check Flag
    private bool isEmptySpaceIncludeForExport = true;
    private string exportArrayKey = "data";

    private string targetExcelFilePath = string.Empty;
    private string targetExcelFileName = string.Empty;

    private int sheetAttributeRowIndex = 5; // Category Row 6
    private int versionCode = 1;
    private GameDataList gameDataType = GameDataList.None;

    private List<ExcelSheetSetting> sheetList = new List<ExcelSheetSetting>();
    private List<DataAttributeSetting> gameDataList = new List<DataAttributeSetting>();
    private List<string> unknownDataList = new List<string>();
    private List<string> serializeFailedDataList = new List<string>();
    private List<string> exportDataList = new List<string>();


    public class ExcelSheetSetting
    {
        public ExcelQuery sheet;
        public string sheetName;
        public bool sheetChecked;
    }
    
    public class DataAttributeSetting
    {
        public bool isUnknownType = false;
        public bool isExpanded;
        public long versionCode;
        public GameDataList gameDataType;
        public string dataName;
    }


    private void OnReset()
    {
        excel = null;

        targetExcelFilePath = string.Empty;
        targetExcelFileName = string.Empty;

        isExcelFileFound = true;
        isExcelInfomationExpanded = true;
        isUnknownListExpanded = true;
        isSerializeFailedListExpanded = true;
        isGameDataAttributeExpanded = true;

        sheetAttributeRowIndex = 5;
        versionCode = 1;
        gameDataType = GameDataList.None;

        sheetList.Clear();
        gameDataList.Clear();
        unknownDataList.Clear();
        serializeFailedDataList.Clear();
        exportDataList.Clear();
    }

    private void DrawEditor()
    {
        if (window)
        {
            viewScrollPos = EditorGUILayout.BeginScrollView(viewScrollPos, GUILayout.Width(window.position.width), GUILayout.Height(window.position.height));

            EditorGUILayout.Separator();
            GUILayout.BeginVertical();
            Sprite logo = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Editor/AT/TermConvertTool/Resource/logo.png");
            using (new EditorGUILayout.VerticalScope(GUI.skin.box, GUILayout.Height(36f), GUILayout.Width(window.position.size.x)))
            {
                GUILayout.FlexibleSpace();
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (logo)
                    {
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.LabelField(new GUIContent(logo.texture),
                            new GUIStyle() { padding = new RectOffset(0, 0, 0, 0) },
                            GUILayout.Height(28f), GUILayout.Width(logo.texture.width));
                        GUILayout.FlexibleSpace();
                    }
                }
                GUILayout.FlexibleSpace();
            }
            GUILayout.Label("Game Data Convert Editor", headerStyle);
            GUILayout.EndVertical();
            EditorGUILayout.Separator();

            GUILayout.BeginVertical();

            DrawExcelSelect();

            if (isExcelFileFound)
            {
                if (sheetList.Count > 0)
                {
                    DrawSheetInfo();
                    DrawAttributeSetting();
                    DrawExportSetting();
                }
                else
                {
                    EditorGUILayout.HelpBox("Please Press [Read File] Button", MessageType.Warning);
                }
            }

            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
    }

    private void DrawExcelSelect()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Target Excel File", GUILayout.Width(100));
        targetExcelFilePath = GUILayout.TextField(targetExcelFilePath, GUILayout.MaxWidth(300));
        if (GUILayout.Button("..", EditorStyles.miniButtonLeft))
        {
            targetExcelFilePath = Application.dataPath;
            string folder = System.IO.Path.GetDirectoryName(EditorPath);
            targetExcelFilePath = EditorUtility.OpenFilePanel("Open Excel File", folder, "excel files;*.xls;*.xlsx");
        }
        if (GUILayout.Button("Open Default Path Folder", EditorStyles.miniButtonRight))
        {
            string folderPath = System.IO.Path.GetDirectoryName("Assets/GameDataExcel/");
            targetExcelFilePath = EditorUtility.OpenFilePanel("Open Excel File", folderPath, "excel files;*.xls;*.xlsx");
        }
        GUILayout.EndHorizontal();

        if (!string.IsNullOrEmpty(targetExcelFilePath))
        {
            string[] splitTags = targetExcelFilePath.Split('/');
            targetExcelFileName = splitTags[splitTags.Length - 1];
        }
        else
            targetExcelFileName = string.Empty;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Target File Name", GUILayout.Width(100));
        GUIStyle targetFileStyle = new GUIStyle(GUI.skin.label);
        targetFileStyle.fontStyle = FontStyle.Bold;
        EditorGUILayout.LabelField(targetExcelFileName, targetFileStyle);
        GUILayout.EndHorizontal();
        EditorGUILayout.HelpBox("Need Full Path in Project\nSample : Assets/GameDataOriginal/Sample.xlsx", MessageType.Info);
        EditorGUILayout.Separator();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Read File", EditorStyles.miniButtonLeft))
        {
            if (string.IsNullOrEmpty(targetExcelFilePath))
            {
                targetExcelFileName = "Please Select Target Excel File";
            }
            else
            {
                excel = new ExcelQuery(targetExcelFilePath);
                if (excel == null)
                {
                    string error = "Could not Serach Excel File";
                    EditorUtility.DisplayDialog("Error", error, "OK");
                    isExcelFileFound = false;
                }
                else
                {
                    isExcelFileFound = true;
                    sheetList.Clear();

                    float progress = 0.0f;
                    string progressMsg = string.Empty;
                    int current = 0;
                    string[] sheetNames = excel.GetSheetNames();
                    EditorUtility.DisplayProgressBar("Excel File Read", progressMsg, progress);
                    foreach (string sheetName in sheetNames)
                    {
                        progressMsg = string.Format("Reading [{0}] Sheet", sheetName);
                        progress = ((float)current / sheetNames.Length);
                        EditorUtility.DisplayProgressBar("Excel File Read", progressMsg, progress);

                        if (sheetName.Length > 0 && sheetName[0] == '#')
                            continue;

                        ExcelSheetSetting newSheet = new ExcelSheetSetting();
                        newSheet.sheet = new ExcelQuery(targetExcelFilePath, sheetName);
                        newSheet.sheetName = sheetName;
                        newSheet.sheetChecked = false;
                        sheetList.Add(newSheet);

                        current++;
                    }

                    EditorUtility.ClearProgressBar();
                }
            }
        }

        if (GUILayout.Button("Open Target File Folder", EditorStyles.miniButtonMid))
        {
            if (!string.IsNullOrEmpty(targetExcelFilePath))
                EditorUtility.RevealInFinder(targetExcelFilePath);
        }

        if (GUILayout.Button("Open Asset GameData Folder", EditorStyles.miniButtonMid))
        {
            string folder = System.IO.Path.GetDirectoryName(EditorResPath);
            EditorUtility.OpenFilePanel("Open Excel File", folder, "excel files;*.xls;*.xlsx;*.json;");
        }

        if (GUILayout.Button("Reset", EditorStyles.miniButtonRight))
        {
            OnReset();
        }

        GUILayout.EndHorizontal();
    }

    private void DrawSheetInfo()
    {
        EditorGUILayout.Separator();
        GUILayout.Label("Sheet Information Setting", headerStyle);
        isExcelInfomationExpanded = EditorGUILayout.Foldout(isExcelInfomationExpanded, "Excel File Infomation", true);
        if (isExcelInfomationExpanded)
        {
            GUILayout.BeginVertical();
            sheetListScrollPos = EditorGUILayout.BeginScrollView(sheetListScrollPos, GUILayout.Width(window.position.width - 10), GUILayout.MinHeight(120));
            foreach (var sheet in sheetList)
            {
                string itemMsg = string.Format("Sheet Name : {0}", sheet.sheetName);
                GUILayout.BeginHorizontal();
                GUILayout.Label(itemMsg, GUILayout.Width(230));
                sheet.sheetChecked = EditorGUILayout.Toggle(sheet.sheetChecked);
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            bool isCheckedItemIn = false;
            foreach(var sheet in sheetList)
            {
                if (!sheet.sheetChecked)
                    continue;

                isCheckedItemIn = true;
                break;
            }

            if(! isCheckedItemIn)
                EditorGUILayout.HelpBox("Please Check Target Sheets", MessageType.Info);

            GUILayout.BeginHorizontal();
            EditorGUILayout.Separator();
            if (GUILayout.Button("Select All Sheet", EditorStyles.miniButtonLeft))
            {
                foreach (var sheet in sheetList)
                    sheet.sheetChecked = true;
            }
            if (GUILayout.Button("Reset", EditorStyles.miniButtonRight))
            {
                foreach (var sheet in sheetList)
                    sheet.sheetChecked = false;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();            
        }
    }

    private void DrawAttributeSetting()
    {
        EditorGUILayout.Separator();
        GUILayout.Label("Game Data Attribute Setting", headerStyle);
        isGameDataAttributeExpanded = EditorGUILayout.Foldout(isGameDataAttributeExpanded, "Excel File Infomation", true);
        if (isGameDataAttributeExpanded)
        {
            GUILayout.BeginVertical();
            sheetAttributeRowIndex = EditorGUILayout.IntField("Sheet Column Title Index", sheetAttributeRowIndex);
            attributeScrollPos = EditorGUILayout.BeginScrollView(attributeScrollPos, GUILayout.Width(window.position.width - 10), GUILayout.MinHeight(180));

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Check Game Data", EditorStyles.miniButtonLeft))
            {
                if(EditorSceneManager.GetActiveScene().name.Equals(EditorSceneName))
                {
                    gameDataList.Clear();
                    unknownDataList.Clear();
                    serializeFailedDataList.Clear();
                    exportDataList.Clear();

                    foreach (var sheet in sheetList)
                    {
                        if (sheet.sheetChecked == false)
                            continue;

                        DataAttributeSetting dataSetting = new DataAttributeSetting();
                        dataSetting.isExpanded = false;
                        dataSetting.gameDataType = GetDataTypeFromSheetName(sheet.sheetName);
                        dataSetting.versionCode = GetVersionCodeFromJsonFile(sheet.sheetName);
                        dataSetting.isUnknownType = GetFieldType(sheet.sheetName) == null ? true : false;
                        dataSetting.dataName = sheet.sheetName;

                        if (!dataSetting.isUnknownType && dataSetting.gameDataType != GameDataList.None)
                            exportDataList.Add(sheet.sheetName);

                        gameDataList.Add(dataSetting);
                    }
                }
            }

            if(GUILayout.Button("Load GameData Model", EditorStyles.miniButtonRight))
            {
                var editorScene = EditorSceneManager.OpenScene(Application.dataPath + EditorScenePath, OpenSceneMode.Additive);
                EditorSceneManager.SetActiveScene(editorScene);
            }
            GUILayout.EndHorizontal();

            if(EditorSceneManager.GetActiveScene().name.Equals(EditorSceneName) == false)
                EditorGUILayout.HelpBox("Please Click |Load GameData Model|", MessageType.Warning);

            EditorGUILayout.Separator();

            ///Unknown Type List Warning
            if (unknownDataList.Count > 0)
            {
                GUI.color = Color.red;
                isUnknownListExpanded = EditorGUILayout.Foldout(isUnknownListExpanded, "Unknown Data Type List", true);
                if(isUnknownListExpanded)
                {
                    GUILayout.BeginVertical();

                    for(int i = 0; i < unknownDataList.Count; i++)
                        GUILayout.Label(string.Format("{0}. Unknown Type : {1}", i + 1, unknownDataList[i]));

                    GUILayout.EndVertical();
                }
                GUI.color = Color.white;
            }

            ///Serialize Failed Type List Warning
            if(serializeFailedDataList.Count > 0)
            {
                GUI.color = Color.red;
                isSerializeFailedListExpanded = EditorGUILayout.Foldout(isSerializeFailedListExpanded, "Serialize Failed Type List", true);
                if(isSerializeFailedListExpanded)
                {
                    GUILayout.BeginVertical();

                    for (int i = 0; i < serializeFailedDataList.Count; i++)
                        GUILayout.Label(string.Format("{0}. Serialize Failed Type : {1}", i + 1, serializeFailedDataList[i]));

                    GUILayout.EndVertical();
                }
                GUI.color = Color.white;
            }
            
            int serializableCount = 0;
            string attributeName = string.Empty;
            foreach(var gameData in gameDataList)
            {
                if (gameData.gameDataType == GameDataList.None)
                    continue;

                if (gameData.isUnknownType)
                    continue;

                serializableCount++;
                attributeName = gameData.gameDataType.ToString();
                gameData.isExpanded = EditorGUILayout.Foldout(gameData.isExpanded, attributeName, true);
                if(gameData.isExpanded)
                {
                    GUILayout.BeginHorizontal();
                    gameData.versionCode = EditorGUILayout.IntField("Version", (int)gameData.versionCode);
                    gameData.versionCode = gameData.versionCode < 0 ? 0 : gameData.versionCode;
                    if (GUILayout.Button("+", GUILayout.Width(30)))
                    {
                        gameData.versionCode++;
                    }
                    if (GUILayout.Button("-", GUILayout.Width(30)))
                    {
                        gameData.versionCode = gameData.versionCode > 0 ? gameData.versionCode - 1 : 0;
                    }
                    GUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndScrollView();

            if(serializableCount <= 0)
                EditorGUILayout.HelpBox("Possible Serialize Data are Empty", MessageType.Warning);

            GUILayout.EndVertical();
        }
    }

    private void DrawExportSetting()
    {
        EditorGUILayout.Separator();
        GUILayout.Label("Game Data Attribute Setting", headerStyle);
        isExportExpanded = EditorGUILayout.Foldout(isExportExpanded, "Export Setting", true);
        if (isExportExpanded)
        {
            GUILayout.BeginVertical();
            isEmptySpaceIncludeForExport = GUILayout.Toggle(isEmptySpaceIncludeForExport, "Is Consider Empty Cell");
            exportArrayKey = EditorGUILayout.TextField("Data Array Field Name", exportArrayKey);

            if (exportDataList.Count > 0)
            {
                // Export To *.Json
                if (GUILayout.Button("Export To *.Json", GUILayout.MinHeight(25)))
                {
                    int convertIndex = 0;
                    float progress = 0;

                    string progressMsg = string.Empty;
                    string error = string.Empty;

                    // Convert Time (Now Time:Tick)
                    long convertTime = System.DateTime.Now.Ticks;

                    // Export Result String
                    string exportResult = string.Empty;
                    string resultPath = string.Empty;
                    System.Text.StringBuilder sb;
                    foreach (var target in exportDataList)
                    {
                        sb = new System.Text.StringBuilder();
                        exportResult = string.Empty;
                        progress = (float)convertIndex / exportDataList.Count;
                        progressMsg = string.Format("Converting : {0} ", target);
                        EditorUtility.DisplayProgressBar("Convert To Json", progressMsg, progress);

                        error = string.Empty;
                        try
                        {
                            ExcelQuery sheet = sheetList.Find(x => x.sheetName == target).sheet;
                            // Total Title Column Count Without "# Comment Column" 
                            string[] titles = sheet.GetTitle(sheetAttributeRowIndex, ref error);

                            // Included "$" Tag Column Check
                            bool[] stringFormColumns = sheet.GetStringFormColumns(sheetAttributeRowIndex, titles.Length);

                            string[] rowData = null;

                            string rowJsonForm = string.Empty;
                            List<string> idList = sheet.GetColumns(sheetAttributeRowIndex + 1, 0, true);

                            // Array Name
                            sb.Append("{\"" + exportArrayKey + "\": [");

                            // Data Array
                            int index = 0;
                            foreach(string id in idList)
                            {
                                if(false == string.IsNullOrEmpty(id))
                                {
                                    rowData = sheet.GetRow(sheetAttributeRowIndex + (index + 1), titles.Length, isEmptySpaceIncludeForExport);
                                    rowJsonForm = MakeJsonFormat(titles, rowData, stringFormColumns);
                                    sb.Append($"{rowJsonForm}{(index < idList.Count - 1 ? "," : "")}");
                                }
                                index++;
                            }
                            
                            // Version, UpTime
                            long version = gameDataList.Find(x => x.dataName == target).versionCode;
                            long uptime = convertTime;

                            sb.Append("],");
                            sb.AppendFormat("\"{0}\": {1},", "version", version);
                            sb.AppendFormat("\"{0}\": {1}", "uptime", uptime);
                            sb.Append("}");

                            exportResult = sb.ToString();
                            resultPath = EditorResPath + target + ".json";
                            FileManager.WriteFileFromString(resultPath, exportResult);
                        }
                        catch(System.Exception e)
                        {
                            error = e.Message;
                            Debug.LogError(error);
                        }
                        convertIndex++;
                    }
                    EditorUtility.ClearProgressBar();
                }

                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                builder.Append("Export Result :\n");
                foreach(var data in exportDataList)
                {
                    builder.AppendFormat("{0}.json |", data);
                }
                EditorGUILayout.HelpBox(builder.ToString(), MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("Convert Data List is Empty", MessageType.Warning);
            }
            GUILayout.EndVertical();
        }
    }

    public string MakeJsonFormat(string[] key, string[] value, bool[] isStringFormColumns)
    {
        string result = string.Empty;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        sb.Append("{");
        sb.Append("\n");
        for (int i = 0; i < key.Length; i++)
        {
            // Key
            sb.AppendFormat("\t\"{0}\"", key[i]);

            // Value
            if (isStringFormColumns[i])
                sb.AppendFormat(": \"{0}\"", i < value.Length ? value[i] : string.Empty);
            else
                sb.AppendFormat(": {0}", i < value.Length ? value[i] : string.Empty);

            // Rest
            sb.Append(i < key.Length - 1 ? "," : string.Empty);
            sb.Append("\n");
        }
        sb.Append("}");

        result = sb.ToString();
        return result;
    }

    public int GetVersionCodeFromJsonFile(string sheetName)
    {
        int result = 0;
        //System.Text.StringBuilder builder = new System.Text.StringBuilder();
        //builder.Append(EditorResPath);
        //builder.AppendFormat("{0}.json", sheetName);
        //string readData = string.Empty;
        //if(AT.FileManager.FileManager.ReadFileData(builder.ToString(), out readData))
        //{
        //}

        return result;
    }

    public GameDataList GetDataTypeFromSheetName(string sheetName)
    {
        GameDataList resultType = GameDataList.None;
        int lastIndex = (int)GameDataList.End;
        for (int index = 0; index < lastIndex; index++)
        {
            string typeString = ((GameDataList)index).ToString();
            if(typeString.Equals(sheetName))
            {
                resultType = (GameDataList)index;
                break;
            }
        }

        if (resultType == GameDataList.None)
            unknownDataList.Add(sheetName);

        if(GetFieldType(sheetName) == null)
            serializeFailedDataList.Add(sheetName);

        return resultType;
    }

    public System.Type GetFieldType(string fieldName)
    {
        if (GameDataModel.Instance == null) return null;

        System.Reflection.FieldInfo fieldInfo = GameDataModel.Instance.GetType().GetField(fieldName);
        if (fieldInfo != null)
            return fieldInfo.FieldType;

        return null;
    }

    public void SetFieldValue(string fieldName, object data)
    {
        if (GameDataModel.Instance == null) return;

        var type = GameDataModel.Instance.GetType();
        var field = type.GetField(fieldName);
        field.SetValue(GameDataModel.Instance, data);
    }
}
