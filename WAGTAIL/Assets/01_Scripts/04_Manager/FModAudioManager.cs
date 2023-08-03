using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using UnityEngine.UIElements;
using System;
using System.Reflection;
using UnityEngine.Networking.Types;
using System.Net.NetworkInformation;
using FMOD;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.PackageManager;
using static UnityEditor.ObjectChangeEventStream;
#endif

public interface IFModAudioFadeComplete { void FModFadeComplete(int fadeID, float goalVolume); }
public delegate void FModEventFadeCompleteNotify( int fadeID, float goalVolume );

public sealed class FModAudioManager : MonoBehaviour
{
    #region Editor_Extension
    /********************************
     * 에디터 확장을 위한 private class
     ***/
#if UNITY_EDITOR
    private sealed class FMODSoundManagerWindow : EditorWindow
    {
        //==================================
        ////          Fields           ///// 
        //==================================
        private const string _ResourcePath          = "Assets/Plugins/FMOD/src/FMODAudioManagerDefine.cs";
        private const string _EditorDataPath        = "Assets/Plugins/FMOD/Resources/FModAudioManagerSettings.asset";
        private const string _EditorSettingsPath    = "Assets/Plugins/FMOD/Resources/FMODStudioSettings.asset";
        private const string _ScriptDefine          = "FMOD_Event_ENUM";
        private const string _ScriptVersion         = "v1.230730";
        private const string _EventRootPath         = "event:/";
        private string       _CategoryCountColor    = "";

        private Vector2 _ScrollPos = Vector2.zero;

        /*********************************
         * Editor Settings Datas
         ***/
        private static FMODAudioEventData _EventData;
        private static FMODUnity.Settings _SettingData;


        /***********************************
         * Categorys
         ***/
        private int _EventCategorySelect = 0;
        private static readonly string[] _EventCategories = new string[] { "BGM", "SFX", "NoGroup" };
        private static readonly string[] _DescPropertyStr = new string[] { "BGMEvents", "SFXEvents", "NoneEvents" };


        /**************************
         * Textures
         ***/
        private static Texture _Banner;
        private static Texture _BannerBlack;
        private static Texture _SearchIcon;
        private static Texture _AddIcon;
        private static Texture _RemoveIcon;
        private static Texture _xIcon;
        private static Texture _WarningIcon;
        private static Texture _studioIcon;

        /**********************************
         * Show and Warning bools
         ***/
        private bool _addCategory = false;
        private string _addName = string.Empty;
        private bool _addWarning = false;
        private bool _showRefreshHelp = false;

        private int _checkOverlapIndex = -1;
        private bool _eventIsOverlap = false;
        private string _overlapEventName = string.Empty;

        private bool _showStudioPath = false;
        private bool _showApplySettings = false;


        /************************************
         * GUIStyles
         ***/
        private static GUIStyle _notValidEventStyle;
        private static GUIStyle _BannerTxtStyle;
        private static GUIStyle _FoldTxtStyle;
        private static GUIStyle _toolbarStyle;


        //==================================
        ////        Core Methods        //// 
        //==================================
        [MenuItem("Managers/FMODAudio Settings")]
        public static void OpenWindow()
        {
            EditorWindow.GetWindow(typeof(FMODSoundManagerWindow), false, "FMODAudio Settings");
        }

        private void OnFocus()
        {
            _showRefreshHelp = false;

            //EventManager.RefreshBanks();

            //중복체크
            if (_checkOverlapIndex > -1 && _eventIsOverlap == false)
            {
                List<FMODUnity.EventReference> lists = GetEventList(_EventCategorySelect);

                bool EventRefIsNull = lists[_checkOverlapIndex].IsNull;
                string EventPath = EventRefIsNull ? "-" : lists[_checkOverlapIndex].Path;
                string[] PathSplit = EventPath.Split('/');
                _overlapEventName = PathSplit[PathSplit.Length - 1];

                if (_eventIsOverlap == false && CheckEventIsOverlap(_checkOverlapIndex, GetEventList(_EventCategorySelect))) {
                    _eventIsOverlap = true;
                    lists[_checkOverlapIndex] = new FMODUnity.EventReference();
                }
                else
                {
                    _checkOverlapIndex = -1;
                }
            }

        }

        private void OnEnable()
        {
            //에디터 세팅 정보를 가져온다.
            if(_EventData==null) _EventData = AssetDatabase.LoadAssetAtPath<FMODAudioEventData>(_EditorDataPath);
            if(_SettingData==null) _SettingData = AssetDatabase.LoadAssetAtPath<FMODUnity.Settings>(_EditorSettingsPath);

            //존재하지 않는다면 생성.
            if (_EventData == null) {
                _EventData = new FMODAudioEventData();
                _EventData.BGMBusName = "BGM";
                _EventData.SFXBusName = "SFX";
                _EventData.BGMGroupRootFolder = "BGM";
                _EventData.SFXGroupRootFolder = "SFX";
                AssetDatabase.CreateAsset(_EventData, _EditorDataPath);
            }
        }

        public void OnGUI()
        {
            Intialized();

            //최상단 라벨
            _ScrollPos = EditorGUILayout.BeginScrollView(_ScrollPos, true, true, GUILayout.Height(position.height));

            /*Scroll Start**********************/
            EditorGUI.indentLevel++;

            //최상단 FMOD Image 출력
            ShowFMODImage();
            EditorGUILayout.Space(10f);

            //Fmod Bus목록 표시
            ShowFMODBusList();
            EditorGUILayout.Space(30f);

            //Event목록 표시
            ShowEventList();
            EditorGUILayout.Space(30f);

            EditorGUI.indentLevel--;
            /*Scroll End***********************/

            EditorGUILayout.EndScrollView();
        }


