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
        get => startTransform.position;
        set => startTransform.position = value;
    }

    protected override Vector3 EndVector3 {
        get => endTransform.position;
        set => endTransform.position = value;
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
        get => startVector;
        set => startVector = value;
    }

    protected override Vector3 EndVector3 {
        get => endVector;
        set => endVector = value;
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
    [Tooltip("Whenever it should return only the highest vector or a random vector made by two vectors.")]
    public bool isRandom = false;

    /// <summary>
    /// Return the highest bound of the range.<br/>
    /// </summary>
    public float Max => Mathf.Max(start, end);

    /// <summary>
    /// Return the lowest bound of the range.<br/>
    /// </summary>
    public float Min => Mathf.Min(start, end);

    /// <summary>
    /// Return a random number between <see cref="start"/> and <see cref="end"/>.
    /// </summary>
    /// <param name="x"><see cref="FloatRangeTwo"/> instance used to determine the random float.</param>
    public static explicit operator float(FloatRangeTwo x) => x.isRandom ? Random.Range(x.Min, x.Max) : x.Max;

    /// <summary>
    /// Return a random number between <see cref="start"/> and <see cref="end"/>.<br/>
    /// The result is always a whole number.  Decimal numbers are used as chance to increment by one.
    /// </summary>
    /// <param name="x"><see cref="FloatRangeTwo"/> instance used to determine the random int.</param>
    public static explicit operator int(FloatRangeTwo x) => FloatToIntByChance((float)x);

    /// <summary>
    /// Return an whole number. Decimal numbers are used as chance to increment by one.
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static int FloatToIntByChance(float number) => (int)number + (Random.value < (number - (int)number) ? 1 : 0);
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
    public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T element)
    {
        if (source is null) throw new System.ArgumentNullException(nameof(source));

        return source.Concat(new T[] { element });
    }

    /// <summary>
    /// Check if the <paramref name="source"/> contains an elements which match the given criteria by <paramref name="selector"/>.
    /// </summary>
    /// <typeparam name="T">Type of the element inside <paramref name="source"/>.</typeparam>
    /// <param name="source">Source to look for a matching element.</param>
    /// <param name="selector">Check if the element match the criteria.</param>
    /// <returns>Whenever the matched item was found or not.</returns>
    public static bool ContainsBy<T>(this IEnumerable<T> source, System.Func<T, bool> selector)
    {
        if (source is null) throw new System.ArgumentNullException(nameof(source));
        if (selector is null) throw new System.ArgumentNullException(nameof(selector));

        foreach (T item in source)
        {
            if (selector(item))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Return the element which the highest property returned by <paramref name="selector"/>, using <paramref name="comparer"/>.
    /// </summary>
    /// <typeparam name="TSource">Type the of the <paramref name="source"/>.</typeparam>
    /// <typeparam name="TKey">Type returned by the <paramref name="selector"/>.</typeparam>
    /// <param name="source"><seealso cref="IEnumerable{T}"/> to get the highest value.</param>
    /// <param name="selector">Function which provides the property to compare.</param>
    /// <param name="comparer">Comparer used to compare the values returned by <paramref name="selector"/>.</param>
    /// <returns>The element with the highest property.</returns>
    public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, System.Func<TSource, TKey> selector, IComparer<TKey> comparer = null)
    {
        if (source is null) throw new System.ArgumentNullException(nameof(source));
        if (selector is null) throw new System.ArgumentNullException(nameof(selector));

        comparer = comparer ?? Comparer<TKey>.Default;
        return source.Aggregate((a, c) => comparer.Compare(selector(a), selector(c)) > 0 ? a : c);
    }

    /// <summary>
    /// Return the element which the lowest property returned by <paramref name="selector"/>, using <paramref name="comparer"/>.
    /// </summary>
    /// <typeparam name="TSource">Type the of the <paramref name="source"/>.</typeparam>
    /// <typeparam name="TKey">Type returned by the <paramref name="selector"/>.</typeparam>
    /// <param name="source"><seealso cref="IEnumerable{T}"/> to get the lowest value.</param>
    /// <param name="selector">Function which provides the property to compare.</param>
    /// <param name="comparer">Comparer used to compare the values returned by <paramref name="selector"/>.</param>
    /// <returns>The element with the lowest property.</returns>
    public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, System.Func<TSource, TKey> selector, IComparer<TKey> comparer = null)
    {
        if (source is null) throw new System.ArgumentNullException(nameof(source));
        if (selector is null) throw new System.ArgumentNullException(nameof(selector));

        comparer = comparer ?? Comparer<TKey>.Default;
        return source.Aggregate((a, c) => comparer.Compare(selector(a), selector(c)) < 0 ? a : c);
    }

    /// <summary>
    /// Returns a random element from <paramref name="source"/>.
    /// </summary>
    /// <typeparam name="T">Type of the element inside <paramref name="source"/>.</typeparam>
    /// <param name="source">Source to look for a random element.</param>
    /// <returns></returns>
    public static T RandomElement<T>(this IEnumerable<T> source)
    {
        if (source is null) throw new System.ArgumentNullException(nameof(source));

        return source.ElementAt(Random.Range(0, source.Count()));
    }

    /// <summary>
    /// Performs the specified <paramref name="action"/> on each element of the <paramref name="source"/>.
    /// </summary>
    /// <typeparam name="T">Type of the element inside <paramref name="source"/>.</typeparam>
    /// <param name="source">Source to look for element to perform the <paramref name="action"/></param>
    /// <param name="action">Action to perform on each element of <paramref name="source"/></param>
    public static void ForEach<T>(this IEnumerable<T> source, System.Action<T> action)
    {
        if (source is null) throw new System.ArgumentNullException(nameof(source));
        if (action is null) throw new System.ArgumentNullException(nameof(action));

        foreach (T element in source)
        {
            action(element);
        }
    }
}

