using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlatformApplyTiming : int
{
    None = 0,
    BehaviorStart = 1,
    BehaviorEnd = 2,
    OnObjectEnter = 4,
    OnObjectStay = 8,
    OnObjectExit = 16,
}

public abstract class PlatformBehaviorBase : MonoBehaviour
{
    public virtual void BehaviorStart( PlatformObject affectedPlatform ) { }
    public virtual void BehaviorEnd(PlatformObject changedTarget) { }
    public virtual void PhysicsUpdate( PlatformObject affectedPlatform ) { }
    public virtual void OnObjectPlatformEnter( PlatformObject affectedPlatform, GameObject standingTarget, Vector3 standingPoint, Vector3 standingNormal) { }
    public virtual void OnObjectPlatformStay( PlatformObject affectedPlatform, GameObject standingTarget, Vector3 standingPoint, Vector3 standingNormal) { }
    public virtual void OnObjectPlatformExit( PlatformObject affectedPlatform, GameObject exitTarget) { }
}
