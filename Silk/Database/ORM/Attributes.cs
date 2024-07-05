namespace Silk.ORM
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class PrimaryKeyAttribute : Attribute
    {}

    public class AutoIncrementAttribute : Attribute
    {}

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class TableAttribute : Attribute
    {
        public string Name { get; }
        public TableAttribute(string name)
        {
            Name = name;
        }
    }
}
