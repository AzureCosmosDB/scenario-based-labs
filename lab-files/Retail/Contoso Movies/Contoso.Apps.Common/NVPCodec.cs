using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace Contoso.Apps.Common
{
    /// <summary>
    /// Encodes and Decodes Name Value Pairs for transmitting data to/from
    /// our APIs.
    /// </summary>
    public sealed class NVPCodec : NameValueCollection
    {
        private const string AMPERSAND = "&";
        private const string EQUALS = "=";
        private static readonly char[] AMPERSAND_CHAR_ARRAY = AMPERSAND.ToCharArray();
        private static readonly char[] EQUALS_CHAR_ARRAY = EQUALS.ToCharArray();

        /// <summary>
        /// Serializes the collection to an Http-encoded string.
        /// </summary>
        /// <returns></returns>
        public string Encode()
        {
            var sb = new StringBuilder();
            var firstPair = true;
            foreach (var kv in AllKeys)
            {
                if (!firstPair)
                {
                    sb.Append(AMPERSAND);
                }

                sb.Append(HttpUtility.UrlEncode(kv)) // name
                    .Append(EQUALS)
                    .Append(HttpUtility.UrlEncode(this[kv])); // value

                firstPair = false;
            }
            return sb.ToString();
        }

        /// <summary>
        /// Deserializes an an Http-encoded string.
        /// </summary>
        /// <param name="nvpstring"></param>
        public void Decode(string nvpstring)
        {
            Clear();
            foreach (var nvp in nvpstring.Split(AMPERSAND_CHAR_ARRAY))
            {
                var tokens = nvp.Split(EQUALS_CHAR_ARRAY);
                if (tokens.Length >= 2)
                {
                    Add(
                        HttpUtility.UrlDecode(tokens[0]), // name
                        HttpUtility.UrlDecode(tokens[1]) //value
                        );
                }
            }
        }

        public void Add(string name, string value, int index)
        {
            this.Add(GetArrayName(index, name), value);
        }

        public void Remove(string arrayName, int index)
        {
            this.Remove(GetArrayName(index, arrayName));
        }

        public string this[string name, int index]
        {
            get
            {
                return this[GetArrayName(index, name)];
            }
            set
            {
                this[GetArrayName(index, name)] = value;
            }
        }

        private static string GetArrayName(int index, string name)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "index cannot be negative : " + index);
            }
            return name + index;
        }
    }
}
