using UnityEngine;

public class TriggerActive : AbstractObjectScript
{
    public override void Interact()
    {

        AbstractObjectScript[] objects = FindObjectsByType<AbstractObjectScript>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (AbstractObjectScript obj in objects)
        {
            if (obj == this) continue;
            if (obj.Triggerable)
            {
                obj.Trigger();

            }
            
        }
    }

}
