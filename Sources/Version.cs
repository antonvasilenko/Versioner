using System;
using System.Collections.Generic;
using System.Linq;

namespace Versioner
{
    public class Version
    {
        private readonly uint?[] _parts;

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

        public override string ToString()
        {
            List<string> textParts = _parts.Select(p => p == null ? "#" : p.ToString()).ToList();
            return string.Join(".", textParts);
        }
    }
}