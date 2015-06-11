using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Defines the invisibility effect/power-up. This is a temporary affect that
/// applies an invisibility (transparent) material to the player for a 
/// limited time.
/// 
/// The parameters of this effect are tunable via the public attributes of 
/// the InvisibilityPickup class.
/// </summary>
/// <seealso cref="InvisibilityPickup"/>
public class InvisibilityPlayerEffect : PlayerEffect
{
	/// <summary>
	/// The invisibility material to apply to the player when the effect 
	/// becomes active.
	/// </summary>
	/// <seealso cref="InvisibilityPickup.InvisibilityMaterial"/>
	private Material mInvisibilityMaterial;
	
	/// <summary>
	/// The original material applied to the player when this effect was 
	/// created.
	/// </summary>
	/// <seealso cref="PlayerControllerBase.PlayerMeshRenderer.material"/>
	private Material mPlayerMaterial;
	
	/// <summary>
	/// Determines how long to visually lerp in and out of the effect once 
	/// the effect has started/ended.
	/// </summary>
	private float mVisualLerpDuration;
	
	/// <summary>
	/// Used to capture the original "render state" of a weapon model before
	/// applying the invisibility material. This original state is used to
	/// restore the materials when the invisibility effect is finished.
	/// </summary>
	struct WeaponRenderState
	{
		/// <summary>
		/// The child Renderer objects of the WeaponModelPrefab of the weapon
		/// </summary>
		/// <seealso cref="GunBase.FPWeaponModel"/>
		public List<Renderer> weaponRenderers;
		
		/// <summary>
		/// The Materials of all the child Renderer objects of the 
		/// WeaponModelPrefab of the weapon.
		/// </summary>
        /// <seealso cref="GunBase.FPWeaponModel"/>
		public List<Material> originalWeaponMaterials;
	};
	
	/// <summary>
	/// Maps a weapon getting applied an invisibility material to its 
	/// original render state (so the render state can later be restored).
	/// The render state consists of renderers mapped to their array of 
	/// original materials (before invisibility state).
	/// </summary>
	/// <seealso cref="InvisibilityPlayerEffect.ApplyInvisibilityToWeapon"/>
	private Dictionary< GunBase, Dictionary<Renderer, Material[]> > mWeaponModels;
	
	/// <summary>
	/// A reference to the player's equipped weapon so we can easily detect
	/// when the equipped weapon has changed (such as when the player 
	/// switches or picks up a new weapon).
	/// </summary>
	private GunBase mEquippedWeapon;
	
	/// <summary>
	/// Delegate used to capture "shot fired" events from the weapon manager.
	/// <seealso cref="WeaponManagementScript.RegisterInstantShotFiredCallback"/>
	/// <seealso cref="WeaponManagementScript.RegisterExplosiveShotFiredCallback"/>
	/// <seealso cref="WeaponManagementScript.RegisterKineticShotFiredCallback"/>
	/// <seealso cref="WeaponManagementScript.RegisterSpreadShotFiredCallback"/>
	/// </summary>
	private WeaponManagementScript.WeaponFiredDelegate mShotFired;
	
	/// <summary>
	/// How long it should take the player to LERP from being visible back
	/// to invisible after firing a "normal" weapon.
	/// </summary>
	private float mNormalWeaponExposeDuration;
	
	/// <summary>
	/// The duration of how long the player should take to lerp from visible
	/// to invisible when firing a "quick-cloaking" weapon.
	/// </summary>
	private float mQuickCloakingWeaponExposeDuration;
	
	/// <summary>
	/// The most recent time at which a "normal" weapon is fired. Initialized
	/// to zero.
	/// </summary>
	private float mNormalWeaponShotFiredTime;
	
	/// <summary>
	/// The most recent time at which a "quick-cloaking" weapon was fired.
	/// Initialized to zero.
	/// </summary>
	private float mQuickCloakingWeaponShotFiredTime;
	
	/// <summary>
	/// When a normal-cloaking weapon is fired, the user can switch to a 
	/// quick-cloaking weapon within a threshold of time to have the quick-
	/// cloaking duration applied, even though a normal-cloaking weapon was
	/// fired.
	/// </summary>
	private float mQuickCloakingWeaponSwitchThreshold;
	
	/// <summary>
	/// The amount of time it takes to LERP back to invisibility after the 
	/// player becomes exposed due to taking damage.
	/// </summary>
	private float mPlayerDamageExposeDuration;

