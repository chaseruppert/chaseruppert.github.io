using UnityEngine;
using System.Collections;

public class InvisibilityPickup : ItemPickupScript 
{
	public Material InvisibilityMaterial;
	public float InvisibilityDuration;
	public float VisualLerpDuration;
	public float NormalWeaponShotExposeDuration;
	public float QuickCloakingWeaponShotExposeDuration;
	public float QuickCloakingWeaponSwitchThreshold;
	public float PlayerDamageExposeDuration;
	
	// Use this for initialization
	void Start () 
	{		
		if(rigidbody != null)
		{
			rigidbody.detectCollisions = true;
			rigidbody.velocity = Vector3.zero;
			rigidbody.useConeFriction = false;
			rigidbody.solverIterationCount = 50;
			rigidbody.Sleep();
		}
		if(GameBase.CurrentGame != null && GameBase.CurrentGame.GameLogic != null)
		{
        	GameBase.CurrentGame.GameLogic.RegisterExplosiveInteractiveObject(gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{		
		if (PickupSpawnScript.kDisabledSpawns)
        {
            GameObject.DestroyImmediate(gameObject);
        }

		if(!mInitialized)
		{
			if(GameBase.CurrentGame != null && GameBase.CurrentGame.GameLogic != null)
			{
	        	GameBase.CurrentGame.GameLogic.RegisterExplosiveInteractiveObject(gameObject);
				mInitialized = true;
			}
		}
	}
	
	void FixedUpdate()
	{
		if(rigidbody != null && !rigidbody.IsSleeping())
		{
			rigidbody.detectCollisions = true;
		}
	}
	
	override public void PerformEffectOnPlayer(PlayerControllerBase player)
	{
		// Only consume the pickup and activate the effect if the player
		// doesn't have this effect active already.
		if (!player.HasPlayerEffect(typeof(InvisibilityPlayerEffect)))
		{
			// "Pick up" the item.
			ConsumePickup();
			
			PlayerEffect effect = new InvisibilityPlayerEffect(
				player, 
				InvisibilityDuration,
				InvisibilityMaterial,
				VisualLerpDuration,
				NormalWeaponShotExposeDuration,
				QuickCloakingWeaponShotExposeDuration,
				QuickCloakingWeaponSwitchThreshold,
				PlayerDamageExposeDuration);
			
			player.ActivatePlayerEffect(effect);
		}
	}
}
