using UnityEngine;
using System.Collections;
using Crest;
using System;
using System.Collections.Generic;

namespace FPCSwimmer
{
    public class FPCSwimmer : MonoBehaviour
    {

        #region PARAMETERS

        [Header("Underwater particles")]
        [Space(20)]

        [Tooltip("If true, the underwater particles will be shown when underwater.")]
        public bool showUnderwaterParticles;

        [Tooltip("The particles that will be shown to the player when underwater.")]
        public GameObject underwaterParticles;

        [Header("Underwater sounds")]
        [Space(20)]

        [Tooltip("A list of underwater additional Audio Sources that will be controlled by the FPC swimmer, to " +
            "emulate the transition between land and underwater.")]
        public List<AudioSource> underwaterAdditionalAudioSources = new List<AudioSource>();

        [Tooltip("The collection of waves and splashes sounds used by this controller to play audio.")]
        public WaterSoundsCollection waterSoundCollection;

        [Tooltip("How much time before a new swimming sound is played when moving in water.")]
        public float timeBetweenSwimSound = 1f;

        [Tooltip("The volume of the sounds played when swimming.")]
        public float swimSoundVolume = 0.3f;

        [Tooltip("The minimum pitch that will be applied when swimming underwater.")]
        public float minPitchUnderwater = 0.4f;

        [Tooltip("A curve that represents the trend of the pitch when going underwater.")]
        public AnimationCurve swimSoundPitchCurve;

        [Tooltip("The volume of the underwater sounds when the player is on land.")]
        public float underwaterSoundVolumeWhenOnLand = 0f;

        [Tooltip("The volume of the underwater when the player is underwater.")]
        public float underwaterSoundVolumeWhenUnderground = 1f;


        [Header("Land sounds")]
        [Space(20)]

        [Tooltip("A list of land additional Audio Sources that will be controlled by the FPC swimmer, to " +
            "emulate the transition between land and underwater.")]
        public List<AudioSource> landAdditionalAudioSources = new List<AudioSource>();

        [Tooltip("The volume of the sounds of the Land Audio Source Object when the player goes underwater.")]
        public float landSoundVolumeWhenUnderwater = 0f;

        [Tooltip("The volume of the sounds of the landAudioSourceObject when the player is on land.")]
        public float landSoundVolumeWhenOnGround = 1f;


        [Header("Sounds transitions")]
        [Space(20)]

        [Tooltip("The threshold that will decide if the player transitioned from being underwater to being on land, " +
            " or viceversa. The higher the value, the more difficult will be for the player to switch from " +
            " being underwater (or land) to being on land (or underwater).")]
        public float emersionSoundThreshold = 0.1f;

        [Tooltip("How fast will the land volume change be when emerging / going underwater.")]
        public float submersionVolumeChangeSpeed = 10f;

        [Tooltip("How fast will the underwater volume change be when emerging / going underwater.")]
        public float emersionVolumeChangeSpeed = 10f;


        [Header("Water effects")]
        [Space(20)]

        [Tooltip("A particle that will simulate splash when going into the water.")]
        public ParticleSystem ImmersionWaterSplash;

        [Tooltip("How many particles to emit when entering the water.")]
        public int submergeParticlesCount = 5;

        [Tooltip("The distance of the splashes from the player when entering into the water.")]
        public float splashDistanceFromPlayerBody = 0.5f;

        [Tooltip("A particle that will simulate splash on left hand when swimming fast.")]
        public ParticleSystem LeftWaterSplash;

        [Tooltip("A particle that will simulate splash on right hand when swimming fast.")]
        public ParticleSystem RightWaterSplash;

        [Tooltip("When player's y - water y is lower than this value, the land sound volume will " +
            "be set back to the ground volume, otherwise to the underwater volume. " +
            "This is to simulate ears going underwater and sounds adapting to it.")]
        public float playerEarsFromWaterLevelThreshold = -0.2F;


        [Header("Mouse Look Settings")]
        [Space(20)]

        [Tooltip("The behavior of the cursor, just like the standard First Person Controllers.")]
        public CursorLockMode lockCursor;

        [Tooltip("How sensible the mouse is to rotations.")]
        public float mouseSensitivity = 100.0f;

        [Tooltip("How much the rotation angle will be clamped (from -clampAngle to + clampAngle).")]
        public float clampAngle = 80.0f;


