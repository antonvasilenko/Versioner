using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Versioner
{
    public class Version
    {
        private uint?[] _parts;

        public uint? A
        {
            get { return _parts[0]; }
            set { _parts[0] = value; }
        }

        public uint? B
        {
            get { return _parts[1]; }
            set { _parts[1] = value; }
        }

        public uint? C
        {
            get { return _parts[2]; }
            set { _parts[2] = value; }
        }

        public uint? D
        {
            get { return _parts[3]; }
            set { _parts[3] = value; }
        }

        public Version(uint?[] parts)
        {
            if (parts == null)
                throw new ArgumentNullException("parts");
            if (parts.Length != 4)
                throw new ArgumentException("parts array should contain 4 elements");
            _parts = new uint?[parts.Length];
            Array.Copy(parts, _parts, parts.Length);
        }

        public Version(uint? a, uint? b, uint? c, uint? d)
        {
            A = a;
            B = b;
            C = c;
            D = d;
        }

        public Version(string versionMask)
        {
            string error;
            if (!Parse(versionMask, out error))
            {
                throw new ArgumentException("Wrong version mask format. " + error);
            }
        }

        private bool Parse(string versionMask, out string error)
        {
            if (String.IsNullOrWhiteSpace(versionMask))
            {
                error = "Version mask should be provided for 'read' execution mode";
                return false;
            }

            var stringParts = versionMask.Split('.').ToList();
            if (stringParts.Count != 4)
            {
                error = "Version mask should in format 'a.b.c.d' where each letter stands for positive number or #";
                return false;
            }

            var errors = new StringBuilder();
            var numberParts = new uint?[4];
            for (int i = 0; i < stringParts.Count; i++)
            {
                var part = stringParts[i];
                if (part == "#")
                    continue;
                uint number;
                if (!uint.TryParse(part, out number))
                {
                    errors.AppendFormat("Version part '{0}' cannot be parsed neither to positive number nor to #", part);
                    errors.AppendLine();
                }
                else
                {
                    numberParts[i] = number;
                }
            }
            if (errors.Length > 0)
            {
                error = errors.ToString();
                return false;
            }
            error = null;
            _parts = numberParts;
            return true;
        }

        public override string ToString()
        {
            List<string> textParts = _parts.Select(p => p == null ? "#" : p.ToString()).ToList();
            return string.Join(".", textParts);
        }
    }
}