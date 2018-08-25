using System;

namespace StorageRest.App
{
    public class VisitToDoctorTransaction
    {
        public string Id { get; set; }
        public string UserAddress { get; set; }
        
        public string DoctorId { get; set; }
        public string ClinicId { get; set; }
        public string ServiceId { get; set; }
        
        public string Description { get; set; }
        
        public DateTime Date { get; set; }
    }
}