        //==================================
        ////         GUI Methods       ///// 
        //==================================
        private void Intialized()
        {
            #region Ommission

            bool isBlackSkin = EditorGUIUtility.isProSkin;

            /**********************************
             * 이미지 로드
             ***/
            //바로가기 아이콘 로드
            if (_studioIcon == null) {

                _studioIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Plugins/FMOD/images/StudioIcon.png", typeof(Texture));
            }

            //돋보기 버튼 로드
            if (_SearchIcon == null){

                _SearchIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Plugins/FMOD/images/SearchIconBlack.png", typeof(Texture));
            }

            //배너 로드
            if (_Banner == null){

                _Banner = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Plugins/FMOD/images/FMODLogoWhite.png", typeof(Texture));
            }

            if (_BannerBlack == null)
            {
                _BannerBlack = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Plugins/FMOD/images/FMODLogoBlack.png", typeof(Texture));
            }

            //추가 버튼 로드
            if (_AddIcon == null){

                _AddIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Plugins/FMOD/images/AddIcon.png", typeof(Texture));
            }

            //삭제 버튼 로드
            if (_xIcon == null){

                _xIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Plugins/FMOD/images/CrossYellow.png", typeof(Texture));
            }

            //돋보기 버튼 로드
            if (_SearchIcon == null){

                _SearchIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Plugins/FMOD/images/SearchIconBlack.png", typeof(Texture));
            }

            //삭제 버튼 로드
            if (_RemoveIcon == null){

                _RemoveIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Plugins/FMOD/images/Delete.png", typeof(Texture));
            }

            //경고 아이콘 로드
            if (_RemoveIcon == null){

                _RemoveIcon = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Plugins/FMOD/images/NotFound.png", typeof(Texture));
            }

            /*******************************
             * 스타일 초기화
             ***/
            //툴바 스타일 생성
            if (_toolbarStyle == null) {

                _toolbarStyle = new GUIStyle(EditorStyles.selectionRect);
                _toolbarStyle.richText = true;
            }

            //접기 스타일 생성
            if (_FoldTxtStyle == null){

                _FoldTxtStyle = new GUIStyle(EditorStyles.foldout);
                //_FoldTxtStyle.normal.textColor = Color.white;
                _FoldTxtStyle.richText = true;
            }

            //배너 텍스트 스타일
            if (_BannerTxtStyle == null)
            {
                _BannerTxtStyle = new GUIStyle();
                _BannerTxtStyle.normal.textColor = (isBlackSkin?Color.white:Color.black);
                _BannerTxtStyle.fontStyle = FontStyle.Bold;
            }

            _CategoryCountColor = (isBlackSkin ? "#8DFF9E":"#1B882B");

            #endregion
        }

        private void ShowFMODImage()
        {
            #region Ommision
            /****************************************
             * 배너 표시.
             ***/
            //Banner이미지가 없다면 생성.
            if (_Banner != null) {

                Texture useBanner = (EditorGUIUtility.isProSkin==false ? _BannerBlack : _Banner);
                GUILayout.Box(useBanner, GUILayout.Width(position.width), GUILayout.Height(100f));
                GUILayout.BeginArea(new Rect(position.width * .3f, 100f - 20, 300, 30));
                GUILayout.Label("FMODAudio Settings Editor " + _ScriptVersion, _BannerTxtStyle);
                GUILayout.EndArea();
            }

            /*************************************
             * Studio 바로가기
             ***/
            if (GUI.Button(new Rect(position.width * .9f, 70f, 30f, 30f), _studioIcon))
            {
                System.Diagnostics.Process.Start(Application.dataPath.Replace("Assets", "") + _SettingData.SourceProjectPath);
            }

            GUILayout.BeginArea(new Rect(position.width * .9f - 30f, 75f, 30f, 30f));
            _showStudioPath = EditorGUILayout.Foldout(_showStudioPath, "");
            GUILayout.EndArea();

            //경로지정 표시
            if (_showStudioPath)
            {
                EditorGUILayout.LabelField("FMOD Studio project file Setting", _BannerTxtStyle, GUILayout.Width(position.width * .3f));
                GUILayout.Space(3f);
                EditorGUILayout.TextField("Studio Project Path: ", _SettingData.SourceProjectPath, GUILayout.Width(position.width*.88f));

                //돋보기 버튼을 눌렀을 경우
                if(GUI.Button(new Rect(position.width * .88f + 10f, 122f, 30f, 30f), _SearchIcon)) {

                    SerializedObject obj = new SerializedObject(_SettingData);
                    FMODUnity.SettingsEditor.BrowseForSourceProjectPath(obj);
                }


            }

            #endregion
        }

        private void ShowFMODBusList()
        {
            #region Omission
            if (_EventData == null) return;
            EditorGUILayout.LabelField("FMOD Bus Settings", _BannerTxtStyle);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Master Bus", "Master", GUILayout.Width(position.width * .9f));
            EditorGUI.EndDisabledGroup();

            _EventData.BGMBusName = EditorGUILayout.TextField("BGM Bus", _EventData.BGMBusName, GUILayout.Width(position.width * .9f));
            _EventData.SFXBusName = EditorGUILayout.TextField("SFX Bus", _EventData.SFXBusName, GUILayout.Width(position.width * .9f));
            #endregion
        }

        private void ShowEventList()
        {
            #region Omission
            if (_EventData == null) return;
            EditorGUILayout.LabelField("FMOD Event Settings", _BannerTxtStyle, GUILayout.Width(position.width * .3f));

            EditorGUILayout.Space(5f);
            GUILayout.BeginHorizontal();

            //Refrsh Settings
            if (GUILayout.Button("Update And Save Settings"))
            {
                CreateEnumScript();
                UnityEngine.Debug.LogWarning("FModBGMEventType & FModSFXEventType 열거형이 갱신됬습니다.");
                _showRefreshHelp = true;
                
                if(_EventData!=null)
                {
                    EditorUtility.SetDirty(_EventData);
                }

                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, _ScriptDefine);
                AssetDatabase.Refresh();
            }

