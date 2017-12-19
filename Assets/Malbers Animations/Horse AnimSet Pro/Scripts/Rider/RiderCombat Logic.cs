using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using MalbersAnimations.Weapons;

namespace MalbersAnimations.HAP
{
    ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    /// LOGIC
    ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    public partial class RiderCombat
    {
        #region Start Update LateUpdate
        void Start()
        {
            InitRiderCombat();
        }
        void Update()
        {
            if (LockCombat) return;                     //Skip if Lock Combat is On
            CombatLogicUpdate();
        }

        void FixedUpdate()
        {
            if (LockCombat) return;                     //Skip if Lock Combat is On

            if (ActiveAbility)                          //If there's an Active Ability do the FixedUpate Ability thingy
                ActiveAbility.FixedUpdateAbility();
        }
        void LateUpdate()
        {
            if (LockCombat) return;                     //Skip if Lock Combat is On

            if (ActiveAbility)
                ActiveAbility.LateUpdateAbility();      //If there's an Active Ability do the Late Ability thingy

            if (IsInCombatMode) Anim.SetInteger(Hash.IDInt, -1);                             //Remove the IntID so the attack while 
        }
        #endregion

        /// <summary>
        /// Initialize all variables for the Rider
        /// </summary>
        protected virtual void InitRiderCombat()
        {
            _transform = transform;                                                 //Get this Transform   
            _anim = GetComponent<Animator>();                                       //Get the Animator

            _Head = _anim.GetBoneTransform(HumanBodyBones.Head);                     //Get the Rider Head transform
            _Chest = _anim.GetBoneTransform(HumanBodyBones.Chest);                   //Get the Rider Head transform

            _RightHand = _anim.GetBoneTransform(HumanBodyBones.RightHand);           //Get the Rider Right Hand transform
            _LeftHand = _anim.GetBoneTransform(HumanBodyBones.LeftHand);             //Get the Rider Left  Hand transform

            _RightShoulder = _anim.GetBoneTransform(HumanBodyBones.RightUpperArm);   //Get the Rider Right Shoulder transform
            _LeftShoulder = _anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);     //Get the Rider Left  Shoulder transform

            SetActiveHolder(ActiveHolderSide);                                       //Set one Holder to Draw Weapons

            Layer_RiderArmLeft = _anim.GetLayerIndex("Rider Arm Left");              //Gets the Left Arm Layer Index
            Layer_RiderArmRight = _anim.GetLayerIndex("Rider Arm Right");            //Gets the Right Arm Layer Index   
            Layer_RiderCombat = _anim.GetLayerIndex("Rider Combat");                 //Gets the Combat Later Index

            if (rider.MainCamera)
            {
                _cam = rider.MainCamera.transform;
            }

            aimSent = true;                                                         //Reset  aimsent;       
            if (AimDot) AimDot.gameObject.SetActive(false);                         //If is there an AimDot visible set it to false

            foreach (var ability in CombatAbilities)
            {
                ability.StartAbility(this);
            }

            //Event Listeners
            rider.OnStartMounting.AddListener(OnStartMounting);
            //rider.OnStartDismounting.AddListener(OnStartDismounting);
            //rider.OnEndMounting.AddListener(OnEndMounting);
            //rider.OnEndDismounting.AddListener(OnEndDismounting);
        }

        #region Events Listeners
        public virtual void OnStartMounting()
        {
            if (rider.AnimalControl)
            {
                DefaultMonturaInputCamBaseInput = rider.AnimalControl.cameraBaseInput;  //Store The Default Animal Type of Input
            }
        }

        #endregion

