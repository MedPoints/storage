using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedPoints.Core
{
    public enum TransactionType
    {
        MoneyTransfer,
        Booking,
        StoreMedicalRecord,
        TransferMedicalRecord,
        HealthInsuranceFundContribution,
        CharityFundDonation,
        Registration,
        Recharge,
        AddingMedicalRecordsToHistory
    }
}