            //Apply Studio Settings
            if (GUILayout.Button("Apply FMod Studio Event Settings"))
            {
                RefreshStudioChanges();
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(4f);

            EditorGUI.indentLevel--;
            _showApplySettings = EditorGUILayout.Foldout(_showApplySettings, "");
            EditorGUI.indentLevel++;

            //Studio Event Settings
            if (_showApplySettings)
            {
                EditorGUILayout.Space(10f);
                EditorGUILayout.LabelField("FMod Studio Event Apply Settings", _BannerTxtStyle);
                EditorGUILayout.Space(3f);
                _EventData.BGMGroupRootFolder = EditorGUILayout.TextField("BGMGroup Root Folder: ", _EventData.BGMGroupRootFolder, GUILayout.Width(position.width*.9f));
                _EventData.SFXGroupRootFolder = EditorGUILayout.TextField("SFXGroup Root Folder: ", _EventData.SFXGroupRootFolder, GUILayout.Width(position.width * .9f));
            }


            EditorGUILayout.Space(10f);

            if (_showRefreshHelp) EditorGUILayout.HelpBox("FModBGMEventType & FModSFXEventType 열거형이 갱신되었습니다!", MessageType.Info);

            //BGM or SFX 선택 툴바 표시
            _EventCategories[0] = $"BGM({_EventData.BGMEvents.Count})";
            _EventCategories[1] = $"SFX({_EventData.SFXEvents.Count})";
            _EventCategories[2] = $"NoGroup({_EventData.NoneEvents.Count})";

            int newSelect = GUILayout.Toolbar(_EventCategorySelect, _EventCategories, GUILayout.Width(position.width), GUILayout.Height(50f));
            if (_EventCategorySelect != newSelect)
            {
                _EventCategorySelect = newSelect;
                _showRefreshHelp = false;
            }

            List<FMODUnity.EventReference>  events  = GetEventList(_EventCategorySelect);
            List<FMODEventCategoryDesc>     descs   = GetCategoryList(_EventCategorySelect);

            GUILayout.BeginHorizontal();
            /*Horizontal Start***********************************/

            //Add Category
            if (GUILayout.Button("Add Category"))
            {
                _checkOverlapIndex = -1;
                _showRefreshHelp = false;
                _addCategory = true;
                _addWarning = false;
            }

            /*Horizontal End***********************************/
            GUILayout.EndHorizontal();

            //카테고리 추가창
            if (_addCategory)
            {
                _addName = EditorGUILayout.TextField("New Category Name: ", _addName);

                if (_addWarning) EditorGUILayout.HelpBox("카테고리는 중복될 수 없습니다!", MessageType.Warning);

                GUILayout.BeginHorizontal();
                //카테고리 추가 취소
                if (GUILayout.Button("Back", GUILayout.Width(position.width * .5f)))
                {
                    _showRefreshHelp = false;
                    _addCategory = false;
                }

                //카테고리 추가
                if (GUILayout.Button("Create", GUILayout.Width(position.width * .5f)))
                {
                    _showRefreshHelp = false;
                    if (CheckCategoryExist(_addName, descs) != -1)
                    {
                        _addWarning = true;
                    }
                    else
                    {
                        _addCategory = false;
                        _addWarning = false;
                        descs.Add(new FMODEventCategoryDesc { categoryName = _addName, show = false, Count = 0 });
                    }
                }
                GUILayout.EndHorizontal();
            }


            /*****************************************
             * 추가한 카테고리들을 표시한다.
             ***/

            int Count = descs.Count;
            int startIndex = 0;

            for (int i = 0; i < Count; i++)
            {
                FMODEventCategoryDesc desc = descs[i];
                int EventCount = desc.Count;

                EditorGUILayout.BeginHorizontal();

                //카테고리 삭제 버튼
                if (GUILayout.Button(_xIcon, GUILayout.Width(25f), GUILayout.Height(25f)))
                {
                    _showRefreshHelp = false;

                    //이 카테고리에 속해있는 모든 이벤트를 삭제한다.
                    for (int d = desc.Count; d > 0; d--) {
                        events.RemoveAt(startIndex);
                    }

                    descs.RemoveAt(i);
                    i--;
                    Count--;
                    EditorGUILayout.EndHorizontal();
                    continue;
                }

                desc.show = EditorGUILayout.Foldout(desc.show, desc.categoryName + $"<color={_CategoryCountColor}>({EventCount})</color>", _FoldTxtStyle);
                EditorGUILayout.EndHorizontal();

                descs[i] = desc;

                //접기 상태가 아니라면 해당 카테고리에 속해있는 모든 이벤트 목록을 보여준다.
                if (desc.show) {

                    EditorGUILayout.Space(20f);

                    for (int i2 = startIndex; i2 < (startIndex + EventCount); i2++)
                    {
                        ShowEventField(ref i2, ref EventCount, events, ref desc);
                        descs[i] = desc;
                    }

                    //추가버튼을 눌렀을 경우
                    if (GUILayout.Button(_AddIcon))
                    {
                        _showRefreshHelp = false;
                        _checkOverlapIndex = -1;
                        events.Insert(startIndex + desc.Count, new FMODUnity.EventReference());
                        desc.Count++;
                        descs[i] = desc;
                        EventCount++;
                    }
                }

                EditorGUILayout.Space(5);
                var rect = EditorGUILayout.BeginHorizontal();
                Handles.color = Color.gray;
                Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(10);

                startIndex += EventCount;
            }

            #endregion
        }

        private void ShowEventField(ref int index, ref int count, List<FMODUnity.EventReference> lists, ref FMODEventCategoryDesc desc)
        {
            #region Ommission
            if (_EventData == null) return;

            //유효하지 않은 이벤트 텍스트필드 스타일
            if (_notValidEventStyle == null)
            {
                _notValidEventStyle = new GUIStyle(EditorStyles.textField);
                _notValidEventStyle.normal.textColor = Color.red;
                _notValidEventStyle.onNormal.textColor = Color.red;
                _notValidEventStyle.fontStyle = FontStyle.Bold;
            }

            float baseHeight = GUI.skin.textField.CalcSize(GUIContent.none).y;

            Rect removeRect = new Rect(position.width * .9f, baseHeight, 35f, 50f);
            Rect searchRect = new Rect(position.width * .82f, baseHeight, 35f, 50f);
            Rect headerRect = position;
            headerRect.width = EditorGUIUtility.labelWidth;
            headerRect.height = baseHeight;

            Rect pathRect = position;
            pathRect.xMin = headerRect.xMax;
            pathRect.xMax = searchRect.x - 3;
            pathRect.height = baseHeight;

            //이벤트 이름 표시
            bool EventIsValid = CheckEventIsValid(index, lists);
            bool EventRefIsNull = lists[index].IsNull;
            string EventPath = EventRefIsNull ? "-" : lists[index].Path;
            string[] PathSplit = EventPath.Split('/');
            string EventName = PathSplit[PathSplit.Length - 1].Replace(' ', '_');

            GUILayout.BeginHorizontal();
            EditorGUI.indentLevel++;
            EditorGUI.indentLevel++;

            if (EventIsValid == true)
            {
                EditorGUILayout.TextField(EventName.Replace(' ', '_'), EventPath, GUILayout.Width(position.width * .85f), GUILayout.Height(25f));
            }
            else EditorGUILayout.TextField(EventName.Replace(' ', '_'), EventPath, _notValidEventStyle, GUILayout.Width(position.width * .85f), GUILayout.Height(25f));

            //돋보기 버튼을 눌렀을 경우
            if (GUILayout.Button(_SearchIcon, GUILayout.Width(25f), GUILayout.Height(25f)))
            {
                var eventBrowser = ScriptableObject.CreateInstance<EventBrowser>();

                SerializedObject dataObject = new SerializedObject(_EventData);
                SerializedProperty eventsProperty = dataObject.FindProperty(_DescPropertyStr[_EventCategorySelect]);
                SerializedProperty elementProperty = eventsProperty.GetArrayElementAtIndex(index);

                eventBrowser.ChooseEvent(elementProperty);
                var windowRect = searchRect;
                windowRect.xMin = pathRect.xMin;
                windowRect.position = GUIUtility.GUIToScreenPoint(windowRect.position);
                windowRect.height = searchRect.height + 1;
                windowRect.width = Mathf.Max(windowRect.width, 300f);
                eventBrowser.ShowAsDropDown(windowRect, new Vector2(windowRect.width, 400));

                _eventIsOverlap = false;
                _checkOverlapIndex = index;
            }

            //제거 버튼을 눌렀을 경우
            if (GUILayout.Button(_RemoveIcon, GUILayout.Width(25f), GUILayout.Height(25f)))
            {
                _checkOverlapIndex = -1;
                lists.RemoveAt(index);
                index -= 1;
                count--;

                desc.Count--;
            }
            GUILayout.EndHorizontal();

            if (_checkOverlapIndex == index && _eventIsOverlap) {
                string categoryName = (_EventCategorySelect == 0 ? "BGM" : "SFX");
                EditorGUILayout.HelpBox($"{_overlapEventName}는 이미{categoryName}카테고리에 존재하는 이벤트입니다.", MessageType.Error);
            }

            if (EventIsValid == false)
            {
                EditorGUILayout.HelpBox($"{EventName}은(는) FMod Studio Project에서 존재하지 않는 이벤트입니다.", MessageType.Error);
            }

            GUILayout.Space(3f);

            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
            #endregion
        }

