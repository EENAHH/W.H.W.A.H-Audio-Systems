using System.Collections.Generic;
using UnityEngine;

namespace WhwahAudio
{
    [CreateAssetMenu(fileName = "new Playlist", menuName = "Audio/Playlist")]
    public class MusicPlaylist : ScriptableObject
    {
        int index = 0;
        public List<AudioContainer> playlist;

        public AudioContainer CurrentTrack => playlist[index];

        public void LoadPlaylistData() => playlist.ForEach(m => m.Clip().LoadAudioData());

        public AudioContainer NextTrack()
        {
            index = index >= playlist.Count - 1 ? 0 : index + 1;
            return playlist[index];
        }

        public AudioContainer PreviousTrack()
        {
            index = index <= 0 ? playlist.Count - 1 : index - 1;
            return playlist[index];
        }

        public AudioContainer SetTrack(int i)
        {
            index = Mathf.Clamp(i, 0, playlist.Count - 1);
            return playlist[index];
        }
}
}