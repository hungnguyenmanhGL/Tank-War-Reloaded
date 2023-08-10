using UnityEngine;
using System;
using System.Collections;

namespace HAVIGAME.Audios {
    public class AudioPlayer {
        private float totalVolume;
        private float channelVolume;
        private AudioSource audioSource;
        private AudioChannel channel;
        private float fadeSpeed;
        private Audio audio;
        private Coroutine fadeCoroutine;

        public AudioChannel Channel => channel;
        public bool Enabled => channelVolume > 0 && totalVolume > 0;
        public float TotalVolume {
            get {
                return totalVolume;
            }
            set {
                if (totalVolume != value) {
                    totalVolume = value;

                    if (audio != null) FadeVolume(audio.Volume * channelVolume * totalVolume);
                }
            }
        }
        public float ChannelVolume {
            get {
                return channelVolume;
            }
            set {
                if (channelVolume != value) {
                    channelVolume = value;

                    if (audio != null) FadeVolume(audio.Volume * channelVolume * totalVolume);
                }
            }
        }

        public AudioPlayer(AudioSource audioSource, float totalVolume, float fadeSpeed, AudioChannel channel, float channelVolume) {
            this.audioSource = audioSource;
            this.totalVolume = totalVolume;
            this.fadeSpeed = fadeSpeed;
            this.channelVolume = channelVolume;
            this.channel = channel;
        }

        public void Play(Audio audio) {
            if (!Enabled) {
                return;
            }

            AudioClip clip = audio.Clip;

            if (clip == null) {
                return;
            }

            this.audio = audio;

            if (audioSource.clip != null) {
                FadeVolume(0, () => {
                    audioSource.clip = clip;
                    audioSource.loop = audio.Loop;

                    if (audio.Delay > 0) {
                        audioSource.Play();
                    }
                    else {
                        audioSource.PlayDelayed(audio.Delay);
                    }

                    FadeVolume(audio.Volume * channelVolume * totalVolume);
                });
            }
            else {
                audioSource.volume = 0;
                audioSource.clip = clip;
                audioSource.loop = audio.Loop;

                if (audio.Delay > 0) {
                    audioSource.Play();
                }
                else {
                    audioSource.PlayDelayed(audio.Delay);
                }

                FadeVolume(audio.Volume * channelVolume * totalVolume);
            }
        }

        public void PlayOnShot(Audio audio) {
            if (!Enabled) {
                return;
            }

            AudioClip clip = audio.Clip;

            if (clip == null) {
                return;
            }

            this.audio = audio;

            audioSource.PlayOneShot(clip, audio.Volume * channelVolume * totalVolume);
        }

        public void Stop() {
            if (fadeCoroutine != null) Executor.Instance.Stop(fadeCoroutine);

            audioSource.Stop();
        }

        private void FadeVolume(float volume, Action onCompleted = null) {
            if (fadeCoroutine != null) Executor.Instance.Stop(fadeCoroutine);

            fadeCoroutine = Executor.Instance.Run(IEFadeVolume(volume, onCompleted));
        }

        private IEnumerator IEFadeVolume(float volume, Action onCompleted) {
            if (audioSource.volume < volume) {
                while (audioSource.volume < volume) {
                    audioSource.volume += fadeSpeed * Time.deltaTime;
                    yield return null;
                }
            }
            else if (audioSource.volume > volume) {
                while (audioSource.volume > volume) {
                    audioSource.volume -= fadeSpeed * Time.deltaTime;
                    yield return null;
                }
            }

            audioSource.volume = volume;
            onCompleted?.Invoke();
        }
    }
}
