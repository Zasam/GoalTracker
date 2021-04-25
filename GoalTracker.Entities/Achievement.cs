namespace GoalTracker.Entities
{
    public class Achievement : BaseEntity
    {
        public string Title { get; }
        public string Description { get; }
        public int Experience { get; }

        public string InternalTag { get; }

        private bool unlocked;

        public bool Unlocked
        {
            get => unlocked;
            private set
            {
                unlocked = value;
                OnPropertyChanged();
            }
        }

        public int? UserId { get; }
        public virtual User User { get; }

        public Achievement()
        {
            Title = string.Empty;
            Description = string.Empty;
            Experience = 0;
            Unlocked = false;
        }

        public Achievement(User user, string internalTag, string title, string description, int experience)
        {
            if (user != null)
            {
                User = user;
                UserId = user.Id;
            }

            InternalTag = internalTag;
            Title = title;
            Description = description;
            Experience = experience;
            Unlocked = false;
        }

        /// <summary>
        /// Unlocks the goal if it's not unlocked yet
        /// </summary>
        /// <returns>True if the achievement was unlocked, false if nothing was changed</returns>
        public bool Unlock()
        {
            if (!Unlocked)
            {
                Unlocked = true;
                return true;
            }

            return false;
        }
    }
}