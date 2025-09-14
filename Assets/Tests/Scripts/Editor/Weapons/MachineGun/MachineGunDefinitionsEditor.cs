using Assets.Tests.Scripts.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Editors.Assets;
using Tests.Weapons.MachineGuns;
using UnityEditor;

namespace Tests.Editors.Weapons.MachineGun
{
    [CustomEditor(typeof(Obsolete_MachineGunDefinitions))]
    public class MachineGunDefinitionsEditor : AssetEditor
    {
    }
}
