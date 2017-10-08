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
using Meta.Audio;
using UnityEngine;

namespace Meta.HandInput
{
    /// <summary>
    /// Cursor placed on back of hand will display when it has entered a grabbable collider 
    /// and will provide feedback for when it is grabbing.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class HandCursor: MetaBehaviour
    {
        /// <summary>
        /// Represents an edge of the viewport.
        /// </summary>
        private enum Boundary
        {
            Left = 0,
            Right = 1,
            Top = 2,
            Bottom = 3,
        }

        [SerializeField]
        private bool _playAudio = true;

        [SerializeField]
        private bool _showBoundsIndicator = false;

        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private Transform _ringSprite;

        [SerializeField]
        private SpriteRenderer _circleSpriteRenderer;

        [SerializeField]
        private GameObject _vicinityIndicator;

        [SerializeField]
        private Transform _outOfBoundsText;

        [SerializeField]
        private AudioRandomizer _grabAudio;

        [SerializeField]
        private AudioRandomizer _releaseAudio;

        [SerializeField]
        private Vector3 _outOfBoundsTextTopOffset = new Vector3(0, 2f, 0);

        [SerializeField]
        private Vector3 _outOfBoundsTextSideOffset = new Vector3(3f, 0, 0);

        private float _alpha;
        private float _targetAlpha;
        private float _alphaVelocity;
        private float _normalizedGrabResidual;

        private bool _isOutOfBounds;

        private Vector3 _initialRingScale;
        private Vector3 _initialBoundsTextPosition;
        private readonly Vector3 _targetRingScale = new Vector3(.2f, .2f, .2f);

        private Hand _hand;
        private AudioSource _audioSource;
        private SpriteRenderer _centerOutOfBoundsSpriteRenderer;
        private CenterHandFeature _centerHandFeature;
        private Transform _centerOutOfBoundsSprite;
        private PalmState _lastPalmState = PalmState.Idle;
        private IEventCamera _eventCamera;
        private Boundary _outofBoundary;

        // The approximate bounds of the usable FOV in screen space from the event camera. 
        // (Left, right) and (bottom, top)
        private Vector2 XBoundary = new Vector2(0.22f, 0.78f);
        private Vector2 YBoundary = new Vector2(0.04f, 0.7f);

        private const float MinCircleAlpha = 0.4f;
        private const string AnimateInStateName = "Appear";
        private const string AnimateOutStateName = "Disappear";

        public bool ShowBoundsIndicator
        {
            get { return _showBoundsIndicator; }
            set { _showBoundsIndicator = value; }
        }

        public bool PlayAudio
        {
            get { return _playAudio; }
            set { _playAudio = value; }
        }

        public AudioRandomizer GrabAudio
        {
            get { return _grabAudio; }
            set { _grabAudio = value; }
        }

        public AudioRandomizer ReleaseAudio
        {
            get { return _releaseAudio; }
            set { _releaseAudio = value; }
        }

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _hand = GetComponentInParent<Hand>();
            _centerHandFeature = GetComponent<CenterHandFeature>();
            _centerHandFeature.OnEngagedEvent.AddListener(OnGrab);
            _centerHandFeature.OnDisengagedEvent.AddListener(OnRelease);
            _initialRingScale = _ringSprite.localScale;
            _initialBoundsTextPosition = _outOfBoundsText.localPosition;
            _eventCamera = metaContext.Get<IEventCamera>();
        }

        private void LateUpdate()
        {
            transform.LookAt(Camera.main.transform);

            // Set normalized grab residual
            _normalizedGrabResidual = _centerHandFeature.PalmState != PalmState.Hovering ? 1f : LinearRemap(_hand.GrabValue, 0f, 2f, 1f, 0f);

            if (!float.IsNaN(_normalizedGrabResidual))
            {
                if (_centerHandFeature.PalmState != PalmState.Idle )
                {
                    _ringSprite.localScale = Vector3.Lerp(_initialRingScale, _targetRingScale, Mathf.Clamp(_normalizedGrabResidual, 0f, 1f));
                    if (!IsCursorAnimating(AnimateInStateName) && !IsCursorAnimating(AnimateOutStateName))
                    {
                        _alpha = Mathf.Lerp(MinCircleAlpha, 1, _normalizedGrabResidual);
                        _circleSpriteRenderer.color = new Color(_circleSpriteRenderer.color.r, _circleSpriteRenderer.color.g, _circleSpriteRenderer.color.b, _alpha);
                    }
                }
            }
            SetCursorVisualState();
        }

