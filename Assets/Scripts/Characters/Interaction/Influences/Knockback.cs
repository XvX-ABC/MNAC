using System;
using MNAC.Interaction.Influences;
using MNAC.TPhysics.Locomotion;
using MNAC.Utilities.Timeline;
using MNAC.Utilities.Timeline.Events.Point;
using UnityEngine;
using LocomotionCore = MNAC.Characters.Humanoid.Locomotion.LocomotionCore;

namespace MNAC.Characters.Interaction.Influences
{
    internal class Knockback : InfluenceBase, IKnockback
    {
        internal class Locomotion : LocomotionModuleBase
        {
            Vector3 _force;
            bool _applyMass;
            internal float t;
            internal bool executed;
            public bool ApplyMass { get => _applyMass; set => _applyMass = value; }
            public Vector3 Force { get => _force; set => _force = value; }
            internal bool Executed { get => executed; }

            public override Context OnEnd(Context context)
            {
                return context;
            }

            public override Context OnStart(Context context)
            {
                var rbody = context.Rbody;
                var mode = _applyMass switch
                {
                    true => ForceMode.Impulse,
                    false => ForceMode.VelocityChange,
                };
                rbody.drag = 0;
                rbody.AddForce(_force, mode);
                executed = true;
                return context;
            }

            public override Context OnUpdate(Context context)
            {
                var rbody = context.Rbody;
                rbody.drag = Mathf.Lerp(0, rbody.drag, t);
                return context;
            }
        }
        LocomotionCore _locomotionCore;
        ITimeline _timeline;
        Locomotion _locomotion;
        public Knockback(LocomotionCore locomotionCore = null)
        {
            this.Enabled = true;
            _locomotion = new();
            this.locomotionCore = locomotionCore;
            _timeline = new Timeline(1.5f);
            _timeline.AddPointEvent(1, _ =>
            {
                _locomotionCore.DisableModule(_locomotion);
                _locomotion.executed = false;
                _locomotionCore.RemoveModule(_locomotion);
            });
        }
        public override string Name => "Knockback";

        internal LocomotionCore locomotionCore
        {
            get => _locomotionCore;
            set
            {
                _locomotionCore = value;
            }
        }

        public void ReceiveForce(Vector3 force, bool applyMass = false)
        {
            if (_locomotionCore == null)
                return;
            _locomotionCore.AddModule(_locomotion);
            _locomotionCore.EnableModule(_locomotion);
            _locomotion.Force = force;
            _locomotion.ApplyMass = applyMass;
            _timeline.Restart();
        }
        public override void Update()
        {
            base.Update();
            if (_locomotionCore == null)
                return;
            _locomotion.t = _timeline.NormalizedTime;
            _timeline.OnUpdate(Time.deltaTime);
            //if (_locomotion.executed)
            //{
            //    _locomotionCore.DisableModule(_locomotion);
            //    _locomotion.executed = false;
            //}
        }

    }
}
