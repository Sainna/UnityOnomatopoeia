using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[ExecuteAlways]
public class Onomatopoeia : MonoBehaviour
{
    public enum FacePlayerType
    {
        None,
        AllDirection,
        ZOnly
    }


#region Serialized fields
    [Header("Base data")]
    [SerializeField]
    TMP_Text _TMPro = null;

    public TMP_Text TMPText => _TMPro;

    [SerializeField]
    Color _TextColor = Color.white;

    [SerializeField]
    Vector2 _RandomDirectionBoundary = new Vector2(-0.5f, 0.5f);

    [SerializeField]
    bool _EnableBaseAnim = true;

    [SerializeField]
    bool _AlwaysFacePlayer = false;

    FacePlayerType FacePlayerInfo = FacePlayerType.None;

    [SerializeField]
    bool _DestroyOnAnimEnd = true;

    [SerializeField]
    float _OnAnimEndDelay = 0;

    [SerializeField]
    Onomatopoeia _NextOnomatopoeia = null;

    [SerializeField]
    UnityEvent _OnAnimEnd = null;

    [SerializeField]
    AnimationCurve _climbAnim = null;

    [SerializeField]
    float _baseHeight = 1.0f;

    [SerializeField]
    float _baseDepth = 1.0f;

    [SerializeField]
    AnimationCurve _alphaAnim = null;

    [SerializeField]
    float _baseAnimTime = 1.0f;

    [SerializeField]
    float _baseFontSize = 3.0f;

    [SerializeField]
    TextMeshProAnimations.AnimEnum _TMAnim = 0;

    [SerializeField]
    Vector2 _AllowedDisplacementOnX = Vector2.zero;

    [SerializeField, Tooltip("The X displacement is applied locally to the text. warning: also used to change DestroyOnAnimEnd behaviour")]
    bool _XDisplacementOnLocal = true; //warning: also used to change DestroyOnAnimEnd behaviour

    Quaternion InitialRotation = Quaternion.identity;

    [SerializeField]
    bool _AllowMirrorOnX = false;

    [SerializeField]
    TextMeshProAnimations[] _AnimRefs = new TextMeshProAnimations[System.Enum.GetNames(typeof(TextMeshProAnimations.AnimEnum)).Length];


    [Header("Speech bubble options")]
    [SerializeField]
    SpeechBubble[] _SpeechBubbles = null;
    [SerializeField]
    bool _EnableSpeechBubble = false;
    [SerializeField]
    bool _EnableSpeechBubbleAnim = true;

    [SerializeField]
    bool _FreezeBubbleXScale = false;
    [SerializeField]
    bool _FreezeBubbleYScale = false;

    [SerializeField]
    AnimationCurve _SpeechBubbleSizeCurve = null;

    public Vector3 ExternalSpeechBubbleScaleModifier {get; set;} = Vector3.one;

    public Vector3 SpeechBubbleScaleModifierTotal => SizeModifier * ExternalSpeechBubbleScaleModifier;

    public AnimationCurve SpeechBubbleSizeCurve => _SpeechBubbleSizeCurve;
    private SpeechBubble SelectedSpeechBubble = null;


    [System.Serializable]
    public class AnimFrameEvent : UnityEvent <float>{ }

    [SerializeField]
    AnimFrameEvent _FrameEvents = null;


    [Header("Audio options (if no ColliderImpactSound)")]
    [SerializeField]
    AudioSource _AudioSource = null;
#endregion

#region Animation modifiers variables
    // FONT SIZE
    public float SizeModifier {get;set;} = 1.0f;
    public float ExternalSizeModifier {get;set;} = 1.0f;

    public float SizeCoefficient => SizeModifier * ExternalSizeModifier;
    public float FontSize => _baseFontSize * SizeModifier * ExternalSizeModifier;

    // ANIMATION SPEED (NOT LIFETIME!!!)
    public float SpeedModifier {get;set;} = 1.0f;
    public float ExternalSpeedModifier {get;set;} = 1.0f;
    public float SpeedCoefficient => SpeedModifier * ExternalSpeedModifier;

    // ANIMATION TIME
    public float AnimTime => _baseAnimTime;

    // TEXT LOCAL POSITION
    public float HeightModifier {get;set;} = 1.0f;
    public float Height => _baseHeight * HeightModifier;

    public float Depth => _baseDepth * HeightModifier;

#endregion
#region Animation play control variables

    private float StartTime;
    private float ElapsedTimeAtPause = 0.0f;

