using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sam Robichaud 2022
// NSCC-Truro
// Based on tutorial by (Comp - 3 Interactive)  * with modifications *

namespace UnderwaterHorror
{
    public class FirstPersonController_Sam : MonoBehaviour
    {
        public static FirstPersonController_Sam fpsSam;
        public bool canMove { get; private set; } = true;
        public bool canLook { get; private set; } = true;
        private bool isRunning => canRun && Input.GetKey(runKey);
        private bool shouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded;
        private bool shouldCrouch => Input.GetKeyDown(crouchKey) && !duringCrouchAnimation && characterController.isGrounded;

        #region Settings

        [Header("Functional Settings")]
        [SerializeField] private bool isDashing = false;
        public bool inWater = false;
        public bool carryingHeavyObj = false;
        [SerializeField] private bool canRun = true;
        [SerializeField] private bool canJump = true;
        [SerializeField] private bool canCrouch = true;
        [SerializeField] private bool canUseHeadbob = true;
        [SerializeField] private bool canSlideOnSlopes = true;
        [SerializeField] private bool canZoom = true;
        [SerializeField] private bool canInteract = true;
        [SerializeField] private bool useFootsteps = true;

        [Header("Controls")]
        [SerializeField] private KeyCode runKey = KeyCode.LeftShift;
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;
        [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
        [SerializeField] private KeyCode zoomKey = KeyCode.Mouse1;
        [SerializeField] private KeyCode interactKey = KeyCode.Mouse0;

        [Header("Move Settings")]
        [SerializeField] private float walkSpeed = 4.0f;
        [SerializeField] private float runSpeed = 10.0f;
        [SerializeField] private float crouchSpeed = 2.5f;
        [SerializeField] private float slopeSpeed = 12f;
        [SerializeField] private float suitWalkSpeed = 2.5f;
        [SerializeField] private float suitRunSpeed = 6.5f;
        [SerializeField] private float suitCrouchSpeed = 1.5f;
        [SerializeField] private float suitSlopeSpeed = 8.5f;
        [SerializeField] private float dashSpeed = 15f;
        [SerializeField] private float dashLength = 0.2f;
        private float dashTimer;


        [Header("Look Settings")]
        [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
        [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
        [SerializeField, Range(1, 180)] private float upperLookLimit = 70.0f;
        [SerializeField, Range(-180, 1)] private float lowerLookLimit = -70.0f;

        [Header("Jump Settings")]
        [SerializeField] private float jumpForce = 8.0f;
        [SerializeField] private float gravity = 30f;
        [SerializeField] private float waterJumpForce = 5.0f;
        [SerializeField] private float waterGravity = 15.0f;

        [Header("Crouch Settings")]
        [SerializeField] private float crouchHeight = 0.5f;
        [SerializeField] private float standingHeight = 1.8f;
        [SerializeField] private float timeToCrouch = 0.15f;
        [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
        [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);
        private bool isCrouching;
        private bool duringCrouchAnimation;

        [Header("Headbob Settings")]
        [SerializeField] private float crouchBobSpeed = 6f;
        [SerializeField] private float walkBobSpeed = 9f;
        [SerializeField] private float runBobSpeed = 12f;
        [SerializeField] private float waterCrouchBobSpeed = 3f;
        [SerializeField] private float waterWalkBobSpeed = 6f;
        [SerializeField] private float waterRunBobSpeed = 9f;

        [SerializeField] private float crouchBobAmount = 0.15f;
        [SerializeField] private float walkBobAmount = 0.3f;
        [SerializeField] private float runBobAmount = 0.45f;
        [SerializeField] private float waterCrouchBobAmount = 0.15f;
        [SerializeField] private float waterWalkBobAmount = 0.3f;
        [SerializeField] private float waterRunBobAmount = 0.45f;

        [SerializeField] private float footstepRangeTolerance = 0.1f;
        [SerializeField] private float lowerBobLimit = 0.5f;
        [SerializeField] private float upperBobLimit = 0.9f;
        [SerializeField] private float defaultYPos = 0;
        private float timer;

        [Header("CameraShake Settings")]
        [SerializeField] private float perlinY;
        [SerializeField] private float perlinX;
        [SerializeField] private float amplitudeY;
        [SerializeField] private float amplitudeX;
        [SerializeField] private float frequencyY;
        [SerializeField] private float frequencyX;
        [SerializeField] private float camShakeTime;
        [SerializeField] private float recoveryTimeMultiplier;
        [SerializeField] private bool camShakeReady;
        private float elapsedCamShakeTime;

        [Header("Zoom Settings")]
        [SerializeField] private float timeToZoom = 0.2f;
        [SerializeField] private float zoomFOV = 30f;
        private float defaultFOV;
        private Coroutine zoomRoutine;

        [Header("Footstep Settings")]
        [SerializeField] private float baseStepSpeed = 0.55f;
        [SerializeField] private float crouchStepMultiplier = 1.5f;
        [SerializeField] private float RunStepMultiplier = 0.6f;
        [SerializeField] private float waterStepSpeed = 1.8f;
        [SerializeField] private float waterCrouchStepMultiplier = 2.3f;
        [SerializeField] private float waterRunStepMultiplier = 1.2f;
        [SerializeField] private AudioSource movementAudioSource = default;
        [SerializeField] private AudioClip jumpJetClip = default;
        [SerializeField] private AudioClip jumpLandClip = default;
        [SerializeField] private AudioClip[] outsideFootstepClips = default;
        [SerializeField] private AudioClip[] insideFootstepClips = default;
        [SerializeField] private AudioClip[] grassFootstepClips = default;
        private float footstepTimer = 0f;

        private float GetCurrentOffset => (isCrouching && inWater) ? baseStepSpeed * waterCrouchStepMultiplier : (isRunning && inWater && !carryingHeavyObj) ? baseStepSpeed * waterRunStepMultiplier : inWater ? baseStepSpeed * waterStepSpeed : isCrouching ? baseStepSpeed * crouchStepMultiplier : (isRunning && !carryingHeavyObj) ? baseStepSpeed * RunStepMultiplier : baseStepSpeed ;

        // Sliding Settings
        private Vector3 hitPointNormal;
        private bool isSliding
        {
            get
            {
                if (characterController.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 5.0f))
                {
                    hitPointNormal = slopeHit.normal;

                    //prevents the player from jumping while sliding
                    if (Vector3.Angle(hitPointNormal, Vector3.up) > characterController.slopeLimit)
                    {
                        canJump = false;
                    }
                    else
                    {
                        canJump = true;
                    }
                    return Vector3.Angle(hitPointNormal, Vector3.up) > characterController.slopeLimit;
                }
                else { return false; }
            }
        }

        [Header("Interaction Settings")]
        [SerializeField] private Vector3 interactionRayPoint = new Vector3(0.5f, 0.5f, 0);
        [SerializeField] private float interactionDistance = 2.5f;
        [SerializeField] private LayerMask interactionLayer;
        [SerializeField] private Interactable currentInteractable;

        #endregion

        [SerializeField]
        public Camera playerCamera;
        private CharacterController characterController;

        private Vector3 moveDirection;
        private Vector2 currentInput;

        private float rotationX = 0;

        [SerializeField] private Vector3 NewGamePos = new Vector3(-0.1f, 1.71f, -20.95f);
        [SerializeField] private Quaternion NewGameRot = new Quaternion();
        public Vector3 playerSavedPosition;
        public Quaternion playerSavedRotation;


        private void Awake()
        {
            if (fpsSam == null)
            {
                fpsSam = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (fpsSam != null && fpsSam != this)
            {
                Destroy(this.gameObject);
            }

            playerCamera = GetComponentInChildren<Camera>();
            characterController = GetComponent<CharacterController>();
            defaultYPos = playerCamera.transform.localPosition.y;
            defaultFOV = playerCamera.fieldOfView;        
        }

        private void Start()
        {
            LockPlayerMovement();
        }

        private void Update()
        {
            if (GameManager.gameManager.gameState != GameManager.gameStates.gameplay) return;

            if (canMove)
            {
                HandleMovementInput();

                if (canJump)        { HandleJump();                                         }
                if (canCrouch)      { HandleCrouch();                                       }
                if (canUseHeadbob)  { HandleHeadBob();                                      }
                if (canZoom)        { HandleZoom();                                         }
                if (canInteract)    { HandleInteractionCheck(); HandleInteractionInput();   }
                //if (useFootsteps)   { HandleFootsteps();                                    }
                if (!inWater && carryingHeavyObj) 
                {
                    if (gameObject.GetComponentInChildren<HeavyObject>() == null) return;
                    gameObject.GetComponentInChildren<HeavyObject>().ForceDropObject();
                } 
                CameraShake();
                ApplyFinalMovement();
            }

            if (canLook)
            {
                HandleMouseLook(); // look into moving into Lateupdate if motion is jittery
            }

            EnergyDrain();
            PlayerStats.playerStats.RechargeSuit();
        }

        private void LateUpdate()
        {

        }

        public void LockPlayerMovement()
        {
            if (canMove == false) return; 
            //Debug.Log("Player Movement Locked");
            canMove = false;
            
        }

        public void UnlockPlayerMovement()
        {
            if (gameObject.transform.parent != null) return;
            if (canMove == true) return;
            Debug.Log("Player Movement Unlocked");
            canMove = true;           
        }
        
        public void UnlockPlayerCamera()
        {
            //if (Cursor.lockState == CursorLockMode.None) return;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            canLook = false;
        }

        public void LockPlayerCamera()
        {
            //if (Cursor.lockState == CursorLockMode.Locked) return;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            canLook = true;
        }

        private void HandleMovementInput()
        {
            // Read inputs
            currentInput = new Vector2(Input.GetAxisRaw("Vertical"), Input.GetAxis("Horizontal"));

            // normalizes input when 2 directions are pressed at the same time
            currentInput *= (currentInput.x != 0.0f && currentInput.y != 0.0f) ? 0.7071f : 1.0f;

            // Sets the required speed multiplier
            if (inWater) currentInput *= (isCrouching ? suitCrouchSpeed : (isRunning && PlayerStats.playerStats.suitPower > 0 && !carryingHeavyObj) ? suitRunSpeed : suitWalkSpeed);
            else currentInput *= (isCrouching ? crouchSpeed : (isRunning && !carryingHeavyObj) ? runSpeed : walkSpeed);

            float moveDirectionY = moveDirection.y;
            moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
            moveDirection.y = moveDirectionY;
        }

        private void HandleMouseLook()
        {
            // Rotate camera up/down
            rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
            rotationX = Mathf.Clamp(rotationX, lowerLookLimit, upperLookLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

            // Rotate player left/right
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);

        }

        private void HandleJump()
        {
            if (shouldJump)
            {
                AudioManager.audioManager.StopSound(movementAudioSource);
                AudioManager.audioManager.PlaySound(movementAudioSource, jumpJetClip);
                if (inWater)
                {
                    moveDirection.y = waterJumpForce;
                }
                else
                {
                    moveDirection.y = jumpForce;
                }
            }
        }

        private void HandleCrouch()
        {
            if (shouldCrouch)
            {
                StartCoroutine(CrouchStand());
            }
        }

        private void HandleHeadBob()
        {

            if (Mathf.Abs(moveDirection.x) < 0.1f && Mathf.Abs(moveDirection.z) < 0.1f)
            {
                //Debug.Log("Movement Magnitude insufficient for headbob. " + Mathf.Abs(moveDirection.x) + " " + Mathf.Abs(moveDirection.z));
                return;
            }

            if (useFootsteps && playerCamera.transform.localPosition.y <= (lowerBobLimit + footstepRangeTolerance)) { HandleFootsteps(); }
            
            if (!characterController.isGrounded)
            {
                //Debug.Log("Character not grounded, cancelling headbob");
                return;
            }
            
            if (inWater)
            {
                timer += Time.deltaTime * (isCrouching ? waterCrouchBobSpeed : (isRunning && PlayerStats.playerStats.suitPower > 0 && !carryingHeavyObj) ? waterRunBobSpeed : waterWalkBobSpeed);
                playerCamera.transform.localPosition = new Vector3(
                    playerCamera.transform.localPosition.x,
                    defaultYPos + (Mathf.Sin(timer) * (isCrouching ? waterCrouchBobAmount : (isRunning && PlayerStats.playerStats.suitPower > 0 && !carryingHeavyObj) ? waterRunBobAmount : waterWalkBobAmount))/2,
                    playerCamera.transform.localPosition.z);
            }
            else
            {
                timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : (isRunning && !carryingHeavyObj) ? runBobSpeed : walkBobSpeed);
                playerCamera.transform.localPosition = new Vector3(
                    playerCamera.transform.localPosition.x,
                    defaultYPos + (Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : (isRunning && !carryingHeavyObj) ? runBobAmount : walkBobAmount))/2,
                    playerCamera.transform.localPosition.z);
            }
        }

        public void SetCamShakeReady()
        {
            camShakeReady = true;
        }

        private void CameraShake()
        {
            if (camShakeReady)
            {
                if (elapsedCamShakeTime >= camShakeTime)
                {
                    elapsedCamShakeTime = 0;
                    camShakeReady = false;
                    return;
                }
                elapsedCamShakeTime += Time.deltaTime;
                playerCamera.transform.localPosition = new Vector3(PerlinShake(frequencyX, perlinX, amplitudeX), defaultYPos + PerlinShake(frequencyY, perlinY, amplitudeY), playerCamera.transform.localPosition.z);
            }
            else if (playerCamera.transform.localPosition.y != defaultYPos)
            {
                playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, new Vector3(0, defaultYPos, 0), Time.deltaTime * recoveryTimeMultiplier);
            }
        }

        private float PerlinShake(float frequency, float offset, float amplitude)
        {
            float value = ((Mathf.PerlinNoise(Time.time * frequency, offset) * 2) - 1) * amplitude;
            //Debug.Log(value);
            return value;
        }

        private void HandleZoom()
        {
            if (Input.GetKeyDown(zoomKey))
            {
                if (zoomRoutine != null)
                {
                    StopCoroutine(zoomRoutine);
                    zoomRoutine = null;
                }
                zoomRoutine = StartCoroutine(ToggleZoom(true));
            }

            if (Input.GetKeyUp(zoomKey))
            {
                if (zoomRoutine != null)
                {
                    StopCoroutine(zoomRoutine);
                    zoomRoutine = null;
                }
                zoomRoutine = StartCoroutine(ToggleZoom(false));
            }
        }

        private void HandleInteractionCheck()
        {
            if (Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance))
            {
                if (hit.collider.gameObject.layer == 10 && (currentInteractable == null || hit.collider.gameObject.GetInstanceID() != currentInteractable.GetInstanceID() ) )
                {
                    hit.collider.TryGetComponent(out currentInteractable);

                    if (currentInteractable)
                    {
                        currentInteractable.OnFocus();
                    }
                }
            }
            else if (currentInteractable)
            {
                currentInteractable.OnLoseFocus();
                currentInteractable = null; 
            }
        }

