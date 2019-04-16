using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Speaker : MonoBehaviour
{
    private AudioSource _speaker;
    private float[] _newData = new float[LibretroWrapper.Wrapper.AudioBatchSize];

    private void Start()
    {
        _speaker = GetComponent<AudioSource>();
        if (_speaker == null) return;
        AudioClip clip = AudioClip.Create("Libretro", LibretroWrapper.Wrapper.AudioBatchSize / 2, 2, 44100, true,
            OnAudioRead);
        _speaker.clip = clip;
        _speaker.Play();
        Debug.Log("Unity sample rate: " + AudioSettings.outputSampleRate);
    }

    /// <summary>
    /// Sets the new audio data to be played.
    /// </summary>
    /// <param name="sampleData">The sample data of the new audio.</param>
    public void UpdateAudio(float[] sampleData)
    {
        _newData = sampleData;
        _speaker.PlayOneShot(_speaker.clip);
    }

    /// <summary>
    /// This gets called whenever audio is read.
    /// </summary>
    /// <param name="sampleData">Sample data of the current audio.</param>
    private void OnAudioRead(float[] sampleData)
    {
        Array.Copy(_newData, sampleData, sampleData.Length); // Copy the new audio to the current audio data.
    }
}