using Commons;
using Patterns;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Audio
{
    public class AudioManager : Singleton<AudioManager>
    {
        public AudioSource musicSource;
        public AudioSource sfxSource;

        private Dictionary<string, AudioClip> _musicClips = new Dictionary<string, AudioClip>();
        private Dictionary<string, AudioClip> _sfxClips = new Dictionary<string, AudioClip>();
        private List<AudioSource> _activeSFXSources = new List<AudioSource>();

        private float _musicVolume = 1f;
        private float _sfxVolume = 1f;

        public float SfxVolume { get => _sfxVolume; set => _sfxVolume = value; }
        public float MusicVolume { get => _musicVolume; set => _musicVolume = value; }

        protected override void Awake()
        {
            base.Awake();
            LoadAllSFX();
            LoadAllMusic();
        }

        private void LoadAllSFX()
        {
            AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio/sfx");
            foreach (var clip in clips)
            {
                _sfxClips[clip.name] = clip;
            }
        }

        private void LoadAllMusic()
        {
            AudioClip[] clips = Resources.LoadAll<AudioClip>("Audio/music");
            foreach (var clip in clips)
            {
                _musicClips[clip.name] = clip;
            }
        }


        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (musicSource == null)
            {
                LogUtility.InvalidInfo("AudioManager", "AudioSource is null!");
                return;
            }

            if (musicSource.clip == clip && musicSource.isPlaying) return;

            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.volume = _musicVolume;
            musicSource.Play();
        }

        public void PlayMusic(string clipName, bool loop = true)
        {
            if (_musicClips.TryGetValue(clipName, out AudioClip clip))
            {
                PlayMusic(clip, loop);
            }
            else
            {
                LogUtility.Error("AudioManager", $"Music clip '{clipName}' not found!");
            }
        }

        public void StopMusic()
        {
            musicSource.Stop();
            musicSource.clip = null;
        }


        public void PlaySFX(string clipName)
        {
            PlaySFX(clipName, sfxSource);
        }

        public void PlaySFX(string clipName, AudioSource source)
        {
            if (source == null)
            {
                LogUtility.InvalidInfo("AudioManager", "AudioSource is null!");
                return;
            }

            if (_sfxClips.TryGetValue(clipName, out AudioClip clip))
            {
                source.volume = _sfxVolume;
                source.PlayOneShot(clip);

                if (!_activeSFXSources.Contains(source))
                {
                    _activeSFXSources.Add(source);
                }
            }
            else
            {
                LogUtility.Error("AudioManager", $"SFX clip '{clipName}' not found!");
            }
        }

        public void SetMusicVolume(float volume)
        {
            LogUtility.ValidInfo("AudioManager.SetMusicVolume", $"{volume}");
            _musicVolume = Mathf.Clamp01(volume);
            musicSource.volume = _musicVolume;
        }

        public void SetSFXVolume(float volume)
        {
            LogUtility.ValidInfo("AudioManager.SetSFXVolume", $"{volume}");
            _sfxVolume = Mathf.Clamp01(volume);
            foreach (var source in _activeSFXSources)
            {
                if (source != null) source.volume = _sfxVolume;
            }
        }
    }
}