        [Header("Speed settings")]
        [Space(20)]

        [Tooltip("How fast can the player swim when underwater.")]
        public float underwaterSpeed = 3.0f;

        [Tooltip("How fast can the player walk when on land.")]
        public float walkingSpeed = 10.0f;

        [Tooltip("The higher the value, the higher the speed of the player when running on land or " +
            "swimming fast underwater.")]
        public float runMultiplicator = 2.0f;


        [Header("Camera settings")]
        [Space(20)]

        [Tooltip("If enabled, the player's point of view will simulate the action of sumbersion / emersion" +
            " whenever entering / leaving the water.")]
        public bool elevateCamera = true;

        [Tooltip("How tall the point of view is when the player is on land.")]
        public float cameraElevationWhenOnGround = 10f;

        [Tooltip("How tall the point of view is when the player is floating over the water.")]
        public float cameraElevationWhenOnWater = 1f;

        [Tooltip("How fast does the player emerge when leaving the water going onto the land.")]
        public float cameraEmersionSpeed = 0.5f;

        [Tooltip("How fast does the player submerge when leaving the land going into the water.")]
        public float cameraSubmersionSpeed = 5f;


        [Header("Underwater physics propagation")]
        [Space(20)]

        [Tooltip("If true, the waves will move the player when it is not swimming.")]
        public bool enableWavesForcesOnPlayer = true;

        [Tooltip("Diameter of player, for physics purposes. The larger this value, the more filtered/smooth" +
            "the wave response will be.")]
        public float playerWidth = 2f;

        [Tooltip("Offsets center of the player to raise it (or lower it) in the water.")]
        public float raiseObject = 1f;

        [Tooltip("When the player is under the sea level by this threshold, it will be pushed towards the surface " +
            "automatically to come back over the sea level. This is to facilitate a floating behavior when waves are " +
            "really high. Set this to zero if you don't want this to happen.")]
        public float waterSurfaceFloatThreshold = 1f;

        [Tooltip("How fast will the player come back over the sea level when close to the water surface float threshold.")]
        public float floatingSpeed = 5f;

        [Tooltip("Vertical offset for where drag force should be applied."), SerializeField]
        private float _forceHeightOffset = -0.3f;

        [SerializeField] private float _dragInWaterRight = 2f;
        [SerializeField] private float _dragInWaterForward = 1f;


        [Header("Ground Behaviour")]
        [Space(20)]

        [Tooltip("Jump vertical velocity. The larger the value, the higher the jump."), SerializeField]
        public float jumpVelocity = 2.5f;
        [Tooltip("Gravity force. The smaller the value, the longer the jump."), SerializeField]
        public float gravityValue = -9.81f;

        #endregion

        #region PRIVATE PARAMETERS

        private float verticalVelocity = 0;

        private Vector3 playerVelocityVector;
        private float playerSpeed = 2.0f;

        private float rotY = 0.0f;                              // rotation around the up / y axis
        private float rotX = 0.0f;                              // rotation around the right / x axis
        private float currentWaterHeight = 0f;

        private AudioSource audioSource;
        private bool isSwimming_cache;

        private Rigidbody rb = null;
        private CharacterController cc = null;

        private Camera playerCamera;
        private bool isRunning = false;

        private float currentTimeSwimSound = 0f;
        private float lastPositionY;

        private float waterPlayerHeightDifference;
        private bool isSwimming;
        private bool isMoving;

        private bool lastSplashLeft = false;

        // If false, the waves will not apply any force to the player's rigidbody.
        private bool canWaveAddForce = true;
        private Vector3 lastWaveForceRight;
        private Vector3 lastWaveForceForward;

        // Internal audio sources for underwater and land sounds.
        private List<AudioSource> internalLandAudioSources = new List<AudioSource>();
        private List<AudioSource> internalUnderwaterAudioSources = new List<AudioSource>();

        // Flags to check if a sound can be played or not.
        private bool canPlayNextEmersionSound = true;
        private bool canPlayNextImmersionSound = true;

        // Time to wait before playing a new emersion / immersion / splash sound, to avoid overlapping
        // one shot sounds.
        private readonly float waitBeforeNextEmersionSound = 1f;
        private float currentWaitBeforeNextEmersionSound = 0f;
        private readonly float waitBeforeNextImmersionSound = 1f;
        private float currentWaitBeforeNextImmersionSound = 0f;