    public float SoundCoefficient = 1.0f;

    bool _IsAnimationPaused = true;
    private Vector3 initPosition;
    private Vector3 initScale;

    private Vector3 initPositionBubble;
    public Vector3 initScaleBubble;

    public AudioSource AudioSourceInUse {get; set;} = null;
#endregion

    private Transform PlayerCache = null;

    
    [SerializeField]
    bool _UseSoundProperty = false;
    public float CurrentSoundVolume = 1.0f;
    float SoundVolumeUpdateFrequency = 0.02f;
    float UpdateSoundTimer = 100.0f;


    // Start is called before the first frame update
    void Start()
    {
        ResetAnimation();

        if (!_IsAnimationPaused || !Application.IsPlaying(gameObject))
        {
            SetupAnimation();
        }
    }


    void OnDestroy() {
        if(SelectedSpeechBubble != null)
            SelectedSpeechBubble.transform.localScale = initScaleBubble;    
    }

    //just to see the variables in the editor
    float currentTime;
    float currentTimeNormalized;

    public delegate void OptAnimCharLoop(TMP_Text textComp, TMP_CharacterInfo charInfo, float normalizedAnimProgress, ref Vector3[] vertices);
    public delegate void OptAnimsLoopSetup(TMP_Text textComp, TMP_TextInfo charInfo, float normalizedAnimProgress);
    public delegate void OptAnimsSetup(TMP_Text textComp);
    private OptAnimCharLoop ExternAnimCharLoopDelegate = null;

    protected struct AnimationDelegate
    {
        public OptAnimsSetup Setup;
        public OptAnimCharLoop CharacterLoop;
        public OptAnimsLoopSetup Start;
        public OptAnimsLoopSetup End;

        public bool HasAnim { get; private set; }
        public void ClearAllDelegates()
        {
            CharacterLoop = null;
            Start = null;
            End = null;
            Setup = null;
            HasAnim = false;
        }

        public void AddDelegates(OptAnimsSetup setup, OptAnimsLoopSetup start, OptAnimCharLoop charLoop, OptAnimsLoopSetup end)
        {
            Setup += setup;
            Start += start;
            CharacterLoop += charLoop;
            End += end;
            HasAnim = true;
        }
    }

    private AnimationDelegate OptAnims;
    private AnimationDelegate ExternalAnims;

    /// <summary>
    /// Swap the text's font
    /// </summary>
    public void SwapFont(TMP_FontAsset newFont, Material newFontMaterial = null)
    {
        _TMPro.font = newFont;
        
        if(newFontMaterial != null)
            _TMPro.fontMaterial = newFontMaterial;
        else
            _TMPro.fontMaterial = newFont.material;
    }

    /// <summary>
    /// Change the text'S content
    /// </summary>
    public void OverrideText(string newText)
    {
        if(!string.IsNullOrEmpty(newText))
            _TMPro.text = newText;
    }

    /// <summary>
    /// Add a new ExternalEffect Prefab as a child of the onomatopoeia
    /// </summary>
    public void AddNewExternalEffect(GameObject toAdd)
    {
        GameObject exFxGO = Instantiate(toAdd, transform);

        TextExternalEffect exFx = exFxGO.GetComponent<TextExternalEffect>();
        exFx.TextOwner = this;
    }

    /// <summary>
    /// Removes ALL TextMeshProAnimations
    /// </summary>
    public void RemoveStandardAnimations()
    {
        OptAnims.ClearAllDelegates();
    }

    /// <summary>
    /// Add a TextMeshProAnimations Component external to the current GameObject
    /// </summary>
    /// <param name="animToAdd">The TextMeshProAnimations component to add</param>
    public void AddExternalAnimation(TextMeshProAnimations animToAdd)
    {
        // ExternAnimCharLoopDelegate += animToAdd.AnimationLoop;
        
        ExternalAnims.AddDelegates(animToAdd.AnimationSetup, animToAdd.AnimationStart, animToAdd.AnimationLoop, animToAdd.AnimationEnd);
        
    }

    /// <summary>
    /// Add an OptAnimCharLoop Delegate external to the current GameObject
    /// </summary>
    /// <param name="animCharLoopToAdd">The OptAnimCharLoop (optional animations) delegate to add</param>
    public void AddExternalAnimation(OptAnimCharLoop animCharLoopToAdd)
    {
        Debug.LogWarning("Adding animation via delegate only is DEPRECATED. Please update the calling code.");
        ExternAnimCharLoopDelegate += animCharLoopToAdd;
    }

