using System.Linq;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Z80AssemblyParsing.Parsing
{
    class HexParser
    {
        private string _hexPrefix;
        private string _hexSuffix;
        private Regex _hexByteRegex;
        private Regex _hexWordRegex;
        private Regex _hexByteRegexZ;
        private Regex _hexWordRegexZ;

        public HexParser(string hexPrefix = "", string hexSuffix = "h")
        {
            _hexPrefix = hexPrefix ?? string.Empty;
            _hexSuffix = hexSuffix ?? string.Empty;
            _hexByteRegex = new Regex(_hexPrefix + "[0-9a-f]?[0-9a-f]" + _hexSuffix, RegexOptions.IgnoreCase);
            _hexByteRegexZ = new Regex(_hexPrefix + "0[0-9a-f]?[0-9a-f]" + _hexSuffix, RegexOptions.IgnoreCase);
            _hexWordRegex = new Regex(_hexPrefix + "[0-9a-f][0-9a-f][0-9a-f][0-9a-f]" + _hexSuffix, RegexOptions.IgnoreCase);
            _hexWordRegexZ = new Regex(_hexPrefix + "0[0-9a-f][0-9a-f][0-9a-f][0-9a-f]" + _hexSuffix, RegexOptions.IgnoreCase);
        }

        public bool IsHexNumber(string operandString)
        {
            return _hexWordRegex.IsMatch(operandString)
                || _hexWordRegexZ.IsMatch(operandString)
                || _hexByteRegex.IsMatch(operandString)
                || _hexByteRegexZ.IsMatch(operandString);
        }

        public bool TryByteParse(string sourceString, out byte number)
        {
            if (_hexByteRegex.IsMatch(sourceString) || _hexByteRegexZ.IsMatch(sourceString))
            {
                var hexNoPrefixSuffix = sourceString.ToCharArray().Skip(_hexPrefix.Length).ToArray();
                hexNoPrefixSuffix = hexNoPrefixSuffix.Take(hexNoPrefixSuffix.Length - _hexSuffix.Length).ToArray();
                return byte.TryParse(new string(hexNoPrefixSuffix), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out number);
            }
            if (byte.TryParse(sourceString, out number))
                return true;
            return false;
        }

        public bool TryUShortParse(string sourceString, out ushort number)
        {
            if (_hexWordRegex.IsMatch(sourceString) || _hexWordRegexZ.IsMatch(sourceString))
            {
                var hexNoPrefixSuffix = sourceString.ToCharArray().Skip(_hexPrefix.Length).ToArray();
                hexNoPrefixSuffix = hexNoPrefixSuffix.Take(hexNoPrefixSuffix.Length - _hexSuffix.Length).ToArray();
                return ushort.TryParse(new string(hexNoPrefixSuffix), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out number);
            }
            if (ushort.TryParse(sourceString, out number))
                return true;
            return false;
        }
    }
}
