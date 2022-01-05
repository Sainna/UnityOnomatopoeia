using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sainna.Onomatopoeia
{
public class ColliderImpact : MonoBehaviour
{
    //todo: this as list
    [SerializeField, Tooltip("The prefab that will be isntantiated on hit")]
    GameObject _CollisionEffect = null;

    [SerializeField, Tooltip("Collider based sound, null will use the effect's sound")]
    ColliderImpactSound _ColliderSound = null;

    [SerializeField, Tooltip("Minimum impact speed (input a square rooted value)")]
    Vector2 _MinSqrSpeed = new Vector2(0.25f, float.PositiveInfinity);

    [SerializeField]
    private bool _DisableTriggerEnter = false;

    [SerializeField, Tooltip("Ignore the SphereCast check in OnTriggerEnter (the sphere cast still need to be positive)")]
    bool _SkipTriggerCheck = false; // The SphereCast check will check that there is no item between what was detected as a hit and the actual action
    
    [SerializeField, Tooltip("The animation can only come from a script")]
    bool _ScriptActivationOnly = false;

    [SerializeField, Tooltip("Mute the sound (only in the standard SpawnAnimation")]
    bool _NoSound = false;

    [SerializeField, Tooltip("Force the detected impact velocity at a certain value (-1 to ignore)")]
    float _ForceSqrImpactVelocity = -1.0f;

    [SerializeField, Tooltip("Have the effect face the player")]
    Onomatopoeia.FacePlayerType _FacePlayerInfo = Onomatopoeia.FacePlayerType.AllDirection;

    [SerializeField, Tooltip("Transform that the Onomatopoeia will face (defaults to main camera)")]
    Transform _TransformToFace = null;

    [SerializeField, Tooltip("The mesh renderer (for material override)")]
    MeshRenderer _Renderer = null;

    [SerializeField, Tooltip("The mesh filter (for mesh override)")]
    MeshFilter _MeshFilter = null;

    [Space]
    [Header("Autoplay options")]
    [SerializeField, Tooltip("Times the hit effect should auto play")]
    int _AutoplayCount = 0;
    [SerializeField, Tooltip("Delay before first auto play")]
    float _InitialAutoplayDelay = 1.0f;
    [SerializeField, Tooltip("Delay between auto plays")]
    float _AutoplayDelay = 1.0f;

#pragma warning disable 0414
    [Header("Vibration options")]
    [SerializeField, Range(0.0f, 320.0f)]
    float _HapticsFrequency = 50.0f;
    [SerializeField, Range(0.0f, 1.0f)]
    float _HapticsAmplitude = 0.95f;

    float _HapticsDuration = 0.25f;
#pragma warning restore 0414


    [Header("Latency options")]

    [SerializeField]
    float _AudioLatency = 0.0f;

    //todo: try to use more native audio player than unity's
    //todo: reduce audio latency to the absolute minimum

    [SerializeField]
    float _VisualLatency = 0.0f;

    [Space]

    [Header("Optional")]
    [SerializeField]
    bool _IgnoreBottomCollision = false;

    [FormerlySerializedAs("OwnRb")] [SerializeField, Tooltip("Optionnal own-rigidbody")]
    Rigidbody _OwnRb = null;

    [SerializeField, Tooltip("Add an impulse force when touching this object")]
    bool _ForceOnImpact = false;

    public int InteractionCount {get; private set;} = 0; // Counts how many time the object was interacted with

    [SerializeField, Tooltip("Additional Effect prefab to also instantiate")]
    List<GameObject> _AdditionnalEffects = new List<GameObject>();

    [SerializeField, Tooltip("Which transform should the onomatopoeia parent to (defaults to this GameObject)")]
    private Transform _OnomatopoeiaParent = null;

    private Material DisplayOnHit = null; // Material cache when hiding the material until hit


    public bool WasTouched {get; private set;} = false; // was the object interacted with at least once
    public bool HasInteracted() => WasTouched;

    // Get the current effect length (before modifiers)
    // Warning: Only set after animation spawned (because not needed otherwise)
    public float BaseEffectTime {get; private set;} = 0.33f;

    [FormerlySerializedAs("OverrideString")] [SerializeField]
    string _OverrideString = null;

    public string OverrideStringGet => _OverrideString;


    [SerializeField]
    string[] _AlternateSuccessionStrings = null;
    int AlternateSuccessionStringIndex = 0;

    public void StopAllEffects()
    {
        if(_OnomatopoeiaParent != null)
        {
            foreach(Transform child in _OnomatopoeiaParent)
            {
                Destroy(child.gameObject);
            }
        }

        _ColliderSound?.CutAllSounds();
    }

    void Start()
    {
        if(_TransformToFace == null)
        {
            _TransformToFace = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }

        if(_AutoplayCount > 0)
        {
            StartCoroutine(DelayAutoplay(_CollisionEffect, !_NoSound));
        }
    }


    public void ActivateUnseen(Material unseen, bool invisible)
    {
        DisplayOnHit = _Renderer.material;
        if(unseen != null)
        {
            _Renderer.material = unseen;
        }

        if(invisible)
        {
            _Renderer.forceRenderingOff = true;
        }

    }

    IEnumerator DelayAutoplay(GameObject collisionEffect, bool playSound = true)
    {
        int playCount = 0;
        yield return new WaitForSeconds(_InitialAutoplayDelay);

        while(playCount < _AutoplayCount)
        {
            SpawnCustomAnimation(collisionEffect, playSound);
            playCount++;
            yield return new WaitForSeconds(_AutoplayDelay);
        }
    }

    //Warning: sound override only works when a ColliderImpactSound is present
    public void SpawnCustomAnimation(GameObject collisionEffect, bool playSound = true, AudioClip soundOverride = null, float volume = 1.0f)
    {
        if(collisionEffect != null)
        {
            GameObject hitEffect = Instantiate(collisionEffect, transform.position, Quaternion.LookRotation(-transform.forward, transform.up), _OnomatopoeiaParent);
            Onomatopoeia hitAnimation = hitEffect.GetComponentInChildren<Onomatopoeia>();
            if(hitAnimation)
            {
                if(_FacePlayerInfo != Onomatopoeia.FacePlayerType.None)
                {
                    hitAnimation.TextFacePlayer(_TransformToFace, _FacePlayerInfo);
                }

                if(playSound)
                {
                    if(_ColliderSound == null)
                    {
                        hitAnimation.StartSound(_AudioLatency);
                    }
                    else
                    {
                        // _ColliderSound?.StartSoundAtPosition(1.0f, transform.position, _AudioLatency);
                        _ColliderSound?.CreateAndPlayAudioSourceToOnomatopoeia(hitAnimation, volume, _AudioLatency, soundOverride);
                    }
                }

                foreach(GameObject effect in _AdditionnalEffects)
                {
                    if(effect != null)
                        hitAnimation.AddNewExternalEffect(effect);
                }
                hitAnimation.StartAnimation(_VisualLatency);
            }
        }
        else if(playSound)
        {
            //no hit effect selected, just play sound
            _ColliderSound?.StartSoundAtPosition(volume, transform.position, _AudioLatency, soundOverride);
        }
    }

    public void SpawnDefaultAnimation()
    {
        SpawnAnimation(transform.position, transform.up);
    }

    public void SpawnDefaultAnimationNoRotation()
    {
        SpawnAnimation(transform.position, transform.up, null, true);
    }


    public void SpawnDefaultAnimationVisualDelay(float delay)
    {
        _VisualLatency = delay;
        SpawnAnimation(transform.position, transform.up);
    }


    public void SpawnAnimation(Vector3 hitPoint, Vector3 hitNormal, Collider col = null, bool noRotation = false, float forceSqrVelocity = -1)
    {
        if(_IgnoreBottomCollision && hitNormal.y < 0)
        {
            return;
        }

        GameObject hitEffect;
        Onomatopoeia hitAnimation;
        Vector3 impactVelocity = Vector3.zero;
        Vector3 selfVelocity = Vector3.zero;

        if(_AlternateSuccessionStrings != null && _AlternateSuccessionStrings.Length > 0)
        {
            _OverrideString = _AlternateSuccessionStrings[AlternateSuccessionStringIndex];
            AlternateSuccessionStringIndex = (AlternateSuccessionStringIndex + 1) % _AlternateSuccessionStrings.Length;
        }

        if (col != null)
        {
            // Check if the object got hit by controller or by anything else
            // Controller controller = null;
            // if (col.tag == "ControllerLeft")
            // {
            //     controller = GameManager.Instance.VRInput.Controllers[Controller.LeftHand];
            // }
            // else if (col.tag == "ControllerRight")
            // {
            //     controller = GameManager.Instance.VRInput.Controllers[Controller.RightHand];
            // }
            
            // if (controller != null)
            // {
            //     //hit by controller
            //     impactVelocity = controller.GetVelocity();
            //     controller.Haptics(_HapticsDuration, _HapticsFrequency, _HapticsAmplitude);

            // }
            // else
            // {
                Rigidbody otherRb = col.attachedRigidbody;
                if (otherRb)
                {
                    impactVelocity = otherRb.velocity;
                }

                if (_OwnRb != null)
                {
                    selfVelocity = _OwnRb.velocity;
                    /* Debug.Log($"Self-Velocity of {name}:  {selfVelocity}| sqrMag: {selfVelocity.sqrMagnitude}"); */
                }
                
            // }


            if(_OwnRb && _ForceOnImpact)
            {
                _OwnRb.AddForceAtPosition(-hitNormal * impactVelocity.magnitude, hitPoint, ForceMode.Impulse);
            }
        }
        else
        {
            //effect on click for debug
            // impactVelocity = new Vector3(GameManager.Instance.Debug_Velocity, 0.0f);

            if(_OwnRb)
            {
                _OwnRb.AddForceAtPosition(Vector3.Normalize(transform.position - hitPoint) * impactVelocity.magnitude, hitPoint, ForceMode.Impulse);
            }
        }
        float sqrMagnitude = impactVelocity.sqrMagnitude + selfVelocity.sqrMagnitude;

        if(_ForceSqrImpactVelocity >= 0.0f)
        {
            sqrMagnitude = _ForceSqrImpactVelocity;
        }

        if(forceSqrVelocity >= 0.0f)
        {
            sqrMagnitude = forceSqrVelocity;
        }

        /*Debug.Log($"{transform.parent.name}/{name} -- {sqrMagnitude} >= {_MinSqrSpeed}: {sqrMagnitude >= _MinSqrSpeed}");*/

        if (sqrMagnitude >= _MinSqrSpeed.x && sqrMagnitude <= _MinSqrSpeed.y)
        {
            InteractionCount++;
            //todo: remove magic number, find good value
            //Todo: check if there is an object in front of the item?
            //todo: check if only changing rendering layer is enough
            //Force the direction vector of the animation to point upwards

            // GameManager.Instance?.Log(Logger.LogEvent.Interaction, $"Interacted with item {transform.parent.name}/{name} - Sqrd impact speed: {impactVelocity.sqrMagnitude} - Sqrd Item impact speed: {selfVelocity.sqrMagnitude} - Point of impact: {hitPoint}");

            if(Vector3.Dot(hitNormal, Vector3.up) < -0.1)
            {
                hitNormal = -hitNormal;
            }

            float soundAndSize = Mathf.Lerp(0.25f, 0.9f, sqrMagnitude / 15.0f);

            if(_CollisionEffect != null)
            {
                if(noRotation)
                {
                    hitEffect = Instantiate(_CollisionEffect, _OnomatopoeiaParent);
                }
                else
                {
                    //default
                    hitEffect = Instantiate(_CollisionEffect, hitPoint, Quaternion.FromToRotation(Vector3.up, hitNormal), _OnomatopoeiaParent);
                }
                hitAnimation = hitEffect.GetComponentInChildren<Onomatopoeia>();
                if(hitAnimation)
                {
                    //todo: better values + refactor
                    hitAnimation.SpeedModifier = Mathf.Lerp(0.5f, 1.25f, sqrMagnitude/15.0f);

                    hitAnimation.SizeModifier = soundAndSize;
                    hitAnimation.HeightModifier = soundAndSize;

                    hitAnimation.SoundCoefficient = soundAndSize;

                    BaseEffectTime = hitAnimation.AnimTime;

                    hitAnimation.OverrideText(_OverrideString);

                    if(_FacePlayerInfo != Onomatopoeia.FacePlayerType.None)
                    {
                        hitAnimation.TextFacePlayer(_TransformToFace, _FacePlayerInfo);
                    }

                    if(!_NoSound)
                    {
                        if(_ColliderSound == null)
                        {
                            hitAnimation.StartSound(_AudioLatency);
                        }
                        else
                        {
                            // _ColliderSound?.StartSoundAtPosition(soundAndSize + 0.2f, hitPoint, _AudioLatency);
                            _ColliderSound?.CreateAndPlayAudioSourceToOnomatopoeia(hitAnimation, soundAndSize + 0.2f, _AudioLatency);
                        }
                    }

                    foreach(GameObject effect in _AdditionnalEffects)
                    {
                        if(effect != null)
                            hitAnimation.AddNewExternalEffect(effect);
                    }

                    hitAnimation.StartAnimation(_VisualLatency);
                }
            }
            else if(!_NoSound)
            {
                //no hit effect selected, just play sound
                if(_ColliderSound != null)
                {
                    BaseEffectTime = _ColliderSound.StartSoundAtPosition(soundAndSize + 0.2f, hitPoint, _AudioLatency);
                }
            }


            // For the "unseen until hit" part of the xp
            if(DisplayOnHit != null)
            {
                OverrideMaterial(DisplayOnHit);
            }
            WasTouched = true;
        }
    }


    public void OverrideMaterial(Material newMat)
    {
        _Renderer.material = newMat;
    }


    public void OverrideTextEffect(GameObject newFX)
    {
        _CollisionEffect = newFX;
    }

    public void OverrideText(string newText)
    {
        _OverrideString = newText;
    }


    public void OverrideAdditionalEffects(GameObject[] newAddEffects)
    {
        if(newAddEffects == null)
            _AdditionnalEffects = new List<GameObject>();
        else
            _AdditionnalEffects = new List<GameObject>(newAddEffects);
    }

    public void AddAdditionalEffects(GameObject newAddEffect)
    {
        if(newAddEffect != null)
            _AdditionnalEffects.Add(newAddEffect);
    }

    public void AddAdditionalEffects(Material newMaterial)
    {
        Material[] oldMats = _Renderer.materials;
        Material[] newMats = new Material[oldMats.Length + 1];
        oldMats.CopyTo(newMats, 0);
        newMats[oldMats.Length] = newMaterial;
        _Renderer.materials = newMats;
    }

    public void OverrideMesh(Mesh newMesh)
    {
        if(_MeshFilter != null)
            _MeshFilter.mesh = newMesh;
    }

    //todo
    // public void OverrideTextEffect(List<GameObject> newFX = null)
    // {

    // }

    public void OverrideSoundEffect(List<AudioClip> newSounds)
    {
        _NoSound = false;
        _ColliderSound?.OverrideSE(newSounds);
    }

    public void OverrideSoundEffect(bool nosound = true)
    {
        _NoSound = nosound;
    }

    private List<Rigidbody> CurrentCollisionBody = new List<Rigidbody>();

    void OnCollisionEnter(Collision col)
    {
        if((col.rigidbody != null && CurrentCollisionBody.Contains(col.rigidbody)) || (_DisableTriggerEnter && col.collider.isTrigger))
            return;

        ContactPoint cPoint = col.GetContact(0);
        // RaycastHit hit;

        // Vector3 otherPos = col.rigidbody == null ? col.transform.position : col.rigidbody.position;
        // // Debug.DrawRay(col.transform.position, (col.ClosestPointOnBounds(transform.position) - col.transform.position) * 100, Color.magenta);
        // // if (Physics.Raycast(col.transform.position, col.ClosestPointOnBounds(transform.position) - col.transform.position, out hit, Mathf.Infinity, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        // if (Physics.SphereCast(cPoint.point, 0.005f, cPoint.point - otherPos, out hit, 10.0f))
        // {
        //     if (hit.collider.transform == this.transform || hit.transform == this.transform)
        //     {
                CurrentCollisionBody.Add(col.rigidbody);
                // Debug.Log($"{transform.parent.name}/{name} -- collision by {col.transform.parent.name}/{col.transform.name}");
                if(!_ScriptActivationOnly)
                    SpawnAnimation(cPoint.point, -cPoint.normal, col.collider);
        //     }
        // }
    }

    void OnCollisionExit(Collision col)
    {
        if(col.rigidbody != null && CurrentCollisionBody.Contains(col.rigidbody))
        {
            CurrentCollisionBody.Remove(col.rigidbody);
        }
    }




    void OnTriggerEnter(Collider col)
    {
        if(_ScriptActivationOnly || _DisableTriggerEnter)
            return;

        //Cast a sphere to check the actual point of contact of the controller trigger box to the collider
        //We could also try putting the controller as a solid collider (in order to use OnCollisionEnter), but even with a mass near 0 to glitches out frozen items such as the table
        RaycastHit hit;
        if (Physics.SphereCast(col.transform.position, 0.005f, col.ClosestPointOnBounds(transform.position) - col.transform.position, out hit, 10.0f))
            {
                if (_SkipTriggerCheck || hit.collider.transform == this.transform || hit.transform == this.transform)
                {
                    SpawnAnimation(col.transform.position, hit.normal, col);
                }
            }
        }
    }
}
