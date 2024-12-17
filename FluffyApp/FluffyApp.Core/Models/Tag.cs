namespace FluffyApp.Core.Models
{
    public class Tag
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid UserId { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
