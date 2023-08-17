using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using static ParamSwitcherPlatformBehavior;
using UnityEditor;
using UnityEditor.Rendering;
#endif

public sealed class ParamSwitcherPlatformBehavior : PlatformBehaviorBase
{
    #region Editor_Extension
    /****************************************
     *   ParamSwitcherData Drawer
     * **/
#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomPropertyDrawer(typeof(ParamSwitcherData), true)]
    public class ParamSwitcherDataDrawer : PropertyDrawer
    {
        //======================================
        //////      Private Fields           ///
        //======================================
        private SerializedProperty TargetProperty;
        private SerializedProperty ParamNameProperty;
        private SerializedProperty ParamTypeProperty;
        private SerializedProperty ParamApplyTimingProperty;
        private SerializedProperty MaterialIndexProperty;
        private SerializedProperty ParamFloatValueProperty;
        private SerializedProperty ParamIntValueProperty;
        private SerializedProperty ParamTexValueProperty;
        private SerializedProperty ParamColorValueProperty;
        private SerializedProperty ParamLerpTimeProperty;

        private const float DefaultSpace = 95f;


        //======================================
        //////        GUI Methods           ///
        //======================================
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI_IntializedDrawer(property);

            float baseHeight = GetBaseHeight();
            Rect headerRect = position;
            headerRect.y -= (property.isExpanded ? headerRect.height * .5f - 10f : 0f);
            property.isExpanded = EditorGUI.Foldout(headerRect, property.isExpanded, label);

