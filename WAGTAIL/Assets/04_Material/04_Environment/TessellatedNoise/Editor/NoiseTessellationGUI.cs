using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class NoiseTessellationGUI : ShaderGUI
{
        //_Tess("Tessellation", Range(1, 32)) = 20
		//_MaxTessDistance("Max Tess Distance", float) = 20
		//_FirstNoise("FirstNoise", 2D) = "gray" {}
		//_SecondNoise("SecondNoise",2D) = "gray" {}
		//_Weight("Displacement Amount", float) = 0
		//[HDR] _ColorHigh("High Color",Color) = (1,1,1,1)
		//[HDR] _ColorLow("Low Color",Color) = (0,0,0,0)
		//_XScroll("X Scroll Speed",float) = 0		
		//_YScroll("Y Scroll Speed",float) = 0

    class Styles
    {
        public static GUIContent TessText = new GUIContent("�׼����̼� �ػ�");
        public static GUIContent MaxTessDistanceText = new GUIContent("�׼����̼� �Ÿ�");
        public static GUIContent FirstNoiseText = new GUIContent("������ 1");
        public static GUIContent SecondNoiseText = new GUIContent("������ 2");
        public static GUIContent DisplaceAmountText = new GUIContent("����");
        public static GUIContent ColorHighText = new GUIContent("���� ���� ����");
        public static GUIContent ColorLowText = new GUIContent("��ο� ���� ����");
        public static GUIContent XScrollSpeedText = new GUIContent("X ��ũ�� �ӵ�");
        public static GUIContent YScrollSpeedText = new GUIContent("Y ��ũ�� �ӵ�");
    }

    MaterialProperty tess = null;
    MaterialProperty maxTessDistance = null;
    MaterialProperty firstNoise = null;
    MaterialProperty secondNoise = null;
    MaterialProperty weight = null;
    MaterialProperty colorHigh = null;
    MaterialProperty colorLow = null;
    MaterialProperty Xscroll = null;
    MaterialProperty Yscroll = null;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
    {
        tess = FindProperty("_Tess", props);
        maxTessDistance = FindProperty("_MaxTessDistance", props);
        firstNoise = FindProperty("_FirstNoise", props);
        secondNoise = FindProperty("_SecondNoise", props);
        weight = FindProperty("_Weight", props);
        colorHigh = FindProperty("_ColorHigh", props);
        colorLow = FindProperty("_ColorLow", props);
        Xscroll = FindProperty("_XScroll", props);
        Yscroll = FindProperty("_YScroll", props);

        materialEditor.ShaderProperty(tess, Styles.TessText);
        materialEditor.ShaderProperty(maxTessDistance, Styles.MaxTessDistanceText);
        materialEditor.ShaderProperty(firstNoise, Styles.FirstNoiseText);
        materialEditor.ShaderProperty(secondNoise, Styles.SecondNoiseText);
        materialEditor.ShaderProperty(weight, Styles.DisplaceAmountText);
        materialEditor.ShaderProperty(colorHigh, Styles.ColorHighText);
        materialEditor.ShaderProperty(colorLow, Styles.ColorLowText);
        materialEditor.ShaderProperty(Xscroll, Styles.XScrollSpeedText);
        materialEditor.ShaderProperty(Yscroll, Styles.YScrollSpeedText);

    }
}