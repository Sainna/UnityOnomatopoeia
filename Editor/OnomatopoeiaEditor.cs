using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Onomatopoeia))]
[CanEditMultipleObjects]
public class OnomatopoeiaEditor : Editor 
{

    SerializedProperty TMProRef;

    SerializedProperty TMColor;
    SerializedProperty TMAnim;
    SerializedProperty RandomDirectionBoundsVec2;
    SerializedProperty AlwaysFacePlayer;
    SerializedProperty UseBaseAnim;
    SerializedProperty DestroyOnAnimEnd;
    SerializedProperty NextOnomatopoeia;
    SerializedProperty OnAnimEndDelay;
    SerializedProperty ClimbAnim;
    SerializedProperty BaseHeight;
    SerializedProperty BaseDepth;
    SerializedProperty AlphaAnim;
    SerializedProperty BaseAnimTime;
    SerializedProperty BaseFontSize;
    SerializedProperty XAllowedDisp;
    SerializedProperty XDispOnLocal;
    SerializedProperty XAllowedMirror;


    SerializedProperty SpeechBubbleRefs;
    SerializedProperty UseSpeechBubble;
    SerializedProperty UseSpeechBubbleAnim;
    SerializedProperty SpeechBubbleAnimCurve;
    SerializedProperty FreezeBubbleScaleX;
    SerializedProperty FreezeBubbleScaleY;



    SerializedProperty AudioSourceRef;
    SerializedProperty AudioClips;


    SerializedProperty OptionnalAnimRefs;


    SerializedProperty UseSoundProperty;

    SerializedProperty FrameEvents;
    SerializedProperty OnAnimEnd;



    void OnEnable()
    {
        TMProRef = serializedObject.FindProperty("_TMPro");
        TMColor = serializedObject.FindProperty("_TextColor");

        TMAnim = serializedObject.FindProperty("_TMAnim");
        RandomDirectionBoundsVec2 = serializedObject.FindProperty("_RandomDirectionBoundary");

        AlwaysFacePlayer = serializedObject.FindProperty("_AlwaysFacePlayer");

        UseBaseAnim = serializedObject.FindProperty("_EnableBaseAnim");
        DestroyOnAnimEnd = serializedObject.FindProperty("_DestroyOnAnimEnd");
        OnAnimEndDelay = serializedObject.FindProperty("_OnAnimEndDelay");
        
        NextOnomatopoeia = serializedObject.FindProperty("_NextOnomatopoeia");
        OnAnimEnd = serializedObject.FindProperty("_OnAnimEnd");
        ClimbAnim = serializedObject.FindProperty("_climbAnim");
        BaseHeight = serializedObject.FindProperty("_baseHeight");
        BaseDepth = serializedObject.FindProperty("_baseDepth");
        AlphaAnim = serializedObject.FindProperty("_alphaAnim");
        BaseAnimTime = serializedObject.FindProperty("_baseAnimTime");
        BaseFontSize = serializedObject.FindProperty("_baseFontSize");
        XAllowedDisp = serializedObject.FindProperty("_AllowedDisplacementOnX");
        XAllowedMirror = serializedObject.FindProperty("_AllowMirrorOnX");
        XDispOnLocal = serializedObject.FindProperty("_XDisplacementOnLocal");


        AudioSourceRef = serializedObject.FindProperty("_AudioSource");


        SpeechBubbleRefs = serializedObject.FindProperty("_SpeechBubbles");
        UseSpeechBubble = serializedObject.FindProperty("_EnableSpeechBubble");
        UseSpeechBubbleAnim = serializedObject.FindProperty("_EnableSpeechBubbleAnim");
        SpeechBubbleAnimCurve = serializedObject.FindProperty("_SpeechBubbleSizeCurve");
        FreezeBubbleScaleX = serializedObject.FindProperty("_FreezeBubbleXScale");
        FreezeBubbleScaleY = serializedObject.FindProperty("_FreezeBubbleYScale");


        OptionnalAnimRefs = serializedObject.FindProperty("_AnimRefs");

        UseSoundProperty = serializedObject.FindProperty("_UseSoundProperty");
        FrameEvents = serializedObject.FindProperty("_FrameEvents");
    }

