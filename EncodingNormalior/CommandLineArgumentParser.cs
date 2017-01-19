using System;
using System.Collections.Generic;
using System.Linq;

namespace EncodingNormalior
{
    public class CommandLineArgumentParser
    {
        private readonly List<CommandLineArgument> _arguments;

        public CommandLineArgumentParser(string[] args)
        {
            _arguments = new List<CommandLineArgument>();

            for (var i = 0; i < args.Length; i++)
            {
                _arguments.Add(new CommandLineArgument(_arguments, i, args[i]));
            }
        }

        public static CommandLineArgumentParser Parse(string[] args)
        {
            return new CommandLineArgumentParser(args);
        }

        public CommandLineArgument Get(string argumentName)
        {
            return _arguments.FirstOrDefault(p => p == argumentName);
        }

        public IEnumerable<CommandLineArgument> GetEnumerator()
        {
            foreach (var temp in _arguments)
            {
                yield return temp;
            }
        }

        public bool Has(string argumentName)
        {
            return _arguments.Count(p => p == argumentName) > 0;
        }
    }
}