        private void CreateEnumScript()
        {
            #region Ommision
            if (_EventData == null) return;
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("using UnityEngine;");
            builder.AppendLine("");

            /**************************************
             * Bus Enum 정의
             ***/
            builder.AppendLine("public enum FModBusType");
            builder.AppendLine("{");
            builder.AppendLine($"   MasterBus = 0,");
            builder.AppendLine($"   BGMBus = 1,");
            builder.AppendLine($"   SFXBus = 2");
            builder.AppendLine("}");
            builder.AppendLine("");


            builder.AppendLine("public sealed class FModBusPath");
            builder.AppendLine("{");
            builder.AppendLine($"   public static readonly string MasterBus=\"bus:/\";");
            builder.AppendLine($"   public static readonly string BGMBus=\"bus:/{_EventData.BGMBusName}\";");
            builder.AppendLine($"   public static readonly string SFXBus=\"bus:/{_EventData.SFXBusName}\";");
            builder.AppendLine("}");
            builder.AppendLine("");

            /**************************************
             * BGM Enum 정의
             ***/
            int total = 0;

            builder.AppendLine("public enum FModBGMEventType");
            builder.AppendLine("{");

            int Count = _EventData.BGMEvents.Count;
            for (int i = 0; i < Count; i++)
            {
                if (_EventData.BGMEvents[i].IsNull || !CheckEventIsValid(i, _EventData.BGMEvents))
                    continue;

                string comma = (i == Count - 1 ? "" : ",");
                string[] pathSplit = _EventData.BGMEvents[i].Path.Split('/');
                string enumName = pathSplit[pathSplit.Length - 1].Replace(' ', '_');

                builder.AppendLine($"   {enumName} = {i}{comma}");
            }

            builder.AppendLine("}");
            builder.AppendLine("");


            /**************************************
             * SFX Enum 정의
             ***/
            total += Count;
            builder.AppendLine("public enum FModSFXEventType");
            builder.AppendLine("{");

            Count = _EventData.SFXEvents.Count;
            for (int i = 0; i < Count; i++)
            {
                if (_EventData.SFXEvents[i].IsNull || !CheckEventIsValid(i, _EventData.SFXEvents))
                    continue;

                string   comma       = (i == Count - 1 ? "" : ",");
                string[] pathSplit   = _EventData.SFXEvents[i].Path.Split('/');
                string   enumName    = pathSplit[pathSplit.Length - 1].Replace(' ', '_'); ;

                builder.AppendLine($"   {enumName} = {i+total}{comma}");
            }

            builder.AppendLine("}");
            builder.AppendLine("");

            /**************************************
             * None Enum 정의
             ***/
            builder.AppendLine("public enum FModNoGroupEventType");
            builder.AppendLine("{");

            total += Count;
            Count = _EventData.NoneEvents.Count;
            for (int i = 0; i < Count; i++)
            {
                if (_EventData.SFXEvents[i].IsNull || !CheckEventIsValid(i, _EventData.SFXEvents))
                    continue;

                string   comma       = (i == Count - 1 ? "" : ",");
                string[] pathSplit   = _EventData.NoneEvents[i].Path.Split('/');
                string   enumName    = pathSplit[pathSplit.Length - 1].Replace(' ', '_'); ;

                builder.AppendLine($"   {enumName} = {i+total}{comma}");
            }

            builder.AppendLine("}");
            builder.AppendLine("");


            /***************************************
             * Event Reference List class 정의
             ***/
            builder.AppendLine("public sealed class FModEventReferenceList");
            builder.AppendLine("{");
            builder.AppendLine("    public static readonly FMOD.GUID[] Events = new FMOD.GUID[]");
            builder.AppendLine("    {");

            AddEventListScript(builder, _EventData.BGMEvents); //Add BGMEvents
            AddEventListScript(builder, _EventData.SFXEvents); //Add SFXEvents
            AddEventListScript(builder, _EventData.NoneEvents,true); //Add NoneEvents


            builder.AppendLine("    };");
            builder.AppendLine("}");
            builder.AppendLine("");


            //생성 및 새로고침
            File.WriteAllText(_ResourcePath, builder.ToString());

            #endregion
        }

        private void AddEventListScript(StringBuilder builder, List<FMODUnity.EventReference> list, bool lastWork=false)
        {
            int Count = list.Count;
            for (int i = 0; i < Count; i++)
            {
                if (list[i].IsNull) continue;

                string comma = (i == Count - 1 && lastWork ? "" : ",");
                string guidValue = $"Data1={list[i].Guid.Data1}, Data2={list[i].Guid.Data2}, Data3={list[i].Guid.Data3}, Data4={list[i].Guid.Data4}";

                builder.AppendLine("        new FMOD.GUID{ " + guidValue + " }" + comma);
            }
        }

