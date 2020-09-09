using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace myFinPort.Models
{
    public class Invitation
    {
        public int Id { get; set; }

        public int HouseholdId { get; set; }

        public virtual Household Household { get; set; }

        public string Body { get; set; }

        // this should start as true and then be flipped to false under certain conditions
        public bool IsValid { get; set; }

        public DateTime Created { get; set; }

        // TTL - Time to Live - the number of days the invitation is valid for
        public int TTL { get; internal set; }

        // if(DateTime.Now > Created.AddDays(TTL)) {IsValid = false;}

        [Display(Name = "Recipient Email")]
        public string RecipientEmail { get; set; }

        public Guid Code { get; set; }

        public Invitation()
        {
            Created = DateTime.Now;
            IsValid = true;
            TTL = 7;  // invitation is valid for seven days.
        }

        public Invitation(int hhId)
        {
            Created = DateTime.Now;
            IsValid = true;
            TTL = 7;  // invitation is valid for seven days.
            HouseholdId = hhId;
        }

    }
}