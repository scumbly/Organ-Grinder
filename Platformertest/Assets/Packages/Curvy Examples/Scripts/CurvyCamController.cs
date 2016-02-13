// =====================================================================
// Copyright 2013-2015 Fluffy Underware
// All rights reserved
// 
// http://www.fluffyunderware.com
// =====================================================================

using UnityEngine;
using System.Collections;
using FluffyUnderware.Curvy.Controllers;
using FluffyUnderware.DevTools;
using UnityStandardAssets.ImageEffects;


namespace FluffyUnderware.Curvy.Examples
{
    public class CurvyCamController : SplineController
    {
        [Section("Curvy Cam")]
        public float MinSpeed;
        public float MaxSpeed;
        public float Mass;
        public float Down;
        public float Up;
        public float Fric = 0.9f;
        
        
        DepthOfField FX_DOF;
        

        protected override void OnEnable()
        {
            base.OnEnable();
            FX_DOF = GetComponent<DepthOfField>();
            Speed = MinSpeed;
        }

        protected override void Advance(ref float tf, ref int direction, CurvyController.MoveModeEnum mode, float absSpeed, CurvyClamping clamping)
        {
            base.Advance(ref tf, ref direction, mode, absSpeed, clamping);
            // Get directional vector    
            var tan = GetTangent(tf);
            float acc;
            // accelerate when going down, deccelerate when going up
            if (tan.y < 0)
                acc = Down * tan.y * Fric;
            else
                acc = Up * -tan.y * Fric;
            
            // alter speed
            Speed = Mathf.Clamp(Speed + Mass * acc * DeltaTime,MinSpeed,MaxSpeed);
            // stop at spline's end
            if (tf == 1)
                Speed = 0;
        }

        void OnPreRender()
        {
            // apply Depth Of field based on speed
            FX_DOF.aperture = Mathf.Lerp(0, 15, (Speed - MinSpeed) / (MaxSpeed - MinSpeed));
        }
    }
}