    /// <summary>
    /// Change the text capitalization
    /// </summary>
    /// <param name="newCap">FontStyles.LowerCase or FontStyles.UpperCase</param>
    public void ForceCapitalization(TMPro.FontStyles newCap)
    {
        _TMPro.fontStyle &= ~FontStyles.LowerCase;
        _TMPro.fontStyle &= ~FontStyles.UpperCase;
        _TMPro.fontStyle |= newCap;
    }

    /// <summary>
    /// Add a new flag to the current font's style
    /// </summary>
    public void AddFontStyle(TMPro.FontStyles newStyle)
    {
        _TMPro.fontStyle |= newStyle;
    }

    /// <summary>
    /// Change the text color
    /// </summary>
    public void SwapColor(Color newColor)
    {
        _TextColor = newColor;
        _TMPro.color = newColor;
    }

    /// <summary>
    /// Change the SpeechBubble
    /// </summary>
    public void OverrideSpeechBubble(SpeechBubble newSpeechBubble = null)
    {
        if(SelectedSpeechBubble != null)
        {
            SelectedSpeechBubble.gameObject.SetActive(false);
        }

        if(newSpeechBubble != null)
        {
            SelectedSpeechBubble = newSpeechBubble;
            // initPositionBubble = SelectedSpeechBubble.transform.position;
            initScaleBubble = SelectedSpeechBubble.transform.localScale;
            SelectedSpeechBubble.transform.localScale = Vector3.zero;
            SelectedSpeechBubble.transform.localPosition = initPositionBubble;
            SelectedSpeechBubble.OwningOnomatopoeia = this;
        }
    }

    /// <summary>
    /// Enable the Speech Bubble component
    /// </summary>
    public void EnableSpeechBubble(bool enableSpeechBubble)
    {
        _EnableSpeechBubble = enableSpeechBubble;
        if(!_EnableSpeechBubble)
        {
            SelectedSpeechBubble.gameObject.SetActive(false);
        }
    }

    private void BaseAnimation()
    {
        if(_UseSoundProperty)
        {
            //draft snippet for using the sound amplitude instead of pre determined animation
            if(UpdateSoundTimer >= SoundVolumeUpdateFrequency)
            {
                float[] awawa = new float[2048];
                AudioSourceInUse.clip.GetData(awawa, AudioSourceInUse.timeSamples);
                CurrentSoundVolume = 0.0f;
                foreach(var aa in awawa)
                {
                    CurrentSoundVolume += Mathf.Abs(aa);
                }
                CurrentSoundVolume /= 2048.0f;
                UpdateSoundTimer = 0.0f;

                //high pass filter-like stuff
                if(CurrentSoundVolume - 0.075f < 0.0f || Mathf.Approximately(CurrentSoundVolume - 0.075f, 0.0f))
                {
                    CurrentSoundVolume = 0.0f;
                }
                //amplify what gets out but not too much
                else
                {
                    CurrentSoundVolume = Mathf.Lerp(0.5f, 1.0f, CurrentSoundVolume + 0.25f);
                }
            }

            _TMPro.alpha = CurrentSoundVolume;

            _TMPro.fontSize = CurrentSoundVolume * FontSize;

            //todo: remove new
            _TMPro.rectTransform.parent.localPosition = new Vector3(initPosition.x, 
                                                            initPosition.y + Mathf.Lerp(0, Height, CurrentSoundVolume), 
                                                            initPosition.z + Mathf.Lerp(0, Depth, CurrentSoundVolume));
        }
        else
        {
            CurrentSoundVolume = 1.0f;
            _TMPro.alpha = _alphaAnim.Evaluate(currentTimeNormalized * SpeedCoefficient);

            _TMPro.fontSize = (_climbAnim.Evaluate(currentTimeNormalized * SpeedCoefficient) * FontSize);

            //todo: remove new
            _TMPro.rectTransform.parent.localPosition = new Vector3(initPosition.x, 
                                                            initPosition.y + Mathf.Lerp(0, Height, (_climbAnim.Evaluate(currentTimeNormalized * SpeedCoefficient))), 
                                                            initPosition.z + Mathf.Lerp(0, Depth, (_climbAnim.Evaluate(currentTimeNormalized * SpeedCoefficient))));
        }
    }

