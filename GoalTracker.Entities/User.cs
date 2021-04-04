using System.Collections.Generic;
using System.Linq;

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
        }

        public int GetExperience()
        {
            var unlocked = Achievements != null && Achievements.Count(a => a.Unlocked) > 0;
            int experience = 0;

            if (unlocked)
            {
                foreach (var achievement in Achievements.Where(a => a.Unlocked))
                    experience += achievement.Experience;
            }

            return experience;
        }
    }
}