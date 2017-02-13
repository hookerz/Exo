using UnityEngine;

public class TowerForceField : ForceField
{
    public GameObject tower;
    private ILaserStrikeTarget laserStrikeTarget;

    protected override void Start()
    {
        base.Start();

        laserStrikeTarget = tower.GetComponent<ILaserStrikeTarget>();
        
        laserStrikeTarget.LaserStrikeStarted += LaserStrikeTarget_LaserStrikeStarted;
        laserStrikeTarget.LaserStrikeEnded += Laser_LaserStrikeEnded;
    }

    private void LaserStrikeTarget_LaserStrikeStarted()
    {
        on = true;
    }

    private void Laser_LaserStrikeEnded()
    {
        on = false;
    }
}
