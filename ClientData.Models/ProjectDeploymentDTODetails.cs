using System;
using System.Collections.Generic;
using System.Text;

namespace ClientData.Models
{
    public class ProjectDeploymentDTODetails
    {
        public int ProjectDeploymentId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public DateTime DeploymentDate { get; set; }
        public int ServerId { get; set; }
        public string ServerName { get; set; }
        public string Features { get; set; }
        public string Version { get; set; }
        public string ActualFileName { get; set; }
        public string StoreAsFileName { get; set; }
        public string Notes { get; set; }
    }
}
