namespace ProductionSystem
{
    public class Fact
    {
        public Fact(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; }

        public string Name { get; }

        // TODO change for your needs
        public override string ToString()
        {
            return Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj?.GetType() != typeof(Fact))
                return false;
            return this.Equals(obj as Fact);
        }

        private bool Equals(Fact fact)
        {
            return this.Id == fact.Id;
        }
    }
}