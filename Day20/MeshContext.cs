namespace AOC2023.Day20;

public partial class Test
{
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
}
