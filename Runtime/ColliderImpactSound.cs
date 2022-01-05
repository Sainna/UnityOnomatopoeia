using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sainna.Onomatopoeia
{
    public class ColliderImpactSound : MonoBehaviour
    {
        [SerializeField]
        AudioSource _AudioSource = null;
        [SerializeField, Tooltip("Possible audio clips for this onomatopoeia")]
        List<AudioClip> _AudioClips = new List<AudioClip>();


        [SerializeField]
        GameObject _AudioSourceSkeleton = null;

        [SerializeField]
        Transform _OneShotClipParent = null;

        public void OverrideSE(List<AudioClip> newList)
        {
            _AudioClips = newList;
        }

        public void StartSound(float volume)
        {
            StartSound(volume, 0.0f);
        }


        public void StartSound(float volume, float audioLatency)
        {
            if(_AudioClips.Count == 0)
                return;

            
            if (_AudioSource)
            {
                _AudioSource.clip = _AudioClips[Random.Range(0, _AudioClips.Count)];
                //todo: find good values for sound
                _AudioSource.volume = volume;
                // sound.pitch += Random.Range(0.05f, -0.05f);
            }

            //Audio latency requested
            if(!Mathf.Approximately(audioLatency, 0.0f))
            {
                _AudioSource.Stop();
                _AudioSource.PlayDelayed(audioLatency);
            }
            else
            {
                _AudioSource.Play();
            }
        }


        public void PlayClipAtPoint(AudioClip clip, Vector3 position, float volume, float audioLatency = 0.0f)
        {
            if(_AudioSourceSkeleton != null)
            {
                GameObject newSource = Instantiate(_AudioSourceSkeleton, position, Quaternion.identity, _OneShotClipParent);

                var newAudio = newSource.GetComponent<AudioSource>();
                newAudio.clip = clip;
                newAudio.volume = volume;
                newAudio.PlayDelayed(audioLatency);

                if(!newAudio.loop)
                    Destroy(newSource, audioLatency + clip.length + 1.0f);
            }
        }

        public float StartSoundAtPosition(float volume, Vector3 position, float audioLatency = 0.0f, AudioClip soundOverride = null)
        {
            if(_AudioClips.Count == 0 && soundOverride == null)
                return -1.0f;


            AudioClip clipToPlay = soundOverride != null ? soundOverride : _AudioClips[Random.Range(0, _AudioClips.Count)];


            if(_AudioSourceSkeleton != null)
            {
                PlayClipAtPoint(clipToPlay, position, volume, audioLatency);
            }
            else
            {
                //Audio latency requested
                if(!Mathf.Approximately(audioLatency, 0.0f))
                {
                    StartCoroutine(PlayClipAtPointDelayed(volume, position, audioLatency, clipToPlay));
                }
                else
                {
                    AudioSource.PlayClipAtPoint(clipToPlay, position, volume);
                }
            }

            return clipToPlay.length;
        }


        // Add an audio source to the specified onomatopoeia and set it as the source in use, then play the sound
        public float CreateAndPlayAudioSourceToOnomatopoeia(Onomatopoeia onomatope, float volume, float audioLatency = 0.0f, AudioClip soundOverride = null)
        {
            if(_AudioClips.Count == 0 && soundOverride == null)
                return -1.0f;

            AudioSource audioSource;

            if(!onomatope.transform.parent.TryGetComponent<AudioSource>(out audioSource))
                audioSource = onomatope.gameObject.AddComponent<AudioSource>();

            audioSource.spatialBlend = 1.0f;
            if(soundOverride == null)
                audioSource.clip = _AudioClips[Random.Range(0, _AudioClips.Count)];
            else
                audioSource.clip = soundOverride;

            audioSource.volume = volume;

            onomatope.AudioSourceInUse = audioSource;

            if(!Mathf.Approximately(audioLatency, 0.0f))
            {
                onomatope.AudioSourceInUse.PlayDelayed(audioLatency);
            }
            else
            {
                onomatope.AudioSourceInUse.Play();
            }

            return audioSource.clip.length;
        
        }




        private IEnumerator PlayClipAtPointDelayed(float volume, Vector3 position, float delay, AudioClip clipToPlay)
        {
            yield return new WaitForSeconds(delay);
            AudioSource.PlayClipAtPoint(clipToPlay, position, volume);
        }


        public void OverrideSE(AudioClip[] newClips)
        {
            _AudioClips = new List<AudioClip>(newClips);
        }


        public void CutAllSounds()
        {
            StopAllCoroutines();
            _AudioSource.Stop();

            if(_OneShotClipParent != null)
            {
                foreach (Transform child in _OneShotClipParent) {
                    Destroy(child.gameObject);
                }
            }
        }
    }
}