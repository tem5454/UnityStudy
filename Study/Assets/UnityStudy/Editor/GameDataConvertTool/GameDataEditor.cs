//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//namespace AT.Editor
//{
//    public partial class GameDataEditor : EditorWindow
//    {

//        public enum GameDataType
//        {
//            UNKNOWN_TYPE = -1,
//            GameData_Survivors = 0,
//            GameData_Weapons,
//            TYPE_LAST,
//        }

//        private static GameDataEditor window;
//        private const string GameDataDesignFolderPath = "Assets/GameData Design/";
//        private const string GameDataStaticFolderName = "GameData Design";

//        public GameDataType editorGameDataType = GameDataType.GameData_Survivors;
//        public TextAsset gameDataAsset;
//        public ScriptableObject scriptDataAsset;
//        public string loadedGameData;
//        public string survivorGroupFileName;

//        [MenuItem("PROJECT_AT/GameData Window/GameData Editor")]
//        private static void InitEditor()
//        {
//            window = (GameDataEditor)GetWindow(typeof(GameDataEditor));
//            window.titleContent = new GUIContent("GameData Editor");
//            window.Show();
//        }

//        private void OnGUI()
//        {
//            GUILayout.Label("PROJECT_AT Game Data Editor", EditorStyles.boldLabel);
//            GUILayout.Label(string.Format("GameData Path(Input): {0}", GameDataDesignFolderPath));
//            GUILayout.Label(string.Format("GameData Path(Output) : {0}", "Assets/Resources/"));

//            editorGameDataType = (GameDataType)EditorGUILayout.EnumPopup(editorGameDataType);

//            EditorGUILayout.Space();
//            GUILayout.Label("Help : Drag & Drop DataFile (*.Json) To Below GameData File Object");
//            EditorGUILayout.Space();

//            gameDataAsset = (TextAsset)EditorGUILayout.ObjectField("GameData File", gameDataAsset, typeof(TextAsset), true);
//            OnCheckDataType();

//            if (editorGameDataType == GameDataType.UNKNOWN_TYPE)
//            {
//                GUILayout.Space(3);
//                GUILayout.Label("UNKNOWN Data Type is not Supported");
//            }
//            else
//            {
//                OnDrawEditorWindowByDataType();
//            }

//            scriptDataAsset = (ScriptableObject)EditorGUILayout.ObjectField("Scriptable File", scriptDataAsset, typeof(ScriptableObject), true);
//            if (scriptDataAsset)
//            {
//                OnDrawScriptibleObjectDataEditor();
//            }
//        }

//        private void OnInspectorUpdate()
//        {
//            Repaint();
//        }

//        private void OnCheckDataType()
//        {
//            if (gameDataAsset != null && GUI.changed)
//            {
//                for (int index = 0; index < (int)GameDataType.TYPE_LAST; index++)
//                {
//                    string dataCheckName = ((GameDataType)(index)).ToString();
//                    if (gameDataAsset.name.Equals(dataCheckName))
//                    {
//                        editorGameDataType = (GameDataType)(index);
//                        break;
//                    }
//                    else
//                        editorGameDataType = GameDataType.UNKNOWN_TYPE;
//                }

//                loadedGameData = gameDataAsset.text;
//            }

//            if (gameDataAsset == null)
//                editorGameDataType = GameDataType.UNKNOWN_TYPE;
//        }

//        private void OnDrawEditorWindowByDataType()
//        {
//            switch (editorGameDataType)
//            {
//                case GameDataType.GameData_Survivors: OnDrawSurvivorDataEditor(); break;
//                case GameDataType.GameData_Weapons: OnDrawWeaponDataEditor(); break;
//            }
//        }


//        private int assetJobCount = 0;
//        private string progressMsg = "";
//        private float progressValue = 0.0f;
//        private long versionCode = 0;
//        private void OnDrawSurvivorDataEditor()
//        {
//            survivorGroupFileName = EditorGUILayout.TextField("Group Data File Name", survivorGroupFileName);
//            versionCode = EditorGUILayout.LongField("Version Code", versionCode);

//            if (GUILayout.Button(string.Format("Make Survivor Assets From {0}.json File", GameDataType.GameData_Survivors.ToString())))
//            {
//                TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(GameDataDesignFolderPath + GameDataType.GameData_Survivors.ToString() + ".json");
//                string loadData = textAsset.text;
//                SurvivorObjectBase[] convertData = Newtonsoft.Json.JsonConvert.DeserializeObject<SurvivorObjectBase[]>(loadData);

//                long publishedTime = AT.Common_Utility.Common_Utility.GetNowTimeToLongTicks();