        /// <summary>
        /// Updates the visuals for the cursor which are not dependent upon the grab residual.
        /// </summary>
        private void SetCursorVisualState()
        {

            bool vicinityTargetState = false;
            bool outOfBounds = false;
            switch (_centerHandFeature.PalmState)
            {
                case PalmState.Hovering:
                    if (_lastPalmState == PalmState.Idle)
                    {
                        _animator.Play(AnimateInStateName);
                    }
                    break;
                case PalmState.Idle:
                    if (_lastPalmState == PalmState.Hovering)
                    {
                        _ringSprite.localScale = Vector3.one;
                        _animator.Play(AnimateOutStateName);
                    }
                    else if (_centerHandFeature.NearObjects.Count != 0)
                    {
                        vicinityTargetState = true;
                    }
                    break;
                case PalmState.Grabbing:
                    outOfBounds = IsOutOfBounds();
                    break;
            }
            if (_showBoundsIndicator)
            {
                ToggleOutOfBoundsIndicator(outOfBounds);
            }
            _isOutOfBounds = outOfBounds;
            _lastPalmState = _centerHandFeature.PalmState;
            _vicinityIndicator.SetActive(vicinityTargetState);
        }

        private void OnGrab(HandFeature handFeature)
        {
            ToggleEngageIndicator(true);
            PlayAudioClip(true);
        }

        private void OnRelease(HandFeature handFeature)
        {
            ToggleEngageIndicator(false);
            PlayAudioClip(false);
            _isOutOfBounds = false;
        }

        /// <summary>
        /// Checks if the hand is in the out of bounds region for the field of view.
        /// </summary>
        /// <returns>True, if the hand is is outside the pre-defined boundary regions.</returns>
        private bool IsOutOfBounds()
        {
            Vector3 screenPos = _eventCamera.EventCameraRef.WorldToViewportPoint(_centerHandFeature.Position);

            if (screenPos.x < XBoundary.x)
            {
                _outofBoundary = Boundary.Left;
            }
            else if (screenPos.x > XBoundary.y)
            {
                _outofBoundary = Boundary.Right;
            }
            else if (screenPos.y < YBoundary.x)
            {
                _outofBoundary = Boundary.Bottom;
            }
            else if (screenPos.y > YBoundary.y)
            {
                _outofBoundary = Boundary.Top;
            }
            else
            {
                return false;
            }
            return true;
        }

        private bool IsCursorAnimating(string animationName)
        {
            return _animator.GetCurrentAnimatorStateInfo(0).IsName(animationName);
        }

        private void PlayAudioClip(bool isGrabbing)
        {
            if (PlayAudio)
            {
                if (isGrabbing)
                {
                    _grabAudio.Play(_audioSource);
                }
                else
                {
                    _releaseAudio.Play(_audioSource);
                }
            }
        }

        private void ToggleOutOfBoundsIndicator(bool showBoundsIndicator)
        {
            if (!_isOutOfBounds && showBoundsIndicator)
            {
                _animator.Play("OutOfBounds");
            }
            else if (_isOutOfBounds && !showBoundsIndicator)
            {
                _animator.Play("InOfBounds");
            }
            Vector3 textPos = _initialBoundsTextPosition;
            switch (_outofBoundary)
            {
                case Boundary.Left:
                    textPos += _outOfBoundsTextSideOffset;
                    break;
                case Boundary.Right:
                    textPos -= _outOfBoundsTextSideOffset;
                    break;
                case Boundary.Top:
                    textPos -= _outOfBoundsTextTopOffset;
                    break;
                case Boundary.Bottom:
                    textPos += _outOfBoundsTextTopOffset;
                    break;
            }
            _outOfBoundsText.localPosition = textPos;
            _outOfBoundsText.gameObject.SetActive(showBoundsIndicator);
        }

        private void ToggleEngageIndicator(bool isGrabbed)
        {
            if (isGrabbed)
            {
                _animator.Play("Engage");
            }
            else
            {
                if (_isOutOfBounds)
                {
                    _animator.Play("DisengageOutOfBounds");
                }
                else
                {
                    _animator.Play("Disengage");
                }
            }
        }

        private float LinearRemap(float value, float valueRangeMin, float valueRangeMax, float newRangeMin, float newRangeMax)
        {
            return (value - valueRangeMin) / (valueRangeMax - valueRangeMin) * (newRangeMax - newRangeMin) + newRangeMin;
        }
    }
}
