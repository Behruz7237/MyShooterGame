namespace Assets.Scripts.GameAssetsControl
{
    public class GunInfoHolder
    {
        public int FireRate;
        public int AmmoCount;
        public int AmmoMaxCount;

        public GunInfoHolder(int fireRate, int ammoCount, int ammoMax)
        {
            FireRate = fireRate;
            AmmoCount = ammoCount;
            AmmoMaxCount = ammoMax;
        }
    }
}