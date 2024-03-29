﻿using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.REPORTS_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIRETD.DATA.CORE.INTERFACES.REPORTS_INTERFACE
{
    public interface IReports
    {

        Task<PagedList<DtoWarehouseReceivingReports>> WarehouseReceivingReports(UserParams userParams, string DateFrom , string DateTo , string Search);

        Task<PagedList<DtoMoveOrderReports>> WarehouseMoveOrderReports(UserParams userParams, string DateFrom , string DateTo, string Search);

        Task<PagedList<DtoMiscReports>> MiscReports(UserParams userParams, string DateFrom, string DateTo, string Search);

        Task<PagedList<DtoMiscIssue>> MiscIssue (UserParams userParams, string DateFrom, string DateTo, string Search);



        Task<PagedList<BorrowedTransactionReportsDto>> BorrowedTransactionReports(UserParams userParams, string DateFrom, string DateTo, string Search);

        Task<PagedList<DtoBorrowedAndReturned>> ReturnBorrowedReports (UserParams userParams, string DateFrom, string DateTo, string Search);





        Task<PagedList<DtoCancelledReports>> CancelledReports(UserParams userParams, string DateFrom , string DateTo, string Search);
        Task<PagedList<DtoInventoryMovement>> InventoryMovementReports (UserParams userParams, string DateFrom , string DateTo , string PlusOne);

        Task<PagedList<DtoTransactReports>> TransactedMoveOrderReport(UserParams userParams, string DateFrom, string DateTo, string Search);

    }
}
