//#define LOG_ENABLED

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using GuardianEnums;

public abstract partial class PlayerControllerBase : MonoBehaviour, IDamageReceiver 
{	
	/// <summary>
	/// Any "player effects" that are associated with the player (such as 
	/// power-ups).
	/// 
	/// :TODO: need to add code to handle case if player picks up an 
	/// identical power-up that's still active. (Chase)
	/// 
	/// :TODO: need to add method that clears all active effects when the
	/// player dies, match ends, etc.
	/// </summary>
	/// <seealso cref="PlayerControllerBase.PlayerEffectsUpdate"/>
	protected List<PlayerEffect> mPlayerEffects;

    protected virtual void PostStart()
    {
        mPlayerEffects = new List<PlayerEffect>();
    }
	
	/// <summary>
	/// Updates any "player effects" that are considered active on the player.
	/// Also removes any player effects that are no longer considered active.
	/// </summary>
	/// <seealso cref="PlayerEffect"/>
	protected void PlayerEffectsUpdate()
	{
        for (int i = mPlayerEffects.Count - 1; i >= 0; --i)
        {
            PlayerEffect effect = mPlayerEffects[i];
            effect.Update();
            
		    // Remove all player effects from the list that aren't active anymore.
		    // Note that this should be called after PlayerEffect::Update since
		    // Update could change the state of whether the effect is still 
		    // considered active.
            if (!effect.Active())
			{
                mPlayerEffects.RemoveAt(i);
			}
        }
	}

    public virtual void HandleDeath(PlayerControllerBase attacker)
    {
		// Notify all active player effects that the player has died.
		foreach (PlayerEffect currentEffect in mPlayerEffects)
		{
			currentEffect.OnPlayerDeath();
		}
	}
	
	/// <summary>
	/// Activates the given player effect by adding it to this player's list
	/// of player effects and starts the effect. If there's an effect of the
	/// same type already active for the player, the effect is ignored.
	/// 
	/// :TODO: ideally, when an effect is active, and the user picks up the 
	/// effect again, there should be a case-by-case basis of handling 
	/// what to do. For example, for invisibility, you may want to extend the
	/// duration of how long invisibility lasts for.
	/// 
	/// </summary>
	/// <seealso cref="PlayerControllerBase.mPlayerEffects"/>
	public void ActivatePlayerEffect(PlayerEffect effect)
	{
		var effectType = effect.GetType();
		
		foreach (PlayerEffect currentEffect in mPlayerEffects)
		{
			var currentEffectType = currentEffect.GetType();
			
			// Prevent effects of the same type getting applied twice
			if (effectType.IsAssignableFrom(currentEffectType) || currentEffectType.IsAssignableFrom(effectType))
			{
				return;
			}
		}
		
		mPlayerEffects.Add (effect);
		effect.StartEffect();
	}
	
	/// <summary>
	/// Used to verify if this player has the given effect. For example, this
	/// method can be used to guard against consuming a pickup if the player
	/// already has the given pickup effect active.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this player has the given player effect, <c>false</c> otherwise.
	/// </returns>
	/// <param name='effectType'>
	/// The PlayerEffect object type to check for.
	/// </param>
	public bool HasPlayerEffect(Type effectType)
	{
		bool hasEffect = false;
		
		foreach (PlayerEffect currentEffect in mPlayerEffects)
		{
			var currentEffectType = currentEffect.GetType();
			
			if (currentEffectType == effectType)
			{
				hasEffect = true;
				break;
			}
		}
		
		return hasEffect;
	}
}
