﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.WAREHOUSE_MODEL
{
    public class Warehouse_Receiving : BaseEntity
    {
        public int PoSummaryId { get; set; }
        public string ItemCode { get; set; }

        public string ItemDescription { get; set; }
        public int PoNumber { get; set; }
        public string Uom { get; set; }
        public string Supplier { get; set; }

        [Column(TypeName = "Date")]
        public DateTime ReceivingDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ActualDelivered { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ActualGood { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalReject { get; set; }

        public string LotSection { get; set; }
        public string TransactionType { get; set; }
        public int? MiscellaneousReceiptId { get; set; }
        public string Reason { get; set; }
        public bool IsActive { get; set; } = true;
        public bool? IsWarehouseReceived { get; set; }
        public bool? ConfirmRejectByWarehouse { get; set; }
        public string AddedBy { get; set; }
        public DateTime ActualReceivingDate { get; set; } = DateTime.Now;

        public string SINumber { get; set; }
        public decimal UnitPrice { get; set; }

        public string AccountCode { get; set; }
        public string AccountTitles { get; set; }
        public string EmpId { get; set; }
        public string FullName { get; set; }



    }
}