	/// <summary>
	/// Defines at what point in time the player started to visually
	/// LERP the inviz effect (either from visible to inviz or inviz
	/// to visible). The duration of the visual LERP is defined by
	/// <see cref="InvisibilityPlayerEffect.mExposeDuration"/>.
	/// </summary>
	/// <seealso cref="InvisibilityPlayerEffect.mExposeDuration"/>
	private float mExposeStartTime;

	/// <summary>
	/// The duration of the most recent visual LERP to/from 
	/// visible/inviz. Different actions will expose the player for
	/// different periods of time. For example, picking up the inviz
	/// power up maybe visually LERP from visible to inviz over a 5
	/// second period, whereas firing a quick-cloaking weapon will 
	/// only take 2 seconds to go from visible to invisible.
	/// </summary>
	private float mExposeDuration;
	
	/// <summary>
	/// Creates a new invisibility effect. Note that the effect won't be 
	/// active until <see cref="PlayerEffect.StartEffect"/> is called.
	/// </summary>
	/// <param name='player'>
	/// The player affected by this effect - this is passed from the pickup 
	/// trigger method.
	/// </param>
	/// <param name='duration'>
	/// Duration of the effect - should be a positive value.
	/// </param>
	/// <param name='invisibilityMaterial'>
	/// Invisibility material to be applied to the given player.
	/// </param>
	public InvisibilityPlayerEffect (
		PlayerControllerBase player, 
		float duration,
		Material invisibilityMaterial,
		float visualLerpDuration,
		float normalWeaponExposeDuration,
		float quickCloakingWeaponExposeDuration,
		float quickCloakingWeaponSwitchThreshold,
		float playerDamageExposeDuration) :
		
		// The visual lerp duration occurs at the start and end of the 
		// effect. Therefore we add twice the visual lerp duration to the
		// actual effect duration to account for visually lerping in/out of
		// the effect.
		base(player, duration + visualLerpDuration*2)
	{
		mInvisibilityMaterial = invisibilityMaterial;
		mPlayerMaterial = Material.Instantiate(player.PlayerMeshRenderer.material) as Material;		
		mVisualLerpDuration = visualLerpDuration;
		
		mWeaponModels = new Dictionary< GunBase, Dictionary<Renderer, Material[]> >();
		
		// Variables that control "expose" logic
		mNormalWeaponExposeDuration = normalWeaponExposeDuration;
		mNormalWeaponShotFiredTime = 0.0f;
		
		mQuickCloakingWeaponExposeDuration = quickCloakingWeaponExposeDuration;
		mQuickCloakingWeaponShotFiredTime = 0.0f;
		
		mQuickCloakingWeaponSwitchThreshold = quickCloakingWeaponSwitchThreshold;
		
		mPlayerDamageExposeDuration = playerDamageExposeDuration;

		mExposeStartTime = mExposeDuration = 0.0f;
	}
	
	/// <summary>
	/// Starts the effect by assigning the invisibility material to the player.
	/// </summary>
	protected override void InitEffect()
	{		
		// Since this effect transitions (LERPs) from the base player model
		// shader to the invisibility refraction shader, we init the LERP 
		// value to zero to start the transition going from the base player
		// model to the refraction effect.
		mInvisibilityMaterial.SetFloat("_RefractionEffectLerp", 0.0f);
		
		// Apply the invisibility material to the player model
		mPlayer.PlayerMeshRenderer.material = mInvisibilityMaterial;
		
		// Keep track of the equipped weapon. We'll detect when the weapon 
		// changes so we can apply the invisibility effect to the new/changed
		// weapon also
		mEquippedWeapon = mPlayer.WeaponManager.CurrentWeapon;		
		ApplyInvisibilityToWeapon(mEquippedWeapon);
		
		// Listen for weapon fires
		mShotFired += OnShotFired;
		mPlayer.WeaponManager.RegisterInstantShotFiredCallback(mShotFired);
		mPlayer.WeaponManager.RegisterExplosiveShotFiredCallback(mShotFired);
		mPlayer.WeaponManager.RegisterKineticShotFiredCallback(mShotFired);
		mPlayer.WeaponManager.RegisterSpreadShotFiredCallback(mShotFired);

		// Listen for player taking damage
		mPlayer.ShieldHealthManager.mOnDamageTaken += OnDamageTaken;

		// Visually lerp the effect in
		mExposeStartTime = Time.time;
		mExposeDuration = mVisualLerpDuration;
	}	
	
