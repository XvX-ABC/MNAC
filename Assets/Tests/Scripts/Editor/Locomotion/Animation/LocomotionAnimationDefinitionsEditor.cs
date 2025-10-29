using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Editors.Assets;
using Tests.Locomotion_Obsolete.Animation;
using UnityEditor;

namespace Tests.Editors.Locomotion.Animation
{
    [CustomEditor(typeof(LocomotionAnimationDefinitions))]
    internal class LocomotionAnimationDefinitionsEditor : AssetEditor
    {
    }
}