        private void RefreshStudioChanges()
        {
            #region Ommission
            if (_EventData == null) return;

            //기존 데이터 초기화
            _EventData.BGMDesc.Clear();
            _EventData.BGMEvents.Clear();

            _EventData.SFXDesc.Clear();
            _EventData.SFXEvents.Clear();

            _EventData.NoneEvents.Clear();
            _EventData.NoneDesc.Clear();

            //모든 이벤트들을 FMOD Studio의 값대로 갱신.
            List<EditorEventRef> events = EventManager.Events;
            List<EditorBankRef> banks = EventManager.Banks;
            string[] pathSplits;


            foreach (EditorEventRef eventRef in events)
            {
                pathSplits = eventRef.Path.Split('/');

                List<FMODEventCategoryDesc> descLists;
                List<FMODUnity.EventReference> eventLists;

                if (pathSplits[1] == _EventData.BGMGroupRootFolder) {

                    descLists = _EventData.BGMDesc;
                    eventLists= _EventData.BGMEvents;
                }
                else if (pathSplits[1] == _EventData.SFXGroupRootFolder){

                    descLists = _EventData.SFXDesc;
                    eventLists = _EventData.SFXEvents;
                }
                else{

                    descLists = _EventData.NoneDesc;
                    eventLists = _EventData.NoneEvents;
                }


                string eventName = pathSplits[pathSplits.Length - 1];
                string eventpath = eventRef.Path.Replace($"/{eventName}", "").Replace("event:/", "");
                if (eventpath == "event:") eventpath = "NoGroup";

                int categoryIndex = CheckCategoryExist(eventpath, descLists);

                eventpath = eventpath.Replace($"{{pathSplits[0]}}/{{pathSplits[1]","");

                //카테고리가 존재하지 않다면 생성.
                if (categoryIndex == -1) {

                    descLists.Add(new FMODEventCategoryDesc() { categoryName = eventpath, Count = 0, show = false });
                    categoryIndex = descLists.Count - 1;
                }
                FMODEventCategoryDesc desc = descLists[categoryIndex];

                //카테고리에 추가.
                int startIndex = GetCategoryEventStartIndex(descLists, categoryIndex);
                eventLists.Insert(startIndex, new FMODUnity.EventReference() { Guid = eventRef.Guid, Path = eventRef.Path });
                desc.Count++;
                descLists[categoryIndex] = desc;
            }
            #endregion
        }

        private List<FMODUnity.EventReference> GetEventList(int index)
        {
            if (_EventData == null) return null;

            switch (index)
            {
                case (0): return _EventData.BGMEvents;
                case (1): return _EventData.SFXEvents;
                case (2): return _EventData.NoneEvents;
            }

            return null;
        }

        private List<FMODEventCategoryDesc> GetCategoryList(int index)
        {
            if (_EventData == null) return null;

            switch(index)
            {
                case (0): return _EventData.BGMDesc;
                case (1): return _EventData.SFXDesc;
                case (2): return _EventData.NoneDesc;
            }

            return null;
        }

        private string GetCategoryName(int index)
        {
            if (_EventData == null) return null;

            switch (index)
            {
                case (0): return "BGM";
                case (1): return "SFX";
                case (2): return "NoGroup";
            }

            return null;
        }

        private int GetCategoryEventStartIndex(List<FMODEventCategoryDesc> list, int index)
        {
            int total = 0;

            for(int i=0; i<index; i++)
            {
                total += list[i].Count;
            }

            return total;
        }

        private int CheckCategoryExist(string name, List<FMODEventCategoryDesc> list)
        {
            for(int i=list.Count-1; i>=0; i--)
            {
                if (list[i].categoryName == name) return i;
            }

            return -1;
        }

        private bool CheckEventIsOverlap(int index, List<FMODUnity.EventReference> guidList)
        {
            #region Omission
            List<FMODUnity.EventReference> list;
            FMOD.GUID guid = guidList[index].Guid;

            //중복 체크를 실행한다.

            //BGM
            list = _EventData.BGMEvents;
            int Count = list.Count;
            for (int i=0; i<Count; i++)
            {
                if (i == index || list[i].IsNull) continue;

                //중복 확인이 되었을 경우
                if (guid.Equals(list[i].Guid)) return true;
            }

            //SFX
            list = _EventData.SFXEvents;
            Count = list.Count;
            for (int i = 0; i < Count; i++)
            {
                if (i == index || list[i].IsNull) continue;

                //중복 확인이 되었을 경우
                if (guid.Equals(list[i].Guid)) return true;
            }

            //NoneGroup
            list = _EventData.NoneEvents;
            Count = list.Count;
            for (int i = 0; i < Count; i++)
            {
                if (i == index || list[i].IsNull) continue;

                //중복 확인이 되었을 경우
                if (guid.Equals(list[i].Guid)) return true;
            }

            return false;
            #endregion
        }

        private bool CheckEventIsValid( int index, List<FMODUnity.EventReference> lists )
        {
            if (Settings.Instance.EventLinkage == EventLinkage.Path && !lists[index].Guid.IsNull)
            {
                EditorEventRef eventRef = EventManager.EventFromGUID(lists[index].Guid);

                if (eventRef == null || eventRef != null && (eventRef.Path != lists[index].Path)) {
                    return false;
                }
            }
            return true;
        }

    }
#endif

    #endregion

    private enum FadeState
    {
        NONE=0,
        PLAYING,
        PENDING_KILL
    }

    private enum BGMFadeState
    {
        NONE,
        SMALL_Apply,
        SMALL_COMPLETE,
        BIG_Apply,
        BIG_COMPLETE
    }

    private sealed class FadeInfo
    {
       public int             fadeID     = -1;
       public int             fadeCount  = 0;
       public FadeState       fadeState  = FadeState.NONE;
    }

    //==================================
    ////          Property          //// 
    //==================================
    public static bool  AutoFadeInOutBGM    { get; set; } = false;
    public static float AutoFadeBGMDuration { get; set; } = 3f;
    public const  int   AutoFadeBGMID = -9724;
    public static int   TCount = 0;

    public FModEventFadeCompleteNotify OnEventFadeComplete;


    //==================================
    ////          Fields            //// 
    //==================================
    private static FModAudioManager _Ins;
    private int             _BGMuuid;
    private Coroutine       _BGMFadeCoroutine;

    #if FMOD_Event_ENUM
    private FModBGMEventType _NextBGMType;
    private bool              _NextBGMFade = false;
    private float            _NextBGMVolume = 1f;
    private int              _NextBGMstartPosition = 0;
    private string           _NextBGMParam = String.Empty;
    private float            _NextBGMParamValue = 0f;
    #endif

    private FMOD.Studio.Bus[]                          _BusList  = new FMOD.Studio.Bus[3];
    private Dictionary<int, FMOD.Studio.EventInstance> _InsList  = new Dictionary<int, EventInstance>();
    private List<FadeInfo>                             _FadeList = new List<FadeInfo>(10);


