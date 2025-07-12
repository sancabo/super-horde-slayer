using System.Collections.Generic;

public interface ISpellProjectile
{
    public void ApplyDamageMultiplier(float multiplier);
    public int GetDamage();
    public void SetDammage(int damage);
    public void SetOnHitEffects(List<IOnHitEffect> onHitEffects);

    public void SetOnKillEffects(List<IOnKillEffect> effect);
    public float GetDamageMultiplierr();
}