        private void HandleInteractionInput()
        {
            if (!InputManager.inputManager.eCycled) return;
            if (InputManager.inputManager.ePressed && currentInteractable != null && Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance, interactionLayer))
            {
                AudioManager.audioManager.StopSound(AudioManager.audioManager.playerInventoryAudio);
                AudioManager.audioManager.PlaySound(AudioManager.audioManager.playerInventoryAudio, AudioManager.audioManager.pickupItem);
                currentInteractable.OnInteract();
                InputManager.inputManager.SetECycledfalse();
            }
        }

        private void HandleFootsteps()
        {
            //if (!characterController.isGrounded) return;
            //if (currentInput == Vector2.zero) return;

            //footstepTimer -= Time.deltaTime;


            //if (footstepTimer <= 0)
            //{
            //Debug.LogWarning("Playing footsteps");
                if (Physics.Raycast(this.transform.position, Vector3.down, out RaycastHit hit, 3))
                {
                    if (movementAudioSource.isPlaying) return;
                    switch (inWater)
                    {
                        case true:
                            if (outsideFootstepClips.Length == 0) return;
                            AudioManager.audioManager.PlaySound(movementAudioSource, outsideFootstepClips[Random.Range(0, outsideFootstepClips.Length)]);
                            break;

                        case false:
                            if (insideFootstepClips.Length == 0) return;
                            AudioManager.audioManager.PlaySound(movementAudioSource, insideFootstepClips[Random.Range(0, insideFootstepClips.Length)]);
                            break;
                    }
                }

        }