public static class OthersExtension
{
    /// <summary>
    /// Deconstruction of <seealso cref="KeyValuePair{TKey, TValue}"/>.
    /// </summary>
    public static void Deconstruct<T1, T2>(this KeyValuePair<T1, T2> tuple, out T1 key, out T2 value)
    // https://stackoverflow.com/questions/42549491/deconstruction-in-foreach-over-dictionary
    {
        key = tuple.Key;
        value = tuple.Value;
    }
}

public static class ListExtension
{
    /// <summary>
    /// Removes an element from a list if matches a criteria determined by <paramref name="selector"/>.
    /// </summary>
    /// <typeparam name="T">Type of element.</typeparam>
    /// <param name="source">List to remove item.</param>
    /// <param name="selector">Function to determine if the item must be removed.</param>
    /// <param name="ascendOrder">Whenever it must remove in ascending or descending order.</param>
    /// <param name="removeAmount">Amount of items which must the criteria must be removed. If 0, remove all the matched elements.</param>
    /// <returns><paramref name="source"/>.</returns>
    private static List<T> RemoveBy<T>(this List<T> source, System.Func<T, bool> selector, bool ascendOrder = true, int removeAmount = 1)
    {
        if (source is null) throw new System.ArgumentNullException(nameof(source));
        if (selector is null) throw new System.ArgumentNullException(nameof(selector));

        if (removeAmount < 0)
            throw new System.Exception($"{nameof(removeAmount)} parameter can't be negative.");
        int removed = 0;
        for (int i = ascendOrder ? 0 : source.Count; i < (ascendOrder ? source.Count : 0); i += ascendOrder ? 1 : -1)
        {
            if (selector(source[i]))
            {
                source.RemoveAt(i);
                removed++;
                if (removeAmount == 0 || removed >= removeAmount) break;
            }
        }
        return source;
    }

