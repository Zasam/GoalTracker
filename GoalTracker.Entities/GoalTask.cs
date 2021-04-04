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

        public string Title { get; set; }
        public int? GoalId { get; set; }
        public virtual Goal Goal { get; set; }

        public string Notes { get; set; }

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