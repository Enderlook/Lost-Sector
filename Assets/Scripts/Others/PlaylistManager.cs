using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlaylistManager : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Playlists.")]
    public Playlist[] playlists;
    [Tooltip("Default playlist set at start.")]
    public int startingPlaylistIndex;
    private int playlistsIndex = 0;
    [Tooltip("Master volume.")]
    public float masterVolume = 1;
    [Tooltip("Play on start.")]
    public bool playOnStart;

    [Header("Setup")]
    [Tooltip("Audio Source used to play music.")]
    public AudioSource audioSource;

    private bool isPlaying = false;

    public int playlistsAmount => playlists.Length;

    private void Start()
    {
        audioSource.loop = false;
        isPlaying = playOnStart;
        playlistsIndex = startingPlaylistIndex;
    }

    public void Update()
    {
        if (!Settings.IsMusicActive)
            audioSource.Stop();
        else if (isPlaying && !audioSource.isPlaying && playlists.Length > 0 && playlists[playlistsIndex].playlist.Length > 0)
        {
            (Sound sound, float volume) = playlists[playlistsIndex].GetSound();
            audioSource.clip = sound.audioClip;
            audioSource.volume = sound.Volume * volume * masterVolume;
            audioSource.pitch = sound.Pitch;
            audioSource.Play();
        }
    }

    /// <summary>
    /// Set a playlist. It doesn't reset current playing sound.
    /// </summary>
    /// <param name="index">Index of the playlist.</param>
    /// <returns><see langword="true"/> on success. <see langword="false"/> if the <paramref name="index"/> was outside the range of the <see cref="playlists"/>.</returns>
    public bool SetPlaylist(int index)
    {
        if (index >= playlists.Length)
            return false;
        playlistsIndex = index;
        return true;
    }

    /// <summary>
    /// Set a playlist. It doesn't reset current playing sound.
    /// </summary>
    /// <param name="name">Name of the playlist.</param>
    /// <returns><see langword="true"/> on success. <see langword="false"/> if the <paramref name="name"/> was not found in <see cref="playlists"/>.</returns>
    public bool SetPlaylist(string name)
    {
        for (int i = 0; i < playlists.Length; i++)
        {
            if (playlists[i].name == name)
            {
                playlistsIndex = i;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Play or unpause the sound.
    /// </summary>
    public void Play()
    {
        isPlaying = true;
        audioSource.UnPause();
    }

    /// <summary>
    /// Pauses the played sound.
    /// </summary>
    public void Pause()
    {
        isPlaying = false;
        audioSource.Pause();
    }

    /// <summary>
    /// Stop the played sound.
    /// </summary>
    public void Stop()
    {
        isPlaying = false;
        audioSource.Stop();
    }

    /// <summary>
    /// Reset the playlist to the first sound.
    /// </summary>
    public void Reset()
    {
        playlists[playlistsIndex].ResetIndex();
        audioSource.Stop();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (audioSource == null)
            Debug.LogWarning($"GameObject {gameObject.name} requires an AudioSource component assigned in {nameof(audioSource)} field.");
    }
#endif
}

[System.Serializable]
public class Playlist
{
    [Tooltip("Name of the playlist, used to be access by other scripts.")]
    public string name;
    [Tooltip("Playlist. It will be looped.")]
    public Sound[] playlist;
    private int playlistIndex = 0;
    [Tooltip("Playlist master volume.")]
    public float volume = 1;
    [Tooltip("Is the playlist random.")]
    public bool isRandom = false;

    /// <summary>
    /// Get random sound from <see cref="playlist"/>.
    /// </summary>
    /// <returns>Sound to play and its playlist volume.</returns>
    public (Sound sound, float volume) GetRandomSound() => (playlist[playlistIndex = Random.Range(0, playlist.Length)], volume);

    /// <summary>
    /// Get the next sound from <see cref="playlist"/>. It loops to beginning when reach the end of the <see cref="playlist"/>.
    /// </summary>
    /// <returns>Sound to play and its playlist volume.</returns>
    public (Sound sound, float volume) GetNextSound() => (playlist[playlistIndex = (playlistIndex + 1) % playlist.Length], volume);

    /// <summary>
    /// Get a sound from <see cref="playlist"/>. It can be random or not depending of <see cref="isRandom"/>.
    /// </summary>
    /// <returns>Sound to play and its playlist volume.</returns>
    public (Sound sound, float volume) GetSound() => isRandom ? GetRandomSound() : GetNextSound();

    /// <summary>
    /// Reset the <see cref="playlistIndex"/> to 0.
    /// </summary>
    public void ResetIndex() => playlistIndex = 0;
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
    /// Play the sound on the specified <paramref name="position"/>.
    /// </summary>
    /// <param name="position">Position to play the sound.</param>
    /// <param name="volumeMultiplier">Volume of the sound, from 0 to 1.</param>
    public void PlayAtPoint(Vector3 position, float volumeMultiplier = 1) => AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier);
}