    private void TMProAnimations()
    {
        _TMPro.ForceMeshUpdate();

        Vector3[] vertices = _TMPro.textInfo.meshInfo[0].vertices;

        int characterCount = _TMPro.textInfo.characterCount;

        float boundsMinX = _TMPro.bounds.min.x;
        float boundsMaxX = _TMPro.bounds.max.x;
        
        OptAnims.Start?.Invoke(_TMPro, _TMPro.textInfo, currentTimeNormalized);

        for (int i = 0; i < characterCount; i++)
        {
            TMP_CharacterInfo charInfo = _TMPro.textInfo.characterInfo[i];

            // Skip Characters that are not visible
            if (!charInfo.isVisible)
                continue;


            OptAnims.CharacterLoop?.Invoke(_TMPro, charInfo, currentTimeNormalized, ref vertices);

            // DEPRECATED 2021/11/09
            if(ExternAnimCharLoopDelegate != null)
                ExternAnimCharLoopDelegate.Invoke(_TMPro, charInfo, currentTimeNormalized, ref vertices);
            
            
            
        }

        var textInfo = _TMPro.textInfo;
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            _TMPro.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }
        
        OptAnims.End?.Invoke(_TMPro, _TMPro.textInfo, currentTimeNormalized);
    }

    private void AnimationEnd()
    {
        _TMPro.rectTransform.parent.localPosition = initPosition;
        _TMPro.rectTransform.localScale = initScale;
        _TMPro.fontSize = 0;
        _TMPro.alpha = 0;

        if(SelectedSpeechBubble)
        {
            SelectedSpeechBubble.transform.localPosition = initPositionBubble;
            SelectedSpeechBubble.transform.localScale = Vector3.zero;
        }

        #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                _TMPro.fontSize = _baseFontSize;
                _TMPro.alpha = 1;
            }
        #endif

        _OnAnimEnd.Invoke();


        _IsAnimationPaused = true;
        if(_NextOnomatopoeia != null)
        {
            _NextOnomatopoeia.gameObject.SetActive(true);
            _NextOnomatopoeia.SpeedModifier = SpeedModifier;

            _NextOnomatopoeia.SizeModifier = SizeModifier;
            _NextOnomatopoeia.HeightModifier = HeightModifier;

            _NextOnomatopoeia.SoundCoefficient = SoundCoefficient;

            if(PlayerCache != null)
                _NextOnomatopoeia.TextFacePlayer(PlayerCache);
            
            _NextOnomatopoeia.StartAnimation(_OnAnimEndDelay);

            Destroy(gameObject); 
        }
        else if(_DestroyOnAnimEnd)
        {
            if(_XDisplacementOnLocal && transform.parent != null && transform.parent.parent != null)
            {
                //dirty hack
                if (Application.isPlaying)
                {
                    Destroy(transform.parent.parent.gameObject);
                }
            }
            else if(transform.parent != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(transform.parent.gameObject);
                }
            }
            else
            {
                if (Application.isPlaying)
                {
                    Destroy(gameObject);
                }
            }
        } 
    }

    /// <summary>
    /// To be called each frame, progresses the Onomatopoeia Animation
    /// </summary>
    private void WordAnimation()
    {
        //todo: seperate in functions
        currentTime = Time.time - StartTime;
        currentTimeNormalized = currentTime / AnimTime;

        UpdateSoundTimer += Time.deltaTime;

        #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                currentTime = Time.realtimeSinceStartup - StartTime;
                currentTimeNormalized = currentTime / AnimTime;

                UpdateSoundTimer += Time.deltaTime;
            }
        #endif

        if(_AlwaysFacePlayer)
        {
            TextFacePlayer(PlayerCache, FacePlayerInfo, false);
        }

        if(currentTimeNormalized < 1.0f)
        {
            if (_EnableBaseAnim)
            {
                BaseAnimation();
            }

            if(OptAnims.HasAnim || ExternalAnims.HasAnim || 
               /* DEPRECATED 2021/11/09 */
               ExternAnimCharLoopDelegate != null )
            {
                TMProAnimations();
            }


            if (SelectedSpeechBubble)
            {
                float newX = initScaleBubble.x * SpeechBubbleScaleModifierTotal.x * (_FreezeBubbleXScale ? 1.0f : _SpeechBubbleSizeCurve.Evaluate(currentTimeNormalized * SpeedCoefficient));
                float newY = initScaleBubble.y*SpeechBubbleScaleModifierTotal.y*(_FreezeBubbleYScale ? 1.0f : _SpeechBubbleSizeCurve.Evaluate(currentTimeNormalized * SpeedCoefficient));

                SelectedSpeechBubble.transform.localScale = new Vector3(
                    newX,
                    newY,
                    initScaleBubble.z*SpeechBubbleScaleModifierTotal.z*_SpeechBubbleSizeCurve.Evaluate(currentTimeNormalized * SpeedCoefficient)
                );

                // SelectedSpeechBubble.transform.position = _TMPro.textBounds.center;
                //todo: scale with sound amplitude
            }

            _FrameEvents.Invoke(currentTimeNormalized);
        }
        else
        {
            AnimationEnd();
        }
    }


    /// <summary>
    /// Make the Onomatopoeia's Transform face the player
    /// </summary>
    //todo: better random for up vector
    public void TextFacePlayer(Transform player, FacePlayerType faceType = FacePlayerType.AllDirection, bool firstCall = true)
    {
        if(player)
        {
            Quaternion newRot = transform.parent.rotation;
            if(faceType == FacePlayerType.AllDirection)
            {
                Vector3 upVector;
                if(firstCall)
                {
                    upVector = new Vector3(Random.Range(_RandomDirectionBoundary.x, _RandomDirectionBoundary.y), 
                                            1.0f, 
                                            Random.Range(_RandomDirectionBoundary.x, _RandomDirectionBoundary.y));
                }
                else
                {
                    upVector = transform.up;
                }
                newRot = Quaternion.LookRotation(player.forward, upVector);
            }
            else if(faceType == FacePlayerType.ZOnly)
            {
                Vector3 forwardVect = transform.forward, upVect = transform.up;
                if(Vector3.Dot(transform.forward, player.position - transform.position) > 0)
                {
                    forwardVect = - forwardVect;
                }
                if(Vector3.Dot(transform.up, player.up) < -0.15)
                {
                    upVect = -upVect;
                }
                newRot = Quaternion.LookRotation(forwardVect, upVect);
            }
            // _TMPro.rectTransform.rotation = newRot;
            InitialRotation = transform.parent.rotation;
            transform.parent.rotation = newRot;
            // transform.rotation = newRot;
            PlayerCache = player;
            FacePlayerInfo = faceType;
        }
    }

    void Update()
    {
        if(!_IsAnimationPaused && Application.isPlaying)
        {
            WordAnimation();
        }
    }


    #if UNITY_EDITOR
    public bool EditorAnimation() {
        if(!_IsAnimationPaused)
        {
            WordAnimation();
        }
        return _IsAnimationPaused;
    }
    #endif

    
