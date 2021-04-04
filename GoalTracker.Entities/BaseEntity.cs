using System.Runtime.CompilerServices;
using System.ComponentModel;
using System;

namespace GoalTracker.Entities
{
    public class BaseEntity : INotifyPropertyChanged
    {
        /// <summary>
        /// Id of the specified model
        /// </summary>
        public int Id { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;

            changed?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}