        #endregion

        #region CONSTANTS

        const string kInternalLandAudioSource = "LandAudioSource";
        const string kInternalUnderwaterAudioSource = "UnderwaterAudioSource";

        #endregion



        #region UNITY LIFE CYCLE
        private void Awake()
        {
            InitializeAudioSources();
        }

        private void Start()
        {
            // Get underwater background sound and initialize it on relative audio source
            InitializeUnderwaterAudio();

            // Init rigid body
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;

            // Init character controller
            cc = GetComponent<CharacterController>();

            Vector3 rot = transform.localRotation.eulerAngles;
            rotY = rot.y;
            rotX = rot.x;

            // Init player in water status
            UpdatePlayerInWaterStatus();

            // Get player's camera
            playerCamera = Camera.main;

            // Init last position
            lastPositionY = transform.localPosition.y;

            // Init isMoving flag
            isMoving = false;
        }

        private void LateUpdate()
        {
            // Update player in water status
            UpdatePlayerInWaterStatus();

            // Update position, taking into account if we were underwater or not.
            isMoving = CalculateNewPosition(isSwimming);

            // Updates waiting times before playing same sounds again
            UpdateSoundTimers();

            // Update player in water status
            UpdatePlayerInWaterStatus();

            // Update land sounds status
            UpdateLandSoundsStatus();

            // Update underwater sound status
            UpdateUnderwaterSoundStatus();

            // Update immersion and emersion audio
            UpdateImmersionEmersionAudioStatus();

            // Update underwater particles status
            UpdateUnderwaterParticlesStatus();

            // Update swimming sounds if swimming, according to speed
            UpdateUnderwaterAudioStatusWhenMoving();
        }
        #endregion



        #region UNDERWATER MANAGEMENT

        /// <summary>
        /// Returns true if the ears of the player are underwater
        /// </summary>
        /// <returns></returns>
        private bool AreEarsOfPlayerUnderwater()
        {
            return waterPlayerHeightDifference <= playerEarsFromWaterLevelThreshold && isSwimming;
        }

        private void CalculateWaterResponse()
        {
            // Use water response only if waves forces are enabled and no collision is happening.
            if (!enableWavesForcesOnPlayer || !canWaveAddForce)
            {
                return;
            }

            UnityEngine.Profiling.Profiler.BeginSample("SimpleFloatingObject.FixedUpdate");

            if (OceanRenderer.Instance == null)
            {
                UnityEngine.Profiling.Profiler.EndSample();
                return;
            }

            // Retrieve displacement to object, normal, water surface velocity from Crest.
            UnderWaterDetector.Master.GetDisplacementNormalSurfaceVel(
                playerWidth, out var _displacementToObject, out var normal, out var waterSurfaceVel);

            var undispPos = transform.position - _displacementToObject;
            undispPos.y = OceanRenderer.Instance.SeaLevel;

            //if (QueryFlow.Instance)
            //{
            //    _sampleFlowHelper.Init(transform.position, playerWidth);

            //    Vector2 surfaceFlow = Vector2.zero;
            //    _sampleFlowHelper.Sample(out surfaceFlow);
            //    waterSurfaceVel += new Vector3(surfaceFlow.x, 0, surfaceFlow.y);
            //}

            var velocityRelativeToWater = rb.velocity - waterSurfaceVel;

            var dispPos = undispPos + _displacementToObject;

            // apply drag relative to water
            var forcePosition = rb.position + _forceHeightOffset * Vector3.up;

            // Calculate wave forces
            lastWaveForceRight = transform.right * Vector3.Dot(transform.right, -velocityRelativeToWater) * _dragInWaterRight;
            lastWaveForceForward = transform.forward * Vector3.Dot(transform.forward, -velocityRelativeToWater) * _dragInWaterForward;

            rb.AddForceAtPosition(lastWaveForceRight, forcePosition, ForceMode.Acceleration);
            rb.AddForceAtPosition(lastWaveForceForward, forcePosition, ForceMode.Acceleration);

            UnityEngine.Profiling.Profiler.EndSample();
        }

        #endregion


        #region PLAYER MOVEMENTS

