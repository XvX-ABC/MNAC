using UnityEngine;

namespace Assets.Scripts.Arms.Actions
{
    public class Target : MonoBehaviour, IArmTarget
    {
        public Vector3 Position { get => this.transform.position; set => this.transform.position = value; }
    }
}
