using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextExternalEffect : MonoBehaviour
{
    enum Capitalization
    {
        Inherit,
        Uppercase,
        Lowercase
    }

    enum FontStyleEffect
    {
        Inherit,
        Bold,
        Italic
    }

    enum SpeechBubbleActivation
    {
        Inherit,
        On,
        Off
    }

    [SerializeField]
    TextMeshProAnimations[] _TMAnims = null;

    [SerializeField]
    bool _CancelOtherAnims = false;

    [SerializeField]
    float _FontSizeMultiplier = -1.0f;

    [SerializeField]
    float _AnimSpeedMultiplier = -1.0f;

    [SerializeField]
    Color _NewTextColor = Color.white;

    [SerializeField]
    Capitalization _TextCapitalization = Capitalization.Inherit;

    [SerializeField]
    FontStyleEffect _FontStyle = FontStyleEffect.Inherit;

    [Header("Font & Materials")]
    [SerializeField]
    TMP_FontAsset _OverrideFont = null;
    [SerializeField]
    Material _OverrideMaterial = null;

    [Header("Speech bubble")]
    [SerializeField]
    SpeechBubbleActivation _SpeechBubbleActivation = SpeechBubbleActivation.Inherit;

    [SerializeField]
    SpeechBubble _SpeechBubble = null;


    [Header("Particle system")]
    [SerializeField]
    ParticleSystem _ParticleSystem = null;

    [SerializeField]
    float _ParticleSystemCut = -1.0f;

    public Onomatopoeia TextOwner {get; set;} = null;
    // Start is called before the first frame update
    void Start()
    {
        if(TextOwner == null)
        {
            Debug.LogError($"No parent Onomatopoeia was set for {name}");
        }
        else
        {
            if(_OverrideFont != null)
            {
                TextOwner.SwapFont(_OverrideFont, _OverrideMaterial);
            }

            if(_CancelOtherAnims == true)
            {
                TextOwner.RemoveStandardAnimations();
            }

            if(_TextCapitalization != Capitalization.Inherit)
            {
                TextOwner.ForceCapitalization(_TextCapitalization == Capitalization.Lowercase ? TMPro.FontStyles.LowerCase : TMPro.FontStyles.UpperCase);
            }

            if(_FontStyle != FontStyleEffect.Inherit)
            {
                TextOwner.AddFontStyle(_FontStyle == FontStyleEffect.Bold ? TMPro.FontStyles.Bold : TMPro.FontStyles.Italic);
            }

            if(_NewTextColor != Color.white)
            {
                TextOwner.SwapColor(_NewTextColor);
            }

            if(_SpeechBubble != null)
            {
                TextOwner.OverrideSpeechBubble(_SpeechBubble);
            }

            if(_SpeechBubbleActivation != SpeechBubbleActivation.Inherit)
            {
                TextOwner.EnableSpeechBubble(_SpeechBubbleActivation == SpeechBubbleActivation.On ? true : false);
            }

            if(_FontSizeMultiplier >= 0.0f)
            {
                TextOwner.ExternalSizeModifier = _FontSizeMultiplier;
                TextOwner.ExternalSpeechBubbleScaleModifier *= _FontSizeMultiplier;
            }

            if(_AnimSpeedMultiplier >= 0.0f)
            {
                TextOwner.ExternalSpeedModifier = _AnimSpeedMultiplier;
            }

            foreach(TextMeshProAnimations anim in _TMAnims)
            {
                TextOwner.AddExternalAnimation(anim);
            }

            if(_ParticleSystem != null)
            {
                float stdDuration = _ParticleSystem.main.simulationSpeed * _ParticleSystem.main.duration;
                var particleSystemMain = _ParticleSystem.main;
                particleSystemMain.simulationSpeed *= TextOwner.SpeedCoefficient;

                particleSystemMain.startSpeedMultiplier *= TextOwner.SizeCoefficient;
                
                
                if(_ParticleSystemCut > 0.0f)
                    TextOwner.AddExternalAnimation(CutParticleSystem);
            }
        }
    }


    public void CutParticleSystem(TMP_Text textComp, TMP_CharacterInfo charInfo, float normalizedAnimProgress, ref Vector3[] vertices)
    {
        if(normalizedAnimProgress >= _ParticleSystemCut)
        {
            _ParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    
}
