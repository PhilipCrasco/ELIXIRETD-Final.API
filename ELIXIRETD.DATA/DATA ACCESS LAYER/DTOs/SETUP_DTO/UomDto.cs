﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO
{
    public class UomDto
    {

        public int Id { get; set; }
        public string UomCode { get; set; }
        public string UomDescription { get; set; }
        public string DateAdded { get; set; }
        public string AddedBy { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public string SyncStatus { get; set; }

        public string ModifyDate { get; set; }
        public string ModifyBy { get; set; }

        public string SyncDate { get; set; }




    }
}