            /****************************************
             *   확장했을 경우...
             * ***/
            if (property.isExpanded){

                headerRect.y += 10f;
                using(var scope = new EditorGUI.IndentLevelScope())
                {
                    float spaceMul = 0f;
                    GUI_TargetField(ref headerRect, ref spaceMul);

                    spaceMul++;
                    GUI_ShowMaterialIndex(ref headerRect, ref spaceMul);

                    spaceMul++;
                    GUI_ShowParamName(ref headerRect, ref spaceMul);

                    spaceMul++;
                    GUI_ShowParamType(ref headerRect, ref spaceMul);

                    spaceMul++;
                    GUI_ShowApplyTiming(ref headerRect, ref spaceMul);

                    spaceMul++;
                    GUI_ShowValueField(ref headerRect, ref spaceMul);

                    scope.Dispose();
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            #region Omit
            GUI_IntializedDrawer(property);

            float baseHeight = GetBaseHeight();
            float height;
            bool isLerp = IsLerpType();

            if (property.isExpanded)
            {
                height = baseHeight * 9.5f;
                if (isLerp) height += baseHeight;
            }
            else
            {
                height = baseHeight;
            }

            return height;
            #endregion
        }



        //===========================================
        //////        GUI Content Methods         ///
        //==========================================
        private void GUI_IntializedDrawer(SerializedProperty property)
        {
            #region Omit
            TargetProperty              = property.FindPropertyRelative("Target");
            MaterialIndexProperty       = property.FindPropertyRelative("MaterialIndex");
            ParamNameProperty           = property.FindPropertyRelative("ParamName");
            ParamTypeProperty           = property.FindPropertyRelative("ParamType");
            ParamApplyTimingProperty    = property.FindPropertyRelative("ParamApplyTiming");
            ParamFloatValueProperty     = property.FindPropertyRelative("ParamFloatValue");
            ParamIntValueProperty       = property.FindPropertyRelative("ParamIntValue");
            ParamTexValueProperty       = property.FindPropertyRelative("ParamTexValue");
            ParamColorValueProperty     = property.FindPropertyRelative("ParamColorValue");
            ParamLerpTimeProperty       = property.FindPropertyRelative("LerpTime");
            #endregion
        }

        private void GUI_TargetField( ref Rect header, ref float spaceMul, float space=22f )
        {
            #region Omit
            if (TargetProperty == null) return;

            Rect targetFieldRect = header;
            targetFieldRect.height = 20f;
            targetFieldRect.y += ( DefaultSpace + space*spaceMul );

            EditorGUI.PropertyField(targetFieldRect, TargetProperty);
            #endregion
        }

        private void GUI_ShowParamName(ref Rect header, ref float spaceMul, float space = 22f)
        {
            #region Omit
            if (ParamNameProperty == null) return;

            Rect rect = header;
            rect.height = 20f;
            rect.y += ( DefaultSpace + space * spaceMul);

            ParamNameProperty.stringValue = EditorGUI.TextField(rect, "Param Name", ParamNameProperty.stringValue);
            #endregion
        }

        private void GUI_ShowMaterialIndex(ref Rect header, ref float spaceMul, float space = 22f)
        {
            #region Omit
            if (MaterialIndexProperty == null) return;

            Rect rect = header;
            rect.height = 20f;
            rect.y += (DefaultSpace + space * spaceMul);

            MaterialIndexProperty.intValue = EditorGUI.IntField(rect, "Material Index", MaterialIndexProperty.intValue);
            #endregion
        }


        private void GUI_ShowParamType(ref Rect header, ref float spaceMul, float space =22f)
        {
            #region Omit
            if (ParamTypeProperty == null) return;

            Rect paramTypeRect = header;
            paramTypeRect.height = 20f;
            paramTypeRect.y += ( DefaultSpace + space*spaceMul);

            ParameterType typeValue = (ParameterType)ParamTypeProperty.intValue;
            System.Enum typeResult = EditorGUI.EnumPopup(paramTypeRect, "ParameterType", typeValue);
            ParamTypeProperty.SetEnumValue(typeResult);

            //타입이 러프타입일 경우...
            if(IsLerpType())
            {
                paramTypeRect.y += space;
                spaceMul++;

                using (var scope = new EditorGUI.IndentLevelScope(1))
                {
                    ParamLerpTimeProperty.floatValue = EditorGUI.FloatField(paramTypeRect, "Lerp Time",ParamLerpTimeProperty.floatValue);
                    scope.Dispose();
                }
            }

            #endregion
        }

        private void GUI_ShowApplyTiming(ref Rect header, ref float spaceMul, float space=22f)
        {
            #region Omit
            if (ParamApplyTimingProperty== null) return;

            Rect rect = header;
            rect.height = 20f;
            rect.y += ( DefaultSpace + space * spaceMul);

            ApplyTiming typeValue = (ApplyTiming)ParamApplyTimingProperty.intValue;
            System.Enum typeResult = EditorGUI.EnumFlagsField(rect, "Apply Timings", typeValue);
            ParamApplyTimingProperty.SetEnumValue(typeResult);
            #endregion
        }

        private void GUI_ShowValueField(ref Rect header, ref float spaceMul, float space=22f)
        {
            #region Omit
            if (ParamFloatValueProperty == null || ParamIntValueProperty == null || ParamTexValueProperty == null)
                return;

            ParameterType type = (ParameterType)ParamTypeProperty.intValue;
            Rect rect = header;
            rect.height = 20f;
            rect.y += ( DefaultSpace + space * spaceMul);

            /*************************************
             *   지정한 타입별로 필드를 표시한다.
             * ***/
            switch(type){

                    case (ParameterType.FLOAT_LERP):
                    case (ParameterType.FLOAT):
                    {
                        ParamFloatValueProperty.floatValue = EditorGUI.FloatField(rect, "Param Float Value", ParamFloatValueProperty.floatValue);
                        break;
                    }

                    case (ParameterType.INT):
                    {
                        ParamIntValueProperty.intValue = EditorGUI.IntField(rect, "Param Int Value", ParamIntValueProperty.intValue);
                        break;
                    }

                    case (ParameterType.TEXTURE):
                    {
                        EditorGUI.PropertyField(rect, ParamTexValueProperty, new GUIContent("Param Texture Value"));
                        break;
                    }

                    case (ParameterType.COLOR_LERP):
                    case (ParameterType.COLOR):
                    {
                        ParamColorValueProperty.colorValue = EditorGUI.ColorField(rect, "Param Color Value", ParamColorValueProperty.colorValue);
                        break;
                    }

            }
            #endregion
        }


        //===========================================
        //////         Utility Methods           ///
        //==========================================
        private bool IsLerpType()
        {
            bool proeprtyIsNull = (ParamTypeProperty==null);
            if (proeprtyIsNull) return false;

            return ((ParameterType)ParamTypeProperty.intValue).ToString().IndexOf("LERP")!=-1;
        }

        private static float GetBaseHeight()
        {
            return GUI.skin.textField.CalcSize(GUIContent.none).y;
        }

    }

#endif
    #endregion

