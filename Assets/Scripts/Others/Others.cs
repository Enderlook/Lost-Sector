using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class IVectorRangeTwo
{
    /// <summary>
    /// Whenever it should return only the initial vector or a random vector made by two vectors.
    /// </summary>
    [Tooltip("Whenever it should return only the initial vector or a random vector made by two vectors.")]
    public bool isRandom = true;

    /// <summary>
    /// Return the start parameter in <see cref="UnityEngine.Vector3"/>
    /// </summary>
    protected abstract Vector3 StartVector3 { get; set; }
    /// <summary>
    /// Return the end parameter in <see cref="UnityEngine.Vector3"/>
    /// </summary>
    protected abstract Vector3 EndVector3 { get; set; }
    /// <summary>
    /// Whenever it should return only the initial vector or a random vector made by two vectors.
    /// </summary>
    protected Vector3 Vector3 {
        get {
            if (isRandom)
                return new Vector3(Random.Range(StartVector3.x, EndVector3.x),
                    Random.Range(StartVector3.y, EndVector3.y),
                    Random.Range(StartVector3.z, EndVector3.z));
            else
                return StartVector3;
        }
    }

    /// <summary>
    /// Return the start parameter in <see cref="UnityEngine.Vector2"/>
    /// </summary>
    protected Vector2 StartVector2 => StartVector3;
    /// <summary>
    /// Return the end parameter in <see cref="UnityEngine.Vector2"/>
    /// </summary>
    protected Vector2 EndVector2 => EndVector3;
    /// <summary>
    /// Return a <seealso cref="UnityEngine.Vector2"/> position. If <see cref="IsRandom"/> is <see langword="true"/>, it will return a random <seealso cref="UnityEngine.Vector2"/> between the <see cref="StartVector2"/> and the <see cref="EndVector2"/>. On <see langword="false"/> it will return the position of the <see cref="StartVector2"/>.
    /// </summary>
    protected Vector2 Vector2 {
        get {
            if (isRandom)
                return new Vector2(Random.Range(StartVector2.x, EndVector2.x),
                    Random.Range(StartVector2.y, EndVector2.y));
            else
                return StartVector2;
        }
    }
}


[System.Serializable]
public class TransformRange : IVectorRangeTwo
{
    [Tooltip("Start transform.")]
    public Transform startTransform;
    [Tooltip("End transform.")]
    public Transform endTransform;

    protected override Vector3 StartVector3 {
        get {
            return startTransform.position;
        }
        set {
            startTransform.position = value;
        }
    }

    protected override Vector3 EndVector3 {
        get {
            return endTransform.position;
        }
        set {
            endTransform.position = value;
        }
    }

    /// <summary>
    /// Return a <seealso cref="Vector3"/> position. If <see cref="IVectorRangeTwo.isRandom"/> is <see langword="true"/> it will return the position of the <see cref="StartVector"/>. On <see langword="false"/>, it will return a random <seealso cref="Vector3"/> between the <see cref="StartVector"/> and the <see cref="EndVector"/>.
    /// </summary>
    /// <param name="x"><see cref="TransformRange"/> instance used to determine the random <seealso cref="Vector3"/>.</param>
    public static explicit operator Vector3(TransformRange x) => x.Vector3;
    /// <summary>
    /// Return a <seealso cref="Vector2"/> position. If <see cref="IVectorRangeTwo.isRandom"/> is <see langword="true"/> it will return the position of the <see cref="StartVector"/>. On <see langword="false"/>, it will return a random <seealso cref="Vector2"/> between the <see cref="StartVector"/> and the <see cref="EndVector"/>.
    /// </summary>
    /// <param name="x"><see cref="TransformRange"/> instance used to determine the random <seealso cref="Vector3"/>.</param>
    public static explicit operator Vector2(TransformRange x) => x.Vector2;
}

[System.Serializable]
public class Vector2RangeTwo : IVectorRangeTwo
{
    [Tooltip("Start vector.")]
    public Vector2 startVector;
    [Tooltip("End vector.")]
    public Vector2 endVector;

    protected override Vector3 StartVector3 {
        get {
            return startVector;
        }
        set {
            startVector = value;
        }
    }

    protected override Vector3 EndVector3 {
        get {
            return endVector;
        }
        set {
            endVector = value;
        }
    }

