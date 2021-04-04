using System;

namespace GoalTracker.Entities
{
    public class GoalAppointment : BaseEntity
    {
        public GoalAppointment()
        {
        }

        public GoalAppointment(Goal parent, DateTime appointmentDate)
        {
            Goal = parent;
            GoalId = parent.Id;
            AppointmentDate = appointmentDate;
            Success = null;
            Approved = false;
            ApprovalDate = null;
        }

        public int? GoalId { get; set; }
        public virtual Goal Goal { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public string ApprovalDateText
        {
            get
            {
                if (!ApprovalDate.HasValue)
                    return " - ";
                return ApprovalDate.Value.ToString("dd.MM.yyyy - HH:mm");
            }
        }

        public DateTime AppointmentDate { get; set; }

        public string AppointmentDateText => AppointmentDate.ToString("dd.MM.yyyy - HH:mm");

        public bool Approved { get; set; }
        public bool? Success { get; set; }

        public void Approve(bool success)
        {
            Approved = true;
            Success = success;
            ApprovalDate = DateTime.Now;
        }
    }

    /// <summary>
    ///     The goal notification interval enum
    /// </summary>
    public enum GoalAppointmentInterval
    {
        Halbstündlich = 0,
        Stündlich = 1,
        Halbtäglich = 2,
        Täglich = 3,
        Wöchentlich = 4
    }
}