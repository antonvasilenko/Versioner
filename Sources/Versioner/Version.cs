using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Versioner
{
    public class Version: ICloneable
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

        public uint? this[int index]
        {
            get { return _parts[index]; }
            set { _parts[index] = value; }
        }

        public bool IsMask
        {
            get { return _parts.Any(p => p == null); }
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
            _parts = new uint?[4];
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
                throw new ArgumentException(string.Format("Mask {0} cannot be parsed. {1}", versionMask, error));
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
                error = "Version mask should in format 'a.b.c.d' where each letter stands for positive number or # or *";
                return false;
            }

            var errors = new StringBuilder();
            var numberParts = new uint?[4];
            for (int i = 0; i < stringParts.Count; i++)
            {
                var part = stringParts[i];
                if (part == "#" || part == "*")
                    continue;
                uint number;
                if (!uint.TryParse(part, out number))
                {
                    errors.AppendFormat("Version part '{0}' cannot be parsed neither to positive number nor to # or *", part);
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



        /// <summary>
        ///  2.2.2.2 (1.1.1.1) -> 2.2.2.2 <br/>
        ///  #.#.2.2 (1.1.1.1) -> 1.1.2.2 <br/>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Version ApplyTo(string input)
        {
            if (!IsMask)
                return (Version)Clone();
            
            var output = new Version(input);
            intApplyTo(output);
            return output;
        }

        public Version ApplyTo(Version input)
        {
            if (!IsMask)
                return (Version)Clone();

            var output = (Version)input.Clone();
            intApplyTo(output);
            return output;
        }

        private void intApplyTo(Version input)
        {
            for (int i = 0; i < _parts.Length; i++)
            {
                if (_parts[i] != null)
                {
                    input[i] = _parts[i];
                }
            }
        }

        public override string ToString()
        {
            List<string> textParts = _parts.Select(p => p == null ? "#" : p.ToString()).ToList();
            return string.Join(".", textParts);
        }

        public object Clone()
        {
            return new Version(A, B, C, D);
        }
    }
}