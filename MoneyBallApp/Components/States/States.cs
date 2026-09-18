using MoneyBallApp.Models;

namespace MoneyBallApp.Components.States
{
    public class KeeperState
    {
        public List<Keeper> Keepers { get; set; } = new();
    }

    public class CBState
    {
        public List<CentreBack> Centrebacks { get; set; } = new();
    }
}
