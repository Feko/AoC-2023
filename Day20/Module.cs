namespace AOC2023.Day20;

public partial class Test
{
    public abstract class Module
    {
        protected MeshContext Context;
        public string Name {get; protected set;}
        protected List<string> _destinations;
        protected Module(string line) 
        {
            var parts =  line.Split(" -> ");
            SetName(parts[0]);
            _destinations = parts[1].Split(", ").ToList();
        } 
        public void SetContext(MeshContext context) => Context = context;
        public IReadOnlyList<string> Destinations => _destinations;

        public abstract void Receive(Signal signal);
        protected void Send(SignalStrength strength)
        {
            foreach(var destinations in _destinations)
                Context.Send(new(Name, destinations, strength));
        }

        private void SetName(string name)
        {
            if(name[0] == '&' || name[0] == '%')
                Name = name.Substring(1);
            else
                Name = name;
        }
    }

    public class FlipFlop : Module
    {
        private bool IsOn = false;
        public FlipFlop(string destinations) : base(destinations)
        {
        }

        public override void Receive(Signal signal)
        {
            if(signal.Strength == SignalStrength.High)
                return;
            
            IsOn = !IsOn;
            SignalStrength s = IsOn ? SignalStrength.High : SignalStrength.Low;
            Send(s);
        }
    }

    public class Conjunction : Module
    {
        public Dictionary<string, SignalStrength> Memo = new();
        public Conjunction(string line) : base(line)
        {
        }

        public void InitializeMemo(List<string> connected)
        {
            foreach(var module in connected)
                Memo[module] = SignalStrength.Low;
        }

        public override void Receive(Signal signal)
        {
            Memo[signal.From] = signal.Strength;

            SignalStrength s = Memo.Values.All(x => x == SignalStrength.High) 
                ? SignalStrength.Low : SignalStrength.High;

            Send(s);
        }
    }

    public class Broadcast : Module
    {
        public Broadcast(string line) : base(line)
        {
        }

        public override void Receive(Signal signal) => Send(signal.Strength);
    }

    public class Button : Module
    {
        public Button() : base("button -> broadcaster")
        {
        }

        public override void Receive(Signal signal) => throw new NotImplementedException();
        public void Send() => base.Send(SignalStrength.Low);
    }
}
