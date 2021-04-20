using System.Collections.Generic;

namespace GoalTracker.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; }

        public virtual IEnumerable<Achievement> Achievements { get; set; }

        public User()
        {
            Name = string.Empty;
        }

        public User(string name)
        {
            Name = name;
            Achievements = new List<Achievement>();
        }
    }
}