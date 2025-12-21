using UnityEngine;

public interface IElectric
{
    public bool Charged { get; protected set; }
    public void Charge();

    public void ChargeActive();
}
