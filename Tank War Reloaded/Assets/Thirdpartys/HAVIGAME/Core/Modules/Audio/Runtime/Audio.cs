using UnityEngine;

namespace HAVIGAME.Audios {
    [System.Serializable]
    public class Audio {
        [SerializeField] private AudioClipPropertyReadonly clip = AudioClipPropertyReadonly.Create();
        [SerializeField] private AudioChannel channel = AudioChannel.SoundFX;
        [SerializeField, Range(0f, 1f)] private float volume = 1f;
        [SerializeField] private bool loop = true;
        [SerializeField] private float delay = 0;

        public bool IsEmpty => Clip == null;
        public AudioClip Clip => clip.Get();
        public AudioChannel Channel => channel;
        public float Volume => volume;
        public bool Loop => loop;
        public float Delay => delay;

        public void Play() {
            if (AudioManager.HasInstance) {
                AudioManager.Instance.Play(this);
            }
        }
    }
}