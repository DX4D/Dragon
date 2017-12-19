using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MalbersAnimations.Utilities
{
    public class EffectModifier : ScriptableObject
    {
        [TextArea]
        public string Description = string.Empty;

        public virtual void AwakeEffect(Effect effect) { }

        public virtual void StartEffect(Effect effect) { }

        public virtual void UpdateEffect(Effect effect) { }

        public virtual void LateUpdateEffect(Effect effect) { }

        public virtual void StopEffect(Effect effect) { }
    }
}