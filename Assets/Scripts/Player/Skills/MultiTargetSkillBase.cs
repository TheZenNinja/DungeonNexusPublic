using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace Skills
{
    public abstract class MultiTargetSkillBase : SkillBase
    {
        public override SkillActivationType activationType => SkillActivationType.hold_release;

        [SerializeField] protected int maxTargets = 3;
        [SerializeField] protected List<Entity> targets;
        [Space]
        [SerializeField] protected LayerMask targetLayer;
        [SerializeField] protected LayerMask ignoreLayer;
        [SerializeField] protected int maxRange = 20;
        [Space]
        [SerializeField] protected GameObject targetUIPref;
        [SerializeField] protected List<GameObject> targetUIs;

        protected bool hasTargets => targets.Count > 0;
        protected bool hasMaxTargets => targets.Count >= maxTargets;

        public override void Initialize(Entity caster)
        {
            base.Initialize(caster);
            targets = new List<Entity>();   
        }

        public override void TapSkill(Entity caster)
        {
            if (IsOnCooldown)
                return;
            targets.Clear();
        }

        public override void HoldSkill(Entity caster, float deltaTime)
        {
            if (IsOnCooldown) 
                return;

            if (hasMaxTargets)
                return;

            if (TryToGetTarget(caster.GetSightRay(), out Entity entity))
                if (!targets.Contains(entity))
                {
                    targets.Add(entity);
                    targetUIs.Add(Instantiate(targetUIPref));
                }
        }

        private void LateUpdate()
        {
            if (targets.Count > 0)
                for (int i = 0; i < targets.Count; i++)
                    targetUIs[i].transform.position = targets[i].position;
        }

        protected bool TryToGetTarget(Ray ray, out Entity entity)
        {
            entity = null;
            if (Physics.Raycast(ray, out RaycastHit hit, maxRange, ~ignoreLayer))
                if (MyUtils.LayerIsInLayermask(targetLayer, hit.collider.gameObject.layer))
                    if (hit.collider.gameObject.TryGetComponent(out entity))
                        return true;
  
            return false;
        }

        public override void ReleaseSkill(Entity caster)
        {
            if (!hasTargets)
                return;

            foreach (Entity target in targets)
                Debug.Log(target);

            ClearLists();
        }

        public void ClearLists()
        {
            for (int i = 0; i < targetUIs.Count; i++)
                Destroy(targetUIs[i].gameObject);

            targets.Clear();
            targetUIs.Clear();
        }
    }
}
