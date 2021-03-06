using System;
using System.Collections.Generic;
using System.Text;

namespace ClientData.Models
{
    /// <summary>
    /// To maintain details of particular team member
    /// Specially used to getById method and Edit method of team member
    /// </summary>
    public class TeamMemberDTODetail
    {
        public int TeamMemberId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string AlternateContact { get; set; }
        public DateTime? DOJ{ get; set; }
        public string Notes { get; set; }
    }
}
