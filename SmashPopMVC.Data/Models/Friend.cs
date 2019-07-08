using SmashPopMVC.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SmashPopMVC.Data.Models
{
    public class Friend
    {
        public int ID { get; set; }
        [ForeignKey("RequestedBy")]
        public string RequestedByID { get; set; }
        public virtual ApplicationUser RequestedBy { get; set; }
        [ForeignKey("RequestedTo")]
        public string RequestedToID { get; set; }
        public virtual ApplicationUser RequestedTo { get; set; }

        public DateTime? RequestTime { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public RequestFlag RequestFlag { get; set; }

        [NotMapped]
        public bool Approved => RequestFlag >= RequestFlag.Approved;

    }

    public enum RequestFlag
    {
        None,
        Rejected,
        Blocked,
        Spam,
        Approved,
        PartnerRequest,
        Partnered,
    };
}