        private void ApplyFinalMovement()
        {
            // Apply gravity if the character controller is not grounded
            if (!characterController.isGrounded)
            {
                if (inWater)
                {
                    moveDirection.y -= waterGravity * Time.deltaTime;
                }
                else
                {
                    moveDirection.y -= gravity * Time.deltaTime;
                }
            
            }

            if (characterController.velocity.y < - 1 && characterController.isGrounded)
                moveDirection.y = 0;


            // sliding
            if (canSlideOnSlopes && isSliding)
            {
                if (inWater)
                {
                    moveDirection += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * suitSlopeSpeed;
                }
                else
                {
                    moveDirection += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slopeSpeed;
                }
            }

            // dashing
            //UnderwaterDash();
        
        

            // applies movement based on all inputs
            characterController.Move(moveDirection * Time.deltaTime);
        }

        private IEnumerator CrouchStand()
        {
            if (isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1.0f, ~(LayerMask.GetMask("PostProcessing", "Water"))))
            {
                Debug.Log("CrouchBlocked");
                yield return null;
            }

                duringCrouchAnimation = true;

            float timeElapsed = 0;
            float targetHeight = isCrouching ? standingHeight : crouchHeight;
            float currentHeight = characterController.height;
            Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
            Vector3 currentCenter = characterController.center;