    void EditorAnimation(SceneView view)
    {
        Onomatopoeia target = ((Onomatopoeia)serializedObject?.targetObject);
        if(target && target.EditorAnimation())
        {
            SceneView.duringSceneGui -= EditorAnimation;
        }
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if(GUILayout.Button("Restart", GUILayout.ExpandWidth(true)))
        {
            SceneView.duringSceneGui -= EditorAnimation;
            ((Onomatopoeia)serializedObject.targetObject).SetupAnimation(true);

            if(!Application.isPlaying)
                SceneView.duringSceneGui += EditorAnimation;

        }
        if(GUILayout.Button("Reset text", GUILayout.ExpandWidth(true)))
        {
            ((Onomatopoeia)serializedObject.targetObject).TMPText.fontSize = BaseFontSize.floatValue;
            ((Onomatopoeia)serializedObject.targetObject).TMPText.alpha = 1;
        }
        if(GUILayout.Button("Pause", GUILayout.ExpandWidth(true)))
        {
            ((Onomatopoeia)serializedObject.targetObject).PauseAnimation();
        }
        EditorGUILayout.PropertyField(TMProRef);
        EditorGUILayout.PropertyField(TMColor);
        // EditorGUILayout.PropertyField(TMAnim);
        EditorGUILayout.PropertyField(BaseAnimTime);
        EditorGUILayout.PropertyField(FrameEvents);
        EditorGUILayout.PropertyField(DestroyOnAnimEnd);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(NextOnomatopoeia);
        EditorGUILayout.PropertyField(OnAnimEndDelay);
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.PropertyField(OnAnimEnd);

        
        EditorGUILayout.PropertyField(XAllowedDisp);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(XDispOnLocal);
        EditorGUILayout.PropertyField(XAllowedMirror);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.PropertyField(AlwaysFacePlayer);


        EditorGUILayout.PropertyField(UseSoundProperty);

        EditorGUILayout.PropertyField(RandomDirectionBoundsVec2);

        UseBaseAnim.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(UseBaseAnim.isExpanded, "Basic animations parameters");
        if (UseBaseAnim.isExpanded)
        {
            EditorGUILayout.PropertyField(UseBaseAnim);
            if(UseBaseAnim.boolValue)
            {
                EditorGUILayout.PropertyField(ClimbAnim);
                EditorGUILayout.PropertyField(BaseHeight);
                EditorGUILayout.PropertyField(BaseDepth);
                EditorGUILayout.PropertyField(AlphaAnim);
                EditorGUILayout.PropertyField(BaseFontSize);
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        OptionnalAnimRefs.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(OptionnalAnimRefs.isExpanded, "Optionnal animations parameters");
        if(OptionnalAnimRefs.isExpanded)
        {
            TMAnim.intValue = (int)(TextMeshProAnimations.AnimEnum)EditorGUILayout.EnumFlagsField("Text Animations", (TextMeshProAnimations.AnimEnum)TMAnim.intValue);
            TextMeshProAnimations.AnimEnum animEnum = (TextMeshProAnimations.AnimEnum)TMAnim.intValue;

            var values = (TextMeshProAnimations.AnimEnum[])System.Enum.GetValues(typeof(TextMeshProAnimations.AnimEnum));

            if (values.Length != OptionnalAnimRefs.arraySize)
            {
                OptionnalAnimRefs.arraySize = values.Length;
            }

            for(int i = 0; i < values.Length; i++)
            {
                
                if(animEnum.HasFlag(values[i]))
                {
                    EditorGUILayout.BeginHorizontal();
                    string valueStr = values[i].ToString();
                    SerializedProperty animRef = OptionnalAnimRefs.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(animRef, new GUIContent(valueStr));

                    // Get the correct Component type to add from the AnimEnum attributes
                    if(GUILayout.Button($"Add {valueStr}"))
                    {
                        System.Type animClass = ((TextMeshProAnimations.AnimType)System.Attribute.GetCustomAttribute(typeof(TextMeshProAnimations.AnimEnum).GetField(valueStr), 
                                                                                                typeof(TextMeshProAnimations.AnimType))).AnimClass;
                        animRef.objectReferenceValue = ((Onomatopoeia)serializedObject.targetObject).gameObject.AddComponent(animClass);
                    }
                    EditorGUILayout.EndHorizontal();
                    if (animRef.objectReferenceValue == null)
                    {
                        EditorGUILayout.HelpBox($"No references set for the {values[i].ToString()} animation!", MessageType.Error);
                    }
                }
            }

        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        UseSpeechBubble.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(UseSpeechBubble.isExpanded, "Show speech bubble parameters");
        if(UseSpeechBubble.isExpanded)
        {
            EditorGUILayout.PropertyField(UseSpeechBubble);
            if (UseSpeechBubble.boolValue)
            {
                EditorGUILayout.PropertyField(SpeechBubbleRefs);
                EditorGUILayout.PropertyField(UseSpeechBubbleAnim);
                EditorGUILayout.PropertyField(SpeechBubbleAnimCurve);
                EditorGUILayout.PropertyField(FreezeBubbleScaleX);
                EditorGUILayout.PropertyField(FreezeBubbleScaleY);
            }
            
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.PropertyField(AudioSourceRef);
        serializedObject.ApplyModifiedProperties();
    }
}