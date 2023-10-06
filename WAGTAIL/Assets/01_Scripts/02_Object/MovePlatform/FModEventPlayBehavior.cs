using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Rendering;
#endif

/******************************************************
 *   발판의 움직임에서 발생하는 소리가 정의된 컴포넌트입니다.
 * ***/
[AddComponentMenu("Platform/FModEventPlayBehavior")]
public sealed class FModEventPlayBehavior : PlatformBehaviorBase
{
    #region Editor_Extension
    /**********************************************************
    *  PlatformAudioPlayData의 에디터 확장을 위한 private class..
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
        private SerializedProperty PlayEventParamProperty;


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

                    GUI_ShowEventParamReference(ref headerRect, space);
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
            ApplyTimingProperty     = editorProperty.FindPropertyRelative("timing");
            PlayEventProperty       = editorProperty.FindPropertyRelative("PlayEvent");
            PlayEventParamProperty  = editorProperty.FindPropertyRelative("PlayEventParam");
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

            if (PlayEventProperty.isExpanded) header.y += 80f;
            #endregion
        }

        private void GUI_ShowEventParamReference(ref Rect header, float offset = 5f)
        {
            #region Omit
            if (PlayEventParamProperty == null) return;
            header.y += offset;
            EditorGUI.PropertyField(header, PlayEventParamProperty);
            #endregion
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            GUI_Initialized( property );
            float baseHeight = GetBaseHeight();

            if (PlayEventProperty!=null && PlayEventProperty.isExpanded) baseHeight += 80f;

            return baseHeight + (property.isExpanded? 130f:0f);
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
        public EventReference         PlayEvent;
        public FModParameterReference PlayEventParam;
        public PlatformApplyTiming    timing;

        public void ApplyPlayEvent(PlatformApplyTiming applyTiming, Vector3 position)
        {
            int timingInt = (int)timing;    
            if( (timingInt &= (int)applyTiming)>0 ){

                FModAudioManager.PlayOneShotSFX(PlayEvent, position, PlayEventParam);
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
        ApplyAudioPlays(PlatformApplyTiming.BehaviorStart, affectedPlatform.transform.position);
    }

    public override void BehaviorEnd(PlatformObject changedTarget)
    {
        ApplyAudioPlays(PlatformApplyTiming.BehaviorEnd, changedTarget.transform.position);
    }

    public override void OnObjectPlatformEnter(PlatformObject affectedPlatform, GameObject standingTarget, Rigidbody standingBody, Vector3 standingPoint, Vector3 standingNormal)
    {
        ApplyAudioPlays(PlatformApplyTiming.OnObjectEnter, affectedPlatform.transform.position);
    }

    public override void OnObjectPlatformStay(PlatformObject affectedPlatform, GameObject standingTarget, Rigidbody standingBody, Vector3 standingPoint, Vector3 standingNormal)
    {
       if(UsedStayBehavior) ApplyAudioPlays(PlatformApplyTiming.OnObjectStay, affectedPlatform.transform.position);
    }

    public override void OnObjectPlatformExit(PlatformObject affectedPlatform, GameObject exitTarget, Rigidbody exitBody )
    {
        ApplyAudioPlays(PlatformApplyTiming.OnObjectExit, affectedPlatform.transform.position);
    }



    //=========================================
    /////         Utility Methods         /////
    //=========================================
    private void ApplyAudioPlays(PlatformApplyTiming timing, Vector3 position=default)
    {
        #region Omit
        int Count = Datas.Count;
        for (int i = 0; i < Count; i++)
        {
            Datas[i].ApplyPlayEvent(timing, position);
        }
        #endregion
    }

    public void AddAudioPlayData(FModSFXEventType eventType, PlatformApplyTiming timing)
    {
        EventReference newReference = new EventReference 
        { 
            Guid = FModReferenceList.Events[(int)eventType] 
        };

        Datas.Add(new PlatformAudioPlayData { PlayEvent=newReference, timing=timing });
    }

    public void AddAudioPlayData(FModNoGroupEventType eventType, PlatformApplyTiming timing)
    {
        AddAudioPlayData((FModSFXEventType)eventType, timing);
    }

    public void AddAudioPlayData(FModBGMEventType eventType, PlatformApplyTiming timing)
    {
        AddAudioPlayData((FModBGMEventType)eventType, timing);
    }

    public void RemoveAudioPlayData(FModSFXEventType removeType)
    {
        #region Omit
        FMOD.GUID guid = FModReferenceList.Events[(int)removeType];

        int Count = Datas.Count;
        for(int i=0; i<Count; i++)
        {
            PlatformAudioPlayData data = Datas[i];

            /**동일한 GUID값을 가지는 EventReference를 제거...*/
            if( data.PlayEvent.Guid.Equals(guid) ){

                Datas.RemoveAt(i);
                i--;
                Count--;

            }
        }

        #endregion
    }

}
