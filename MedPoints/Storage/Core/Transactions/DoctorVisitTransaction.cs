using System;

namespace Storage.Core.Transactions
{
    public class AppointmentToTheDoctorTransaction  : ITransaction
    {
        public string Id { get; set; }
        public string Sender { get; set; }
        public string Signature { get; set; }
        public TransactionType Type => TransactionType.VisitToTheDoctor;
        
        public string DoctorId { get; set; }
        public string ClinicId { get; set; }
        public string ServiceId { get; set; }
        
        public string Description { get; set; }
        
        public DateTime Date { get; set; }
    }
}