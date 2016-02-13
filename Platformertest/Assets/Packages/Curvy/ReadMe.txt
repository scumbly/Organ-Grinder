// =====================================================================
// Copyright 2013-2015 Fluffy Underware
// All rights reserved
// 
// http://www.fluffyunderware.com
// =====================================================================


GETTING STARTED
===============
Visit http://www.fluffyunderware.com/curvy/ to access documentation, tutorials and references

EXAMPLE SCENES
==============
Checkout the example scenes at  "Packages/Curvy Examples/Scenes"!

NEED FURTHER HELP
=================
Visit our support forum at http://forum.fluffyunderware.com

VERSION HISTORY
===============
2.0.4
	[FIX] Added full Unity 5.3 compatibility
2.0.3
	[NEW] Added Pooling example scene
	[NEW] Added CurvyGLRenderer.Add() and CurvyGLRenderer.Delete()
	[FIX] CG graph not refreshing properly
	[FIX] CG module window background rendering transparent under Unity 5.2 at some occasions
	[FIX] Precise Movement over connections causing position warps
	[FIX] Fixed Curvy values resetting to default editor settings on upgrade
	[FIX] Control Points not pooled when deleting spline
	[FIX] Pushing Control Points to pool at runtime causing error
	[FIX] Bezier orientation not updated at all occasions
	[FIX] MetaCGOptions: Explicit U unable to influence faces on both sides of hard edges
	[FIX] Changed UITextSplineController to use VertexHelper.Dispose() instead of VertexHelper.Clear()
	[FIX] CurvySplineSegment.ConnectTo() fails at some occasions
2.0.2
	[NEW] Added range option to InputSplinePath / InputSplineShape modules
	[NEW] CG editor improvements
	[NEW] Added more Collider options to CreateMesh module
	[NEW] Added Renderer options to CreateMesh module
	[NEW] Added CurvySpline.IsPlanar(CurvyPlane) and CurvySpline.MakePlanar(CurvyPlane)
	[NEW] Added CurvyController.DampingDirection and CurvyController.DampingUp
	[FIX] Shift ControlPoint Toolbar action fails with some Control Points
	[FIX] IOS deployment code stripping (link.xml)
	[FIX] Controller Inspector leaking textures
	[FIX] Controllers refreshing when Speed==0
	[FIX] VolumeController not using individual faces at all occasions
	[FIX] Unity 5.2.1p1 silently introduced breaking changes in IMeshModifier
	[CHANGE] CurvyController.OrientationDamping now obsolete!
2.0.1
	[NEW] CG path rasterization now has a dedicated angle threshold
	[NEW] Added CurvyController.ApplyTransformPosition() and CurvyController.ApplyTransformRotation()
	[FIX] CG not refreshing as intended in the editor
	[FIX] CG not refreshing when changing used splines
	[FIX] Controllers resets when changing inspector while playing
	A few minor fixes and improvements
2.0.0 Initial Curvy 2 release


THIRD PARTY SOFTWARE USED BY CURVY
=======================================

poly2tri
--------

Poly2Tri Copyright � 2009-2010, Poly2Tri Contributors http://code.google.com/p/poly2tri/

All rights reserved. Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer
 in the documentation and/or other materials provided with the distribution. Neither the name of Poly2Tri nor the names of its 
 contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS �AS IS� AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, 
BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT 
SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