        /// <summary>
        /// Recalculates currentWaterHeight, waterPlayerHeightDifference, and isSwimming
        /// </summary>
        private void UpdatePlayerInWaterStatus()
        {
            // Get updated water height
            currentWaterHeight = UnderWaterDetector.Master.GetCurrentWaterHeight(this.transform, playerWidth);

            // Calculate water - player height difference
            waterPlayerHeightDifference = transform.position.y - currentWaterHeight;

            // Check if we are over or under water
            isSwimming = waterPlayerHeightDifference < UnderWaterDetector.Master.maxHeightAboveWater;
        }

        private void EmergePlayerCamera()
        {
            if (playerCamera.transform.localPosition.y < cameraElevationWhenOnGround)
            {
                // Slowly translate until it reaches that level, showing a sort of "standing up on ground" effect
                Vector3 delta = Vector3.up * Mathf.Lerp(0, cameraElevationWhenOnGround, cameraEmersionSpeed * Time.deltaTime);
                playerCamera.transform.localPosition += delta;
            }
        }

        private void SubmergePlayerCamera()
        {
            if (playerCamera.transform.localPosition.y > cameraElevationWhenOnWater)
            {
                // Slowly translate until it reaches that level, showing a sort of "immersion into water" effect
                Vector3 delta = Vector3.down * Mathf.Lerp(0, cameraElevationWhenOnWater, cameraSubmersionSpeed * Time.deltaTime);
                playerCamera.transform.localPosition += delta;
            }
        }

        private bool CalculateNewPosition(bool isSwimming)
        {
            if (elevateCamera)
            {
                // Update camera position in relation to the standing on ground / swimming into the water
                if (isSwimming)
                {
                    SubmergePlayerCamera();
                }
                else
                {
                    EmergePlayerCamera();
                }
            }

            // Ensure the cursor state is always updated
            Cursor.lockState = lockCursor;

            // Apply rotation according to the player's input, regardless of location movements.
            {
                rotY += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
                rotX += -Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
                rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
                transform.localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
            }

            // Start collecting information on the resulting motion for the player
            {
                // Reset from previous frame.
                playerVelocityVector = Vector3.zero;

                // Keep track of whether or not the character is walking or running
                isRunning = Input.GetButton("Run");

                // Calculate movement from horizontal and vertical axis inputs
                playerVelocityVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

                // Follow mouse direction as a moving direction
                playerVelocityVector = (transform.localRotation * playerVelocityVector);

                // Register jump input commands
                if (Input.GetButtonDown("Jump") && !isSwimming && cc.enabled && cc.isGrounded)
                {
                    // Reset vertical velocity to the one we desire
                    verticalVelocity = jumpVelocity;
                }

                // Update flag to report an eventual moving command
                isMoving = playerVelocityVector != Vector3.zero;
            }

            // Use gravity only when not swimming
            rb.useGravity = !isSwimming;

            // Split underwater behaviour from land behaviour
            if (isSwimming)
            {
                // Using rigidbody to move and receive water physics. This will allow to update
                // player's collisions and water response when going underwater.
                if (rb.IsSleeping())
                {
                    rb.WakeUp();
                }

                // Calculate external forces influencing the player's motion
                {
                    // Calculate water response to simulate wave motions over the player's body
                    CalculateWaterResponse();

                    // Update the speed the player can have when underwater.
                    playerSpeed = underwaterSpeed * (isRunning ? runMultiplicator : 1);

                    float currentWaterSurfaceY = UnderWaterDetector.Master.GetCurrentWaterHeight(this.transform, playerWidth);
                    if (transform.localPosition.y + playerVelocityVector.y > currentWaterSurfaceY)
                    {
                        // Correct y to be at most at the height of the sea
                        playerVelocityVector.y = currentWaterSurfaceY - transform.localPosition.y;
                    }
                    else if (!isMoving && currentWaterSurfaceY - transform.localPosition.y < waterSurfaceFloatThreshold)
                    {
                        // If the player is not moving and it is close to the surface, slowly push it towards the surface
                        // and try to keep it floating over the waves.
                        playerVelocityVector.y = currentWaterSurfaceY - transform.localPosition.y;
                    }
                }
            }
            else
            {
                // Disable rigidbody to not interfere with the players movements (we don't need physics).
                // Note: gravity will be calculated manually by this script.
                if (!rb.IsSleeping())
                {
                    rb.Sleep();
                }

                // Calculate external forces influencing the player's motion
                {
                    // Update the speed the player can have when on land.
                    // Note: allow change of speed only if grounded, otherwise we would go
                    // faster while jumping for example.
                    playerSpeed = walkingSpeed * (isRunning && cc.isGrounded ? runMultiplicator : 1);

                    // Update vertical position based on the current jump
                    // velocity and the gravity force, keeping in mind physics:
                    // Position = position0 + velocity0 * delta_t
                    // Velocity = velocity0 + gravity * delta_t
                    float newVerticalPositionOffset = verticalVelocity * Time.deltaTime;
                    verticalVelocity += gravityValue * Time.deltaTime;

                    // I divide the position by delta and player speed since I will multiply again
                    // the vector by that value after.
                    playerVelocityVector.y = newVerticalPositionOffset / (Time.deltaTime * playerSpeed);
                }
            }

            // Apply the movement to the player
            cc.Move(playerVelocityVector * Time.deltaTime * playerSpeed);

            // Return true if the player used inputs commands to move.
            return isMoving;
        }

