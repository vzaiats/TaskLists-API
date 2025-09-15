namespace TaskListsAPI.Domain.Entities
{
    // User
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<TaskCollection> OwnedCollections { get; private set; } = new List<TaskCollection>();
        public ICollection<Share> SharedCollections { get; private set; } = new List<Share>();

        public User() { }

        public User(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(Constants.ErrorMessages.EmptyUserName);

            Id = Guid.NewGuid();
            Name = name;
        }
    }
}