using System;
using System.Collections.Generic;
using System.Text;

namespace Plugins.ToolKits.Validatement
{
    public interface IErrorCollection
    {
        int Count { get; }

        ICollection<string> this[string propertyName] { get; }

        bool TryGetErrors(string propertyName, out ICollection<string> values);

    }


    internal class ErrorCollection : Dictionary<string, ICollection<string>>, IErrorCollection
    {
        public bool TryGetErrors(string propertyName, out ICollection<string> values)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            return TryGetValue(propertyName, out values);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, ICollection<string>> item in this)
            {
                //sb.Append(item.Key).AppendLine(); 
                foreach (string v in item.Value)
                {
                    sb.Append(v).AppendLine();
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}