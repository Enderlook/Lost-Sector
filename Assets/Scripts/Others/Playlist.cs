using UnityEngine;

[CreateAssetMenu(fileName = "Playlist", menuName = "Playlist")]
public class Playlist : ScriptableObject
{
    [Tooltip("Name of the playlist, used to be access by other scripts.")]
    public string playlistName;
    [Tooltip("Playlist. It will be looped.")]
    public Sound[] playlist;
    private int playlistIndex = 0;
    [Tooltip("Playlist master volume.")]
    public float volume = 1;
    [Tooltip("Is the playlist random.")]
    public bool isRandom = false;

    /// <summary>
    /// Use <see cref="Mode.Random"/> to play sound by <seealso cref="GetRandomSound"/>.<br/>
    /// Use <see cref="Mode.Next"/> to play a sound by <seealso cref="GetNextSound"/>.<br/>
    /// Use <see cref="Mode.Configured"/> to play sound using <see cref="isRandom"/> to determine if use <seealso cref="GetRandomSound"/> or <seealso cref="GetNextSound"/>.
    /// </summary>
    public enum Mode { Random, Next, Configured }

    /// <summary>
    /// Get random sound from <see cref="playlist"/>.
    /// </summary>
    /// <returns>Sound to play and its playlist <see cref="volume"/>.</returns>
    public (Sound sound, float volume) GetRandomSound() => (playlist[playlistIndex = Random.Range(0, playlist.Length)], volume);

    /// <summary>
    /// Get the next sound from <see cref="playlist"/>. It loops to beginning when reach the end of the <see cref="playlist"/>.
    /// </summary>
    /// <returns>Sound to play and its playlist <see cref="volume"/>.</returns>
    public (Sound sound, float volume) GetNextSound() => (playlist[playlistIndex = (playlistIndex + 1) % playlist.Length], volume);

    /// <summary>
    /// Get a sound from <see cref="playlist"/>. It can be random or not depending of <see cref="isRandom"/>.
    /// </summary>
    /// <returns>Sound to play and its playlist <see cref="volume"/>.</returns>
    public (Sound sound, float volume) GetSound() => isRandom ? GetRandomSound() : GetNextSound();

    /// <summary>
    /// Reset the <see cref="playlistIndex"/> to 0.
    /// </summary>
    public void ResetIndex() => playlistIndex = 0;

    /// <summary>
    /// Play a sound from the <seealso cref="playlist"/> using a method described by <seealso cref="Mode"/>.
    /// </summary>
    /// <param name="audioSource"><see cref="AudioSource"/> to play the sonud.</param>
    /// <param name="isSoundActive">Whenever sound is active or not. On <see langword="false"/> no sound will be played.</param>
    /// <param name="mode">Mode to get the sound form the <seealso cref="playlist"/>.</param>
    /// <param name="volumeMultiplier">Multiplier of the volume.</param>
    public void Play(AudioSource audioSource, bool isSoundActive, Mode mode = Mode.Configured, float volumeMultiplier = 1) => Play(audioSource, isSoundActive, IsRandomFromMode(mode), volumeMultiplier);

    /// <summary>
    /// Play a sound from the <seealso cref="playlist"/>
    /// </summary>
    /// <param name="audioSource"><see cref="AudioSource"/> to play the sound.</param>
    /// <param name="isSoundActive">Whenever sound is active or not. On <see langword="false"/> no sound will be played.</param>
    /// <param name="isRandom">If <see langword="true"/> a random sound from <see cref="playlist"/> will be played. On <see langword="false"/> the next sound from the <see cref="playlist"/> will be played.</param>
    /// <param name="volumeMultiplier">Multiplier of the volume.</param>
    public void Play(AudioSource audioSource, bool isSoundActive, bool isRandom, float volumeMultiplier = 1)
    {
        (Sound sound, float volume) = GetSound(isRandom);
        sound.Play(audioSource, isSoundActive, volume * volumeMultiplier);
    }

    /// <summary>
    /// Play a sound from the <seealso cref="playlist"/> using a method described by <seealso cref="Mode"/> on the specified <paramref name="position"/>.
    /// </summary>
    /// <param name="position">Position to play the sound.</param>
    /// <param name="isSoundActive">Whenever sound is active or not. On <see langword="false"/> no sound will be played.</param>
    /// <param name="mode">Mode to get the sound form the <seealso cref="playlist"/>.</param>
    /// <param name="volumeMultiplier">Multiplier of the volume.</param>
    public void PlayAtPoint(Vector3 position, bool isSoundActive, Mode mode = Mode.Configured, float volumeMultiplier = 1) => PlayAtPoint(position, isSoundActive, IsRandomFromMode(mode), volumeMultiplier);

