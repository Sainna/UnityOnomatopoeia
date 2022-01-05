using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Sainna.Onomatopoeia
{
    public abstract class TextMeshProAnimations : MonoBehaviour
    {
        [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
        public class AnimType : PropertyAttribute
        {
            public System.Type AnimClass { get; }

            public AnimType(System.Type type)
            {
                AnimClass = type;
            }
        }

    
        //todo: reflection
        [System.Flags]
        public enum AnimEnum
        {
            [AnimType(typeof(TMAnimDangling))]
            Dangling = 1 << 0,
            [AnimType(typeof(TMAnimCircleCurve))]
            CircleCurve = 1 << 1,
            
            Unused = 1 << 2,
            [AnimType(typeof(TMAnimJitter))]
            Jitter = 1 << 3,
            [AnimType(typeof(TMAnimShakeB))]
            ShakeB = 1 << 4,
            [AnimType(typeof(TMAnimColor))]
            Color = 1 << 5,
            [AnimType(typeof(TMAnimSquishy))]
            Squishy = 1 << 6,
            [AnimType(typeof(TMAnimSmokey))]
            Smokey = 1 << 7,
            [AnimType(typeof(TMAnimStretch))]
            Stretch = 1 << 8,
            [AnimType(typeof(TMAnimKerning))]
            Kerning = 1 << 9,
            [AnimType(typeof(TMAnimSpeedBasedLinearTranslate))]
            SpeedBasedLinearTranslate = 1 << 10,
            [AnimType(typeof(TMAnim3DCurve))]
            Surface3D = 1 << 11,
            [AnimType(typeof(TMAnimShaderVertexOffset))]
            ShaderVertex = 1 << 12
        }

    
        protected Matrix4x4 matrix;

        public static void UpdateMesh(TMP_Text textComp, Vector3[] _vertex, int index)
        {
            textComp.mesh.vertices = _vertex;
            textComp.mesh.uv = textComp.textInfo.meshInfo[index].uvs0;
            textComp.mesh.uv2 = textComp.textInfo.meshInfo[index].uvs2;
            textComp.mesh.colors32 = textComp.textInfo.meshInfo[index].colors32;
        }
    
    
        public virtual void AnimationSetup(TMP_Text textComp)
        {
        
        }

        public virtual void AnimationStart(TMP_Text textComp, TMP_TextInfo textInfo, float normalizedAnimProgress)
        {
        
        }
    
        public virtual void AnimationEnd(TMP_Text textComp, TMP_TextInfo textInfo, float normalizedAnimProgress)
        {
        
        }

        public abstract void AnimationLoop(TMP_Text textComp, TMP_CharacterInfo charInfo, float normalizedAnimProgress, ref Vector3[] vertices);
    }
}