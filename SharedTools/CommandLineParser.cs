using System.Runtime.CompilerServices;

namespace SharedTools
{
    public class CommandLineParser
    {
        private Dictionary<string, string> _options;
        private List<string> _otherArgs;

        public IEnumerable<string> OtherArgs 
        {
            get => _otherArgs;
        }

        public void Parse(string[] args)
        {
            _options = new Dictionary<string, string>();
            _otherArgs = new List<string>();

            if(args == null || args.Length == 0)
            {
                return;
            }

            for(int i=0; i<args.Length; i++)
            {
                string arg = args[i];
                if (arg.StartsWith("--"))
                {
                    if(i == arg.Length - 1) 
                    {
                        throw new Exception($"Missing {arg} value");
                    }
                    if (args[i + 1].IsFlag())
                    {
                        throw new Exception($"Missing {arg} value");
                    }

                    _options[arg] = args[i + 1];
                    i++;
                    continue;
                }
                else if (arg.StartsWith("-"))
                {
                    if (i == arg.Length - 1)
                    {
                        throw new Exception($"Missing {arg} value");
                    }
                    if (args[i + 1].IsFlag())
                    {
                        throw new Exception($"Missing {arg} value");
                    }

                    _options[arg] = args[i + 1];
                    i++;
                    continue;
                }

                _otherArgs.Add(arg);
            }
        }

        public string GetOption(string name)
        {
            bool result = _options.TryGetValue(name, out string value);
            
            if(result == false)
            {
                throw new Exception($"Missing {name} option");
            }
            
            return value;
        }

        public string GetOption(string name1, string name2)
        {
            bool result1 = _options.TryGetValue(name1, out string value1);
            bool result2 = _options.TryGetValue(name2, out string value2);
            
            if(result1 && result2)
            {
                throw new Exception($"Duplicated option: {name1}, {name2}");
            }

            if(!result1 && !result2)
            {
                throw new Exception($"Missing value for option: {name1}, {name2}");
            }

            return result1 ? value1 : value2;
        }       
    }

    internal static class SharedExtensions
    {
        public static bool IsFlag(this string arg)
        {
            return arg.StartsWith("--") || arg.StartsWith("-");
        }
    }
}