using UnityEngine;
using System;

/// <summary>
/// A PlayerEffect is associated with a given player, typically for a limited
/// duration (such as a power-up, poison, etc.).
/// 
/// PlayerEffect is an abstract base class that defines the framework that
/// all player effects should have. Typically these properties are: a
/// reference to the player that's affected, when the effect was started,
/// the duration of the effect, and whether the effect is active.
/// 
/// The PlayerControllerBase class' FixedUpdate method will iterate through
/// a list of all PlayerEffects that are associated with the player, and call
/// each effect's Update method.
/// 
/// Derived classes need to define how their effect is initialized, how it
/// should be updated, and what happens when the effect ends.
/// </summary>
/// 
/// <seealso cref="PlayerControllerBase.PlayerEffectsUpdate"/>
/// <seealso cref="PlayerControllerBase.mPlayerEffects"/>
public abstract class PlayerEffect
{
	/// <summary>
	/// Reference to the affected player.
	/// </summary>
	protected PlayerControllerBase mPlayer;
	
	/// <summary>
	/// When the effect was started.
	/// </summary>
	/// <seealso cref="PlayerEffect.StartEffect"/>
	protected float mStartTime;
	
	protected float mEndTime;
	
	/// <summary>
	/// How long the effect should last. If no duration is passed to the
	/// PlayerEffect constructor, the duration is infinity (effectively,
	/// the effect will be permanent).
	/// </summary>
	protected float mEffectDuration;
	
	/// <summary>
	/// Whether the effect still affects the player. Until StartEffect is 
	/// called, the effect is considered inactive.
	/// </summary>
	/// <seealso cref="PlayerEffect.Update"/>
	/// <seealso cref="PlayerEffect.DisableActiveEffect"/>
	protected bool mActive;
	
	/// <summary>
	/// Base class constructor that associates this effect with the given
	/// player. Initializes the effect start time to zero and sets the effect
	/// as inactive by default.
	/// </summary>
	/// <param name='player'>
	/// Reference to the player affected.
	/// </param>
	/// <param name='duration'>
	/// How long the effect should last for. If no duration is passed, the
	/// effect is permament by setting the duration to infinity.
	/// </param>
	public PlayerEffect (PlayerControllerBase player, float duration = float.PositiveInfinity)
	{
		mPlayer = player;
		mStartTime = 0;
		mEffectDuration = duration;
		mActive = false;
	}
	
	/// <summary>
	/// Sets the effect to active, sets the start time and calls InitEffect.
	/// </summary>
	/// <seealso cref="PlayerEffect.InitEffect"/>
	public void StartEffect()
	{		
		mActive = true;
		mStartTime = Time.time;
		mEndTime = mStartTime + mEffectDuration;
		
		// Perform effect-specific initialization
		InitEffect();
	}
	
	/// <summary>
	/// Updates/ticks this effect. If the effect has expired, the effect will
	/// become inactive/disabled.
	/// </summary>
	/// <seealso cref="PlayerEffect.UpdateActiveEffect"/>
	public void Update()
	{
		float timeActive = Time.time - mStartTime;
		bool effectActive = timeActive < mEffectDuration;
		
		if (effectActive)
		{
			UpdateActiveEffect();
		}
		
		else
		{
			DisableActiveEffect();
		}
	}
	
	/// <summary>
	/// Returns whether the effect still affects the player.
	/// </summary>
	public bool Active()
	{		
		return mActive;
	}

	/// <summary>
	/// Called when the player dies; child classes must define how they respond to the
	/// death of a player (but can also choose to do "nothing" if desired).
	/// </summary>
	public abstract void OnPlayerDeath();
	
	/// <summary>
	/// Effect-specific initialization should be performed in child classes.
	/// </summary>
	protected abstract void InitEffect();
	
	/// <summary>
	/// Effect-specific frame update.
	/// </summary>
	protected abstract void UpdateActiveEffect();
	
	/// <summary>
	/// Child classes define what needs to be done when the effect is no longer active.
	/// </summary>
	protected abstract void EndEffect();

	/// <summary>
	/// Sets this effect to inactive and calls the child-class EndEffect method.
	/// </summary>
	/// <seealso cref="PlayerEffect.EndEffect"/>
	protected void DisableActiveEffect()
	{	
		// Prevent effect from getting diabled twice.
		if (mActive)
		{
			mActive = false;
			
			EndEffect();
		}
	}
}

