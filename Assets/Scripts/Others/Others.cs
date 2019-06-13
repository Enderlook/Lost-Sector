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
    public Vector3 StartVector => startTransform.position;

    /// <summary>
    /// Vector3 of endTransform.
    /// </summary>
    public Vector3 EndVector => endTransform.position;

    /// <summary>
    /// Return a Vector3 position. If notRandom is true it will return the position of the startTransfom. On false, it will return a random Vector3 between the startTransform and the endTransform.
    /// </summary>
    /// <returns>A random Vector3 between the values of start and end transform.</returns>
    public Vector3 GetVector()
    {
        if (notRandom)
            return startTransform.position;
        else
            return new Vector3(Random.Range(startTransform.position.x, endTransform.position.x),
                Random.Range(startTransform.position.y, endTransform.position.y),
                Random.Range(startTransform.position.z, endTransform.position.z));
    }
}

[System.Serializable]
public class Vector2RangeTwo
{
    [Tooltip("Start vector.")]
    public Vector2 startVector;
    [Tooltip("End vector.")]
    public Vector2 endVector;
    [Tooltip("If not random will use only the start vector.")]
    public bool notRandom = false;

    /// <summary>
    /// Return a Vector3 position. If notRandom is true it will return the position of the StartVector. On false, it will return a random Vector3 between the StartVector and the EndVector.
    /// </summary>
    /// <returns></returns>
    public Vector2 GetVector()
    {
        if (notRandom)
            return startVector;
        else
            return new Vector2(Random.Range(startVector.x, endVector.x), Random.Range(startVector.y, endVector.y));
    }

    /// <summary>
    /// Constructor of <see cref="Vector2RangeTwo"/>.
    /// </summary>
    /// <param name="StartVector">Initial <see cref="Vector2"/>.</param>
    /// <param name="EndVector">End <see cref="Vector2"/>.</param>
    /// <param name="notRandom">Determines if the returned <see cref="Vector2"/> from <seealso cref="GetVector()"/> should be random between <see cref="startVector"/> an <see cref="endVector"/> or just be <see cref="startVector"/>.</param>
    public Vector2RangeTwo(Vector2 startVector, Vector2 endVector, bool notRandom)
    {
        this.startVector = startVector;
        this.endVector = endVector;
        this.notRandom = notRandom;
    }

    /// <summary>
    /// Multiplicatives a given range of two <seealso cref="Vector2"/> (<seealso cref="Vector2RangeTwo"/>) with a <see langword="float"/>.<br/>
    /// The float multiplies each <seealso cref="Vector2"/>.
    /// </summary>
    /// <param name="left"><see cref="Vector2RangeTwo"/> to multiply.</param>
    /// <param name="right"><see langword="float"/> to multiply.</param>
    /// <returns>The multiplication of the <seealso cref="Vector2"/> inside <paramref name="left"/> with the number <paramref name="right"/>.</returns>
    public static Vector2RangeTwo operator *(Vector2RangeTwo left, float right)
    {
        return new Vector2RangeTwo(left.startVector * right, left.endVector * right, left.notRandom);        
    }
}

[System.Serializable]
public class FloatRangeTwo
{
    [Tooltip("Start.")]
    public float start;
    [Tooltip("End.")]
    public float end;
    [Tooltip("If not random will use only the start number.")]
    public bool notRandom = false;

    /// <summary>
    /// Calculates a random number between <see cref="start"/> and <see cref="end"/>.
    /// </summary>
    /// <returns>If <see cref="notRandom"/> is <see langword="true"/> it will return the position of the start. On <see langword="false"/>, it will <see langword="return"/> a random <see langword="float"/> between the start and the en.</returns>
    public float Value => notRandom ? start : Random.Range(start, end);

    /// <summary>
    /// Calculates a random number between <see cref="start"/> and <see cref="end"/>. The result is casted to <see langword="int"/>.
    /// </summary>
    /// <returns>If <see cref="notRandom"/> is <see langword="true"/> it will <see langword="return"/> the position of the start. On <see langword="false"/>, it will <see langword="return"/> a random <see langword="float"/> between the <see cref="start"/> and the <see cref="end"/>.</returns>
    public int ValueInt => (int)Value;
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

    /// <summary>
    /// Calculates a random volume between the given by the two first elements of <see cref="volume"/>.
    /// </summary>
    /// <returns>Random volume. If <c><see cref="volume"/>.lenght <= 1</c> it <see langword="return"/> the <c><see cref="volume"/>[1]</c>.</returns>
    /// <seealso cref="GetRandom(float[])"/>
    private float GetVolume()
    {
        return GetRandom(volume);
    }

    /// <summary>
    /// Calculates a random pitch between the given by the two first elements of <see cref="pitch"/>.
    /// </summary>
    /// <returns>Random volume. If <c><see cref="pitch"/>.lenght <= 1</c> it <see langword="return"/> the <c><see cref="pitch"/>[1]</c>.</returns>
    /// <seealso cref="GetRandom(float[] array)"/>
    private float GetPitch()
    {
        return GetRandom(pitch);
    }

    /// <summary>
    /// Calculates a random value between the given by the two first elements of <paramref name="array"/>.
    /// </summary>
    /// <param name="array"></param>
    /// <returns>Random volume. If <c><paramref name="array"/>.lenght <= 1</c> it <see langword="return"/> the <c><paramref name="array"/>[1]</c>.</returns>
    private float GetRandom(float[] array)
    {
        if (array.Length > 1)
            return Random.Range(array[0], array[1]);
        else
            return array[0];
    }

    /// <summary>
    /// Play the sound on the specified <paramref name="audioSource"/>.
    /// </summary>
    /// <param name="audioSource"><see cref="AudioSource"/> where the sound will be played.</param>
    /// <param name="volumeMultiplier">Volume of the sound, from 0 to 1.</param>
    public void Play(AudioSource audioSource, float volumeMultiplier)
    {
        if (Settings.IsSoundActive) {
            audioSource.pitch = GetPitch();
            audioSource.PlayOneShot(audioClip, GetVolume() * volumeMultiplier);
        }
    }
}

public static class LayerMaskExtension
{
    // https://forum.unity.com/threads/get-the-layernumber-from-a-layermask.114553/#post-3021162
    /// <summary>
    /// Convert a <see cref="LayerMask"/> into a layer number.<br/>
    /// This should only be used if the <paramref name="layerMask"/> has a single layer.
    /// </summary>
    /// <param name="layerMask"><see cref="LayerMask"/> to convert.</param>
    /// <returns>Layer number.</returns>
    public static int ToLayer(this LayerMask layerMask)
    {
        int bitMask = layerMask.value;
        int result = bitMask > 0 ? 0 : 31;
        while (bitMask > 1)
        {
            bitMask = bitMask >> 1;
            result++;
        }
        return result;
    }
}