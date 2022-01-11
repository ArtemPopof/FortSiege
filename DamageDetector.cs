using Godot;

public class DamageController : Node {

    [Signal]
    public delegate void OnDestroyed();

    public bool destroyed;
    private float collisionEnergy;
    private Vector2 previousLinearVelocity;
    private RigidBody2D body;

    private int forceThreshold = 80000;
    private bool accelerationBasedDetection;

    private Vector2 firstContactPoint;

    private bool firstContact;

    public DamageController Init(RigidBody2D initBody)
    {
        destroyed = false;
        firstContact = true;
        accelerationBasedDetection = true;

        initBody.ContactsReported = 3;
        initBody.ContactMonitor = true;

        this.body = initBody;

        return this;
    }

    public DamageController WithForceThreshold(int force)
    {
        this.forceThreshold = force;

        return this;
    }

    public DamageController WithAccelerationBasedDetectionEnabled(bool enabled)
    {
        accelerationBasedDetection = enabled;
        return this;
    }

    public void CalculateDamage(Physics2DDirectBodyState physicsState)
    {
        if (destroyed) {
            return;
        }

        if (physicsState.GetContactCount() == 0) {
            previousLinearVelocity = body.LinearVelocity;
            return;
        }

        if (firstContact) {
            firstContactPoint = physicsState.GetContactColliderPosition(0);
            GD.Print("FIRST POINT: " + firstContactPoint);
            firstContact = false;
        }

        // for (int i = 0; i < physicsState.GetContactCount(); i++)
        // {
        //     GD.Print(physicsState.GetContactColliderObject(i));
        // }

        if (accelerationBasedDetection) {
            var accelerationForce = (previousLinearVelocity.LengthSquared() - body.LinearVelocity.LengthSquared()) / physicsState.Step;
            accelerationForce = Mathf.Abs(accelerationForce);
            
            //GD.Print("Force: " + accelerationForce);

            collisionEnergy = accelerationForce;

            if (collisionEnergy > forceThreshold) {
                GD.Print("Deadly energy: " + collisionEnergy);

                destroyed = true;

                EmitSignal("OnDestroyed");
            } 

            // var colliderObject = physicsState.GetContactColliderObject(0);
            // if ((colliderObject as Node2D).IsInGroup("Static")) {
            //     collisionEnergy = accelerationForce / 25;
            //     if (collisionEnergy > forceThreshold) GD.Print("FALLEN");
            // } else {
            //     if (previousLinearVelocity.y > previousLinearVelocity.x * 4) {
            //         collisionEnergy = collisionEnergy / 5;
            //         if (collisionEnergy > forceThreshold) GD.Print("FALLEN on rigid");
            //     } else {
            //         collisionEnergy = accelerationForce;
            //         if (collisionEnergy > forceThreshold) GD.Print("collided with some");
            //     }
            // }
        } else {
            collisionEnergy = 0;
        }

            if (firstContactPoint.DistanceTo(physicsState.GetContactColliderPosition(0)) < 400)
            {
               // GD.Print("GROUND SHAKING");
                collisionEnergy /= 5;
            }
            

        if (collisionEnergy > forceThreshold || collisionEnergy < forceThreshold * -1) {
            GD.Print("Deadly energy: " + collisionEnergy);
            
            destroyed = true;

            EmitSignal("OnDestroyed");
        }

        previousLinearVelocity = body.LinearVelocity;

        var dynamicCollider = FindNotStaticCollider(physicsState);
        if (dynamicCollider == null) { 
            return;
        }

        var energy = dynamicCollider.Weight * 5 * dynamicCollider.previousLinearVelocity.LengthSquared();
        if (energy > collisionEnergy) {
            //GD.Print("DYNAMIC COLLIDER: " + dynamicCollider);
            collisionEnergy = energy;
        }

        if (firstContactPoint.DistanceTo(physicsState.GetContactColliderPosition(0)) < 400)
            {
                //GD.Print("GROUND SHAKING");
                collisionEnergy /= 5;
            }

        if (collisionEnergy > forceThreshold || collisionEnergy < forceThreshold * -1) {
            GD.Print("Deadly energy: " + collisionEnergy);

            destroyed = true;

            EmitSignal("OnDestroyed");
        }
    }

     private Prefab FindNotStaticCollider(Physics2DDirectBodyState physicsState) {
        for (int i = 0; i < physicsState.GetContactCount(); i++) {
            var collider = (physicsState.GetContactColliderObject(0) as Node2D);
            if (collider is Prefab) {
                return collider as Prefab;
            }
        }

        return null;
    }
}