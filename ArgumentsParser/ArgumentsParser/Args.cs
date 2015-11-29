using ArgumentsParser.Marshalers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgumentsParser
{
    public class Args
    {
        private Dictionary<char, IArgumentMarshaler> marshalers;
        private HashSet<char> argsFound;
        private ITwoWayEnumerator<string> currentArgument;

        public Args(string schema, string[] args)
        {
            marshalers = new Dictionary<char, IArgumentMarshaler>();
            argsFound = new HashSet<char>();
            
            ParseSchema(schema);
            ParseArgumentStrings(args.ToList());
        }

        private void ParseSchema(string schema)
        {
            foreach (string element in schema.Split(','))
            {
                if (element.Length > 0)
                {
                    ParseSchemaElement(element.Trim());
                }
            }
        }

        private void ParseSchemaElement(string element)
        {
            char elementId = element[0];
            string elementTail = element.Substring(1);
            ValidateSchemaElementId(elementId);
            if (elementTail.Length == 0)
                marshalers.Add(elementId, new BooleanArgumentMarshaler());
            else if (elementTail.Equals("*"))
                marshalers.Add(elementId, new StringArgumentMarshaler());
            else if (elementTail.Equals("#"))
                marshalers.Add(elementId, new IntegerArgumentMarshaler());
            else if (elementTail.Equals("##"))
                marshalers.Add(elementId, new DoubleArgumentMarshaler());
            else if (elementTail.Equals("[*]"))
                marshalers.Add(elementId, new StringArrayArgumentMarshaler());
            else
                throw new ArgsException(ErrorCode.INVALID_ARGUMENT_FORMAT, elementId, elementTail);
        }

        private void ValidateSchemaElementId(char elementId)
        {
            if (!char.IsLetter(elementId))
            {
                throw new ArgsException(ErrorCode.INVALID_ARGUMENT_NAME, elementId, null);
            }
        }

        private void ParseArgumentStrings(List<string> argsList)
        {
            currentArgument = argsList.GetTwoWayEnumerator();
            while (currentArgument.MoveNext())
            {
                string argString = currentArgument.Current;
                if (argString.StartsWith("-"))
                {
                    ParseArgumentCharacters(argString.Substring(1));
                }
                else
                {
                    currentArgument.MovePrevious();
                    break;
                }
            }
        }

        private void ParseArgumentCharacters(string argChars)
        {
            for (int i = 0; i < argChars.Length; i++)
            {
                ParseArgumentCharacter(argChars[i]);
            }
        }

        private void ParseArgumentCharacter(char argChar)
        {
            IArgumentMarshaler m = marshalers[argChar];
            if (m == null)
            {
                throw new ArgsException(ErrorCode.UNEXPECTED_ARGUMENT, argChar, null);
            }
            else
            {
                argsFound.Add(argChar);
                try
                {
                    m.Set(currentArgument);
                }
                catch (ArgsException ex)
                {
                    ex.ErrorArgumentId = argChar;
                    throw;
                }
            }
        }

        public bool Has(char arg)
        {
            return argsFound.Contains(arg);
        }

        public int NextArgument()
        {
            throw new NotImplementedException();
        }

        public bool GetBoolean(char arg)
        {
            return BooleanArgumentMarshaler.GetValue(marshalers[arg]);
        }

        public string GetString(char arg)
        {
            return StringArgumentMarshaler.GetValue(marshalers[arg]);
        }

        public int GetInt(char arg)
        {
            return IntegerArgumentMarshaler.GetValue(marshalers[arg]);
        }

        public double GetDouble(char arg)
        {
            return DoubleArgumentMarshaler.GetValue(marshalers[arg]);
        }

        public string[] GetStringArray(char arg)
        {
            return StringArrayArgumentMarshaler.GetValue(marshalers[arg]);
        }
    }
}