    #region Define
    public enum ParameterType : int
    {
        INT,
        FLOAT,
        FLOAT_LERP,
        TEXTURE,
        COLOR,
        COLOR_LERP
    }

    public enum ApplyTiming : int
    {
        None            = 0,
        BehaviorStart   = 1,
        BehaviorEnd     = 2,
        OnObjectEnter   = 4,
        OnObjectStay    = 8,
        OnObjectExit    = 16,
    }

    [System.Serializable]
    private class ParamSwitcherData
    {
        public GameObject   Target;
        public Material     Material;
        public int          ParamType;
        public int          ParamApplyTiming;
        public int          MaterialIndex;

        public string       ParamName;
        public float        ParamFloatValue;
        public int          ParamIntValue;
        public Texture      ParamTexValue;
        public Color        ParamColorValue;

        public float        LerpTimeDiv;
        public float        LeftTime;
        public float        LerpTime;
        public float        StartValue;
        public Color        StartColorValue;

        public void SwitchParamValue(ref List<ParamSwitcherData> lerpLists,  ref int lerpCount,  ApplyTiming currTiming=ApplyTiming.None)
        {
            #region Omit
            if (currTiming == ApplyTiming.None) return;
            if (ContainTiming(currTiming) == false) return;

            if (Material == null) return;

            switch(ParamType){

                    case ((int)ParameterType.INT):
                    {
                        Material.SetInt(ParamName, ParamIntValue); 
                        break;
                    }

                    case ((int)ParameterType.COLOR_LERP):
                    case ((int)ParameterType.FLOAT_LERP):
                    {
                        //중복 파라미터를 제어하는 러프를 제거한다.
                        for(int i=0; i<lerpCount; i++)
                        {
                            ParamSwitcherData data = lerpLists[i];
                            if(data.ParamName==ParamName)
                            {
                                lerpLists.RemoveAt(i);
                                lerpCount--;
                                i--;
                            }

                        }

                        if(!lerpLists.Contains(this)){

                            lerpLists.Add(this);
                            lerpCount++;
                        }

                        LeftTime = LerpTime;
                        LerpTimeDiv = ( 1f / LerpTime );

                        //타입에 알맞는 시작값을 가져온다...
                        if(ParamType==(int)ParameterType.FLOAT_LERP) StartValue = Material.GetFloat(ParamName);
                        if(ParamType == (int)ParameterType.COLOR_LERP) StartColorValue = Material.GetColor(ParamName);
                        break;
                    }

                    case ((int)ParameterType.FLOAT):
                    {
                        Material.SetFloat(ParamName, ParamFloatValue);
                        break;
                    }

                    case ((int)ParameterType.COLOR):
                    {
                        Material.SetColor(ParamName, ParamColorValue);
                        break;
                    }


                    case ((int)ParameterType.TEXTURE):
                    {
                        Material.SetTexture(ParamName, ParamTexValue);
                        break;
                    }

            }
            #endregion
        }

