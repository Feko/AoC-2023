using Microsoft.VisualStudio.TestPlatform.Utilities;

namespace AOC2023.Day20;

public class Test
{
    public enum SignalStrength { Low, High }
    public record Signal(string From, string To, SignalStrength Strength);

    public class MeshContext
    {
        public long CountLow = 0;
        public long CountHigh = 0;
        public Queue<Signal> Queue = new();
        public Dictionary<string, Module> Modules = new();
        private Button Button;
        public Dictionary<string, Action<Signal>> ProcessSendCallback = new();

        public void PushButton() => Button.Send();

        public void Initialize(List<string> lines)
        {
            Button = new Button();
            Modules["button"] = Button;

            foreach(var line in lines)
                AddModule(line);

            InitializeConjunctions();

            foreach(var module in Modules)
                module.Value.SetContext(this);
        }

        public void Send(Signal signal)
        {
            if(signal.Strength == SignalStrength.Low)
                CountLow++;
            else
                CountHigh++;
            
            Queue.Enqueue(signal);
        }

        public void AddModule(string line)
        {
            Module module = line[0] switch
            {
                'b' => new Broadcast(line),
                '%' => new FlipFlop(line),
                '&' => new Conjunction(line),
                _ => throw new Exception("This is not supposed to happen")
            };
            Modules[module.Name] = module;
        }

        private void InitializeConjunctions()
        {
            var conjunctions = Modules.Values.Where(x => x.GetType() == typeof(Conjunction));
            foreach(var c in conjunctions)
            {
                var connected = Modules.Where(m => m.Value.Destinations.Contains(c.Name)).Select(m => m.Key).ToList();
                (c as Conjunction).InitializeMemo(connected);
            }
        }

        public void ProcessSignals()
        {
            while (Queue.Any())
            {
                var signal = Queue.Dequeue();
                if (ProcessSendCallback.ContainsKey(signal.From))
                    ProcessSendCallback[signal.From](signal);

                if (!Modules.ContainsKey(signal.To))
                    continue;

                Modules[signal.To].Receive(signal);
            }
        }

        public long FactorSignals() => CountHigh * CountLow;
        public Module GetModule(string name) => Modules[name];
    }

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

    [Fact]
    public void Part1()
    {
        var lines = File.ReadAllLines("C:\\DEV\\AoC-2023\\Day20\\sample1.txt").ToList(); long expected = 32000000;
        //var lines = File.ReadAllLines("C:\\DEV\\AoC-2023\\Day20\\sample2.txt").ToList(); long expected = 11687500;
        //var lines = File.ReadAllLines("C:\\DEV\\AoC-2023\\Day20\\input.txt").ToList(); long expected = 11687500;
        var context = new MeshContext();
        PushButton(context, lines, 1000);
        Assert.Equal(expected, context.FactorSignals());
    }

    [Fact]
    public void Part2()
    {
        var lines = File.ReadAllLines("C:\\DEV\\AoC-2023\\Day20\\input.txt").ToList(); 
        var context = new MeshContext();
        long result = PushButtonWithCallback(context, lines);
        Assert.Equal(231657829136023, result);
    }

    private long PushButtonWithCallback(MeshContext context, List<string> lines)
    {
        // Brute-forced for 5 hours, up to 3B with no luck
        // Inspecting my visual aid, I can see that it will only receive a low pulse if all the Conjunction above of "lg" output HIGH simultaneously.
        // Let's try something else

        context.Initialize(lines);
        int pushAmount = 0;

        // Get module name above RX
        var moduleName = lines.First(l => l.IndexOf(" -> rx") > 0).Split(" -> ")[0].Substring(1);

        // What we really need is the modules ABOVE the one above RX ("vc", "nb", "ls" and "vg" in my case)
        var aboveRx = (context.GetModule(moduleName) as Conjunction);
        var conjunctionKeys = aboveRx.Memo.Keys.ToList();

        
        Dictionary<string, List<int>> loopConjunctions = conjunctionKeys.ToDictionary(k => k, v => new List<int>());

        // Add some callbacks to find out WHEN those four modules actually send a "High" output.
        foreach (var c in conjunctionKeys)
        {
            context.ProcessSendCallback[c] = signal =>
            {
                if (signal.Strength == SignalStrength.High)
                {
                    loopConjunctions[signal.From].Add(pushAmount);
                }
            };
        }

        // Let it run for a while()
        for (int i = 0; i < 10_000; i++)
        {
            pushAmount++;
            context.PushButton();
            context.ProcessSignals();
        }

        // Factor the results - All prime numbers, so no need for LCM
        var loopSizes = loopConjunctions.Values.Select(x => x.First()).ToList();

        long result = 1;
        foreach (var d in loopSizes)
            result *= d;

        return result;
    }

    private void PushButton(MeshContext context, List<string> lines, int amount)
    {
        context.Initialize(lines);
        for (int i = 0; i < amount; i++)
        {
            context.PushButton();
            context.ProcessSignals();
        }
    }
}
