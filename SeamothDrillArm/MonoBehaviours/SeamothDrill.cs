using UnityEngine;

namespace SeamothDrillArm.MonoBehaviours
{
    public class SeamothDrill : MonoBehaviour
    {
        public bool toggle;

        private float timeNextDrill;
        public bool isDrilling;

        private SeaMoth seamoth;

        void StopEffects()
        {
            // Stop sounds.
            Main.DrillLoop.Stop();
            Main.DrillLoopHit.Stop();
        }

        void Start()
        {
            // Get Seamoth component on the current GameObject.
            seamoth = GetComponent<SeaMoth>();
        }

        void Update()
        {
            // If its not selected, we don't want to run the rest of the function
            if (!toggle) return;

            // Some checks to see if we can drill or not.
            if (seamoth.modules.GetCount(SeamothModule.SeamothDrillModule) <= 0) return;
            if (!seamoth.GetPilotingMode()) return;
            if (Player.main.GetPDA().isOpen) return;

            // Update hovering.
            UpdateActiveTarget(seamoth);

            // If we're pressing the Left Mouse Button and we're not drilling
            if(GameInput.GetButtonDown(GameInput.Button.LeftHand) && !isDrilling)
            {
                // We're now set to drilling, and the drill will start 0.5 seconds from now
                isDrilling = true;
                timeNextDrill = Time.time + 0.5f;

                // Start the sound.
                Main.DrillLoop.Play();
            }
            
            // If we let up the Left Mouse Button
            if(GameInput.GetButtonUp(GameInput.Button.LeftHand))
            {
                // We're no longer drilling and sounds have stopped.
                isDrilling = false;
                StopEffects();
            }

            // If we can drill
            if(Time.time > timeNextDrill && isDrilling)
            {
                // Drill!
                Drill(seamoth);
                timeNextDrill = Time.time + 0.12f;
            }
        }

        void UpdateActiveTarget(Vehicle vehicle)
        {
            // Get the GameObject we're looking at
            var activeTarget = default(GameObject);
            Targeting.GetTarget(vehicle.gameObject, 6f, out activeTarget, out float dist, null);

            // Check if not null
            if (activeTarget != null)
            {
                // Get the root object, or the hit object if root is null
                var root = UWE.Utils.GetEntityRoot(activeTarget) ?? activeTarget;
                if (root.GetComponentProfiled<Drillable>())
                    activeTarget = root;
                else
                    root = null;
            }

            // Get the GUIHand component
            var guiHand = Player.main.GetComponent<GUIHand>();
            if (activeTarget)
            {
                // Send the Hover message to the GameObject we're looking at.
                GUIHand.Send(activeTarget, HandTargetEventType.Hover, guiHand);
            }
        }

        void Drill(SeaMoth moth)
        {
            var pos = Vector3.zero;
            var hitObj = default(GameObject);

            // Get the GameObject we're looking at
            UWE.Utils.TraceFPSTargetPosition(moth.gameObject, 6f, ref hitObj, ref pos, true);

            // Check if not null
            if (hitObj)
            {
                // Find the BetterDrillable component and play sounds.
                var drillable = hitObj.FindAncestor<BetterDrillable>();
                Main.DrillLoopHit.Play();

                // If we found the drillable
                if (drillable)
                {
                    // Send the "drill" message to the Drillable
                    drillable.OnDrill(transform.position, moth, out GameObject hitMesh);
                }
                else // Otherwise if we did not hit a drillable object
                {
                    // Get the LiveMixin component in the found GameObject
                    LiveMixin liveMixin = hitObj.FindAncestor<LiveMixin>();
                    if (liveMixin) // If not null
                    {
                        // Make it take a bit of damage
                        liveMixin.TakeDamage(4f, pos, DamageType.Drill, null);
                    }

                    // Also send a "hit" message.
                    hitObj.SendMessage("BashHit", this, SendMessageOptions.DontRequireReceiver);
                }
            }
            else // If its null
            {
                // Stop all sounds
                StopEffects();
            }
        }
    }
}
