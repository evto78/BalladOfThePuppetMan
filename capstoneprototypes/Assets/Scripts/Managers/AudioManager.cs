using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public class SFXSource
    {
        public AudioSource source;
        public SFXObject sfxObj;
    }
    public enum SFXType { Music, Effect, UI }

    public float masterVol;
    public float musicVol;
    public float effectVol;
    public float uiVol;

    int masterVolSlider = 2;
    int musicVolSlider = 10;
    int effectVolSlider = 10;
    int uiVolSlider = 15;

    public Image[] masterVolImage;
    public Image[] musicVolImage;
    public Image[] effectVolImage;
    public Image[] uiVolImage;

    public GameObject audioSourcePrefab;
    public Transform unactiveGroup;
    public Transform activeGroup;

    public List<SFXSource> unactiveSources;
    public List<SFXSource> activeSources;
    SFXSource musicSource;
    List<List<SFXObject>> sfxData; //0 is effect list, 1 is music list, 2 is UI list.
    List<SFXObject> effectAudioData;
    List<SFXObject> musicAudioData;
    List<SFXObject> uiAudioData;

    private void Awake()
    {
        unactiveSources = new List<SFXSource>();
        activeSources = new List<SFXSource>();

        sfxData = new List<List<SFXObject>>();
        effectAudioData = new List<SFXObject>();
        musicAudioData = new List<SFXObject>();
        uiAudioData = new List<SFXObject>();

        sfxData.Add(effectAudioData);
        sfxData.Add(musicAudioData);
        sfxData.Add(uiAudioData);

        effectAudioData.AddRange(Resources.LoadAll<SFXObject>("Audio/Effects"));
        musicAudioData.AddRange(Resources.LoadAll<SFXObject>("Audio/Music"));
        uiAudioData.AddRange(Resources.LoadAll<SFXObject>("Audio/UI"));

        effectAudioData = SortSFXObjectData(effectAudioData);
        musicAudioData = SortSFXObjectData(musicAudioData);
        uiAudioData = SortSFXObjectData(uiAudioData);

        for (int i = 0; i < 50; i++)
        {
            unactiveSources.Add(CreateNewSource());
        }

        //Make the music source
        musicSource = new SFXSource();
        AudioSource source = Instantiate(audioSourcePrefab, transform).GetComponent<AudioSource>();
        source.gameObject.name = "SFXSource-MUSIC";
        musicSource.source = source;
    }
    List<SFXObject> SortSFXObjectData(List<SFXObject> inputData)
    {
        List<int> comparisonList = new List<int>();
        List<SFXObject> sortedItemData = new List<SFXObject>();
        for (int i = 0; i < inputData.Count; i++) { comparisonList.Add(i); sortedItemData.Add(null); }
        for (int i = 0; i < inputData.Count; i++)
        {
            sortedItemData[comparisonList.IndexOf(inputData[i].id)] = inputData[i];
        }
        return sortedItemData;
    }
    List<SFXSource> deleteList;

    private void Start()
    {
        masterVolSlider = PlayerPrefs.GetInt("MasterVolume", 20);
        musicVolSlider = PlayerPrefs.GetInt("MusicVolume", 20);
        effectVolSlider = PlayerPrefs.GetInt("EffectVolume", 20);
        uiVolSlider = PlayerPrefs.GetInt("UIVolume", 20);

        masterVol = masterVolSlider / 20f;
        musicVol = musicVolSlider / 20f;
        effectVol = effectVolSlider / 20f;
        uiVol = uiVolSlider / 20f;

        UpdateVolumeImages();
    }

    private void Update()
    {
        //Delete all sound effects that are not currently playing.
        deleteList = new List<SFXSource>();
        foreach (SFXSource sfxs in activeSources)
        {
            if (!sfxs.source.isPlaying) { deleteList.Add(sfxs); }
        }
        foreach (SFXSource sfxs in deleteList)
        {
            StopSound(sfxs);
        }
    }
    //Instantiates, Sets up, Plays, and Returns the requested sound effect.
    //If any sound effects are to be ended early, it is up to the creator to end that sound effect. That is why it is returned.
    //Otherwise, all sound effects are deleted when they are done playing.
    public SFXSource PlaySound(int clipID, SFXType sfxType, float vol, float pitch)
    {
        SFXObject sfxObj = null;
        SFXSource sfxSource;
        
        if (unactiveSources.Count == 0) 
        {
            unactiveSources.Add(CreateNewSource());
        }

        sfxSource = unactiveSources[0];
        sfxSource.source.transform.parent = activeGroup;
        unactiveSources.Remove(sfxSource);

        float volMod = 1f;

        switch (sfxType)
        {
            case SFXType.Effect: sfxObj = sfxData[0][clipID]; volMod = effectVol; break;
            case SFXType.Music: sfxObj = sfxData[1][clipID]; volMod = musicVol; break;
            case SFXType.UI: sfxObj = sfxData[2][clipID]; volMod = uiVol; break;
        }

        sfxSource.source.volume = vol; sfxSource.source.pitch = pitch;
        sfxSource.source.volume *= volMod; sfxSource.source.volume *= masterVol;
        sfxSource.source.clip = sfxObj.clip;

        sfxSource.source.Play();

        sfxSource.sfxObj = sfxObj;
        activeSources.Add(sfxSource);
        return sfxSource;
    }
    public void StopSound(SFXSource sfxSource)
    {
        sfxSource.source.Stop();
        activeSources.Remove(sfxSource);
        unactiveSources.Add(sfxSource);
        sfxSource.source.transform.parent = unactiveGroup;
    }
    SFXSource CreateNewSource()
    {
        SFXSource newSFX = new SFXSource();
        //All audio sources are created as a child of this gameobject, for organizational purposes!
        AudioSource source = Instantiate(audioSourcePrefab, unactiveGroup).GetComponent<AudioSource>();
        source.gameObject.name = "SFXSource-" + (unactiveSources.Count + activeSources.Count);
        newSFX.source = source;

        return newSFX;
    }
    public void PlayMusic(int clipID, float vol, float pitch)
    {
        SFXObject sfxObj = sfxData[1][clipID];
        float volMod = musicVol;

        musicSource.source.volume = vol; musicSource.source.pitch = pitch;
        musicSource.source.volume *= volMod; musicSource.source.volume *= masterVol;
        musicSource.source.clip = sfxObj.clip;

        musicSource.source.Play();

        musicSource.sfxObj = sfxObj;
    }
    public void StopMusic()
    {
        musicSource.source.Stop();
    }

    public void ChangeMasterVolume(int valueChange)
    {
        masterVolSlider += valueChange;
        masterVolSlider = Mathf.Clamp(masterVolSlider, 0, 20);
        masterVol = masterVolSlider / 20f;

        PlayerPrefs.SetInt("MasterVolume", masterVolSlider);
        PlayerPrefs.Save();

        UpdateVolumeImages();

    }

    public void ChangeMusicVolume(int valueChange)
    {
        musicVolSlider += valueChange;
        musicVolSlider = Mathf.Clamp(musicVolSlider, 0, 20);
        musicVol = musicVolSlider / 20f;

        PlayerPrefs.SetInt("MusicVolume", musicVolSlider);
        PlayerPrefs.Save();

        UpdateVolumeImages();

    }

    public void ChangeEffectVolume(int valueChange)
    {
        effectVolSlider += valueChange;
        effectVolSlider = Mathf.Clamp(effectVolSlider, 0, 20);
        effectVol = effectVolSlider / 20f;

        PlayerPrefs.SetInt("EffectVolume", effectVolSlider);
        PlayerPrefs.Save();

        UpdateVolumeImages();

    }

    public void ChangeUIVolume(int valueChange)
    {
        uiVolSlider += valueChange;
        uiVolSlider = Mathf.Clamp(uiVolSlider, 0, 20);
        uiVol = uiVolSlider / 20f;

        PlayerPrefs.SetInt("UIVolume", uiVolSlider);
        PlayerPrefs.Save();

        UpdateVolumeImages();

    }

    void UpdateVolumeImages()
    {
        for (int i = 0; i < masterVolImage.Length; i++)
        {
            if (masterVolSlider >= i + 1)
            {
                masterVolImage[i].color = Color.white;
            }
            else
            {
                masterVolImage[i].color = Color.grey;
            }
        }

        for (int i = 0; i < musicVolImage.Length; i++)
        {
            if (musicVolSlider >= i + 1)
            {
                musicVolImage[i].color = Color.white;
            }
            else
            {
                musicVolImage[i].color = Color.grey;
            }
        }

        for (int i = 0; i < effectVolImage.Length; i++)
        {
            if (effectVolSlider >= i + 1)
            {
                effectVolImage[i].color = Color.white;
            }
            else
            {
                effectVolImage[i].color = Color.grey;
            }
        }

        for (int i = 0; i < uiVolImage.Length; i++)
        {
            if (uiVolSlider >= i + 1)
            {
                uiVolImage[i].color = Color.white;
            }
            else
            {
                uiVolImage[i].color = Color.grey;
            }
        }
    }
}
