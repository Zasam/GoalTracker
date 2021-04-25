namespace GoalTracker.Entities
{
    public class GoalTask : BaseEntity
    {
        private bool completed;

        public GoalTask()
        {
        }

        public GoalTask(Goal parent, string title, string notes, bool completed)
        {
            Goal = parent;
            GoalId = parent.Id;
            Title = title;
            Notes = notes;
            Completed = completed;
        }

        public string Title { get; }
        public int? GoalId { get; }
        public virtual Goal Goal { get; }

        public string Notes { get; }

        public bool Completed
        {
            get => completed;
            set
            {
                completed = value;
                OnPropertyChanged();
            }
        }
    }
}