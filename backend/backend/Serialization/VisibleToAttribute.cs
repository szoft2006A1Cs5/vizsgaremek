namespace backend.Serialization
{
    [Flags]
    public enum VisibilityLevel
    {
        Public = 0,     // Mindenki lathatja
        InRelation = 1, // Csak olyan felhasznalok lathatjak, akik voltak vele aktiv berlesbe
        OwnerOnly = 2,  // Csak maga 
        AdminOnly = 3
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class VisibilityKey : Attribute
    { }

    [AttributeUsage(AttributeTargets.Property)]
    public class VisibleToAttribute : Attribute
    {
        public VisibilityLevel VisibilityLevel { get; }
        public VisibleToAttribute(VisibilityLevel level)
        {
            VisibilityLevel = level;
        }
    }
}