#region Animation control

    private void ResetAnimation()
    {
        initPosition = _TMPro.rectTransform.parent.localPosition;
        initScale = _TMPro.rectTransform.localScale;

        if(_XDisplacementOnLocal)
            initPosition.x += Random.Range(_AllowedDisplacementOnX.x, _AllowedDisplacementOnX.y);
        else
        {
            if(transform.parent != null && transform.parent.parent != null)
            {
                Vector3 parentLocal = transform.parent.parent.localPosition;
                parentLocal.x += Random.Range(_AllowedDisplacementOnX.x, _AllowedDisplacementOnX.y);
                transform.parent.parent.localPosition = parentLocal;
            }
        }

        if(_SpeechBubbleSizeCurve == null || _SpeechBubbleSizeCurve.keys.Length == 0)
        {
            _SpeechBubbleSizeCurve = new AnimationCurve(_climbAnim.keys);
            _climbAnim.keys.CopyTo(_SpeechBubbleSizeCurve.keys, 0);
        }

        if(_AllowMirrorOnX && Random.Range(0,2) == 1)
        {
            initPosition.x = -initPosition.x;
        }

        _TMPro.color = _TextColor;

        if (_SpeechBubbles != null && _EnableSpeechBubble)
        {
            //todo: shuffle the array to avoid too many repetition
            SelectedSpeechBubble = _SpeechBubbles[Random.Range(0, _SpeechBubbles.Length)];
            initPositionBubble = SelectedSpeechBubble.transform.localPosition;
            if(SelectedSpeechBubble.transform.localScale != Vector3.zero)
                initScaleBubble = SelectedSpeechBubble.transform.localScale;
            SelectedSpeechBubble.transform.localScale = Vector3.zero;
            SelectedSpeechBubble.OwningOnomatopoeia = this;
        }

        if (_EnableBaseAnim)
        {
            _TMPro.alpha = 0.0f;
            _TMPro.fontSize = 0.0f;
        }
        _TMPro.rectTransform.parent.localPosition = initPosition;

        // if (_SpeechBubbles != null && _EnableSpeechBubble)
        // {
        //     //todo: shuffle the array to avoid too many repetition
        //     SelectedSpeechBubble = _SpeechBubbles[Random.Range(0, _SpeechBubbles.Length)];
        //     SelectedSpeechBubble.transform.localScale = Vector3.zero;
        //     SelectedSpeechBubble.transform.localPosition = initPositionBubble;
        //     SelectedSpeechBubble.OwningOnomatopoeia = this;
        // }
    }

    public void SetupAnimation(bool alsoReset = false)
    {
        if (_baseAnimTime < 0)
            _baseAnimTime = float.PositiveInfinity;
        
        if(alsoReset)
        {
            StartSound();
            ResetAnimation();
        }

        if(_EnableSpeechBubble)
        {
            if(!_EnableSpeechBubbleAnim)
            {
                SelectedSpeechBubble.transform.localScale *= SizeModifier;
            }
            SelectedSpeechBubble.transform.Translate(0, Height, 0, Space.Self);
        }


        if (_TMAnim != 0)
        {
            var values = (TextMeshProAnimations.AnimEnum[])System.Enum.GetValues(typeof(TextMeshProAnimations.AnimEnum));
            
            
            OptAnims.ClearAllDelegates();
            for(int i = 0; i < values.Length; i++)
            {
                if(_TMAnim.HasFlag(values[i]))
                {
                    OptAnims.AddDelegates(_AnimRefs[i].AnimationSetup, _AnimRefs[i].AnimationStart, _AnimRefs[i].AnimationLoop, _AnimRefs[i].AnimationEnd);
                }
            }
            
            if(OptAnims.HasAnim)
                OptAnims.Setup.Invoke(_TMPro);
            
            float h,s,v;
            Color.RGBToHSV(_TMPro.color, out h, out s, out v);
            _TMPro.color = Color.HSVToRGB(h, s * Mathf.Lerp(0.5f, 1.5f, SizeModifier), v);

            // _TMPro.textInfo.ResetVertexLayout(false);


            TMP_TextInfo textInfo = _TMPro.textInfo;
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                var newVertexPositions = textInfo.meshInfo[i].vertices;

                // Upload the mesh with the revised information
                TextMeshProAnimations.UpdateMesh(_TMPro, newVertexPositions, 0);
            }

            _TMPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            _TMPro.ForceMeshUpdate(); // Generate the mesh and populate the textInfo with data we can use and manipulate.
        }

        StartTime = Time.time;
        #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                StartTime = Time.realtimeSinceStartup;
            }
        #endif
        _IsAnimationPaused = false;
    }

    void DefaultSetupAnimation()
    {
        SetupAnimation();
    }
    public void StartAnimation(float visualLatency = 0.0f)
    {
        if(!Mathf.Approximately(visualLatency, 0.0f))
        {
            Invoke("DefaultSetupAnimation", visualLatency);
        }
        else
        {
            _IsAnimationPaused = false;
        }
    }

    public void PauseAnimation()
    {
        ElapsedTimeAtPause = Time.time - StartTime;
        #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                ElapsedTimeAtPause = Time.realtimeSinceStartup - StartTime;
            }
        #endif
        _IsAnimationPaused = true;
    }

    public void ResumeAnimation()
    {
        StartTime = StartTime + ElapsedTimeAtPause;
        _IsAnimationPaused = false;
    }

    public void StopAnimation()
    {
        _IsAnimationPaused = true;
        Destroy(this.gameObject);
    }

    public TextMeshProAnimations GetAnimRef(TextMeshProAnimations.AnimEnum animType)
    {
        return _AnimRefs[Mathf.RoundToInt(Mathf.Log((int) animType, 2))];
    }
#endregion

#region Sound control
    public void StartSound(float audioLatency = 0.0f)
    {
        if(AudioSourceInUse == null)
            AudioSourceInUse = _AudioSource;

        if (AudioSourceInUse)
        {
            //todo: find good values for sound
            AudioSourceInUse.volume = SoundCoefficient + 0.2f;
            // sound.pitch += Random.Range(0.05f, -0.05f);

            //Audio latency requested
            if(!Mathf.Approximately(audioLatency, 0.0f))
            {
                AudioSourceInUse.Stop();
                AudioSourceInUse.PlayDelayed(audioLatency);
            }
            else
            {
                AudioSourceInUse.Play();
            }
        }



    }

#endregion

}
