﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.SETUP_DTO
{
    public class AccountDto
    {

        public int Id { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public bool IsActive { get; set; }
        public string AddedBy { get; set; }
        public string DateAdded { get; set; }


    }
}
