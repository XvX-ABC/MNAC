using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Tests.Obsolete_Assets
{

    [Serializable]
    public class Data
    {
        public string Field_0;
        public string Field_1;
        public string Field_2;
        public string Field_3;
    }
    public class JsonAsset_Test : MonoBehaviour
    {

        [SerializeField]
        JsonAssetAgent_Managed<Data> _assetAgent;
        [SerializeField]
        JsonAssetAgent_Managed<Data>[] _assetAgents;
        [SerializeField]
        PrefabAssetAgent_Managed _prefabAgent;
    }
}