    /// <summary>
    /// Removes the fist(s) element(s) from a list which matches a criteria determined by <paramref name="selector"/>.
    /// </summary>
    /// <typeparam name="T">Type of element.</typeparam>
    /// <param name="source">List to remove item.</param>
    /// <param name="selector">Function to determine if the item must be removed.</param>
    /// <param name="removeAmount">Amount of items which must the criteria must be removed. Value can't be 0.</param>
    /// <returns><paramref name="source"/>.</returns>
    /// <see cref="RemoveBy{T}(List{T}, System.Func{T, bool}, bool, int)"/>
    /// <seealso cref="RemoveLastBy{T}(List{T}, System.Func{T, bool}, int)"/>
    /// <seealso cref="RemoveByAll{T}(List{T}, System.Func{T, bool})"/>
    public static List<T> RemoveFirstBy<T>(this List<T> source, System.Func<T, bool> selector, int removeAmount = 1)
    {
        if (removeAmount == 0)
            throw new System.Exception($"{nameof(removeAmount)} parameter can't be 0.");
        return source.RemoveBy(selector, removeAmount: removeAmount);
    }

    /// <summary>
    /// Removes the last(s) element(s) from a list which matches a criteria determined by <paramref name="selector"/>.
    /// </summary>
    /// <typeparam name="T">Type of element.</typeparam>
    /// <param name="source">List to remove item.</param>
    /// <param name="selector">Function to determine if the item must be removed.</param>
    /// <param name="removeAmount">Amount of items which must the criteria must be removed. Value can't be 0.</param>
    /// <returns><paramref name="source"/>.</returns>
    /// <see cref="RemoveBy{T}(List{T}, System.Func{T, bool}, bool, int)"/>
    /// <seealso cref="RemoveFirstBy{T}(List{T}, System.Func{T, bool}, int)"/>
    /// <seealso cref="RemoveByAll{T}(List{T}, System.Func{T, bool})"/>
    public static List<T> RemoveLastBy<T>(this List<T> source, System.Func<T, bool> selector, int removeAmount = 1)
    {
        if (removeAmount == 0)
            throw new System.Exception($"{nameof(removeAmount)} parameter can't be 0.");
        return source.RemoveBy(selector, ascendOrder: false, removeAmount: removeAmount);
    }

    /// <summary>
    /// Removes all the elements from a list which matches a criteria determined by <paramref name="selector"/>.
    /// </summary>
    /// <typeparam name="T">Type of element.</typeparam>
    /// <param name="source">List to remove item.</param>
    /// <param name="selector">Function to determine if the item must be removed.</param>
    /// <returns><paramref name="source"/>.</returns>
    /// <see cref="RemoveBy{T}(List{T}, System.Func{T, bool}, bool, int)"/>
    /// <seealso cref="RemoveFirstBy{T}(List{T}, System.Func{T, bool}, int)"/>
    /// <seealso cref="RemoveLastBy{T}(List{T}, System.Func{T, bool}, int)"/>
    public static List<T> RemoveByAll<T>(this List<T> source, System.Func<T, bool> selector) => source.RemoveBy(selector, removeAmount: 0);
}

public static class VectorExtension
{
    /// <summary>
    /// Returns absolute <seealso cref="Vector2"/> of <paramref name="source"/>.
    /// </summary>
    /// <param name="source"><seealso cref="Vector2"/> to become absolute.</param>
    /// <returns>Absolute <seealso cref="Vector2"/>.</returns>
    public static Vector2 Abs(this Vector2 source) => new Vector2(Mathf.Abs(source.x), Mathf.Abs(source.y));
    /// <summary>
    /// Returns absolute <seealso cref="Vector3"/> of <paramref name="source"/>.
    /// </summary>
    /// <param name="source"><seealso cref="Vector3"/> to become absolute.</param>
    /// <returns>Absolute <seealso cref="Vector3"/>.</returns>
    public static Vector3 Abs(this Vector3 source) => new Vector3(Mathf.Abs(source.x), Mathf.Abs(source.y), Mathf.Abs(source.z));
    /// <summary>
    /// Returns absolute <seealso cref="Vector4"/> of <paramref name="source"/>.
    /// </summary>
    /// <param name="source"><seealso cref="Vector4"/> to become absolute.</param>
    /// <returns>Absolute <seealso cref="Vector4"/>.</returns>
    public static Vector4 Abs(this Vector4 source) => new Vector4(Mathf.Abs(source.x), Mathf.Abs(source.y), Mathf.Abs(source.z), Mathf.Abs(source.w));
}