    /// <summary>
    /// Play a sound from the <seealso cref="playlist"/> using a method described by <seealso cref="Mode"/> on the specified <paramref name="position"/>.
    /// </summary>
    /// <param name="position">Position to play the sound.</param>
    /// <param name="isSoundActive">Whenever sound is active or not. On <see langword="false"/> no sound will be played.</param>
    /// <param name="isRandom">If <see langword="true"/> a random sound from <see cref="playlist"/> will be played. On <see langword="false"/> the next sound from the <see cref="playlist"/> will be played.</param>
    /// <param name="volumeMultiplier">Multiplier of the volume.</param>
    public void PlayAtPoint(Vector3 position, bool isSoundActive, bool isRandom, float volumeMultiplier = 1)
    {
        if (isSoundActive)
        {
            (Sound sound, float volume) = GetSound(isRandom);
            AudioSource.PlayClipAtPoint(sound.audioClip, position, sound.Volume * volume * volumeMultiplier);
        }
    }

    private (Sound sound, float volume) GetSound(bool isRandom) => isRandom ? GetRandomSound() : GetNextSound();
    private bool IsRandomFromMode(Mode mode = Mode.Configured) => (mode == Mode.Configured && isRandom) || mode == Mode.Random;

}

[System.Serializable]
public class Sound
{
    [Tooltip("Sound clip.")]
    public AudioClip audioClip;

    [Tooltip("Volume. Use range size 1 to avoid random volume.")]
    public float[] volume = new float[1] { 1 };

    [Tooltip("Pitch. Use range size 1 to avoid random volume.")]
    public float[] pitch = new float[1] { 1 };

    /// <summary>
    /// Calculates a random volume between the given by the two first elements of <see cref="volume"/>.
    /// </summary>
    /// <returns>Random volume. If <c><see cref="volume"/>.lenght <= 1</c> it <see langword="return"/> the <c><see cref="volume"/>[1]</c>.</returns>
    /// <seealso cref="GetRandom(float[])"/>
    public float Volume => GetRandom(volume);

    /// <summary>
    /// Calculates a random pitch between the given by the two first elements of <see cref="pitch"/>.
    /// </summary>
    /// <returns>Random volume. If <c><see cref="pitch"/>.lenght <= 1</c> it <see langword="return"/> the <c><see cref="pitch"/>[1]</c>.</returns>
    /// <seealso cref="GetRandom(float[] array)"/>
    public float Pitch => GetRandom(pitch);

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
    /// <param name="isSoundActive">Whenever sound is active or not. On <see langword="false"/> no sound will be played.</param>
    /// <param name="volumeMultiplier">Volume of the sound, from 0 to 1.</param>
    public void PlayOneShoot(AudioSource audioSource, bool isSoundActive, float volumeMultiplier = 1)
    {
        // audioSource != null shouldn't be used but it's to prevents a bug
        if (isSoundActive && audioSource != null)
        {
            audioSource.pitch = Pitch;
            audioSource.PlayOneShot(audioClip, Volume * volumeMultiplier);
        }
    }

    /// <summary>
    /// Play the sound on the specified <paramref name="audioSource"/>.
    /// </summary>
    /// <param name="audioSource"><see cref="AudioSource"/> where the sound will be played.</param>
    /// <param name="isSoundActive">Whenever sound is active or not. On <see langword="false"/> no sound will be played.</param>
    /// <param name="volumeMultiplier">Volume of the sound, from 0 to 1.</param>
    public void Play(AudioSource audioSource, bool isSoundActive, float volumeMultiplier = 1)
    {
        // audioSource != null shouldn't be used but it's to prevents a bug
        if (isSoundActive && audioSource != null)
        {
            audioSource.pitch = Pitch;
            audioSource.volume = Volume * volumeMultiplier;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }

    /// <summary>
    /// Play the sound on the specified <paramref name="position"/>.
    /// </summary>
    /// <param name="position">Position to play the sound.</param>
    /// <param name="isSoundActive">Whenever sound is active or not. On <see langword="false"/> no sound will be played.</param>
    /// <param name="volumeMultiplier">Volume of the sound, from 0 to 1.</param>
    public void PlayAtPoint(Vector3 position, bool isSoundActive, float volumeMultiplier = 1)
    {
        if (isSoundActive)
            AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier);
    }
}
