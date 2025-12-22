using UnityEngine;

public class TriggerActive : AbstractObjectScript
{
    public override void Interact()
    {
        base.Interact();
        AbstractObjectScript[] objects = FindObjectsByType<AbstractObjectScript>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (AbstractObjectScript obj in objects)
        {
            if (obj.Triggerable)
            {
                obj.Trigger();

            }
            
        }
    }

}
