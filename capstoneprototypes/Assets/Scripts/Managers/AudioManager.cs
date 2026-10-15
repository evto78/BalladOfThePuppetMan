using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public GameObject audioSourcePrefab;
    public Transform unactiveGroup;
    public Transform activeGroup;

    public List<SFXSource> unactiveSources;
    public List<SFXSource> activeSources;
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

        sfxSource.source.volume *= volMod; sfxSource.source.volume *= masterVol;
        sfxSource.source.pitch *= pitch;
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
}
