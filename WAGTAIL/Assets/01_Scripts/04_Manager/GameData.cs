using System;

[Serializable]

public class GameData
{
    // ภ๚ภๅวา Property
    public float masterVolume;
    public float bgmVolume;
    public float sfxVolume;
    
    public Chapter currentChapterType;
    public int coin;
    public bool[] cocosi;
    
    public void Init()
    {
        masterVolume = FModAudioManager.GetBusVolume(FModBusType.Master);
        bgmVolume = FModAudioManager.GetBusVolume(FModBusType.BGM);
        sfxVolume = FModAudioManager.GetBusVolume(FModBusType.SFX);

        currentChapterType = GameManager.GetInstance().LastActiveChapter;
        cocosi = GameManager.GetInstance().cocosi;
        coin = GameManager.GetInstance().PrevCoin;
    }
    
    public void Load()
    {
        FModAudioManager.SetBusVolume(FModBusType.Master, masterVolume);
        FModAudioManager.SetBusVolume(FModBusType.BGM, bgmVolume);
        FModAudioManager.SetBusVolume(FModBusType.SFX, sfxVolume);

        GameManager.GetInstance().LastActiveChapter = currentChapterType;
        GameManager.GetInstance().cocosi = cocosi;
    }

    public void Reset()
    {
        masterVolume = 1f;
        bgmVolume = 1f;
        sfxVolume = 1f;
        
        currentChapterType = GameManager.GetInstance().LastActiveChapter;
        cocosi = GameManager.GetInstance().cocosi;
        coin = GameManager.GetInstance().PrevCoin;
    }
}