            while (timeElapsed < timeToCrouch)
            {
                characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
                characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
        
            characterController.height = targetHeight;
            characterController.center = targetCenter;

            isCrouching = !isCrouching;

            duringCrouchAnimation = false;
        }

        private IEnumerator ToggleZoom(bool isEnter)
        {
            float targetFOV = isEnter ? zoomFOV : defaultFOV;
            float startingFOV = playerCamera.fieldOfView; // capture reference to current FOV
            float timeElapsed = 0;

            while (timeElapsed < timeToZoom)
            {
                playerCamera.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, timeElapsed / timeToZoom);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            playerCamera.fieldOfView = targetFOV;
            zoomRoutine = null;
        }

        private void UnderwaterDash()
        {
            if (PlayerStats.playerStats.suitPower <= 0 || carryingHeavyObj) return;
            if (Input.GetButtonDown("Dash"))
            {
                Debug.Log("Dash Input Pressed");
                isDashing = true;
            }

            if (!inWater) 
            {
                isDashing = false;
                dashTimer = 0;
                return;
            }
            else if (!isDashing)
            {
                dashTimer = 0;
                return;
            }
            else
            {
                dashTimer += Time.deltaTime;
            }

            if (dashTimer < dashLength && isDashing)
            {
                Debug.Log("Dashing");
                Debug.Log(currentInput.x + " " + currentInput.y);
                moveDirection = ((transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y)) * dashSpeed;
            }
            else
            {
                PlayerStats.playerStats.SuitDashDrain();
                isDashing = false;
                return;
            }
        }