        protected virtual void CombatLogicUpdate()
        {
            if (!rider.IsRiding) return;                                                                //Just work while is in the horse

            if (!isAiming) MountStartRotation = MountPoint.localRotation;                               //Take the Start Local Rotation of the Link to Easily Return to it when finish AIMING

            float TargetSide = 0;
            float CameraSide = 0;

            if (Target)
                TargetSide = Vector3.Dot((_transform.position - Target.position).normalized, _transform.right); //Calculate the side from the Target

            if (_cam) CameraSide = Vector3.Dot(_cam.transform.right, _transform.forward);                         //Get the camera Side Float
            

            this.TargetSide = TargetSide > 0;                                                           //Get the camera Side Left/Right
            this.CameraSide = CameraSide > 0;

            if (UseHolders) Toogle_Weapon();                                                            //Call Tooge Weapon (to change between weapons)

            if (isInCombatMode)                                                                         //If there's a Weapon Active
            {
                Anim.speed = 1;                                                                         //Set  Animator Speed back to 1
                Anim.SetInteger(Hash.IDInt, -1);                                                        //Remove the IntID so the attack while 

                if (ActiveAbility)
                {
                    if (Active_IMWeapon != null)
                    {
                        if (ActiveAbility.WeaponType() != GetWeaponType())
                        {
                            ActiveAbility = CombatAbilities.Find(ability => ability.WeaponType() == GetWeaponType()); //Find the Ability for the IMWeapon 
                        }
                    }
                    if (ActiveAbility.CanAim()) AimMode();

                    ActiveAbility.UpdateAbility();                                                    //Update The Active Ability
                }
            }


            //Used for Invoke On Target
            if (Target != lastTarget)
            {
                OnTarget.Invoke(Target);
                lastTarget = Target;

                if (rider.AnimalControl && StrafeOnTarget)
                {
                    rider.AnimalControl.cameraBaseInput = Target ? true : DefaultMonturaInputCamBaseInput;
                }
            }
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Updates Animator Parameters
        /// </summary>
        public virtual void LinkAnimator()
        {
            _anim.SetInteger(Hash_WeaponHolder, (int)ActiveHolderSide);  //Set the ActiveHolder in the Animator
            _anim.SetInteger(Hash_WeaponType, (int)_weaponType);          //Set the WeaponType in the Animator
            _anim.SetInteger(Hash_WeaponAction, (int)_weaponAction);     //Set the WeaponAction in the Animator
        
        }

        #region AIMSTUFFS

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// This method is used to activate the AIM mode when the right click is pressed
        /// </summary>
        public virtual void AimMode()
        {
            LookDirection();

            if (InputAim.GetPressed != InputButton.Press)               //If the Aim is set to One Click
            {
                if (InputAim.GetInput)                                  //Get the AIM input 
                {
                    isAiming = !isAiming;                               //Toogle the Aim Mode
                    aimSent = false;
                    CurrentCameraSide = !CameraSide;                    //Camera Currentside = the opposite of the real CamSide to be able to enter in a state while Aiming
                }
            }
            else                                                         //If the Aim is set to Pressed
            {
                if (isAiming != InputAim.GetInput)
                {
                    isAiming = InputAim.GetInput;                       //is Aiming will be equal to the Input Entry   
                    aimSent = false;
                }
            }

            if (isAiming)
            {
                Transform origin = ActiveAbility.AimRayOrigin();       //Gets the Origing from where is going to Start the Ray Aim;

                if (AimDot) AimDot.gameObject.SetActive(true);

                if (Target)
                {
                    aimDirection = Utilities.MalbersTools.DirectionTarget(origin, Target);
                }
                else
                {
                    Vector3 screenTarget = AimDot != null ? AimDot.position : new Vector3(Screen.width * 0.5f, Screen.height * 0.5f); //Gets the Center of the Aim Dot Transform
                    aimDirection = Utilities.MalbersTools.DirectionFromCamera(origin, screenTarget, out aimRayHit, HitMask);
                }

                if (debug) Debug.DrawLine(origin.position, AimRayCastHit.point, Color.red); // Debug.DrawRay(origin.position, aimDirection * 50, Color.red);

                if (CurrentCameraSide != CameraSide)                                            //Reset AimSent Values
                {
                    aimSent = false;
                }

                if (CameraSide && !aimSent)                                                     //Change the camera to the Right Side
                {
                    if (!(!ActiveAbility.ChangeAimCameraSide() && Active_IMWeapon.RightHand))   //Prevent the Camera Swap if is Using A Bow
                        SetAim(true);
                    else
                        SetAim(false);
                }
                else if (!CameraSide && !aimSent)                                               //Change the camera to the Left Side
                {
                    if (!(!ActiveAbility.ChangeAimCameraSide() && !Active_IMWeapon.RightHand))  //Prevent the Camera Swap if is Using A Bow
                        SetAim(false);
                    else
                        SetAim(true);
                }

                if (rider.Montura != null)
                {

                    rider.Montura.StraightAim(true);

                    //MountPoint.rotation =
                    //    Quaternion.FromToRotation(rider.Montura.transform.up, Vector3.up) * rider.Montura.MountPoint.rotation; //Keep a Straight Spine while Aiming
                }
            }
            else                                                            //If is not Aiming go to IsMounted Camera State
            {
                if (!aimSent)
                {
                    aimSent = true;
                    CurrentCameraSide = !CameraSide;                        // Camera Currentside = the opposite of the real CamSide to be able to enter in a state while Aiming

                    SetAction(WeaponActions.Idle);                          //Sent Action to Idle again
                    OnAimSide.Invoke(0);                                    //Send that is not aiming anymore

                    if (AimDot) AimDot.gameObject.SetActive(false);         //Hide the AimDot;


                    rider.Montura.StraightAim(false);

                    //MountPoint.localRotation = MountStartRotation;

                    if ((_weaponAction == WeaponActions.AimLeft || _weaponAction == WeaponActions.AimRight
                        || WeaponAction == WeaponActions.Hold || WeaponAction == WeaponActions.Fire_Proyectile)) //Fix theACtion if is not Aiming 
                    {
                        SetAction(IsInCombatMode ? WeaponActions.Idle : WeaponActions.None);
                    }
                }
            }
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Change the Camera Sides for better Aiming
        /// </summary>
        void SetAim(bool Side)
        {
            aimSent = true;
            CurrentCameraSide = CameraSide;                                 //Set the current camera = to the actual camera side

            if (   _weaponAction != WeaponActions.Hold
                && _weaponAction != WeaponActions.ReloadLeft
                && _weaponAction != WeaponActions.ReloadRight)
            {
                SetAction(Active_IMWeapon.RightHand ? WeaponActions.AimRight : WeaponActions.AimLeft);   //If the weapon is Right Handed set the action to AimRight else AimLeft
            }
            OnAimSide.Invoke(Side ? 1 : -1);
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Set the Animator Aim Side to look to the Camera or Target Forward Direction
        /// </summary>
        protected virtual void LookDirection()
        {
            Vector3 dir = Target ? aimDirection : AimDot ? Utilities.MalbersTools.DirectionFromCameraNoRayCast(AimDot.position) : Camera.main.transform.forward;

            dir.y = 0;

            float NewHorizontalAngle = (Vector3.Angle(dir, transform.root.forward) * ((Target ? TargetSide : CameraSide) ? 1 : -1)) / 180; //Get the Normalized value for the look direction

            horizontalAngle = Mathf.Lerp(HorizontalAngle, NewHorizontalAngle, Time.deltaTime * 15f); //Smooth Swap between 1 and -1
            _anim.SetFloat(Hash_AimSide, HorizontalAngle);
        }
        #endregion

        #region Draw Store Equip Unequip Weapons
        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        ///Set the Active Holder Transform for the Active Holder Side
        /// </summary>
        public virtual void SetActiveHolder(WeaponHolder holder)
        {
            ActiveHolderSide = holder;
            switch (ActiveHolderSide)
            {
                case WeaponHolder.None:
                    ActiveHolderTransform = HolderBack; // Set BACK As default the HolderBack
                    break;
                case WeaponHolder.Left:
                    ActiveHolderTransform = HolderLeft ? HolderLeft : HolderBack;
                    break;
                case WeaponHolder.Right:
                    ActiveHolderTransform = HolderRight ? HolderRight : HolderBack;
                    break;
                case WeaponHolder.Back:
                    ActiveHolderTransform = HolderBack;
                    break;
                default:
                    break;
            }
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// If the Rider had a weapon before mounting.. equip it.
        /// The weapon need an |IMWeapon| Interface, if not it wont be equiped
        /// </summary>
        public virtual void SetWeaponBeforeMounting(GameObject weapon)
        {
            if (weapon == null) return;
            if (weapon.GetComponent<IMWeapon>() == null) return;                                        //If the weapon doesn't have IMweapon Interface do nothing


            /// Try the set to false the weapon if is not a Prefab ///

            SetActiveWeapon(weapon);

            isInCombatMode = true;

            Active_IMWeapon.Equiped();                                                                   //Let the weapon know that it has been Equiped

            EnableMountAttack(false);

            SetActiveHolder(Active_IMWeapon.Holder);                                                     //Set the Active Holder for the Active Weapon

            _weaponType = GetWeaponType();                                                              //Set the Weapon Type

            SetAction(WeaponActions.Idle);

            SetWeaponIdleAnimState(Active_IMWeapon.RightHand);                                           //Set the Correct Idle Hands Animations

            Active_IMWeapon.HitMask = HitMask;                                                           //Link the Hit Mask

            ActiveAbility = CombatAbilities.Find(ability => ability.WeaponType() == GetWeaponType());   //Find the Ability for the IMWeapon 

            if (ActiveAbility)
            {
                ActiveAbility.ActivateAbility();
            }
            else
            {
                Debug.LogWarning("The Weapon is combatible but there's no Combat Ability available for it, please Add the matching ability it on the list of Combat Abilities");
            }

            OnEquipWeapon.Invoke(ActiveWeapon);

            LinkAnimator();
        }

        /// <summary>
        /// Toogle if the Animal can attack while the rider is in combat mode
        /// </summary>
        /// <param name="value"></param>
        public void EnableMountAttack(bool value)
        {
            if (rider.AnimalControl)
            {
                rider.AnimalControl.EnableInput("Attack1", value);                                //Disable Attack Animations of the Animal
                rider.AnimalControl.EnableInput("Attack2", value);                                //Disable Attack Animations of the Animal
            }
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        ///Called to store and/or  draw a weapon
        /// </summary>
        protected virtual void Toogle_Weapon()
        {
            if (_weaponAction == WeaponActions.None || _weaponAction == WeaponActions.Idle || _weaponAction == WeaponActions.AimLeft || _weaponAction == WeaponActions.AimRight)   //Toogle weapon only If we are on Action none or action Idle
            {
                if (InputWeapon.GetInput)  // Will draw
                {
                    if (ActiveWeapon)
                    {
                        if (_weaponAction == WeaponActions.Idle)
                            Store_Weapon();                                         //Draw a weapon if we are on Action Idle 
                    }
                    else
                    {
                        Draw_Weapon();
                    }
                }

                if (UseHolders)
                {
                    if (HBack.GetInput)                                              //Set the Weapon to Back Holder
                    {
                        Change_Weapon_Holder_Inputs(WeaponHolder.Back);
                        if (debug) Debug.Log("Change Holder to 'Back'. ");
                    }

                    if (HLeft.GetInput)
                    {
                        Change_Weapon_Holder_Inputs(WeaponHolder.Left);
                        if (debug) Debug.Log("Change Holder to 'Left'. ");
                    }

                    if (HRight.GetInput)
                    {
                        Change_Weapon_Holder_Inputs(WeaponHolder.Right);
                        if (debug) Debug.Log("Change Holder to 'Right'. ");
                    }
                }
            }
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Is called to swap weapons
        /// </summary>
        protected virtual void Change_Weapon_Holder_Inputs(WeaponHolder holder)
        {
            if (ActiveHolderSide != holder && _weaponAction == WeaponActions.Idle)        //if there's a weapon on hand, Store it and draw the other weapon from the next holder
            {
                StartCoroutine(SwapWeaponsHolder(holder));
            }
            else if (ActiveHolderSide != holder && _weaponAction == WeaponActions.None)   //if there's no weapon draw the weapon from the next holder
            {
                SetActiveHolder(holder);
                Draw_Weapon();
                LinkAnimator();
            }
            else
            {
                if (!isInCombatMode)
                {
                    if (_weaponAction == WeaponActions.None)
                        Draw_Weapon();                                                      //Draw a weapon if we are on Action None
                }
                else
                {
                    if (_weaponAction == WeaponActions.Idle)
                        Store_Weapon();                                                    //Store a weapon if we are on Action Idle 
                }
                LinkAnimator();                                                             //Links the animator to the values
            }
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Is called to swap weapons
        /// </summary>
        IEnumerator SwapWeaponsHolder(WeaponHolder HoldertoSwap)
        {
            Store_Weapon();
            LinkAnimator();
            while (_weaponAction == WeaponActions.StoreToLeft || _weaponAction == WeaponActions.StoreToRight) // Wait for the weapon is Unequiped Before it can Draw Another
            {
                yield return null;
            }

            SetActiveHolder(HoldertoSwap);
            Draw_Weapon();                                  //Set the parameters so draw a weapon
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Sets the weapon that are in the Inventory
        /// </summary>
        /// <param name="Next_Weapon">Game object that it should have an IMWeapon Interface</param>
        public virtual void SetWeaponByInventory(GameObject Next_Weapon)
        {
            StopAllCoroutines();
            if (!rider.IsRiding) return;                                //Work Only when is riding else Skip

            if (Next_Weapon == null)                                    //That means Store the weapon
            {
                if (ActiveWeapon)
                    Store_Weapon();                                     //Debug.Log("Active Weapon NOT NULL Store the Active Weapon");
                return;
            }

            IMWeapon Next_IMWeapon = Next_Weapon.GetComponent<IMWeapon>();

            if (Next_IMWeapon == null)
            {
                if (ActiveWeapon)
                    Store_Weapon();                                     //Debug.Log("Active Weapon NOT NULL and Store because  Next Weapon is not Compatible");
                return;                                                 //If the Next Weapon doesnot have the IMWeapon Interface dismiss... the next weapon is not compatible
            }

            if (Active_IMWeapon == null)
            {
                if (!AlreadyInstantiated)
                {
                    Next_Weapon = Instantiate(Next_Weapon, rider.transform);    //Instanciate the Weapon GameObject
                    Next_Weapon.SetActive(false);
                }
                SetActiveWeapon(Next_Weapon);
                Draw_Weapon();                                                  //Debug.Log("Active weapon is NULL so DRAW");
                return;
            }

            if (Active_IMWeapon.Equals(Next_IMWeapon))                          //Option 2 If the Weapon  has the Same ID
            {
                if (!IsInCombatMode)
                {
                    Draw_Weapon();                                              //Debug.Log("Active weapon is the same as the NEXT Weapon and we are NOT in COmbat so DRAW");
                }
                else
                {
                    Store_Weapon();                                             //Debug.Log("Active weapon is the same as the NEXT Weapon and we ARE  in COmbat so STORE");
                }
            }
            else                                                                //If the weapons are different Swap it
            {
                StartCoroutine(SwapWeaponsInventory(Next_Weapon));              //Debug.Log("Active weapon is DIFFERENT to the NEXT weapon so Switch" + Next_Weapon);
            }
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Is called to swap weapons
        /// </summary>
        IEnumerator SwapWeaponsInventory(GameObject nextWeapon)
        {
            Store_Weapon();

            while (_weaponAction == WeaponActions.StoreToLeft || _weaponAction == WeaponActions.StoreToRight) // Wait for the weapon is Unequiped Before it can Draw Another
            {
                yield return null;
            }

            if (!AlreadyInstantiated)
            {
                nextWeapon = Instantiate(nextWeapon, rider.transform);        //instanciate the Weapon GameObject
                nextWeapon.SetActive(false);
            }
            SetActiveWeapon(nextWeapon);                    //Set the New Weapon
            SetActiveHolder(Active_IMWeapon.Holder);

            Draw_Weapon();                                  //Set the parameters so draw a weapon
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Equip Weapon from Holders or from Inventory
        /// </summary>
        public virtual void Equip_Weapon()
        {
            SetAction(WeaponActions.Equip);                                             //Set the Action to Equip
            isInCombatMode = true;

            if (Active_IMWeapon == null) return;

            if (debug) Debug.Log("Equip_Weapon");

            Active_IMWeapon.HitMask = HitMask;                                           //Update the Hit Mask in the weapon

            if (UseHolders)                                                             //If Use Holders Means that the weapons are on the Holders
            {
                if (ActiveHolderTransform.transform.childCount > 0)                     //If there's a Weapon on the Holder
                {
                    SetActiveWeapon(ActiveHolderTransform.GetChild(0).gameObject);      //Set the Active Weapon as the First Child Inside the Holder

                    ActiveWeapon.transform.parent =
                        Active_IMWeapon.RightHand ? RightHandEquipPoint : LeftHandEquipPoint; //Parent the Active Weapon to the Right/Left Hand
                    Active_IMWeapon.Holder = ActiveHolderSide;

                    StartCoroutine(SmoothWeaponTransition
                        (ActiveWeapon.transform, Active_IMWeapon.PositionOffset, Active_IMWeapon.RotationOffset, 0.3f)); //Smoothly put the weapon in the hand
                }
            }
            else if (UseInventory)                                                            //If Use Inventory means that the weapons are on the inventory
            {
                if (!AlreadyInstantiated)                                                     //Do this if the Instantiation is not handled Externally
                {
                    ActiveWeapon.transform.parent =
                        Active_IMWeapon.RightHand ? RightHandEquipPoint : LeftHandEquipPoint; //Parent the Active Weapon to the Right/Left Hand

                    ActiveWeapon.transform.localPosition = Active_IMWeapon.PositionOffset;    //Set the Correct Position
                    ActiveWeapon.transform.localEulerAngles = Active_IMWeapon.RotationOffset; //Set the Correct Rotation
                }
                ActiveWeapon.gameObject.SetActive(true);                                      //Set the Game Object Instance Active    
            }

            Active_IMWeapon.Equiped();                                                        //Inform the weapon that it has been equipped

            OnEquipWeapon.Invoke(ActiveWeapon);                                               //Let everybody know that the weapon is equipped

            if (ActiveAbility) ActiveAbility.ActivateAbility();                                //Call For the first activation of the weapon when first Equipped
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Unequip Weapon from Holders or from Inventory
        /// </summary>
        public virtual void Unequip_Weapon()
        {
            _weaponType = WeaponType.None;
            SetAction(WeaponActions.Unequip);
            LinkAnimator();

            if (Active_IMWeapon == null) return;
            if (debug) Debug.Log("Unequip_Weapon");

            Active_IMWeapon.Unequiped();                     //Let the weapon know that it has been unequiped
            OnUnequipWeapon.Invoke(ActiveWeapon);            //Let the rider know that the weapon has been unequiped.

            if (UseHolders)                                 //If Use Holders Parent the ActiveMWeapon the the Holder
            {
                ActiveWeapon.transform.parent = ActiveHolderTransform.transform;        //Parent the weapon to his original holder          
                StartCoroutine(SmoothWeaponTransition(ActiveWeapon.transform, Vector3.zero, Vector3.zero, 0.3f));
            }
            else if (UseInventory && !AlreadyInstantiated && ActiveWeapon)
            {
                Destroy(ActiveWeapon);
            }
            activeWeapon = null;
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Draw (Set the Correct Parameters to play Draw Weapon Animation)
        /// </summary>
        public virtual void Draw_Weapon()
        {
            EnableMountAttack(false);                                                                   //Disable Attack Animations of the Animal
            ResetRiderCombat();

            if (UseInventory)                                                                           //If is using inventory
            {
                if (Active_IMWeapon != null)
                {
                    SetActiveHolder(Active_IMWeapon.Holder);                                             //Set the Current Holder to the weapon asigned holder
                }
            }
            else //if Use Holders
            {
                if (ActiveHolderTransform.childCount == 0) return;
                IMWeapon isCombatible = ActiveHolderTransform.GetChild(0).GetComponent<IMWeapon>();     //Check if the Child on the holder Has a IMWeapon on it

                if (isCombatible == null) return;

                SetActiveWeapon(ActiveHolderTransform.GetChild(0).gameObject);                          //Set Active Weapon to the Active Holder Child 
            }
            _weaponType = GetWeaponType();                                                              //Set the Weapon Type (For the correct Animations)

            SetAction(Active_IMWeapon.RightHand ? WeaponActions.DrawFromRight : WeaponActions.DrawFromLeft); //Set the  Weapon Action to -1 to Draw Weapons From Right or from left -2

            SetWeaponIdleAnimState(Active_IMWeapon.RightHand);

            ActiveAbility = CombatAbilities.Find(ability => ability.TypeOfAbility(Active_IMWeapon)); //Find the Ability for the IMWeapon 


            LinkAnimator();

            if (debug) Debug.Log("Draw: " + ActiveWeapon.name);  //Debug
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Store (Set the Correct Parameters to play Store Weapon Animation)
        /// </summary>
        public virtual void Store_Weapon()
        {
            if (Active_IMWeapon == null || !isInCombatMode) return;                          //Skip if there's no Active Weapon or is not inCombatMode

            ResetRiderCombat();

            EnableMountAttack(true);                                                        //Enable Attack Animations of the Animal
            _weaponType = WeaponType.None;                                                  //Set the weapon ID to None (For the correct Animations)
            SetActiveHolder(Active_IMWeapon.Holder);
            SetAction(Active_IMWeapon.RightHand ? WeaponActions.StoreToRight : WeaponActions.StoreToLeft);   //Set the  Weapon Action to -1 to Store Weapons to Right or -2 to left

            LinkAnimator();

            if (debug) Debug.Log("Store: " + ActiveWeapon.name);
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Reset and Remove the Active Ability
        /// </summary>
        protected virtual void ResetActiveAbility()
        {
            if (ActiveAbility != null)
            {
                ActiveAbility.ResetAbility();
                ActiveAbility = null;
            }
        }

        ///──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        /// <summary>
        ///This Coroutine will smoothly move the weapon from the holder and viceversa in a time if we are using the holders
        /// </summary>
        IEnumerator SmoothWeaponTransition(Transform obj, Vector3 posOfsset, Vector3 rotOffset, float time)
        {
            float elapsedtime = 0;
            Vector3 startPos = obj.localPosition;
            Quaternion startRot = obj.localRotation;

            while (elapsedtime < time)
            {
                obj.localPosition = Vector3.Slerp(startPos, posOfsset, Mathf.SmoothStep(0, 1, elapsedtime / time));
                obj.localRotation = Quaternion.Slerp(startRot, Quaternion.Euler(rotOffset), elapsedtime / time);
                elapsedtime += Time.deltaTime;
                yield return null;
            }
            obj.localPosition = posOfsset;
            obj.localEulerAngles = rotOffset;
        }
        #endregion

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (debug)
            {
                if (isAiming)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(AimRayCastHit.point, 0.05f);
                    Gizmos.DrawSphere(AimRayCastHit.point, 0.05f);

                    Ray RayHand = new Ray(RightShoulder.position, AimDirection);
                    Vector3 FixPoint = RayHand.GetPoint(Vector3.Distance(RightShoulder.position, RightHand.position));

                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(FixPoint, 0.05f);
                    Gizmos.DrawSphere(FixPoint, 0.05f);
                }
            }
        }
#endif
    }
}