	/// <summary>
	/// Handles visual effect lerping and detects weapon changes to ensure
	/// all weapons of the player are also invisible.
	/// </summary>
	protected override void UpdateActiveEffect()
	{
		float lerpStartTime = mExposeStartTime;
		float lerpDuration = mExposeDuration;

		if (Time.time >= mEndTime - mVisualLerpDuration)
		{
			lerpStartTime = 2.0f * Time.time - mEndTime;
			lerpDuration = mVisualLerpDuration;
		}

		float lerp = (Time.time - lerpStartTime)/lerpDuration;
		lerp = Mathf.Min(1.0f, lerp);
		mInvisibilityMaterial.SetFloat("_RefractionEffectLerp", lerp);
		
		// Detect weapon changes to ensure all equipped weapons get the 
		// invisibility material applied
		if (mEquippedWeapon != mPlayer.WeaponManager.CurrentWeapon)
		{
			// Need to determine whether this weapon switch will allow the 
			// player to inherit a quick-cloaking LERP speed if the following
			// are true:
			// 1) Standard cloaking weapon was recently fired AND
			// 2) Gamer is switching to quick-cloaking weapon AND
			// 3) Weapon switch occurs within the threshold
			if (NormalCloakingWeaponShotFiredLast() && 
				QuickCloakingWeapon(mPlayer.WeaponManager.CurrentWeapon) &&
				WeaponSwitchInheritsQuickCloak())
			{
				// Get the current normal-cloaking LERP value
				float normalCloakingLerp = (Time.time - mNormalWeaponShotFiredTime)/mNormalWeaponExposeDuration;
				
				// Use the current normal-cloaking LERP value to derive the
				// time of when a quick-cloaking weapon would have need to 
				// have been fired in order to continue the LERP smoothly.
				// (Basically, continue the LERP as if a quick-cloaking 
				// weapon had been fired instead of a normal-cloaking weapon).
				mQuickCloakingWeaponShotFiredTime = Time.time - (normalCloakingLerp * mQuickCloakingWeaponExposeDuration);
				mExposeStartTime = mQuickCloakingWeaponShotFiredTime;
				mExposeDuration = mQuickCloakingWeaponExposeDuration;
			}

			mEquippedWeapon = mPlayer.WeaponManager.CurrentWeapon;
			ApplyInvisibilityToWeapon(mEquippedWeapon);
		}
	}
	
	/// <summary>
	/// Restores the original materials for 3rd person player model and 
	/// 1st-person weapon models.
	/// </summary>
	protected override void EndEffect()
	{
		// Restore the 3rd-person model material
		mPlayer.PlayerMeshRenderer.material = mPlayerMaterial;
		
		// Iterate through all the weapons that we've applied the 
		// invisibility material to and restore their original materials
		foreach (KeyValuePair< GunBase, Dictionary<Renderer, Material[]> > weaponState in mWeaponModels)
		{
			// Now we iterate through each Renderer object for this gun and
			// restore the original materials array.
			foreach (KeyValuePair<Renderer, Material[]> renderState in weaponState.Value)
			{
				// A renderer can become null if we applied the invisibility
				// material to it at one point in time, but the gun has since
				// been discarded by the player.
				if (renderState.Key == null)
				{
					continue;
				}
				
				renderState.Key.materials = renderState.Value;
			}
		}

		// Unregister delegates
		mPlayer.ShieldHealthManager.mOnDamageTaken -= OnDamageTaken;

		// :TODO: need to unregister "weapon shot fired" delegates?
	}

	/// <summary>
	/// When the player dies, we disable the invisibility power-up.
	/// </summary>
	public override void OnPlayerDeath()
	{
		DisableActiveEffect();
	}
	
	/// <summary>
	/// Applies the invisibility material to the given weapon. If we've 
	/// already applied the material to the weapon, skip it. The weapon's
	/// original material is saved so it will be restored when the effect
	/// ends (see <see cref="InvisibilityPlayerEffect.EndEffect"/>).
	/// </summary>
	/// <remarks>Any 2D text renderers associated with the weapon are ignored
	/// when applying the invisibility material. This is because the 
	/// invisibility material is lit and 2D text is typically unlit.</remarks>
	/// <param name='weapon'>
	/// Weapon to apply invisibility material to.
	/// </param>
	private void ApplyInvisibilityToWeapon(GunBase weapon)
	{
		// Check if this weapon already has invisibility applied
		if (mWeaponModels.ContainsKey(weapon))
		{
			return;
		}
		
		// Need to set invisibility on first-person weapon model
		GameObject weaponModelPrefab = weapon.FPWeaponModel;
		
		Dictionary<Renderer, Material[]> renderState = new Dictionary<Renderer, Material[]>();
		
		// Get all child Renderer objects of the weapon model
		List<Renderer> weaponRenderers = new List<Renderer>(weaponModelPrefab.GetComponentsInChildren<Renderer>());

		foreach (Renderer renderer in weaponRenderers)
		{			
			// Since the invisibility material is a "lit" affect and 2D text
			// renderers are typically unlit, we ignore applying the 
			// invisibility material to 2D text for now.
			//tk2dTextMesh textMesh = renderer.GetComponent<tk2dTextMesh>();
			//if (null != textMesh)
			//{
			//	continue;
			//}
			
			// Save the weapon's original material so we can restore it later
			renderState[renderer] = renderer.materials;

			ApplyInvisibilityToRenderer(renderer);
		}
		
		// Finally, keep track of this weapon so we can restore its render 
		// state when the effect wears off
		mWeaponModels[weapon] = renderState;
	}
	
