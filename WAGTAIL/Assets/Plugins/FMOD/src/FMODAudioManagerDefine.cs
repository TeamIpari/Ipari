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
   BGMIsLooping=6
}

public enum FModLocalParamType
{
   None_Parameter =-1,
   BrokenType=0,
   LandedType=1,
   EnterWaterType=2,
   PlayerHitType=3,
   PlayerWalkType=4,
   TreeActionType=5,
}

public struct FModParamLabel
{
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
       public const float Sand  =1f;
       public const float Stone  =2f;
    }
    public struct TreeActionType
    {
       public const float TreeFallDown  =0f;
       public const float TreeCrash  =1f;
    }
    public struct BGMIsLooping
    {
       public const float Used  =0f;
       public const float UnUsed  =1f;
    }
}

public struct FModParamValueRange
{
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
       public const float Max=2;
    }
    public struct TreeActionType
    {
       public const float Min=0;
       public const float Max=1;
    }
    public struct BGMIsLooping
    {
       public const float Min=0;
       public const float Max=1;
    }
}

public enum FModBGMEventType
{
   CrabBossBGM=35,
   NepenthesBossBGM=36,
   tavuti_ingame2=37,
   Chapter4BGM=38,
   test=39,
   tavuti_ingame1=40,
   Wagtail_bgm_title=41,
   test2=42,
   Chapter5BGM=43,
   NepenthesRoad=44
}

public enum FModSFXEventType
{
   Tree_Obstacle=0,
   Water_Stream=1,
   Mushroom_Jump=2,
   Enter_Water=3,
   Collision_Ground=4,
   Interacting_Vine=5,
   Broken=6,
   GameOver=7,
   Death_CutScene=8,
   UI_Button=9,
   Nepenthes_Shoot=10,
   Nepenthes_Dead=11,
   Get_MiniCocoshi=12,
   Get_Flower=13,
   Get_Bead=14,
   Put_KoKoShi=15,
   Flowers_Burst=16,
   Player_Dead=17,
   Player_Walk=18,
   Player_Hit=19,
   Player_Jump=20,
   Player_Landed=21,
   BossNepen_VineSmash=22,
   BossNepen_Roar=23,
   BossNepen_AcidBoom=24,
   BossNepen_BombBurst=25,
   BossNepen_Dead=26,
   Crab_SeedSpitOut=27,
   Crab_Roar=28,
   Crab_Smash=29,
   Crab_ChangeSand=30,
   Crab_BoomBurst=31,
   Crab_SandWave=32,
   Crab_Dead=33,
   Crab_Atk3Smash=34
}

public enum FModNoGroupEventType
{
}

public sealed class FModReferenceList
{
    public static readonly FMOD.GUID[] Events = new FMOD.GUID[]
    {
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
        new FMOD.GUID{ Data1=919690056, Data2=1177179559, Data3=1080854676, Data4=-114589226 },
        new FMOD.GUID{ Data1=-1665399871, Data2=1278575409, Data3=-1760948335, Data4=-1373955044 },
        new FMOD.GUID{ Data1=1596271017, Data2=1176077993, Data3=693188530, Data4=1557820508 },
        new FMOD.GUID{ Data1=52087645, Data2=1090531362, Data3=-178578771, Data4=1052847077 },
        new FMOD.GUID{ Data1=-830567866, Data2=1247463816, Data3=388917127, Data4=-408534711 },
        new FMOD.GUID{ Data1=-779904658, Data2=1190687713, Data3=-224237938, Data4=809765307 },
        new FMOD.GUID{ Data1=859597537, Data2=1187071565, Data3=907815588, Data4=-766949796 },
        new FMOD.GUID{ Data1=-356480811, Data2=1261563164, Data3=-1823479120, Data4=-733505655 },
        new FMOD.GUID{ Data1=61617350, Data2=1314310624, Data3=-766163584, Data4=319510773 },
        new FMOD.GUID{ Data1=594495364, Data2=1183593499, Data3=849544592, Data4=732205388 },
        new FMOD.GUID{ Data1=2106052319, Data2=1313870343, Data3=385983629, Data4=-1703552368 },
        new FMOD.GUID{ Data1=-67553682, Data2=1132054405, Data3=173544103, Data4=1607568598 },
        new FMOD.GUID{ Data1=-553868207, Data2=1297252664, Data3=-1139210082, Data4=-848624198 },
        new FMOD.GUID{ Data1=465530651, Data2=1104226768, Data3=-1558813780, Data4=1512024309 },
        new FMOD.GUID{ Data1=-1513481970, Data2=1302165862, Data3=377981080, Data4=995136610 },
        new FMOD.GUID{ Data1=840049676, Data2=1183846134, Data3=-1624760407, Data4=-325378633 },
        new FMOD.GUID{ Data1=-812657708, Data2=1301469079, Data3=-1459568511, Data4=-277263995 },
        new FMOD.GUID{ Data1=619787729, Data2=1158591913, Data3=-1318111358, Data4=1640434821 },
        new FMOD.GUID{ Data1=-1418326382, Data2=1278715507, Data3=-1339833168, Data4=-215567928 },
        new FMOD.GUID{ Data1=-724136592, Data2=1309640141, Data3=949501828, Data4=-581693676 },
        new FMOD.GUID{ Data1=1786194984, Data2=1114577088, Data3=-1916655965, Data4=1681444202 },
        new FMOD.GUID{ Data1=1567279138, Data2=1111245456, Data3=-400636011, Data4=191528311 },
        new FMOD.GUID{ Data1=-1272688862, Data2=1140839663, Data3=1957934753, Data4=-1357273977 },
        new FMOD.GUID{ Data1=2087680138, Data2=1137614030, Data3=-974458185, Data4=-1355772786 },
        new FMOD.GUID{ Data1=1035370724, Data2=1231878928, Data3=1320145589, Data4=790482389 },
        new FMOD.GUID{ Data1=1172652772, Data2=1096849244, Data3=1626328197, Data4=1334314980 },
        new FMOD.GUID{ Data1=59827926, Data2=1289788371, Data3=1359709847, Data4=-804997082 },
        new FMOD.GUID{ Data1=-1059833457, Data2=1253542699, Data3=-338358873, Data4=775534634 },
        new FMOD.GUID{ Data1=948676747, Data2=1227712620, Data3=-1017469557, Data4=-1232547804 },
        new FMOD.GUID{ Data1=942327427, Data2=1305162648, Data3=144500900, Data4=-1687243536 },
        new FMOD.GUID{ Data1=-839459200, Data2=1158110859, Data3=-183060576, Data4=1601068035 },
        new FMOD.GUID{ Data1=718605923, Data2=1331647710, Data3=838851234, Data4=-72285666 },
        new FMOD.GUID{ Data1=-1867582431, Data2=1121209320, Data3=-1608370768, Data4=-1609358906 },
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
        "BrokenType",
        "LandedType",
        "EnterWaterType",
        "PlayerHitType",
        "PlayerWalkType",
        "TreeActionType",
        "BGMIsLooping"
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