        public bool ApplyLerpProgress(ref List<ParamSwitcherData> lerpLists, ref int lerpCount)
        {
            #region
            LeftTime -= Time.fixedDeltaTime;
            float progressRatio = Mathf.Clamp((1f - LeftTime * LerpTimeDiv), 0f, 1f);

            //러프 타입에 따라서 별도로 처리...
            switch(ParamType){

                    case ((int)ParameterType.FLOAT_LERP):
                    {
                        float distance = (ParamFloatValue - StartValue);
                        Material.SetFloat(ParamName, StartValue + distance * progressRatio);
                        break;
                    }

                    case ((int)ParameterType.COLOR_LERP):
                    {
                        Color distance = (ParamColorValue - StartColorValue);
                        Material.SetColor(ParamName, StartColorValue + distance * progressRatio);
                        break;
                    }
            }

            //러프 마무리...
            if (progressRatio >= 1f)
            {
                lerpLists.Remove(this);
                lerpCount--;
                return true;
            }

            return false;
            #endregion
        }

        public bool ContainTiming(ApplyTiming checkTiming)
        {
            return (ParamApplyTiming & (int)checkTiming) > 0;
        }
    }
    #endregion

    //======================================
    //////          Property           /////
    //======================================
    [SerializeField] private List<ParamSwitcherData> Infos = new List<ParamSwitcherData>();

    private List<ParamSwitcherData> _LerpTargets = new List<ParamSwitcherData>();
    private int _LerpCount = 0;


    //============================================
    //////       Override Methods            /////
    //============================================
    public override void BehaviorStart(PlatformObject affectedPlatform)
    {
        #region Omit
        /**************************************
         *   값을 바꿀 모든 머터리얼을 구한다.
         * ***/
        int Count = Infos.Count;
        for(int i=0; i<Count; i++)
        {
            ParamSwitcherData data= Infos[i];
            Renderer renderer = data.Target?.GetComponent<Renderer>();

            if (renderer == null)
            {
                //의미없는 값은 제거한다.
                Infos.RemoveAt(i);
                i--;
                Count--;
                continue;
            }

            bool indexIsValid = ( data.MaterialIndex < renderer.materials.Length ) && ( data.MaterialIndex >= 0 );
            if(indexIsValid)
            {
                //최종 적용.
                data.Material = renderer.materials[data.MaterialIndex];
                data.SwitchParamValue(ref _LerpTargets, ref _LerpCount, ApplyTiming.BehaviorStart);
            }
            else {

                //의미없는 값은 제거한다.
                Infos.RemoveAt(i);
                i--;
                Count--;
            }
        }
        #endregion
    }

    public override void BehaviorEnd(PlatformObject changedTarget)
    {
        ApplyParamSwitch(ApplyTiming.BehaviorEnd);
    }

    public override void PhysicsUpdate(PlatformObject affectedPlatform)
    {
        if (_LerpCount <= 0) return;

        for(int i=0; i<_LerpCount; i++)
        {
            //Lerp가 종료되었을 경우...
            if(_LerpTargets[i].ApplyLerpProgress(ref _LerpTargets, ref _LerpCount ))
            {
                i--;
            }
        }
    }

    public override void OnObjectPlatformEnter(PlatformObject affectedPlatform, GameObject standingTarget, Vector3 standingPoint, Vector3 standingNormal)
    {
        ApplyParamSwitch(ApplyTiming.OnObjectEnter);
    }

    public override void OnObjectPlatformExit(PlatformObject affectedPlatform, GameObject exitTarget)
    {
        ApplyParamSwitch(ApplyTiming.OnObjectExit);
    }

    public override void OnObjectPlatformStay(PlatformObject affectedPlatform, GameObject standingTarget, Vector3 standingPoint, Vector3 standingNormal)
    {
        ApplyParamSwitch(ApplyTiming.OnObjectStay); 
    }


    //============================================
    //////         Utility Methods            /////
    //============================================
    private void ApplyParamSwitch(ApplyTiming timing)
    {
        int Count= Infos.Count;
        for(int i=0; i<Count; i++)
        {
            Infos[i].SwitchParamValue(ref _LerpTargets, ref _LerpCount, timing);
        }
    }


}