	/// <summary>
	/// Re-assigns all materials associated with the given Renderer with the
	/// invisibility material.
	/// </summary>
	/// <param name='renderer'>
	/// Renderer to apply invisibility material to.
	/// </param>
	private void ApplyInvisibilityToRenderer(Renderer renderer)
	{
		Material[] invisibleMaterials = new Material[renderer.materials.Length];
		for (int i = 0; i < invisibleMaterials.Length; ++i)
		{
			invisibleMaterials[i] = mInvisibilityMaterial;
		}
		
		renderer.materials = invisibleMaterials;
	}

	/// <summary>
	/// Delegate that gets called when the player fires one of their weapons.
	/// </summary>
	/// <seealso cref="WeaponManagementScript.RegisterInstantShotFiredCallback"/>
	/// <seealso cref="WeaponManagementScript.RegisterExplosiveShotFiredCallback"/>
	/// <seealso cref="WeaponManagementScript.RegisterKineticShotFiredCallback"/>
	/// <seealso cref="WeaponManagementScript.RegisterSpreadShotFiredCallback"/>
	private void OnShotFired(GunBase.GunType gunType, ref Ray rayUsed, bool targetHit)
	{		
		switch (gunType)
		{
			// Quick-cloaking weapons
			case GunBase.GunType.GunType_MachineGun:
			case GunBase.GunType.GunType_Shotgun:
			{
				mQuickCloakingWeaponShotFiredTime = Time.time;
				mExposeStartTime = mQuickCloakingWeaponShotFiredTime;
				mExposeDuration = mQuickCloakingWeaponExposeDuration;
				break;
			}
			
			default:
			{
				// Assume normal-cloaking weapon was fired
				mNormalWeaponShotFiredTime = Time.time;
				mExposeStartTime = mNormalWeaponShotFiredTime;
				mExposeDuration = mNormalWeaponExposeDuration;
				break;
			}
		}
	}
	
	/// <returns>
	/// True if a normal-cloaking weapon was most recently fired, false 
	/// otherwise.
	/// </returns>
	private bool NormalCloakingWeaponShotFiredLast()
	{
		return mNormalWeaponShotFiredTime > mQuickCloakingWeaponShotFiredTime;
	}
	
	/// <returns>
	/// True if a quick-cloaking weapon was most recently fired, false 
	/// otherwise.
	/// </returns>
	private bool QuickCloakingWeaponShotFiredLast()
	{
		return mQuickCloakingWeaponShotFiredTime > mNormalWeaponShotFiredTime;
	}
	
	/// <returns>
	/// True if the given weapon is a quick-cloaking weapon, false otherwise.
	/// </returns>
	private bool QuickCloakingWeapon(GunBase weapon)
	{
		return weapon.mMyGunType == GunBase.GunType.GunType_MachineGun ||
			weapon.mMyGunType == GunBase.GunType.GunType_Shotgun;
	}
	
	/// <returns>
	/// True if the given weapon is a normal-cloaking weapon, false otherwise.
	/// </returns>
	private bool NormalCloakingWeapon(GunBase weapon)
	{
		return !QuickCloakingWeapon(weapon);
	}
	
	/// <returns>
	/// True if the elapsed time from now to the last time a normal-cloaking
	/// weapon was shot falls within the "quick-cloaking weapon switch 
	/// threshold", false otherwise.
	/// </returns>
	/// <seealso cref="InvisibilityPlayerEffect.mQuickCloakingWeaponSwitchThreshold"/>
	private bool WeaponSwitchInheritsQuickCloak()
	{
		return Time.time - mNormalWeaponShotFiredTime <= mQuickCloakingWeaponSwitchThreshold;
	}

	/// <summary>
	/// Delegate that gets called when player takes damage.
	/// </summary>
	/// <seealso cref="ShieldHealthModel.mOnDamageTaken"/>
	public void OnDamageTaken(HitInfo hitInfo)
	{
		mExposeStartTime = Time.time;
		mExposeDuration = mPlayerDamageExposeDuration;
	}
}
