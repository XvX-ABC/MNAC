using Assets.Scripts.Utilities.Assets;
using Assets.Tests.Scripts.Weapons.Assets;
using Codice.CM.Common;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Assets.Tests.Scripts.Weapons.Editor.Assets
{
    //[CustomEditor(typeof(LauncherDefinitions_AB))]
    public class LauncherDefinitionsEditor : LauncherDefinitionsEditorBase<LauncherDefinitions_AB>
    {
    }
}
