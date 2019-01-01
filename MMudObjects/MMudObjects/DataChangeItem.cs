using System.Text.RegularExpressions;

namespace MMudObjects
{
    public class DataChangeItem
    {
        public string targetProperty;
        public GroupCollection groups;

        public DataChangeItem(string targetProperty, GroupCollection groups)
        {
            this.targetProperty = targetProperty;
            this.groups = groups;
        }
    }
}