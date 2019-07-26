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
            PlaySound(sound.audioClip, sound.Volume * volume, sound.Pitch, true);
        }
    }

    /// <summary>
    /// Play a <paramref name="audioClip"/>.
    /// </summary>
    /// <param name="audioClip"><see cref="AudioClip"/> to play.</param>
    /// <param name="volume">Volume of sound.</param>
    /// <param name="pitch">Pitch of sound.</param>
    /// <param name="useMasterVolumeMultplier">If <see langword="true"/> <paramref name="volume"/> will be multiplied by <see cref="masterVolume"/>.</param>
    /// <seealso cref="PlaySound(Sound, bool)"/>
    public void PlaySound(AudioClip audioClip, float volume, float pitch, bool useMasterVolumeMultplier = false)
    {
        audioSource.Stop();
        audioSource.clip = audioClip;
        audioSource.volume = useMasterVolumeMultplier ? volume * masterVolume : volume;
        audioSource.pitch = pitch;
        audioSource.Play();
    }

    /// <summary>
    /// Play a <paramref name="audioClip"/>.
    /// </summary>
    /// <param name="sound">Sound to play.</param>
    /// <param name="useMasterVolumeMultplier">If <see langword="true"/> <paramref name="volume"/> will be multiplied by <see cref="masterVolume"/>.</param>
    /// <seealso cref="PlaySound(AudioClip, float, float, bool)"/>
    public void PlaySound(Sound sound, bool useMasterVolumeMultplier = false) => PlaySound(sound.audioClip, sound.Volume, sound.Pitch, useMasterVolumeMultplier);

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
            if (playlists[i].playlistName == name)
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