//                assetJobCount = convertData.Length;
//                SurvivorDataBase newGroupData = CreateInstance<SurvivorDataBase>();
//                newGroupData.PublishedTime = publishedTime;
//                newGroupData.VersionCode = versionCode;
//                newGroupData.survivorDataList = new List<SurvivorObjectBase>();

//                for (int index = 0; index < convertData.Length; index++)
//                {
//                    EditorUtility.DisplayProgressBar("Creating Survivor Data Asset", progressMsg, progressValue);
//                    SurvivorObjectBase newSurvivorData = CreateInstance<SurvivorObjectBase>();
//                    newSurvivorData.PublishedTime = publishedTime;
//                    newSurvivorData.VersionCode = versionCode;

//                    newSurvivorData.defaultName = convertData[index].defaultName;
//                    newSurvivorData.model_ID = convertData[index].model_ID;
//                    newSurvivorData.id = convertData[index].id;
//                    newSurvivorData.type = convertData[index].type;
//                    newSurvivorData.weaponMain_Id = convertData[index].weaponMain_Id;
//                    newSurvivorData.weaponSub_Id = convertData[index].weaponSub_Id;
//                    newSurvivorData.weaponPistrol_Id = convertData[index].weaponPistrol_Id;
//                    newSurvivorData.skill_1 = convertData[index].skill_1;
//                    newSurvivorData.skill_2 = convertData[index].skill_2;

//                    AssetDatabase.CreateAsset(newSurvivorData, "Assets/Resources/" + string.Format("{0:00}_", index + 1) + newSurvivorData.defaultName + ".asset");
//                    progressMsg = string.Format("{0}.asset Created", index);
//                    progressValue = (float)(index / convertData.Length);
//                    assetJobCount--;

//                    newGroupData.survivorDataList.Add(newSurvivorData);
//                }

//                AssetDatabase.CreateAsset(newGroupData, "Assets/Resources/" + string.Format("00_{0}", survivorGroupFileName) + ".asset");

//                EditorUtility.ClearProgressBar();
//                AssetDatabase.Refresh();
//            }
//        }

//        private void OnDrawWeaponDataEditor()
//        {

//        }

//        private Vector2 scrollPos;
//        private bool isDrawSurvivorDataList = false;
//        private void OnDrawScriptibleObjectDataEditor()
//        {
//            SurvivorDataBase survivorData = scriptDataAsset as SurvivorDataBase;
//            if (survivorData)
//            {
//                EditorGUILayout.BeginHorizontal();
//                if (GUILayout.Button("Survivor Data List Show / Hide", GUILayout.Width(200)))
//                    isDrawSurvivorDataList = !isDrawSurvivorDataList;

//                if (GUILayout.Button("Link Reset"))
//                    scriptDataAsset = null;
//                EditorGUILayout.EndHorizontal();

//                if (isDrawSurvivorDataList)
//                {
//                    scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
//                    EditorGUILayout.Space();
//                    Rect lastChildRect = GUILayoutUtility.GetLastRect();
//                    for (int index = 0; index < survivorData.survivorDataList.Count; index++)
//                    {
//                        Handles.BeginGUI();
//                        Handles.color = Color.red;
//                        Handles.DrawLine(new Vector3(lastChildRect.xMin, lastChildRect.yMax), new Vector3(lastChildRect.xMax, lastChildRect.yMax));

//                        survivorData.survivorDataList[index].RegistTime = EditorGUILayout.LongField("Regist Time", survivorData.survivorDataList[index].RegistTime);
//                        survivorData.survivorDataList[index].PublishedTime = EditorGUILayout.LongField("Published Time", survivorData.survivorDataList[index].PublishedTime);
//                        survivorData.survivorDataList[index].VersionCode = EditorGUILayout.LongField("Published Time", survivorData.survivorDataList[index].VersionCode);
//                        EditorGUILayout.Space();

//                        survivorData.survivorDataList[index].defaultName = EditorGUILayout.TextField("Default Name", survivorData.survivorDataList[index].defaultName);
//                        survivorData.survivorDataList[index].id = EditorGUILayout.LongField("ID", survivorData.survivorDataList[index].id);
//                        survivorData.survivorDataList[index].model_ID = EditorGUILayout.TextField("Model ID", survivorData.survivorDataList[index].model_ID);
//                        EditorGUILayout.Space();

//                        lastChildRect = GUILayoutUtility.GetLastRect();

//                        Handles.EndGUI();
//                    }
//                    EditorGUILayout.EndScrollView();
//                }
//            }
//            else
//                GUILayout.Label(string.Format("This File can not be converted to SurvivorDataBase"));
//        }

//    }
//}
