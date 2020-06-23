using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HyperFabric.Client.Options
{
    public sealed class CommandLineHelper<TOptions> where TOptions : new()
    {
        private readonly string _title;
        private readonly string _description;
        private readonly List<string> _args;
        private readonly List<OptionInfo> _information = new List<OptionInfo>();
        private readonly List<IOptionHandler> _handlers = new List<IOptionHandler>();

        private CommandLineHelper(string[] args, string title, string description)
        {
            _title = title;
            _description = description;
            _args = new List<string>(args);
        }

        public IEnumerable<OptionInfo> Information => _information;
        
        public static CommandLineHelper<TOptions> FromArgs(string[] args, string title, string description)
        {
            return new CommandLineHelper<TOptions>(args, title, description);
        }
        
        public OptionInfoHelper<TOptions> For(Expression<Func<TOptions, object>> value)
        {
            var method = GetMethodInfoFromColumnExpression(value);
            var methodName = method.Name.Replace("get_", string.Empty);
            _information.Add(new OptionInfo(methodName));
            return new OptionInfoHelper<TOptions>(this, methodName);
        }

        public CommandLineHelper<TOptions> WithOptionHandler(IOptionHandler handler)
        {
            _handlers.Add(handler);
            return this;
        }
        
        public TOptions Parse()
        {
            var options = PreParse();

            if (options == null) return default;

            OutputTitleAndDescription();

            foreach (var info in _information)
            {
                var arg = FindArg(info);

                if (arg == null && info.IsOptional) continue;
                
                if (info.IsOptional)
                {
                    HandleOptional(info, arg, options);
                }
                else
                {
                    HandleRequired(info, arg, options);
                }
            }

            Console.WriteLine();
            
            InvokeHandlers(options);

            return options;
        }
        
        private static MethodInfo GetMethodInfoFromColumnExpression(Expression<Func<TOptions, object>> column)
        {
            var body = (MemberExpression)column.Body;
            var propInfo = (PropertyInfo)body.Member;
            return propInfo.GetMethod;
        }

        private static bool ContainsHelp(IEnumerable<string> args)
        {
            return args.Any(a => a == "--help" || a == "-h" || a == "?"|| a == "-?" || a == "/?");
        }

        private void OutputOptions(string error = null)
        {
            OutputTitleAndDescription();
            
            if (!string.IsNullOrWhiteSpace(error))
            {
                Console.WriteLine($"Error: {error}");
                Console.WriteLine();
            }

            foreach (var info in _information)
            {
                if (info.DefaultValue != null)
                {
                    Console.WriteLine(
                        $"{info.ShortOption} | {info.LongOption} -> {info.Description} (Default is " +
                        $"{info.DefaultValue}) ({(info.IsOptional ? "Optional" : "Required")})");
                }
                else
                {
                    Console.WriteLine(
                        $"{info.ShortOption} | {info.LongOption} -> {info.Description} " +
                        $"({(info.IsOptional ? "Optional" : "Required")})");
                }
            }
        }

        private void OutputTitleAndDescription()
        {
            Console.WriteLine($"{_title} v{typeof(Program).Assembly.GetName().Version}");
            Console.WriteLine();
            
            Console.WriteLine(_description);
            Console.WriteLine();
        }

        private TOptions PreParse()
        {
            if (ContainsHelp(_args))
            {
                OutputOptions();
                return default;
            }

            if (_args.Count % 2 != 0)
            {
                OutputOptions("The argument count is invalid.");
                return default;
            }
            
            return new TOptions();
        }

        private string FindArg(OptionInfo info)
        {
            var arg = _args.FirstOrDefault(a => a == info.ShortOption || a == info.LongOption);

            if (arg == null && !info.IsOptional)
            {
                OutputOptions($"Expected argument {info.ShortOption} or {info.LongOption} is missing.");
            }

            return arg;
        }

        private void InvokeHandlers(TOptions options)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
            
            foreach (var handler in _handlers)
            {
                var method = handler.GetType().GetMethod("Handle");
                method?.Invoke(handler, flags, null, new object[] {options}, null);
            }
        }
        
        private void HandleOptional(OptionInfo info, string arg, TOptions options)
        {
            var argIndex = _args.IndexOf(arg);
            var argValue = _args[argIndex + 1];
            
            var value = argValue != null ? Convert.ChangeType(argValue, TypeCode.Object) : info.DefaultValue;
            
            if (value == null) return;
            
            var property = typeof(TOptions).GetProperty(info.Name);
            property?.SetValue(options, value);

            Console.WriteLine($"Property {property?.Name} has value {value}");
        }

        private void HandleRequired(OptionInfo info, string arg, TOptions options)
        {
            var argIndex = _args.IndexOf(arg);
            var argValue = _args[argIndex + 1];
            var value = Convert.ChangeType(argValue, TypeCode.Object);

            var property = typeof(TOptions).GetProperty(info.Name);
            property?.SetValue(options, value);
                    
            Console.WriteLine($"Property {property?.Name} has value {value}");
        }
    }
}