        #endregion


        #region SOUND MANAGEMENT

        /// <summary>
        /// Gets the audio source of the specified prefab child.
        /// </summary>
        /// <param name="prefabChildName"> The name of the internal child from which
        /// we take the audio source.</param>
        /// <returns>The audio source of the specified prefab child, if it exists.</returns>
        private AudioSource GetAudioSourceFromPrefabChild(string prefabChildName)
        {
            Transform internalAudioSource = this.transform.Find(prefabChildName);
            return internalAudioSource != null ? internalAudioSource.GetComponent<AudioSource>() : null;
        }

        /// <summary>
        /// Initializes the internal audio sources that the FPC swimmer will use to manage audio on
        /// land and underwater.
        /// </summary>
        private void InitializeAudioSources()
        {
            AudioSource landAS = GetAudioSourceFromPrefabChild(kInternalLandAudioSource);
            AudioSource underwaterAS = GetAudioSourceFromPrefabChild(kInternalUnderwaterAudioSource);

            if (landAS != null)
            {
                // Add audio sources of our child game object for land audio source
                internalLandAudioSources.Add(landAS);
            }

            if (underwaterAS != null)
            {
                // Add audio sources of our child game object for underwater audio source
                internalUnderwaterAudioSources.Add(underwaterAS);
            }

            // Add the additional ones from the Inspector
            internalLandAudioSources.AddRange(landAdditionalAudioSources);
            internalUnderwaterAudioSources.AddRange(underwaterAdditionalAudioSources);
        }

        /// <summary>
        /// Loads the underwater audio clip from the audio collection and sets it in our internal audio source.
        /// </summary>
        private void InitializeUnderwaterAudio()
        {
            AudioSource underwaterAS = GetAudioSourceFromPrefabChild(kInternalUnderwaterAudioSource);

            // Init underwater audio
            if (underwaterAS != null)
            {
                underwaterAS.loop = true;

                if (waterSoundCollection)
                {
                    underwaterAS.clip = waterSoundCollection.GetUnderwaterSound();
                }
            }
        }

        /// <summary>
        /// Plays one shot the specified audio clip from the internal land audio source.
        /// E.g. Sound of splashes on water when swimming over the water surface.
        /// </summary>
        /// <param name="clip">The clip to play.</param>
        private void PlayLandAudioClipOneShot(AudioClip clip)
        {
            AudioSource landAS = GetAudioSourceFromPrefabChild(kInternalLandAudioSource);

            if (landAS != null)
            {
                landAS.PlayOneShot(clip);
            }
        }

        /// <summary>
        /// Plays one shot the specified audio clip from the internal underwater audio source.
        /// E.g. Sound of bubbles when the player breathes underwater.
        /// </summary>
        /// <param name="clip">The clip to play.</param>
        private void PlayUnderwaterAudioClipOneShot(AudioClip clip)
        {
            AudioSource underwaterAS = GetAudioSourceFromPrefabChild(kInternalUnderwaterAudioSource);

            if (underwaterAS != null)
            {
                underwaterAS.PlayOneShot(clip);
            }
        }

