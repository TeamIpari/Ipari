using UnityEngine;

public enum FModBusType
{
   MasterBus = 0,
   BGMBus = 1,
   SFXBus = 2
}

public sealed class FModBusPath
{
   public static readonly string MasterBus="bus:/";
   public static readonly string BGMBus="bus:/BGM";
   public static readonly string SFXBus="bus:/SFX";
}

public enum FModBGMEventType
{
   Tavuti_ingame2 = 0,
   Tavuti_ingame1 = 1,
   Wagtail_bgm_title = 2
}

public enum FModSFXEventType
{
   Water_Stream = 3,
   Mushroom_Jump = 4,
   Landed_Grass = 5,
   Enter_Water = 6,
   Collision_Ground = 7,
   Stone_Broken = 8,
   Interacting_Vine = 9,
   Landed_Ground = 10,
   Put_KoKoShi = 11,
   Flowers_Burst = 12,
   Get_Flower = 13,
   Get_Bead = 14,
   Player_Jump = 15,
   GameOver = 16,
   Death_CutScene = 17
}

public enum FModNoGroupEventType
{
   No_14 = 18
}

public sealed class FModEventReferenceList
{
    public static readonly FMOD.GUID[] Events = new FMOD.GUID[]
    {
        new FMOD.GUID{ Data1=1172652772, Data2=1096849244, Data3=1626328197, Data4=1334314980 },
        new FMOD.GUID{ Data1=948676747, Data2=1227712620, Data3=-1017469557, Data4=-1232547804 },
        new FMOD.GUID{ Data1=942327427, Data2=1305162648, Data3=144500900, Data4=-1687243536 },
        new FMOD.GUID{ Data1=1783667186, Data2=1173746128, Data3=1226250647, Data4=1383568854 },
        new FMOD.GUID{ Data1=524088536, Data2=1209241081, Data3=979799986, Data4=1160229082 },
        new FMOD.GUID{ Data1=-574186547, Data2=1074710698, Data3=218973597, Data4=499390499 },
        new FMOD.GUID{ Data1=213948051, Data2=1293050876, Data3=-1702922857, Data4=1738409538 },
        new FMOD.GUID{ Data1=-1866121864, Data2=1339704136, Data3=1908183699, Data4=-1513147371 },
        new FMOD.GUID{ Data1=-914784664, Data2=1222737742, Data3=-1850851651, Data4=1549350424 },
        new FMOD.GUID{ Data1=-990469295, Data2=1254419191, Data3=1519357069, Data4=-1122432260 },
        new FMOD.GUID{ Data1=-1304464854, Data2=1121479672, Data3=-1546562640, Data4=-816232933 },
        new FMOD.GUID{ Data1=52087645, Data2=1090531362, Data3=-178578771, Data4=1052847077 },
        new FMOD.GUID{ Data1=-830567866, Data2=1247463816, Data3=388917127, Data4=-408534711 },
        new FMOD.GUID{ Data1=-1665399871, Data2=1278575409, Data3=-1760948335, Data4=-1373955044 },
        new FMOD.GUID{ Data1=1596271017, Data2=1176077993, Data3=693188530, Data4=1557820508 },
        new FMOD.GUID{ Data1=61617350, Data2=1314310624, Data3=-766163584, Data4=319510773 },
        new FMOD.GUID{ Data1=106925798, Data2=1271508359, Data3=327143578, Data4=974880928 },
        new FMOD.GUID{ Data1=1315365849, Data2=1266621721, Data3=-1860747614, Data4=1802208756 },
        new FMOD.GUID{ Data1=-738566357, Data2=1166266538, Data3=1971146173, Data4=-1481470806 }
    };
}