        private void EnergyDrain()
        {
            if (GameManager.gameManager.gameState != GameManager.gameStates.gameplay) return;
            if (!inWater) return;
            PlayerStats.playerStats.SuitPowerLogic();

            if (!isRunning || UI_Manager.ui_Manager.PDAOpen()) return;
            PlayerStats.playerStats.SuitSprintDrain();
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag("Water"))
            {
                this.inWater = true;
                this.gameObject.transform.GetComponentInChildren<FogEffect>().effectActive = true;
                this.gameObject.transform.GetComponentInChildren<UnderWaterEffect>().effectActive = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Water"))
            {
                IndoorTransition();
            }
        }

        public void IndoorTransition()
        {
            this.inWater = false;
            this.gameObject.transform.GetComponentInChildren<FogEffect>().effectActive = false;
            this.gameObject.transform.GetComponentInChildren<UnderWaterEffect>().effectActive = false;
            //Debug.Log("Out of water");
        }

        public void ResetRun()
        {
            isDashing = false;
            inWater = false;
            carryingHeavyObj = false;
            canRun = true;
            canJump = true;
            canCrouch = true;
            canUseHeadbob = true;
            canSlideOnSlopes = true;
            canZoom = true;
            canInteract = true;
            useFootsteps = true;
            canMove = true;
            canLook = true;
            carryingHeavyObj = false;
            currentInteractable = null;
            elapsedCamShakeTime = 0;
            transform.parent = null;
            playerCamera.transform.localPosition = new Vector3(0, defaultYPos, 0);
            this.gameObject.GetComponent<CharacterController>().enabled = false;
            this.gameObject.transform.position = NewGamePos;
            this.gameObject.transform.rotation = NewGameRot;
            //Debug.Log(this.gameObject.transform.position);
            this.gameObject.GetComponent<CharacterController>().enabled = true;
        }

        public void DisableCharacterController()
        {
            if (this.gameObject.GetComponent<CharacterController>().enabled == false) return;
            this.gameObject.GetComponent<CharacterController>().enabled = false;

        }

        public void EnableCharacterController()
        {
            if (this.gameObject.GetComponent<CharacterController>().enabled == true) return;
            this.gameObject.GetComponent<CharacterController>().enabled = true;
        }

        public bool IsCrouching()
        {
            if (isCrouching) return true;
            return false;
        }

        public bool IsRunning()
        {
            if (isRunning) return true;
            return false;
        }

        //use on buttons

        public void SaveCharacterState()
        {
            playerSavedPosition = transform.position;
            playerSavedRotation = transform.rotation;
        }

        public void LoadCharacterState()
        {
            Data_Manager.dataManager.UpdateFPSSam();
            DisableCharacterController();
            transform.position = playerSavedPosition;
            transform.rotation = playerSavedRotation;
            EnableCharacterController();
        }
    }

}
