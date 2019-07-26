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