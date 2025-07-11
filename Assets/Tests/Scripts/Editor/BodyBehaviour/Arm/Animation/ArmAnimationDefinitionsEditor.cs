using Assets.Tests.Scripts.BodyBehaviour.Arm.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Editors.Assets;
using UnityEditor;

namespace Assets.Tests.Scripts.Editor.BodyBehaviour.Arm.Animation
{
    [CustomEditor(typeof(ArmAnimationDefinitions))]
    internal class ArmAnimationDefinitionsEditor : AssetEditor
    {
    }
}
