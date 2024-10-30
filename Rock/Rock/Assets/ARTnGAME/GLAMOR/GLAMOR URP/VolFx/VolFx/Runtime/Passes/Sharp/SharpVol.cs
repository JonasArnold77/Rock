using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Artngame.GLAMOR.VolFx
{
    [Serializable, VolumeComponentMenu("VolFx/Sharp")]
    public sealed class SharpVol : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter         m_Radius  = new ClampedFloatParameter(0, 0, 1);
        public ClampedFloatParameter         m_Radial  = new ClampedFloatParameter(0, 0, 1);
        public ClampedFloatParameter         m_Samples = new ClampedFloatParameter(0, 0, 800);
        public ClampedFloatParameter         m_Aspect  = new ClampedFloatParameter(0, -1, 1);
        public NoInterpClampedFloatParameter m_Angle   = new NoInterpClampedFloatParameter(0, -360f, 360f);

        // =======================================================================
        // Can be used to skip rendering if false
        public bool IsActive() => active && (m_Radius.value > 0 || m_Radial.value > 0);

        public bool IsTileCompatible() => false;
    }
}