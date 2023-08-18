using FMODUnity;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Rendering;
#endif
using UnityEngine;

/******************************************************
 *   발판의 움직임에서 발생하는 소리가 정의된 컴포넌트입니다.
 * ***/
[AddComponentMenu("Platform/FModEventPlayBehavior")]
public sealed class FModEventPlayBehavior : PlatformBehaviorBase
{
    #region Editor_Extension
    /***************************************************
    *  PlatformAudioPlayData Drawer...
    * ***/
#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomPropertyDrawer(typeof(PlatformAudioPlayData))]
    public sealed class PlatformAudioPlayDataDrawer : PropertyDrawer
    {
        //===============================================
        /////            Private Fields              ////
        ///==============================================
        private SerializedProperty ApplyTimingProperty;
        private SerializedProperty PlayEventProperty;


        //===============================================
        /////            Magic Methods               ////
        ///==============================================
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI_Initialized(property);

            float baseHeight  = GetBaseHeight();
            float space       = baseHeight + 5f;
            bool isExpanded   = property.isExpanded;
            Rect headerRect   = position;
            headerRect.height = baseHeight;

            /*******************************************
             *  모든 직렬화 필드들을 표시한다...
             ****/
            using (var scope = new EditorGUI.PropertyScope(position, label, property)){

                //해당 프로퍼티가 확장되어 있다면, 세부 프로퍼티들을 공개한다...
                if (property.isExpanded = EditorGUI.Foldout(headerRect, property.isExpanded, label))
                {
                    GUI_ShowTimingField(ref headerRect, space);

                    GUI_ShowEventReference(ref headerRect, space);
                }

                scope.Dispose();
            }

        }


        //===============================================
        /////              GUI Methods              ////
        ///==============================================
        private void GUI_Initialized(SerializedProperty editorProperty)
        {
            #region Omit
            /**********************************************
             *  모든 필드의 SerializedProperty를 가져옵니다...
             * ***/
            if (ApplyTimingProperty == null){

                ApplyTimingProperty = editorProperty.FindPropertyRelative("timing");
            }

            if (PlayEventProperty == null){

                PlayEventProperty = editorProperty.FindPropertyRelative("PlayEvent");
            }
            #endregion
        }

        private void GUI_ShowTimingField(ref Rect header, float offset=5f)
        {
            #region Omit
            if (ApplyTimingProperty == null) return;

            header.y += offset;
            System.Enum result = EditorGUI.EnumFlagsField(header, "Apply Timing", ApplyTimingProperty.GetEnumValue<PlatformApplyTiming>());
            ApplyTimingProperty.SetEnumValue(result);
            #endregion
        }

        private void GUI_ShowEventReference(ref Rect header, float offset = 5f)
        {
            #region Omit
            if (PlayEventProperty == null) return;
            header.y += offset;
            EditorGUI.PropertyField(header, PlayEventProperty);
            #endregion
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float baseHeight = GetBaseHeight();
            return baseHeight + (property.isExpanded? 60f:0f);
        }


        //===============================================
        /////            Utility Methods             ////
        ///==============================================
        private float GetBaseHeight()
        {
            return GUI.skin.textField.CalcSize(GUIContent.none).y;
        }

    }
#endif

    #endregion

    #region Define
    [System.Serializable]
    public class PlatformAudioPlayData
    {
        public EventReference       PlayEvent;
        public PlatformApplyTiming  timing;

        public void ApplyPlayEvent(PlatformApplyTiming applyTiming)
        {
            int timingInt = (int)timing;    
            if((timingInt &= (int)applyTiming)>0){

                FMODUnity.RuntimeManager.PlayOneShot(PlayEvent);
            }
        }
    }
    #endregion

    //=========================================
    /////       Property And Fields       /////
    //=========================================
    [SerializeField] bool UsedStayBehavior = false;
    [SerializeField] List<PlatformAudioPlayData> Datas = new List<PlatformAudioPlayData>();


    //=========================================
    /////        Override methods         /////
    //=========================================
    public override void BehaviorStart(PlatformObject affectedPlatform)
    {
        ApplyAudioPlays(PlatformApplyTiming.BehaviorStart);
    }

    public override void BehaviorEnd(PlatformObject changedTarget)
    {
        ApplyAudioPlays(PlatformApplyTiming.BehaviorEnd);
    }

    public override void OnObjectPlatformEnter(PlatformObject affectedPlatform, GameObject standingTarget, Vector3 standingPoint, Vector3 standingNormal)
    {
        ApplyAudioPlays(PlatformApplyTiming.OnObjectEnter);
    }

    public override void OnObjectPlatformStay(PlatformObject affectedPlatform, GameObject standingTarget, Vector3 standingPoint, Vector3 standingNormal)
    {
       if(UsedStayBehavior) ApplyAudioPlays(PlatformApplyTiming.OnObjectStay);
    }

    public override void OnObjectPlatformExit(PlatformObject affectedPlatform, GameObject exitTarget)
    {
        ApplyAudioPlays(PlatformApplyTiming.OnObjectExit);
    }


    //=========================================
    /////         Utility Methods       /////
    //=========================================
    private void ApplyAudioPlays(PlatformApplyTiming timing)
    {
        #region Omit
        int Count = Datas.Count;
        for (int i = 0; i < Count; i++)
        {
            Datas[i].ApplyPlayEvent(timing);
        }
        #endregion
    }

}
