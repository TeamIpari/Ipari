using UnityEngine;

public enum FModBusType
{
    Master=0,
   Boss=1,
   Environment=2,
   Item=3,
   BGM=4,
   Player=5,
   SFX=6,
   UI=7
}

public enum FModBankType
{
   Master_strings=0,
   BGM=1,
   Master=2,
   SFX=3
}

public enum FModGlobalParamType
{
   None_Parameter =-1,
   BGMIsLooping=0,
}

public enum FModLocalParamType
{
   None_Parameter =-1,
   BrokenType=1,
   LandedType=2,
   EnterWaterType=3,
   PlayerHitType=4,
   PlayerWalkType=5,
   TreeActionType=6
}

public struct FModParamLabel
{
    public struct BGMIsLooping
    {
       public const float Used  =0f;
       public const float UnUsed  =1f;
    }
    public struct BrokenType
    {
       public const float Wall  =0f;
       public const float Stone  =1f;
    }
    public struct LandedType
    {
       public const float Grass  =0f;
       public const float Ground  =1f;
    }
    public struct EnterWaterType
    {
       public const float Default  =0f;
       public const float Stone  =1f;
    }
    public struct PlayerHitType
    {
       public const float MiniNepenthes_Attack  =0f;
    }
    public struct PlayerWalkType
    {
       public const float Grass  =0f;
    }
    public struct TreeActionType
    {
       public const float TreeFallDown  =0f;
       public const float TreeCrash  =1f;
    }
}

public struct FModParamValueRange
{
    public struct BGMIsLooping
    {
       public const float Min=0;
       public const float Max=1;
    }
    public struct BrokenType
    {
       public const float Min=0;
       public const float Max=1;
    }
    public struct LandedType
    {
       public const float Min=0;
       public const float Max=1;
    }
    public struct EnterWaterType
    {
       public const float Min=0;
       public const float Max=1;
    }
    public struct PlayerHitType
    {
       public const float Min=0;
       public const float Max=0;
    }
    public struct PlayerWalkType
    {
       public const float Min=0;
       public const float Max=0;
    }
    public struct TreeActionType
    {
       public const float Min=0;
       public const float Max=1;
    }
}

public enum FModBGMEventType
{
   tavuti_ingame2=0,
   test=1,
   tavuti_ingame1=2,
   Wagtail_bgm_title=3,
   test2=4
}

public enum FModSFXEventType
{
   Tree_Obstacle=5,
   Water_Stream=6,
   Mushroom_Jump=7,
   Enter_Water=8,
   Collision_Ground=9,
   Interacting_Vine=10,
   Broken=11,
   GameOver=12,
   Death_CutScene=13,
   UI_Button=14,
   Nepenthes_Shoot=15,
   Nepenthes_Dead=16,
   Get_Flower=17,
   Get_Bead=18,
   Put_KoKoShi=19,
   Flowers_Burst=20,
   Player_Walk=21,
   Player_Hit=22,
   Player_Jump=23,
   Player_Landed=24
}

public enum FModNoGroupEventType
{
}

public sealed class FModReferenceList
{
    public static readonly FMOD.GUID[] Events = new FMOD.GUID[]
    {
        new FMOD.GUID{ Data1=1172652772, Data2=1096849244, Data3=1626328197, Data4=1334314980 },
        new FMOD.GUID{ Data1=-1059833457, Data2=1253542699, Data3=-338358873, Data4=775534634 },
        new FMOD.GUID{ Data1=948676747, Data2=1227712620, Data3=-1017469557, Data4=-1232547804 },
        new FMOD.GUID{ Data1=942327427, Data2=1305162648, Data3=144500900, Data4=-1687243536 },
        new FMOD.GUID{ Data1=-839459200, Data2=1158110859, Data3=-183060576, Data4=1601068035 },
        new FMOD.GUID{ Data1=521967865, Data2=1095037392, Data3=-2000264523, Data4=1766900301 },
        new FMOD.GUID{ Data1=1783667186, Data2=1173746128, Data3=1226250647, Data4=1383568854 },
        new FMOD.GUID{ Data1=524088536, Data2=1209241081, Data3=979799986, Data4=1160229082 },
        new FMOD.GUID{ Data1=213948051, Data2=1293050876, Data3=-1702922857, Data4=1738409538 },
        new FMOD.GUID{ Data1=-1866121864, Data2=1339704136, Data3=1908183699, Data4=-1513147371 },
        new FMOD.GUID{ Data1=-990469295, Data2=1254419191, Data3=1519357069, Data4=-1122432260 },
        new FMOD.GUID{ Data1=2066250762, Data2=1228276510, Data3=2128100538, Data4=1940932632 },
        new FMOD.GUID{ Data1=106925798, Data2=1271508359, Data3=327143578, Data4=974880928 },
        new FMOD.GUID{ Data1=1315365849, Data2=1266621721, Data3=-1860747614, Data4=1802208756 },
        new FMOD.GUID{ Data1=-738566357, Data2=1166266538, Data3=1971146173, Data4=-1481470806 },
        new FMOD.GUID{ Data1=888998830, Data2=1291452499, Data3=905047692, Data4=27400666 },
        new FMOD.GUID{ Data1=1372834114, Data2=1135245776, Data3=-1359107406, Data4=-1143480114 },
        new FMOD.GUID{ Data1=-1665399871, Data2=1278575409, Data3=-1760948335, Data4=-1373955044 },
        new FMOD.GUID{ Data1=1596271017, Data2=1176077993, Data3=693188530, Data4=1557820508 },
        new FMOD.GUID{ Data1=52087645, Data2=1090531362, Data3=-178578771, Data4=1052847077 },
        new FMOD.GUID{ Data1=-830567866, Data2=1247463816, Data3=388917127, Data4=-408534711 },
        new FMOD.GUID{ Data1=859597537, Data2=1187071565, Data3=907815588, Data4=-766949796 },
        new FMOD.GUID{ Data1=-356480811, Data2=1261563164, Data3=-1823479120, Data4=-733505655 },
        new FMOD.GUID{ Data1=61617350, Data2=1314310624, Data3=-766163584, Data4=319510773 },
        new FMOD.GUID{ Data1=594495364, Data2=1183593499, Data3=849544592, Data4=732205388 },
    };

    public static readonly string[] Banks = new string[]
    {
        "Master.strings",
        "BGM",
        "Master",
        "SFX"
    };

    public static readonly string[] Params = new string[]
    {
        "BGMIsLooping",
        "BrokenType",
        "LandedType",
        "EnterWaterType",
        "PlayerHitType",
        "PlayerWalkType",
        "TreeActionType"
    };

    public static readonly string[] BusPaths = new string[]
    {
        "bus:/",
        "bus:/SFX/Boss",
        "bus:/SFX/Environment",
        "bus:/SFX/Item",
        "bus:/BGM",
        "bus:/SFX/Player",
        "bus:/SFX",
        "bus:/SFX/UI"
    };

}