        /// <summary>
        /// Updates volume of land sounds according to the player's ears position.
        /// </summary>
        private void UpdateLandSoundsStatus()
        {
            // Update volume of all land sounds
            internalLandAudioSources.ForEach(
                x => x.volume =
                Mathf.Lerp(x.volume,
                    AreEarsOfPlayerUnderwater() ? landSoundVolumeWhenUnderwater : landSoundVolumeWhenOnGround,
                    Time.deltaTime * submersionVolumeChangeSpeed)
            );

            // Update internal land sound
            AudioSource src = GetAudioSourceFromPrefabChild(kInternalLandAudioSource);
            if (src != null)
            {
                if (!src.isPlaying && !AreEarsOfPlayerUnderwater())
                {
                    // Play land sound (note that the volume goes to zero when going underwater).
                    src.Play();
                }
            }
        }

        /// <summary>
        /// Updates volume of underwater sounds according to the player's ears position.
        /// </summary>
        private void UpdateUnderwaterSoundStatus()
        {
            // Update volume of all underwater sounds
            internalUnderwaterAudioSources.ForEach(
                x => x.volume =
                Mathf.Lerp(x.volume,
                    AreEarsOfPlayerUnderwater() ? underwaterSoundVolumeWhenUnderground : underwaterSoundVolumeWhenOnLand,
                    Time.deltaTime * emersionVolumeChangeSpeed)
            );

            // Update internal underwater sound
            AudioSource src = GetAudioSourceFromPrefabChild(kInternalUnderwaterAudioSource);
            if (src != null)
            {
                if (!src.isPlaying && AreEarsOfPlayerUnderwater())
                {
                    // Play underwater sound (note that the volume goes to zero when coming back to land).
                    src.Play();
                }
            }
        }

        /// <summary>
        /// Manages underwater audio when the player is moving or swimming.
        /// </summary>
        private void UpdateUnderwaterAudioStatusWhenMoving()
        {
            AudioSource underwaterAS = GetAudioSourceFromPrefabChild(kInternalUnderwaterAudioSource);

            if (isSwimming && isMoving && waterSoundCollection)
            {
                // Reduce pitch to simulate underwater effect
                // Note: it is reduced according to the depth reached underwater! :)
                float soundDepthImpact = swimSoundPitchCurve.Evaluate(transform.localPosition.y / currentWaterHeight);
                if (underwaterAS != null)
                {
                    underwaterAS.pitch = Mathf.Clamp(soundDepthImpact, 0.2f,
                        AreEarsOfPlayerUnderwater() ? minPitchUnderwater : 1);
                }

                if (currentTimeSwimSound < timeBetweenSwimSound)
                {
                    currentTimeSwimSound += Time.deltaTime * 2; //(isRunning ? Time.deltaTime * 2 : Time.deltaTime);
                }
                else
                {
                    // Makes sure the splash particles are always over the water surface, and show them if "running" on water.
                    if (isRunning)
                    {
                        UpdateSplashPositionAndStatus();
                    }

                    // Retrieve random splash or small wave sound depending on "running" swimming or slow swimming.
                    AudioClip splashSound = isRunning ?
                        waterSoundCollection.GetRandomSplashSound() :
                        waterSoundCollection.GetRandomSmallWaveSound();

                    if (splashSound)
                    {
                        // Play swim one shot and reset counter, but only if we had a current location change not null.
                        PlayLandAudioClipOneShot(splashSound);
                    }

                    currentTimeSwimSound = 0;
                }

                // If I was underwater and I quickly went up, I play one shot of emersion
                if (transform.localPosition.y - lastPositionY > emersionSoundThreshold && !AreEarsOfPlayerUnderwater())
                {
                    var emersionSound = waterSoundCollection.GetRandomEmersionSound();
                    if (canPlayNextEmersionSound) 
                    { 
                        canPlayNextEmersionSound = false; 
                        PlayLandAudioClipOneShot(emersionSound); 
                    }
                }

                lastPositionY = transform.localPosition.y;
            }
            else
            {
                currentTimeSwimSound = timeBetweenSwimSound;

                // Set pitch to normal value
                if (!isSwimming && underwaterAS != null)
                {
                    underwaterAS.pitch = 1f;
                }
            }
        }

