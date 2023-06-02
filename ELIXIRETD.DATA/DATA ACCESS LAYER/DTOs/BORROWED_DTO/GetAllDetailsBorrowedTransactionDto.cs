﻿using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO
{
    public class GetAllDetailsBorrowedTransactionDto
    {

        public int Id;

        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        //public string ItemCode { get; set; }
        //public string ItemDescription { get; set; }

        public string TransactionDate { get; set; }

        public string BorrowedDate { get; set; }

        public bool IsApproved { get; set; }

        public string IsApprovedDate { get; set; }

        public decimal TotalBorrowed { get; set; }

        public bool ? IsApproveReturned { get; set; }

        public string IsApproveReturnedDate { get; set; }

        public decimal Consumed { get; set; }
        public decimal ReturnedQuantity { get; set; }

        public bool ? IsReject { get; set; }

        public string IsRejectDate { get; set; }

        public string Remarks { get; set; }






    }
}
