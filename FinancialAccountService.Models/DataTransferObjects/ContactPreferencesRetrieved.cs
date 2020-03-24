using System;
using System.Collections.Generic;

namespace FinancialAccountService.Models.DataTransferObjects
{
    public class ContactPreferencesRetrieved
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public string Salutation { get; set; }
        public string EmailAddress { get; set; }
        public DateTime? EmailConfirmedDate { get; set; }
        public string HomePhone { get; set; }
        public string BusinessPhone { get; set; }
        public string OtherPhone { get; set; }
        public string PrimaryPhone { get; set; }
        public DateTime? PhoneChangedDate { get; set; }
        public bool StrategyEmails { get; set; }
        public string StrategyEmailsUpdBy { get; set; }
        public DateTime? StrategyEmailsUpdOn { get; set; }
        public bool PaymentReminderEmails { get; set; }
        public string ReminderEmailsUpdBy { get; set; }
        public DateTime? ReminderEmailsUpdOn { get; set; }
        public bool ContactPreferenceEmail { get; set; }
        public bool ContactPreferenceSMS { get; set; }
        public IEnumerable<Address> Address { get; set; }
    }
}
