using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.Video;

namespace Tests.BT
{
    public abstract class Composite : Task
    {
        protected List<ITask> children;
        StringBuilder _sb;
        protected Composite()
        {
            children = new List<ITask>();
            _sb = new();
        }
        public override string ToString()
        {
            _sb.Clear();
            foreach (var c in children)
            {
                _sb.AppendLine(c.ToString());
            }
            return _sb.ToString();
        }
    }
}
