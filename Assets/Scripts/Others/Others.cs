using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TransformRange
{
    [Tooltip("Start transform.")]
    public Transform startTransform;
    [Tooltip("End transform.")]
    public Transform endTransform;
    [Tooltip("If not random will use only the start transform.")]
    public bool notRandom = false;

    /// <summary>
    /// Vector3 of startTransform.
    /// </summary>
    public Vector3 startVector {
        get {
            return startTransform.position;
        }
    }

    /// <summary>
    /// Vector3 of endTransform.
    /// </summary>
    public Vector3 endVector {
        get {
            return endTransform.position;
        }
    }

    /// <summary>
    /// Return a Vector3 position. If notRandom is true it will return the position of the startTransfom. On false, it will return a random Vector3 between the startTransform and the endTransform.
    /// </summary>
    /// <returns>A random Vector3 between the values of start and end transform.</returns>
    public Vector3 getVector()
    {
        if (notRandom)
            return startTransform.position;
        else
            return new Vector3(Random.Range(startTransform.position.x, endTransform.position.x), Random.Range(startTransform.position.y, endTransform.position.y), Random.Range(startTransform.position.z, endTransform.position.z));
    }
}

[System.Serializable]
public class Sound
{
    [Tooltip("Sound clip.")]
    public AudioClip audioClip;

    [Tooltip("Volume. Use range size 1 to avoid random volume.")]
    public float[] volume = new float[2] { 1, 1 };

    [Tooltip("Pitch. Use range size 1 to avoid random volume.")]
    public float[] pitch = new float[2] { 1, 1 };

    private float GetVolume()
    {
        if (volume.Length > 1)
            return Random.Range(volume[0], volume[1]);
        else
            return volume[0];
    }

    private float GetPitch()
    {
        if (pitch.Length > 1)
            return Random.Range(pitch[0], pitch[1]);
        else
            return pitch[0];
    }

    /// <summary>
    /// Play the sound on the specified AudioSource
    /// </summary>
    /// <param name="audioSource">AudioSource where the sound will be played.</param>
    /// <param name="volumeMultiplier">Volume of the sound, from 0 to 1.</param>
    public void Play(AudioSource audioSource, float volumeMultiplier)
    {
        audioSource.pitch = GetPitch();
        audioSource.PlayOneShot(audioClip, GetVolume() * volumeMultiplier);
    }
}