    //==================================
    ////      Public Methods         /// 
    //==================================
#if FMOD_Event_ENUM

    /**************************************
    * Union methods
    ***/
    private FadeInfo GetFadeInfo(int fadeID)
    {
        int Count = _FadeList.Count;
        for (int i = 0; i < Count; i++)
        {
            if (_FadeList[i].fadeID == fadeID) return _FadeList[i];
        }

        return null;
    }

    private static void AddFadeInfo(int fadeID)
    {
        #region Ommission
        int emptyIndex = -1;
        int Count = _Ins._FadeList.Count;

        //이미 같은 ID의 페이드가 실행중이라면
        for (int i = 0; i < Count; i++)
        {
            FadeInfo info = _Ins._FadeList[i];
            if (info.fadeID == fadeID && info.fadeState == FadeState.PLAYING)
            {

                info.fadeCount++;
                return;
            }

            if (emptyIndex == -1 && info.fadeState == FadeState.NONE)
            {
                emptyIndex = i;
            }
        }

        //같은 ID의 페이드가 없다면
        bool isFulled = (emptyIndex == -1);
        FadeInfo newInfo = (isFulled ? new FadeInfo() : _Ins._FadeList[emptyIndex]);

        if (isFulled) _Ins._FadeList.Add(newInfo);
        newInfo.fadeID = fadeID;
        newInfo.fadeCount = 1;
        newInfo.fadeState = FadeState.PLAYING;
        #endregion
    }

    public static bool FadeIsPlaying(int fadeID)
    {
        if (_Ins == null) return false;

        int Count = _Ins._FadeList.Count;
        for (int i = 0; i < Count; i++)
        {
            FadeInfo info = _Ins._FadeList[i];
            bool isPlaying = (info.fadeState == FadeState.NONE);
            bool isSameID = (info.fadeID == fadeID);

            if (isPlaying && isSameID) return true;
        }

        return false;
    }

    public static int GetFadeCount(int fadeID)
    {
        if (_Ins == null) return -1;

        int Count = _Ins._FadeList.Count;
        for (int i = 0; i < Count; i++)
        {
            FadeInfo info = _Ins._FadeList[i];
            bool isPlaying = (info.fadeState == FadeState.PLAYING);
            bool isSameID = (info.fadeID == fadeID);

            if (isPlaying && isSameID) return info.fadeCount;
        }

        return -1;
    }

    public static void StopFadeByID(int fadeID)
    {
        if (_Ins == null) return;

        int Count = _Ins._FadeList.Count;
        for (int i = 0; i < Count; i++)
        {
            bool isPlaying = _Ins._FadeList[i].fadeState == FadeState.PLAYING;

            //changed pendking state
            if (isPlaying && _Ins._FadeList[i].fadeID == fadeID)
            {

                _Ins._FadeList[i].fadeState = FadeState.PENDING_KILL;
                return;
            }
        }
    }

    public static void StopAllFade()
    {
        if (_Ins == null) return;

        int Count = _Ins._FadeList.Count;
        for (int i = 0; i < Count; i++)
        {
            bool isPlaying = _Ins._FadeList[i].fadeState == FadeState.PLAYING;

            //changed pendking state
            if (isPlaying)
            {
                _Ins._FadeList[i].fadeState = FadeState.PENDING_KILL;
            }
        }
    }

    public static void StopAllEvent()
    {
        if (_Ins == null) return;

        StopAllFade();
        StopAllInstance();
        StopBGM();
    }

    public static void DestroyAllEvent()
    {
        StopAllFade();
        DestroyAllInstance();
        DestroyBGM();
    }

    /**************************************
     * Bus
     ***/
    public static void SetBusVolume(FModBusType busType, float volume)
    {
        if (_Ins == null) return;

        int index = (int)busType;
        if (_Ins._BusList[index].isValid() == false) return;

        _Ins._BusList[index].setVolume(volume);
    }

    public static float GetBusVolume(FModBusType busType)
    {
        if (_Ins == null) return -1f;

        int index = (int)busType;
        if (_Ins._BusList[index].isValid() == false) return -1f;

        float volume;
        _Ins._BusList[index].getVolume(out volume);

        return volume;
    }

    /************************************
     * BGM
     **/
    public static void PlayBGM( FModBGMEventType type, float volume=-1f, int startProgress = -1, string paramName="", float paramValue=0f, Vector3 position=default)
    {
        if (_Ins == null) return;

        //이미 배경음이 존재할 경우
        if(InstanceIsValid(_Ins._BGMuuid))
        {
            //자동 페이드를 사용한다면( volume~0 )
            if(AutoFadeInOutBGM) {

                _Ins._NextBGMType = type;
                _Ins._NextBGMVolume = volume;
                _Ins._NextBGMstartPosition = startProgress;
                _Ins._NextBGMParam = paramName;
                _Ins._NextBGMParamValue = paramValue;
                _Ins._NextBGMFade = true;
                ApplyBGMFade(0f, AutoFadeBGMDuration * .5f, AutoFadeBGMID, true);
                return;
            }

            DestroyInstance(_Ins._BGMuuid);
        }

        float startVolume = (AutoFadeInOutBGM?0f:volume);
        CreateInstance(type, out _Ins._BGMuuid, position);
        PlayInstance(_Ins._BGMuuid, startVolume, startProgress, paramName, paramValue );

        //자동 페이드르 사용한다면( 0~volume )
        if(AutoFadeInOutBGM){

            _Ins._NextBGMFade = false;
            ApplyBGMFade(volume, AutoFadeBGMDuration * (_Ins._NextBGMFade?.5f:1f), AutoFadeBGMID);
        }
    }

    public static void PlayBGM(FModNoGroupEventType type, float volume = -1f, int startProgress = -1, string paramName = "", float paramValue = 0f)
    {
        PlayBGM((FModBGMEventType)type, volume, startProgress, paramName, paramValue);
    }
    
    public static void StopBGM(bool applyAutoFade=true)
    {
        if (InstanceIsValid(_Ins._BGMuuid)==false) return;

        if(applyAutoFade && AutoFadeInOutBGM)
        {
            _Ins._NextBGMFade = false;
            ApplyBGMFade(0f, AutoFadeBGMDuration, AutoFadeBGMID);
            return;
        }

        StopInstance(_Ins._BGMuuid);
    }

    public static void DestroyBGM()
    {
        if (InstanceIsValid(_Ins._BGMuuid)==false) return;

        DestroyInstance(_Ins._BGMuuid);
    }