        /// <summary>
        /// Updates audio and plays one shot of immersion / emersion sounds
        /// </summary>
        private void UpdateImmersionEmersionAudioStatus()
        {
            if (waterSoundCollection && isSwimming != isSwimming_cache)
            {
                if (isSwimming)
                {
                    // Play immersion sound one shot
                    var immersionSound = waterSoundCollection.GetRandomImmersionSound();
                    if (canPlayNextImmersionSound)
                    {
                        canPlayNextImmersionSound = false;
                        PlayLandAudioClipOneShot(immersionSound);

                        // Also play particles when playing this sound to simulate splash
                        // entering into water
                        PlaySubmergeSplashParticles();
                    }
                }
                else
                {
                    // Play emersion sound one shot
                    var emersionSound = waterSoundCollection.GetRandomEmersionSound();
                    if (canPlayNextEmersionSound) 
                    {
                        canPlayNextEmersionSound = false;
                        PlayLandAudioClipOneShot(emersionSound); 
                    }
                }
            }

            // Updates cache for next call
            isSwimming_cache = isSwimming;
        }

        #endregion


        #region TIMERS

        void UpdateSoundTimer(ref bool canPlay, ref float currentWait, float totalWait)
        {
            if (!canPlay)
            {
                if (currentWait <= 0)
                {
                    currentWait = totalWait;
                    canPlay = true;
                }
                else
                {
                    currentWait -= Time.deltaTime;
                }
            }
        }

        /// <summary>
        /// Updates the waiting times before playing the same sound again.
        /// </summary>
        void UpdateSoundTimers()
        {
            UpdateSoundTimer(ref canPlayNextEmersionSound, ref currentWaitBeforeNextEmersionSound, waitBeforeNextEmersionSound);
            UpdateSoundTimer(ref canPlayNextImmersionSound, ref currentWaitBeforeNextImmersionSound, waitBeforeNextImmersionSound);
        }

        #endregion


        #region PARTICLES

        /// <summary>
        /// Play particles when you go into water.
        /// </summary>
        private void PlaySubmergeSplashParticles()
        {
            if (ImmersionWaterSplash)
            {                
                Vector3 splashPosition = new Vector3(
                    this.transform.position.x,
                    currentWaterHeight, 
                    this.transform.position.z);

                // Offset splash to be in front of player with specified distance.
                splashPosition += transform.localRotation * Vector3.forward * splashDistanceFromPlayerBody;

                ParticleSystem spawned = Instantiate<ParticleSystem>(
                    ImmersionWaterSplash, splashPosition, Quaternion.identity);
                spawned.Emit((int)playerSpeed * submergeParticlesCount);
            }
        }

        /// <summary>
        /// Takes care of enabling / disabling underwater particles when underwater / on land.
        /// </summary>
        private void UpdateUnderwaterParticlesStatus()
        {
            if (showUnderwaterParticles && underwaterParticles)
            {
                underwaterParticles.SetActive(AreEarsOfPlayerUnderwater());
            }
        }

        private void UpdateSplashPositionAndStatus()
        {
            // Play random splash effect particle system left or right
            if (Mathf.Abs(transform.position.y - currentWaterHeight) < 2)
            {
                if (lastSplashLeft)
                {
                    if (LeftWaterSplash)
                    {
                        LeftWaterSplash.transform.position = new Vector3(LeftWaterSplash.transform.position.x, currentWaterHeight, LeftWaterSplash.transform.position.z);
                        LeftWaterSplash.Play();
                    }
                }
                else
                {
                    if (RightWaterSplash)
                    {
                        RightWaterSplash.transform.position = new Vector3(RightWaterSplash.transform.position.x, currentWaterHeight, RightWaterSplash.transform.position.z);
                        RightWaterSplash.Play();
                    }
                }

                lastSplashLeft = !lastSplashLeft;
            }
        }

        #endregion



        #region COLLISIONS TRIGGERS

        void OnTriggerEnter(Collider other)
        {
            if (isSwimming && other.gameObject != this.gameObject && !rb.IsSleeping())
            {
                // Make sure that when we collide, if underwater and moved by the waves, we
                // stop the forces from the waves to avoid going through existing colliders.
                // We also apply an impulse force opposite to our velocity to stop us and
                // simulate the collusion.
                Vector3 forceVec = -rb.velocity.normalized;
                rb.AddForce(forceVec, ForceMode.Impulse);
                canWaveAddForce = false;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject != this.gameObject && !rb.IsSleeping())
            {
                canWaveAddForce = enableWavesForcesOnPlayer;
            }
        }

        #endregion

    }
}