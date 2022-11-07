namespace ProductionSystem
{
    public class Fact
    {
        public string Id { get; private set; }
        public string Name { get; private set; }

        public Fact(string id, string name)
        {
            Id = id;
            name = name;
        }

        // TODO change for your needs
        public override string ToString()
        {
            return Id;
        }
    }
}