    public static bool IsPlayingBGM()
    {
        return InstanceIsPlaying(_Ins._BGMuuid);
    }

    public static void ApplyBGMFade(float goalVolume, float fadeTime, int fadeID=0, bool completeDestroy=false)
    {
        if ( InstanceIsValid(_Ins._BGMuuid)==false) return;

        //기존에 실행되던 BGM Fade가 있다면 정지시킨다.
        if (_Ins._BGMFadeCoroutine != null)
        {
            FadeInfo deleteInfo = _Ins.GetFadeInfo(fadeID);
            deleteInfo.fadeCount--;
            _Ins.StopCoroutine(_Ins._BGMFadeCoroutine);
        }

        AddFadeInfo(fadeID);

        if(goalVolume<0f) {
            _Ins._InsList[_Ins._BGMuuid].getVolume(out goalVolume);
        }

        _Ins._BGMFadeCoroutine = _Ins.StartCoroutine(_Ins.EventFadeProgress(goalVolume, fadeTime, _Ins._BGMuuid, fadeID, completeDestroy));
    }

    public static void SetBGMParameter( string paramName, float paramValue )
    {
        SetInstanceParameter(_Ins._BGMuuid, paramName, paramValue);
    }

    public static void SetBGMVolume( float volume )
    {
        SetInstanceVolume(_Ins._BGMuuid, volume);
    }

    public static void SetBGMProgress( int progressByMillieSeconds )
    {
        SetInstanceProgress(_Ins._BGMuuid, progressByMillieSeconds);
    }

    public static void SetBGMProgress( float progressRatio )
    {
        SetInstanceProgress(_Ins._BGMuuid, progressRatio);
    }

    /***********************************
     * SFX
     **/
    public static void PlayOneShotSFX( FModSFXEventType type, Vector3 position=default, float volume=-1f, int startProgress=-1, string paramName="", float paramValue=0f, float minDistance=1f, float maxDistance=20f)
    {
        if (_Ins == null) return; 
        var instance = FMODUnity.RuntimeManager.CreateInstance(FModEventReferenceList.Events[(int)type]);

        bool is3D;
        FMOD.Studio.EventDescription desc;
        instance.getDescription(out desc);
        desc.is3D(out is3D);
        if (is3D)
        {
            instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
            instance.setProperty(FMOD.Studio.EVENT_PROPERTY.MINIMUM_DISTANCE, minDistance);
            instance.setProperty(FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, maxDistance);
        }

        instance.start();
        if (volume >= 0) instance.setVolume(volume);
        if (paramName!="") instance.setParameterByName(paramName, paramValue);
        if (startProgress >= 0) instance.setTimelinePosition(startProgress);
        instance.release();
    }

    public static void PlayOneShotSFX(FModNoGroupEventType type, Vector3 position=default, float volume = -1f, int startProgress = -1, string paramName = "", float paramValue = 0f, float minDistance = 1f, float maxDistance = 20f)
    {
        PlayOneShotSFX((FModSFXEventType)type, position, volume, startProgress, paramName, paramValue, minDistance, maxDistance);
    }

    /***************************************
     * Event Instance
     **/
    public static bool InstanceIsValid(int UUID)
    {
        if (_Ins == null) return false;
        return _Ins._InsList.ContainsKey(UUID);
    }

    public static bool CreateInstance(FModBGMEventType type, out int outUUID, Vector3 position=default )
    {
        if (_Ins == null)
        {
            outUUID = default;
            return false;
        }

        int index = (int)type;
        EventInstance newInstance = FMODUnity.RuntimeManager.CreateInstance(FModEventReferenceList.Events[index]);

        bool is3D;
        FMOD.Studio.EventDescription desc;
        newInstance.getDescription(out desc);
        desc.is3D(out is3D);
        if (is3D)
        {
            newInstance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
            newInstance.setProperty(FMOD.Studio.EVENT_PROPERTY.MINIMUM_DISTANCE, 1f);
            newInstance.setProperty(FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, 20f);
        }

        _Ins._InsList.Add(newInstance.GetHashCode(), newInstance);
        outUUID = newInstance.GetHashCode();
        return true;
    }

    public static bool CreateInstance(FModNoGroupEventType type, out int outUUID, Vector3 position=default)
    {
        return CreateInstance((FModBGMEventType)type, out outUUID, position);
    }

    public static bool CreateInstance(FModSFXEventType type, out int outUUID, Vector3 position=default)
    {
        return CreateInstance((FModBGMEventType)type, out outUUID, position);
    }

    public static bool DestroyInstance(int UUID)
    {
        if (_Ins==null || _Ins._InsList.ContainsKey(UUID) == false) return false;
        _Ins._InsList[UUID].stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        _Ins._InsList[UUID].release();
        _Ins._InsList[UUID].clearHandle();
        _Ins._InsList.Remove(UUID);

        return true;
    }

    public static void PlayInstance(int UUID, float volume = -1f, int startProgress = -1, string paramName="", float paramValue=0f)
    {
        if (_Ins==null || _Ins._InsList.ContainsKey(UUID) == false) return;

        _Ins._InsList[UUID].start();
        if(paramName!="") _Ins._InsList[UUID].setParameterByName(paramName, paramValue);
        if (volume >= 0) _Ins._InsList[UUID].setVolume(volume);
        if (startProgress>=0)_Ins._InsList[UUID].setTimelinePosition(startProgress);
    }

    public static void SetInstanceParameter(int UUID, string paramName, float paramValue)
    {
        if (_Ins==null || _Ins._InsList.ContainsKey(UUID) == false) return;

        _Ins._InsList[UUID].start();
        _Ins._InsList[UUID].setParameterByName(paramName, paramValue);
    }