    /// <summary>
    /// Return a <seealso cref="Vector3"/> position. If <see cref="IVectorRangeTwo.isRandom"/> is <see langword="true"/> it will return the position of the <see cref="StartVector"/>. On <see langword="false"/>, it will return a random <seealso cref="Vector3"/> between the <see cref="StartVector"/> and the <see cref="EndVector"/>.
    /// </summary>
    /// <param name="x"><see cref="Vector2RangeTwo"/> instance used to determine the random <seealso cref="Vector3"/>.</param>
    public static explicit operator Vector3(Vector2RangeTwo x) => x.Vector3;
    /// <summary>
    /// Return a <seealso cref="Vector2"/> position. If <see cref="IVectorRangeTwo.isRandom"/> is <see langword="true"/> it will return the position of the <see cref="StartVector"/>. On <see langword="false"/>, it will return a random <seealso cref="Vector2"/> between the <see cref="StartVector"/> and the <see cref="EndVector"/>.
    /// </summary>
    /// <param name="x"><see cref="Vector2RangeTwo"/> instance used to determine the random <seealso cref="Vector3"/>.</param>
    public static explicit operator Vector2(Vector2RangeTwo x) => x.Vector2;

    /// <summary>
    /// Multiplicatives a given range of two <seealso cref="Vector2"/> (<seealso cref="Vector2RangeTwo"/>) with a <see langword="float"/>.<br/>
    /// The float multiplies each <seealso cref="Vector2"/>.
    /// </summary>
    /// <param name="left"><see cref="Vector2RangeTwo"/> to multiply.</param>
    /// <param name="right"><see langword="float"/> to multiply.</param>
    /// <returns>The multiplication of the <seealso cref="Vector2"/> inside <paramref name="left"/> with the number <paramref name="right"/>.</returns>
    public static Vector2RangeTwo operator *(Vector2RangeTwo left, float right) => new Vector2RangeTwo { startVector = left.startVector * right, endVector = left.endVector * right, isRandom = left.isRandom };
}

[System.Serializable]
public class FloatRangeTwo
{
    [Tooltip("Start.")]
    public float start;
    [Tooltip("End.")]
    public float end;
    [Tooltip("Whenever it should return only the initial vector or a random vector made by two vectors.")]
    public bool isRandom = false;

    /// <summary>
    /// Return a random number between <see cref="start"/> and <see cref="end"/>. The result is casted to <see langword="int"/>.
    /// </summary>
    /// <param name="x"><see cref="FloatRangeTwo"/> instance used to determine the random float.</param>
    public static explicit operator float(FloatRangeTwo x) => x.isRandom ? x.start : Random.Range(x.start, x.end);

    /// <summary>
    /// Return a random number between <see cref="start"/> and <see cref="end"/>. The result is casted to <see langword="int"/>.
    /// </summary>
    /// <param name="x"><see cref="FloatRangeTwo"/> instance used to determine the random int.</param>
    public static explicit operator int(FloatRangeTwo x) => x.isRandom ? (int)x.start : Random.Range((int)x.start, (int)x.end);
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
    private float GetVolume() => GetRandom(volume);

    /// <summary>
    /// Calculates a random pitch between the given by the two first elements of <see cref="pitch"/>.
    /// </summary>
    /// <returns>Random volume. If <c><see cref="pitch"/>.lenght <= 1</c> it <see langword="return"/> the <c><see cref="pitch"/>[1]</c>.</returns>
    /// <seealso cref="GetRandom(float[] array)"/>
    private float GetPitch() => GetRandom(pitch);

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
        if (Settings.IsSoundActive)
        {
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

public static class LINQExtension
{
    /// <summary>
    /// Add a the <paramref name="element"/> at the end of the returned <seealso cref="IEnumerable{T}"/> <paramref name="source"/>.
    /// </summary>
    /// <typeparam name="T">Type of the <paramref name="element"/> to append to <paramref name="source"/>.</typeparam>
    /// <param name="source">Source to append the <paramref name="element"/>.</param>
    /// <param name="element">Element to append to <paramref name="source"/>.</param>
    /// <returns><paramref name="source"/> with the <paramref name="element"/> added at the end of it.</returns>
    public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T element) => source.Concat(new T[] { element });
}