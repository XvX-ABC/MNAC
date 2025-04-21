using System;

namespace Tests.Weapons.Launcher
{
    public enum SupplyDepotType
    {
        Launcher
    }
    public interface ISupplyDepot
    {
        public SupplyDepotType Type { get; }
        public bool StartToSupply(Func<int,int> supplyFunc);
        public bool EndToSupply(Func<int,int> supplyFunc);
    }
}