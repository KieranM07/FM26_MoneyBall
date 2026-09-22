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

    public class WFBState
    {
        public List<WingFullBack> WingFullBacks { get; set; } = new();
    }

    public class DMState
    {
        public List<DefMid> DefMids { get; set; } = new();
    }

    public class MState
    {
        public List<Mid> Mids { get; set; } = new();
    }

    public class WState
    {
        public List<Winger> Wingers { get; set; } = new();
    }

    public class STState
    {
        public List<Striker> Strikers { get; set; } = new();
    }
}