    public static void StopInstance(int UUID)
    {
        if (_Ins==null || _Ins._InsList.ContainsKey(UUID) == false) return;
        _Ins._InsList[UUID].stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public static void SetInstanceVolume(int UUID, float volume)
    {
        if (_Ins==null || _Ins._InsList.ContainsKey(UUID) == false) return;
        if (volume < 0f) volume = 0f;
        _Ins._InsList[UUID].setVolume(volume);
    }

    public static void SetInstanceProgress(int UUID, int progressByMillieSeconds)
    {
        if (_Ins==null || _Ins._InsList.ContainsKey(UUID) == false) return;
        if (progressByMillieSeconds < 0) progressByMillieSeconds = 0;
        _Ins._InsList[UUID].setTimelinePosition(progressByMillieSeconds);
    }

    public static void SetInstanceProgress(int UUID, float progressRatio)
    {
        if (_Ins==null || _Ins._InsList.ContainsKey(UUID) == false) return;
        if (progressRatio < 0) progressRatio = 0;

        EventDescription desc;
        _Ins._InsList[UUID].getDescription(out desc);

        int length;
        desc.getLength(out length);
        _Ins._InsList[UUID].setTimelinePosition(Mathf.RoundToInt(length*progressRatio));
    }

    public static void SetInstancePosition(int UUID, Vector3 position, float minDistance=1f, float maxDistance=20f)
    {
        if (_Ins==null || _Ins._InsList.ContainsKey(UUID) == false) return;

        bool is3D;
        FMOD.Studio.EventDescription desc;
        _Ins._InsList[UUID].getDescription(out desc);
        desc.is3D(out is3D);
        if (is3D)
        {
            _Ins._InsList[UUID].set3DAttributes(RuntimeUtils.To3DAttributes(position));
            _Ins._InsList[UUID].setProperty(FMOD.Studio.EVENT_PROPERTY.MINIMUM_DISTANCE, minDistance);
            _Ins._InsList[UUID].setProperty(FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, maxDistance);
        }
    }

    public static void SetInstanceDistance( int UUID, float Min, float Max )
    {
        if (_Ins==null || _Ins._InsList.ContainsKey(UUID) == false) return;
        FMOD.Studio.EventDescription desc;
        _Ins._InsList[UUID].getDescription(out desc);

        bool is3D;
        desc.is3D(out is3D);

        if(is3D)
        {
            _Ins._InsList[UUID].setProperty(FMOD.Studio.EVENT_PROPERTY.MINIMUM_DISTANCE, Min);
            _Ins._InsList[UUID].setProperty(FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, Max);
        }
    }

    public static void ApplyInstanceFade(int UUID, float goalVolume, float fadeTime, int fadeID = 0, bool completeDestroy = false)
    {
        if (_Ins==null || _Ins._InsList.ContainsKey(UUID) == false) return;

        AddFadeInfo(fadeID);
        _Ins.StartCoroutine(_Ins.EventFadeProgress(goalVolume, fadeTime, UUID, fadeID, completeDestroy));
    }

    public static void StopAllInstance()
    {
        if (_Ins == null) return;

        int Count = _Ins._InsList.Count;
        for (int i = 0; i < Count; i++)
        {
            _Ins._InsList[i].stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }

    public static void DestroyAllInstance()
    {
        if (_Ins == null) return;

        foreach (KeyValuePair<int, EventInstance> key in _Ins._InsList)
        {
            int UUID = key.Key;

            _Ins._InsList[UUID].release();
            _Ins._InsList[UUID].clearHandle();
        }

        _Ins._InsList.Clear();
    }

    public static bool InstanceIsPlaying(int uuid)
    {
        if (_Ins == null || InstanceIsValid(uuid)==false) return false;

        FMOD.Studio.PLAYBACK_STATE state;
        _Ins._InsList[uuid].getPlaybackState(out state);

        return (state == FMOD.Studio.PLAYBACK_STATE.PLAYING);
    }

#endif


    //===================================
    ////       Core  methods         //// 
    //===================================
    private void Awake()
    {
        if(_Ins==null)
        {
            _Ins = this;
            TCount = 0;
            DontDestroyOnLoad(gameObject);

            #if FMOD_Event_ENUM
            OnEventFadeComplete += BGMFadeChange;

            _BusList[(int)FModBusType.MasterBus] = FMODUnity.RuntimeManager.GetBus(FModBusPath.MasterBus);
            _BusList[(int)FModBusType.BGMBus] = FMODUnity.RuntimeManager.GetBus(FModBusPath.BGMBus);
            _BusList[(int)FModBusType.SFXBus] = FMODUnity.RuntimeManager.GetBus(FModBusPath.SFXBus);
            #endif

            //FadeInfo 초기화
            if (_FadeList == null) _FadeList = new List<FadeInfo>(10);
            for (int i = 0; i < 10; i++) _FadeList.Add(new FadeInfo());

            return;
        }

        Destroy(gameObject);
    }

    #region Fade_Progress
#if FMOD_Event_ENUM
    private IEnumerator EventFadeProgress( float goalVolume, float fadeTime, int targetUUID, int fadeID, bool completeDestroy )
    {
        #region Omission
        //계산에 필요한 것들을 구한다.
        if (InstanceIsValid(targetUUID) == false) yield break;

        EventInstance targetIns = _InsList[targetUUID];
        FadeInfo info = GetFadeInfo( fadeID );
        if (info == null) yield break;

        float startVolume;
        targetIns.getVolume(out startVolume);    

        float distance      = ( goalVolume- startVolume);
        float fadeTimeDiv   = (1f / fadeTime);
        float fadeTimeRatio = 0f;

        //페이드 효과 적용.
        do
        {
            //Check pendingKill
            bool isPendingKill     = (info.fadeState == FadeState.PENDING_KILL);
            bool InsNotValid       = (targetIns.isValid() == false) || InstanceIsValid(targetUUID)==false;
            bool infoIsNull        = (info == null);

            if (isPendingKill || InsNotValid || infoIsNull)
            {
                info.fadeCount--;
                if(info.fadeCount<=0) {

                    info.fadeState = FadeState.NONE;
                }

                yield break;
            }

            fadeTime -= Time.deltaTime;
            fadeTimeRatio = 1f - (fadeTime * fadeTimeDiv);

            float currVolume;
            targetIns.getVolume(out currVolume);

            float newVolume = startVolume + (distance * fadeTimeRatio);
            if (newVolume < 0) newVolume = 0f;
            if (fadeTimeRatio >= 1f) newVolume = goalVolume;

            targetIns.setVolume(newVolume);

             yield return null;
        }
        while(fadeTimeRatio<1f);

        targetIns.setVolume(goalVolume);

        //페이드 마무리
        info.fadeCount--;
        if (info.fadeCount <= 0) {
            info.fadeState = FadeState.NONE;
        }

        //완료될 때 원한다면 해당 인스턴스를 파괴한다.
        if (completeDestroy) {

            DestroyInstance(targetUUID);
        }

        _BGMFadeCoroutine = null;
        OnEventFadeComplete(fadeID, goalVolume);
        #endregion
    }

    private void BGMFadeChange(int fadeID, float goalVolume)
    {
        if (fadeID != AutoFadeBGMID || _NextBGMFade==false) return;

        PlayBGM(_NextBGMType, _NextBGMVolume, _NextBGMstartPosition, _NextBGMParam, _NextBGMParamValue);
    }

#endif

    #endregion

}