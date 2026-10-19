namespace Voyara.Domain.Enums
{
    /// <summary>Kind of visitor a ticket price applies to.</summary>
    public enum VisitorType
    {
        /// <summary>Regular adult visitor.</summary>
        Adult,

        /// <summary>Child visitor.</summary>
        Child,

        /// <summary>Student with a valid student card.</summary>
        Student,

        /// <summary>Senior visitor.</summary>
        Senior
    }
}
