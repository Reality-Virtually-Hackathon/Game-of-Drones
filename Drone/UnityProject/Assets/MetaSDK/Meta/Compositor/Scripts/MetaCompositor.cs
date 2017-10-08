// Copyright © 2017, Meta Company.  All rights reserved.
// 
// Redistribution and use of this software (the "Software") in binary form, without modification, is 
// permitted provided that the following conditions are met:
// 
// 1.      Redistributions of the unmodified Software in binary form must reproduce the above 
//         copyright notice, this list of conditions and the following disclaimer in the 
//         documentation and/or other materials provided with the distribution.
// 2.      The name of Meta Company (“Meta”) may not be used to endorse or promote products derived 
//         from this Software without specific prior written permission from Meta.
// 3.      LIMITATION TO META PLATFORM: Use of the Software is limited to use on or in connection 
//         with Meta-branded devices or Meta-branded software development kits.  For example, a bona 
//         fide recipient of the Software may incorporate an unmodified binary version of the 
//         Software into an application limited to use on or in connection with a Meta-branded 
//         device, while he or she may not incorporate an unmodified binary version of the Software 
//         into an application designed or offered for use on a non-Meta-branded device.
// 
// For the sake of clarity, the Software may not be redistributed under any circumstances in source 
// code form, or in the form of modified binary code – and nothing in this License shall be construed 
// to permit such redistribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDER "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL META COMPANY BE LIABLE FOR ANY DIRECT, 
// INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, 
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
// LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS 
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//-----------------------------------------------------------
// Copyright (c) 2017 Meta Company. All rights reserved.
//-----------------------------------------------------------
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Meta
{
    public class MetaCompositor : MonoBehaviour
    {
        // Stereo cameras
        public Camera _leftCam;
        public Camera _rightCam;

        // Debug values
        public bool _enableTimewarp = true;
        public bool _DebugAddLatency = false;
        public float _timewarpDt = 0.038f;

        // store last for comparison (we dont want to set this every frame)
        private bool _prevEnableTimewarp = true;
        private bool _prevDebugAddLatency = false;
        private float _prevTimewarpDt = 0.038f;

        // Use this for initialization
        IEnumerator Start()
        {
            MetaCompositorInterop.InitCompositor();

            // Setup rendertargets for stereo cameras
            var rt_left = new RenderTexture(2048, 2048, 24, RenderTextureFormat.ARGB32);
            var rt_right = new RenderTexture(2048, 2048, 24, RenderTextureFormat.ARGB32);
#if UNITY_5_5_OR_NEWER
            rt_left.autoGenerateMips = false;
            rt_right.autoGenerateMips = false;
#else
            rt_left.generateMips = false;
            rt_right.generateMips = false;
#endif
            rt_left.filterMode = rt_right.filterMode = FilterMode.Point;
            rt_left.Create();
            rt_right.Create();

            _rightCam.targetTexture = rt_right;
            _leftCam.targetTexture = rt_left;

            MetaCompositorInterop.SetEyeRenderTargets(
                rt_left.GetNativeTexturePtr(),
                rt_right.GetNativeTexturePtr(),
                rt_left.GetNativeDepthBufferPtr(),
                rt_right.GetNativeDepthBufferPtr()
            );

            // register the callback used before camera renders.  Unfortunately, Unity sets this for ALL cameras,
            //so we can't register a callback for a single camera only.
            Camera.onPreRender += preRender;

            //so we can't register a callback for a single camera only.
            Camera.onPostRender += postRender;

            //ensure that the right camera renders after the left camera.  We need the right camera
            //to render last since we call EndFrame on the Compositor via the right camera and the left camera to render first
            _rightCam.depth = _leftCam.depth + 1;

            yield return StartCoroutine("CallPluginAtEndOfFrames");
        }

        void preRender(Camera cam)
        {
            // perform all scene render initialization here.  The left eye camera gets rendered first,
            //so simply do all compositor setup if this is the OnPreRender call for the left camera.
            if (cam == _leftCam)
            {
                // Begin frame called in UpdateLocalizer for Slam
                // Update view matrices for the cameras
                UpdateCameraMatrices();
            }
        }

        void postRender(Camera cam)
        {
            // perform all scene render initialization here.  The left eye camera gets rendered first,
            //so simply do all compositor setup if this is the OnPreRender call for the left camera.
            if (cam == _rightCam)
            {
                //GL.IssuePluginEvent(MetaCompositorInterop.GetRenderEventFunc(), 1); 
            }
        }

        private void Update()
        {
            // Debugging options
            // -----------------
            if (_prevEnableTimewarp != _enableTimewarp)
            {
                MetaCompositorInterop.EnableTimewarp(_enableTimewarp ? 1 : 0);
                _prevEnableTimewarp = _enableTimewarp;
            }

            if (_prevTimewarpDt != _timewarpDt)
            {
                MetaCompositorInterop.SetTimewarpPrediction(_timewarpDt);
                _prevTimewarpDt = _timewarpDt;
            }

            if (_prevDebugAddLatency != _DebugAddLatency)
            {
                MetaCompositorInterop.SetAddLatency(_DebugAddLatency);
                _prevDebugAddLatency = _DebugAddLatency;
            }
        }

        //get camera matrices from the compositor
        void UpdateCameraMatrices()
        {
            //-------------- left eye --------------------
            Matrix4x4 viewLeftMatrix = Matrix4x4.identity;
            MetaCompositorInterop.GetViewMatrix(0, ref viewLeftMatrix);

            //set the final view matrix for left eye
            _leftCam.worldToCameraMatrix = viewLeftMatrix;

            //-------------- right eye --------------------
            Matrix4x4 viewRightMatrix = Matrix4x4.identity;
            MetaCompositorInterop.GetViewMatrix(1, ref viewRightMatrix);

            //set the final view matrix for right eye
            _rightCam.worldToCameraMatrix = viewRightMatrix;

            //-------------- left eye --------------------
            Matrix4x4 projLeftMatrix = Matrix4x4.identity;
            MetaCompositorInterop.GetProjectionMatrix(0, ref projLeftMatrix);

            //set the final proj matrix for left eye
            _leftCam.projectionMatrix = projLeftMatrix;

            //-------------- right eye --------------------
            Matrix4x4 projRightMatrix = Matrix4x4.identity;
            MetaCompositorInterop.GetProjectionMatrix(1, ref projRightMatrix);

            //set the final proj matrix for right eye
            _rightCam.projectionMatrix = projRightMatrix;
        }

        private IEnumerator CallPluginAtEndOfFrames()
        {
            // RenderLoop for compositor
            while (true)
            {
                // Wait until all frame rendering is done
                yield return new WaitForEndOfFrame(); // this waits for all cams

                GL.IssuePluginEvent(MetaCompositorInterop.GetRenderEventFunc(), 1); //calls EndFrame on Compositor.
            }
        }

        private void OnDestroy()
        {
            MetaCompositorInterop.ShutdownCompositor();
        }
    }
}
