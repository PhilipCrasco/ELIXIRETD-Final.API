﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.WAREHOUSE_DTO
{
    public class WarehouseReceivingDto
    {
        public int Id { get; set; }
        public int PoNumber { get; set; }

        public string SINumber { get; set; }
        public DateTime PoDate { get; set; }
        public int PrNumber { get; set; }
        public DateTime PrDate { get; set; }

        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Supplier { get; set; }
        public string Uom { get; set; }
        public decimal QuantityOrdered { get; set; }
        public decimal ActualGood { get; set; }

        public bool IsActive { get; set; }
        public decimal ActualRemaining { get; set; }
        public string DateReceive { get; set; }

        public decimal TotalReject { get; set; }

        public decimal ActualAdd { get; set; }

        public string LotSection { get; set; }

        public decimal UnitPrice { get; set; }
        
        public decimal TotalUnitPrice { get; set; }

        public string TransactionType { get; set; }



    }
}