public static class CastExtension
{
    /// <summary>
    /// Try to cast <paramref name="obj"/> into <typeparamref name="T"/> in <paramref name="result"/>.<br/>
    /// If <paramref name="obj"/> isn't <typeparamref name="T"/>, <paramref name="result"/> is set with <c>default(<typeparamref name="T"/>)</c>.
    /// </summary>
    /// <typeparam name="T">Type of the value to cast.</typeparam>
    /// <param name="obj"><see cref="Object"/> to cast.</param>
    /// <param name="result">Casted result.</param>
    /// <returns><see langword="true"/> if the cast was successful. <see langword="false"> if it wasn't able to cast.</returns>
    /// <seealso href="https://codereview.stackexchange.com/questions/17982/trycastt-method">Source.</see>
    /// <seealso cref="CastOrDefault{T}(object)"/>
    /// <seealso cref="CastOrNull{T}(object, RequireStruct{T})"/>
    /// <seealso cref="CastOrNull{T}(object, RequireClass{T})"/>
    public static bool TryCast<T>(this object obj, out T result)
    {
        if (obj is T)
        {
            result = (T)obj;
            return true;
        }
        result = default;
        return false;
    }

    /// <summary>
    /// Try to cast <paramref name="obj"/> into <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of the value to cast.</typeparam>
    /// <param name="obj"><see cref="Object"/> to cast.</param>
    /// <returns>Return <c>(<typeparamref name="T"/>)<paramref name="obj"/></c>. <c>default(<typeparamref name="T"/>)<c> if it can't cast.</returns>
    public static T CastOrDefault<T>(this object obj)
    {
        if (obj is T)
        {
            return (T)obj;
        }
        return default;
    }

    /// <summary>
    /// Don't use me.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RequireStruct<T> where T : struct { }
    /// <summary>
    /// Don't use me.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RequireClass<T> where T : class { }

    /// <summary>
    /// Try to cast <paramref name="obj"/> into <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of the value to cast.</typeparam>
    /// <param name="obj"><see cref="Object"/> to cast.</param>
    /// <param name="ignoreMe">Ignore this. Don't put anything here.</param>
    /// <returns>Return <c>(<typeparamref name="T"/>)<paramref name="obj"/></c>. <see langword="null"/> if it can't cast.</returns>
    /// <seealso href="https://stackoverflow.com/questions/2974519/generic-constraints-where-t-struct-and-where-t-class"/>
    /// <seealso cref="TryCast{T}(object, out T)"/>
    /// <seealso cref="CastOrDefault{T}(object)"/>
    /// <seealso cref="CastOrNull{T}(object, RequireClass{T})"/>
    public static T? CastOrNull<T>(this object obj, RequireStruct<T> ignoreMe = null) where T : struct
    {
        if (obj is T)
        {
            return (T)obj;
        }
        return null;
    }

    /// <summary>
    /// Try to cast <paramref name="obj"/> into <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of the value to cast.</typeparam>
    /// <param name="obj"><see cref="Object"/> to cast.</param>
    /// <param name="ignoreMe">Ignore this. Don't put anything here.</param>
    /// <returns>Return <c>(<typeparamref name="T"/>)<paramref name="obj"/></c>. <see langword="null"/> if it can't cast.</returns>
    /// <seealso href="https://stackoverflow.com/questions/2974519/generic-constraints-where-t-struct-and-where-t-class"/>
    /// <seealso cref="TryCast{T}(object, out T)"/>
    /// <seealso cref="CastOrDefault{T}(object)"/>
    /// <seealso cref="CastOrNull{T}(object, RequireStruct{T})"/>
    public static T CastOrNull<T>(this object obj, RequireClass<T> ignoreMe = null) where T : class
    {
        if (obj is T)
        {
            return (T)obj;
        }
        return null;
    }
}