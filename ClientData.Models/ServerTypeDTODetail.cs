using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ClientData.Models
{
    public class ServerTypeDTODetail
    {
       
        public int ServerTypeId { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
