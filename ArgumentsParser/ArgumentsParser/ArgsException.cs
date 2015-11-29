using System;

namespace ArgumentsParser
{
    public class ArgsException : Exception
    {
        public char ErrorArgumentId { get; set; }
        public ErrorCode ErrorCode { get; set; }
        public string ErrorParameter { get; set; }

        public ArgsException(ErrorCode code) : base()
        {
            ErrorCode = code;
        }

        public ArgsException(ErrorCode code, string parameter) : this(code)
        {
            ErrorParameter = parameter;
        }

        public ArgsException(ErrorCode code, char elementId, string parameter) : this(code, parameter)
        {
            ErrorArgumentId = elementId;
        }

        public override string Message
        {
            get
            {
                switch (ErrorCode)
                {
                    case ErrorCode.OK:
                        return "TILT: Should not get here.";
                    case ErrorCode.UNEXPECTED_ARGUMENT:
                        return string.Format("Argument -{0} unexpected.", ErrorArgumentId);
                    case ErrorCode.MISSING_STRING:
                        return string.Format("Could not find string parameter for -{0}.", ErrorArgumentId);
                    case ErrorCode.INVALID_INTEGER:
                        return string.Format("Argument -{0} expects an integer but was '{1}'.", ErrorArgumentId, ErrorParameter);
                    case ErrorCode.MISSING_INTEGER:
                        return string.Format("Could not find integer parameter for -{0}.", ErrorArgumentId);
                    case ErrorCode.INVALID_DOUBLE:
                        return string.Format("Argument -{0} expects a double but was '{1}'.", ErrorArgumentId, ErrorParameter);
                    case ErrorCode.MISSING_DOUBLE:
                        return string.Format("Could not find double parameter for -{1}.", ErrorArgumentId);
                    case ErrorCode.INVALID_ARGUMENT_NAME:
                        return string.Format("'{0}' is not a valid argument name.", ErrorArgumentId);
                    case ErrorCode.INVALID_ARGUMENT_FORMAT:
                        return string.Format("'{0}' is not a valid argument format.", ErrorParameter);
                }
                return string.Empty;
            }
        }
    }
}