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
using UnityEngine;

namespace Meta.HandInput
{
    [System.Serializable]
    public class HandData
    {
        private readonly float _startTime;

        /// <summary> Unique id for hand </summary>
        public int HandId        { get; private set; }
        /// <summary> Hand's top point </summary>
        public Vector3 Top       { get; private set; }
        /// <summary> Hand's palm anchor </summary>
        public Vector3 Palm      { get; private set; }
        /// <summary> Hand's anchor </summary>
        public Vector3 Anchor    { get; private set; }
        /// <summary> Hand's grab anchor </summary>
        public Vector3 GrabAnchor{ get; private set; }
        /// <summary> Hand's grab value </summary>
        public float GrabValue   { get; private set; }
        /// <summary> hand's HandType </summary>
        public HandType HandType { get; private set; }

        /// <summary> Total time since hand became visible. </summary>
        public float TimeAlive
        {
            get
            {
                return Time.time - _startTime;
            }
        }

        public HandData(meta.types.HandData cocoHand, Transform origin = null)
        {
            _startTime = Time.time;
            AdoptProperties(cocoHand, origin);
        }

        /// <summary>
        /// Applies hand properties from input meta.types.HandData to current hand.
        /// </summary>
        /// <param name="cocoHand">Input data</param>
        /// <param name="origin">Depth camera's transform</param>
        public void AdoptProperties(meta.types.HandData cocoHand, Transform origin = null)
        {
            HandId = cocoHand.HandId;

            HandType = cocoHand.HandType == meta.types.HandType.RIGHT ? HandType.Right : HandType.Left;
            GrabValue = cocoHand.IsGrabbing ? 0f : 2f;

            Anchor = Vec3TToVector3(cocoHand.HandAnchor, origin);
            GrabAnchor = Vec3TToVector3(cocoHand.GrabAnchor, origin);
            Palm = Vec3TToVector3(cocoHand.HandAnchor, origin);
            Top = Vec3TToVector3(cocoHand.Top, origin);
        }

        private static Vector3 Vec3TToVector3(meta.types.Vec3T? position, Transform origin = null)
        {
            return origin == null
                ? position.Value.ToVector3()
                : origin.localToWorldMatrix.MultiplyPoint3x4(position.Value.ToVector3());
        }
    };
}
