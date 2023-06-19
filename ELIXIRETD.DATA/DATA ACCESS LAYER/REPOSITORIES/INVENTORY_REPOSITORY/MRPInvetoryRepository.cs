﻿using ELIXIRETD.DATA.CORE.INTERFACES.INVENTORY_INTERFACE;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.IMPORT_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORY_DTO.MRP;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.INVENTORYDTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.MISCELLANEOUS_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;

namespace ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.INVENTORY_REPOSITORY
{
    public class MRPInvetoryRepository : IMRPInventory
    {
        private readonly StoreContext _context;

        public MRPInvetoryRepository(StoreContext context)
        {
            _context= context;
        }

        public async Task<IReadOnlyList<DtoGetAllAvailableInRawmaterialInventory>> GetAllAvailableInRawmaterialInventory()
        {
            return await _context.WarehouseReceived
              .GroupBy(x => new
            {
                  x.ItemCode,
                  x.ItemDescription,
                  x.LotSection,
                  x.Uom,
                  x.IsWarehouseReceived,

            }).Select(inventory => new DtoGetAllAvailableInRawmaterialInventory
            {
                ItemCode = inventory.Key.ItemCode,
                ItemDescription = inventory.Key.ItemDescription,
                LotCategory = inventory.Key.LotSection,
                Uom = inventory.Key.Uom,
                SOH = inventory.Sum(x => x.ActualGood),
                ReceiveIn = inventory.Sum(x => x.ActualGood),
                RejectOrder= inventory.Sum(x => x.TotalReject),
                IsWarehouseReceived = inventory.Key.IsWarehouseReceived,
                

            }).OrderBy(x => x.ItemCode)
            .Where(x => x.IsWarehouseReceived == true)
            .ToListAsync();

        }



        public async Task<IReadOnlyList<DtoMRP>> MRPInventory()
        {
            var EndDate = DateTime.Now;
            var StartDate = EndDate.AddDays(-30);

            var getPoSummary = _context.PoSummaries.Where(x => x.IsActive == true)
                                                   .GroupBy(x => new
                                                   {

                                                       x.ItemCode,

                                                   }).Select(x => new PoSummaryDto
                                                   {
                                                       ItemCode = x.Key.ItemCode,
                                                       UnitPrice = x.Sum(x => x.UnitPrice),
                                                       Ordered = x.Sum(x => x.Ordered),
                                                       TotalPrice = x.Average(x => x.UnitPrice)

                                                   });


            var getWarehouseIn = _context.WarehouseReceived.Where(x => x.IsActive == true)
                                                           .Where(x => x.TransactionType == "Receiving")
                                                           .OrderBy(x => x.ActualReceivingDate)
                                                           .GroupBy(x => new
                                                           {

                                                               x.ItemCode,
                                                           }).Select(x => new WarehouseInventory
                                                           {

                                                               ItemCode = x.Key.ItemCode,
                                                               ActualGood = x.Sum(x => x.ActualGood)
                                                           });

            var getMoveOrderOut = _context.MoveOrders.Where(x => x.IsActive == true)
                                                    .Where(x => x.IsPrepared == true)
                                                    .GroupBy(x => new
                                                    {
                                                        x.ItemCode,

                                                    }).Select(x => new MoveOrderInventory
                                                    {

                                                        ItemCode = x.Key.ItemCode,
                                                        QuantityOrdered = x.Sum(x => x.QuantityOrdered),

                                                    });

            var getReceiptIn = _context.WarehouseReceived.Where(x => x.IsActive == true)
                                                         .Where(x => x.TransactionType == "MiscellaneousReceipt")
                                                         .GroupBy(x => new
                                                         {

                                                             x.ItemCode,

                                                         }).Select(x => new DtoRecieptIn
                                                         {

                                                             ItemCode = x.Key.ItemCode,
                                                             Quantity = x.Sum(x => x.ActualGood),

                                                         });


            //var getReceitOut = _context.WarehouseReceived.Where(x => x.IsActive == true)
            //                                             .Where(x => x.TransactionType == "MiscellaneousReceipt")
            //                                             .GroupBy(x => new
            //                                             {

            //                                                 x.ItemCode,
            //                                             }).Select(x => new DtoReceiptOut
            //                                             {

            //                                                 ItemCode = x.Key.ItemCode,
            //                                                 QuantityReject = x.Sum(x => x.TotalReject),
            //                                             });


            var getIssueOut = _context.MiscellaneousIssueDetail.Where(x => x.IsActive == true)
                                                                .Where(x => x.IsTransact == true)
                                                                .GroupBy(x => new
                                                                {

                                                                    x.ItemCode,

                                                                }).Select(x => new DtoIssueInventory
                                                                {
                                                                    ItemCode = x.Key.ItemCode,
                                                                    Quantity = x.Sum(x => x.Quantity),

                                                                });

            var getBorrowedIssue = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                                .Where(x => x.IsApproved == true)
                                                       .GroupBy(x => new
                                                       {

                                                           x.ItemCode,

                                                       }).Select(x => new DtoBorrowedIssue
                                                       {

                                                           ItemCode = x.Key.ItemCode,
                                                           Quantity = x.Sum(x => x.Quantity),

                                                       });

            var getReturnedBorrow = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                                 .Where(x => x.IsReturned == true)
                                                                 .Where(x => x.IsApprovedReturned == true)
                                                                 .GroupBy(x => new
                                                                 {

                                                                     x.ItemCode,

                                                                 }).Select(x => new DtoBorrowedIssue
                                                                 {

                                                                     ItemCode = x.Key.ItemCode,
                                                                     ReturnQuantity = x.Sum(x => x.ReturnQuantity)

                                                                 });

            var getWarehouseStock = _context.WarehouseReceived.Where(x => x.IsActive == true)
                                                              .GroupBy(x => new
                                                              {
                                                                  x.ItemCode,

                                                              }).Select(x => new WarehouseInventory
                                                              {

                                                                  ItemCode = x.Key.ItemCode,
                                                                  ActualGood = x.Sum(x => x.ActualGood)

                                                              });


            var getOrderingReserve = _context.Orders.Where(x => x.IsActive == true)
                                                    .Where(x => x.IsPrepared == true)
                                                    .GroupBy(x => new
                                                    {
                                                        x.ItemCode,

                                                    }).Select(x => new OrderingInventory
                                                    {
                                                        ItemCode = x.Key.ItemCode,
                                                        QuantityOrdered = x.Sum(x => x.QuantityOrdered),

                                                    });


            var getSOH = (from warehouse in getWarehouseStock
                          join issue in getIssueOut
                          on warehouse.ItemCode equals issue.ItemCode
                          into leftJ1
                          from issue in leftJ1.DefaultIfEmpty()

                          join moveorder in getMoveOrderOut
                          on warehouse.ItemCode equals moveorder.ItemCode
                          into leftJ2
                          from moveorder in leftJ2.DefaultIfEmpty()

                          join borrowed in getBorrowedIssue
                          on warehouse.ItemCode equals borrowed.ItemCode
                          into leftJ3
                          from borrowed in leftJ3.DefaultIfEmpty()

                          join returned in getReturnedBorrow
                          on warehouse.ItemCode equals returned.ItemCode
                          into leftJ4
                          from returned in leftJ4.DefaultIfEmpty()

                          join receipt in getReceiptIn
                          on warehouse.ItemCode equals receipt.ItemCode
                          into leftJ5
                          from receipt in leftJ5.DefaultIfEmpty()

                          group new
                          {

                              warehouse,
                              moveorder,
                              issue,
                              borrowed,
                              returned,
                              receipt,

                          }
                          by new
                          {
                              warehouse.ItemCode

                          } into total

                          select new DtoSOH
                          {

                              ItemCode = total.Key.ItemCode,
                              SOH = total.Sum(x => x.warehouse.ActualGood != null ? x.warehouse.ActualGood : 0) +
                             total.Sum(x => x.returned.ReturnQuantity != null ? x.returned.ReturnQuantity : 0) -
                             total.Sum(x => x.issue.Quantity != null ? x.issue.Quantity : 0) -
                             total.Sum(x => x.borrowed.Quantity != null ? x.borrowed.Quantity : 0) -
                             total.Sum(x => x.moveorder.QuantityOrdered != null ? x.moveorder.QuantityOrdered : 0)

                          });

            var getReserve = (from warehouse in getWarehouseStock
                              join  ordering in getOrderingReserve
                              on warehouse.ItemCode equals ordering.ItemCode
                              into leftJ1
                              from ordering in leftJ1.DefaultIfEmpty()

                              group new
                              {

                                  warehouse,
                                  ordering,

                              } by new
                              {

                                  warehouse.ItemCode

                              } into total

                              select new ReserveInventory
                              {

                                  ItemCode = total.Key.ItemCode,
                                  Reserve = total.Sum(x => x.warehouse.ActualGood != null ? x.warehouse.ActualGood : 0) -
                                  ( total.Sum(x => x.ordering.QuantityOrdered != null ? x.ordering.QuantityOrdered : 0))

                              });


            var getSuggestedPo = (from posummary in getPoSummary
                                  join receive in getWarehouseIn
                                  on posummary.ItemCode equals receive.ItemCode
                                  into leftJ1
                                  from receive in leftJ1.DefaultIfEmpty()

                                  group new
                                  {
                                      posummary,
                                      receive,

                                  }
                                  by new
                                  {

                                      posummary.ItemCode,

                                  } into total
                                  select new DtoPoSummaryInventory
                                  {

                                      ItemCode = total.Key.ItemCode,
                                      Ordered = total.Sum(x => x.posummary.Ordered != null ? x.posummary.Ordered : 0) -
                                                total.Sum(x => x.receive.ActualGood != null ? x.receive.ActualGood : 0)

                                  });

            var getMoveOrderoutPerMonth = _context.MoveOrders.Where(x => x.PreparedDate >= StartDate && x.PreparedDate <= EndDate)
                                                              .Where(x => x.IsActive == true)
                                                              .Where(x => x.IsPrepared == true)
                                                              .GroupBy(x => new
                                                              {
                                                                  x.ItemCode,

                                                              }).Select(x => new MoveOrderInventory
                                                              {
                                                                  ItemCode = x.Key.ItemCode,
                                                                  QuantityOrdered = x.Sum(x => x.QuantityOrdered)

                                                              });

            var getBorrowedOutPerMonth = _context.BorrowedIssueDetails.Where(x => x.PreparedDate >= StartDate && x.PreparedDate <= EndDate)
                                                                      .Where(x => x.IsActive == true)
                                                                      .Where(x => x.IsApproved == true)
                                                                      .GroupBy(x => new
                                                                      {

                                                                          x.ItemCode,

                                                                      }).Select(x => new DtoBorrowedIssue
                                                                      {

                                                                          ItemCode = x.Key.ItemCode,
                                                                          Quantity = x.Sum(x => x.Quantity) - x.Sum(x => x.ReturnQuantity),

                                                                      });

            var getAvarageIssuance = (from warehouse in getWarehouseStock
                                      join moveorder in getMoveOrderoutPerMonth
                                      on warehouse.ItemCode equals moveorder.ItemCode
                                      into leftJ1
                                      from moveorder in leftJ1.DefaultIfEmpty()

                                      join borrowed in getBorrowedOutPerMonth
                                      on warehouse.ItemCode equals borrowed.ItemCode
                                      into leftJ2
                                      from borrowed in leftJ2.DefaultIfEmpty()

                                      group new
                                      {
                                          warehouse,
                                          borrowed,
                                          moveorder,
                                      }
                                      by new
                                      {
                                          warehouse.ItemCode

                                      } into total
                                      select new WarehouseInventory
                                      {

                                          ItemCode = total.Key.ItemCode,
                                          ActualGood = (total.Sum(x => x.borrowed.Quantity != null ? x.borrowed.Quantity : 0) + total.Sum(x => x.moveorder.QuantityOrdered != null ? x.moveorder.QuantityOrdered : 0)) / 30

                                      });

            var getReseverUsage = (from warehouse in getWarehouseStock
                                   join ordering in getOrderingReserve
                                   on warehouse.ItemCode equals ordering.ItemCode
                                   into left
                                   from ordering in left.DefaultIfEmpty()

                                   group new
                                   {
                                       warehouse,
                                       ordering

                                   }
                                   by new
                                   {
                                       warehouse.ItemCode,

                                   } into total
                                   select new ReserveInventory
                                   {
                                       ItemCode = total.Key.ItemCode,
                                       Reserve = total.Sum(x => x.ordering.QuantityOrdered == null ? 0 : x.ordering.QuantityOrdered)

                                   });


            var inventory = (from material in _context.Materials
                             join posummary in getPoSummary
                              on material.ItemCode equals posummary.ItemCode
                              into leftJ1
                             from posummary in leftJ1.DefaultIfEmpty()

                             join warehouse in getWarehouseIn
                             on material.ItemCode equals warehouse.ItemCode
                             into leftJ2
                             from warehouse in leftJ2.DefaultIfEmpty()

                             join moveorders in getMoveOrderOut
                             on material.ItemCode equals moveorders.ItemCode
                             into leftJ3
                             from moveorders in leftJ3.DefaultIfEmpty()

                             join receiptIn in getReceiptIn
                             on material.ItemCode equals receiptIn.ItemCode
                             into leftJ4
                             from receiptin in leftJ4.DefaultIfEmpty()

                             join issueout in getIssueOut
                             on material.ItemCode equals issueout.ItemCode
                             into leftJ6
                             from issueOut in leftJ6.DefaultIfEmpty()

                             join SOH in getSOH
                             on material.ItemCode equals SOH.ItemCode
                             into leftJ7
                             from SOH in leftJ7.DefaultIfEmpty()

                             join borrow in getBorrowedIssue
                             on material.ItemCode equals borrow.ItemCode
                             into leftJ8
                             from borrow in leftJ8.DefaultIfEmpty()

                             join returned in getReturnedBorrow
                             on material.ItemCode equals returned.ItemCode
                             into leftJ9
                             from returned in leftJ9.DefaultIfEmpty()

                             join reserve in getReserve
                             on material.ItemCode equals reserve.ItemCode
                             into leftJ10
                             from reserve in leftJ10.DefaultIfEmpty()

                             join sudggest in getSuggestedPo
                             on material.ItemCode equals sudggest.ItemCode
                             into leftJ11
                             from sudggest in leftJ11.DefaultIfEmpty()

                             join averageissuance in getAvarageIssuance
                             on material.ItemCode equals averageissuance.ItemCode
                             into leftJ12
                             from averageissuance in leftJ12.DefaultIfEmpty()

                             join usage in getReseverUsage
                             on material.ItemCode equals usage.ItemCode
                             into leftJ13
                             from usage in leftJ13.DefaultIfEmpty()

                             group new
                             {
                           
                                 posummary,
                                 warehouse,
                                 moveorders,
                                 receiptin,
                                 issueOut,
                                 SOH,
                                 borrow,
                                 returned,
                                 reserve,
                                 sudggest,
                                 averageissuance,
                                 usage,


                             }
                             by new
                             {

                                 material.ItemCode,
                                 material.ItemDescription,
                                 material.Uom.UomCode,
                                 material.SubCategory.ItemCategory.ItemCategoryName,
                                 material.BufferLevel,
                                 UnitPrice = posummary.UnitPrice != null ? posummary.UnitPrice : 0,
                                 sudggest = sudggest.Ordered != null ? sudggest.Ordered : 0,
                                 warehouseActualGood = warehouse.ActualGood != null ? warehouse.ActualGood : 0,
                                 receiptin = receiptin.Quantity != null ? receiptin.Quantity : 0,
                                 moveorders = moveorders.QuantityOrdered != null ? moveorders.QuantityOrdered : 0,
                                 issueOut = issueOut.Quantity != null ? issueOut.Quantity : 0,
                                 borrow = borrow.Quantity != null ? borrow.Quantity : 0,
                                 returned = returned.ReturnQuantity != null ? returned.ReturnQuantity : 0,
                                 TotalPrice = posummary.TotalPrice != null ? posummary.TotalPrice : 0,
                                 SOH = SOH.SOH != null ? SOH.SOH : 0,
                                 reserve = reserve.Reserve != null ? reserve.Reserve : 0,
                                 averageissuance = averageissuance.ActualGood != null ? averageissuance.ActualGood : 0,
                                 usage = usage.Reserve != null ? usage.Reserve : 0,
                                
                                 

                             }

                              into total
                             select new DtoMRP

                             {

                                 ItemCode = total.Key.ItemCode,
                                 ItemDescription = total.Key.ItemDescription,
                                 Uom = total.Key.UomCode,
                                 ItemCategory = total.Key.ItemCategoryName,
                                 BufferLevel = total.Key.BufferLevel,
                                 Price = total.Key.UnitPrice,
                                 ReceiveIn = total.Key.warehouseActualGood,
                                 MoveOrderOut = total.Key.moveorders,
                                 ReceiptIn = total.Key.receiptin,
                                 IssueOut = total.Key.issueOut,
                                 BorrowedDepartment = total.Key.borrow,
                                 ReturnedBorrowed = total.Key.returned,
                                 TotalPrice = decimal.Round(total.Key.TotalPrice, 2),
                                 SOH = total.Key.SOH,
                                 Reserve = total.Key.reserve + total.Key.returned - total.Key.issueOut - total.Key.borrow,
                                 SuggestedPo = total.Key.sudggest,
                                 AverageIssuance = decimal.Round(total.Key.averageissuance, 2),
                                 ReserveUsage = total.Key.usage,
                                 DaysLevel = decimal.Round(total.Key.reserve / (total.Key.averageissuance != 0 ? total.Key.averageissuance : 1), 2),
                                 BorrowedDifference = total.Key.borrow - total.Key.returned

        });

            return await inventory.ToListAsync();

        }

        public async Task<PagedList<DtoMRP>> GetallItemForInventoryPagination(UserParams userParams)
        {
            var EndDate = DateTime.Now;
            var StartDate = EndDate.AddDays(-30);

            var getPoSummary = _context.PoSummaries.Where(x => x.IsActive == true)
                                                   .GroupBy(x => new
                                                   {

                                                       x.ItemCode,

                                                   }).Select(x => new PoSummaryDto
                                                   {
                                                       ItemCode = x.Key.ItemCode,
                                                       UnitPrice = x.Sum(x => x.UnitPrice),
                                                       Ordered = x.Sum(x => x.Ordered),
                                                       TotalPrice = x.Average(x => x.UnitPrice)

                                                   });


            var getWarehouseIn = _context.WarehouseReceived.Where(x => x.IsActive == true)
                                                           .Where(x => x.TransactionType == "Receiving")
                                                           .OrderBy(x => x.ActualReceivingDate)
                                                           .GroupBy(x => new
                                                           {

                                                               x.ItemCode,
                                                           }).Select(x => new WarehouseInventory
                                                           {

                                                               ItemCode = x.Key.ItemCode,
                                                               ActualGood = x.Sum(x => x.ActualGood)
                                                           });

            var getMoveOrderOut = _context.MoveOrders.Where(x => x.IsActive == true)
                                                    .Where(x => x.IsPrepared == true)
                                                    .GroupBy(x => new
                                                    {
                                                        x.ItemCode,

                                                    }).Select(x => new MoveOrderInventory
                                                    {

                                                        ItemCode = x.Key.ItemCode,
                                                        QuantityOrdered = x.Sum(x => x.QuantityOrdered),

                                                    });

            var getReceiptIn = _context.WarehouseReceived.Where(x => x.IsActive == true)
                                                         .Where(x => x.TransactionType == "MiscellaneousReceipt")
                                                         .GroupBy(x => new
                                                         {

                                                             x.ItemCode,

                                                         }).Select(x => new DtoRecieptIn
                                                         {

                                                             ItemCode = x.Key.ItemCode,
                                                             Quantity = x.Sum(x => x.ActualGood),

                                                         });


            //var getReceitOut = _context.WarehouseReceived.Where(x => x.IsActive == true)
            //                                             .Where(x => x.TransactionType == "MiscellaneousReceipt")
            //                                             .GroupBy(x => new
            //                                             {

            //                                                 x.ItemCode,
            //                                             }).Select(x => new DtoReceiptOut
            //                                             {

            //                                                 ItemCode = x.Key.ItemCode,
            //                                                 QuantityReject = x.Sum(x => x.TotalReject),
            //                                             });

            var getIssueOut = _context.MiscellaneousIssueDetail.Where(x => x.IsActive == true)
                                                                .Where(x => x.IsTransact == true)
                                                                .GroupBy(x => new
                                                                {

                                                                    x.ItemCode,

                                                                }).Select(x => new DtoIssueInventory
                                                                {
                                                                    ItemCode = x.Key.ItemCode,
                                                                    Quantity = x.Sum(x => x.Quantity),

                                                                });

            var getBorrowedIssue = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                                .Where(x => x.IsApproved == true)
                                                       .GroupBy(x => new
                                                       {

                                                           x.ItemCode,

                                                       }).Select(x => new DtoBorrowedIssue
                                                       {

                                                           ItemCode = x.Key.ItemCode,
                                                           Quantity = x.Sum(x => x.Quantity),

                                                       });

            var getReturnedBorrow = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                                 .Where(x => x.IsReturned == true)
                                                                 .Where(x => x.IsApprovedReturned == true)
                                                                 .GroupBy(x => new
                                                                 {

                                                                     x.ItemCode,

                                                                 }).Select(x => new DtoBorrowedIssue
                                                                 {

                                                                     ItemCode = x.Key.ItemCode,
                                                                     ReturnQuantity = x.Sum(x => x.ReturnQuantity)

                                                                 });

            var getWarehouseStock = _context.WarehouseReceived.Where(x => x.IsActive == true)
                                                              .GroupBy(x => new
                                                              {
                                                                  x.ItemCode,

                                                              }).Select(x => new WarehouseInventory
                                                              {

                                                                  ItemCode = x.Key.ItemCode,
                                                                  ActualGood = x.Sum(x => x.ActualGood)

                                                              });


            var getOrderingReserve = _context.Orders.Where(x => x.IsActive == true)
                                                     .Where(x => x.IsPrepared == true)
                                                    .GroupBy(x => new
                                                    {
                                                        x.ItemCode,

                                                    }).Select(x => new OrderingInventory
                                                    {
                                                        ItemCode = x.Key.ItemCode,
                                                        QuantityOrdered = x.Sum(x => x.QuantityOrdered),

                                                    });



            var getSOH = (from warehouse in getWarehouseStock
                          join issue in getIssueOut
                          on warehouse.ItemCode equals issue.ItemCode
                          into leftJ1
                          from issue in leftJ1.DefaultIfEmpty()

                          join moveorder in getMoveOrderOut
                          on warehouse.ItemCode equals moveorder.ItemCode
                          into leftJ2
                          from moveorder in leftJ2.DefaultIfEmpty()

                          join borrowed in getBorrowedIssue
                          on warehouse.ItemCode equals borrowed.ItemCode
                          into leftJ3
                          from borrowed in leftJ3.DefaultIfEmpty()

                          join returned in getReturnedBorrow
                          on warehouse.ItemCode equals returned.ItemCode
                          into leftJ4
                          from returned in leftJ4.DefaultIfEmpty()

                          join receipt in getReceiptIn
                          on warehouse.ItemCode equals receipt.ItemCode
                          into leftJ5
                          from receipt in leftJ5.DefaultIfEmpty()

                          group new
                          {

                              warehouse,
                              moveorder,
                              issue,
                              borrowed,
                              returned,
                              receipt,


                          }
                          by new
                          {
                              warehouse.ItemCode

                          } into total

                          select new DtoSOH
                          {

                              ItemCode = total.Key.ItemCode,
                              SOH = total.Sum(x => x.warehouse.ActualGood != null ? x.warehouse.ActualGood : 0) +
                             total.Sum(x => x.returned.ReturnQuantity != null ? x.returned.ReturnQuantity : 0) -
                             total.Sum(x => x.issue.Quantity != null ? x.issue.Quantity : 0) -
                             total.Sum(x => x.borrowed.Quantity != null ? x.borrowed.Quantity : 0) -
                             total.Sum(x => x.moveorder.QuantityOrdered != null ? x.moveorder.QuantityOrdered : 0)

                          });

            var getReserve = (from warehouse in getWarehouseStock
                              join ordering in getOrderingReserve
                              on warehouse.ItemCode equals ordering.ItemCode
                              into leftJ1
                              from ordering in leftJ1.DefaultIfEmpty()
                            
                              group new
                              {

                                  warehouse,
                                  ordering,

                              } by new
                              {

                                  warehouse.ItemCode

                              } into total

                              select new ReserveInventory
                              {

                                  ItemCode = total.Key.ItemCode,
                                  Reserve = total.Sum(x => x.warehouse.ActualGood != null ? x.warehouse.ActualGood : 0) -
                                  total.Sum(x => x.ordering.QuantityOrdered != null ? x.ordering.QuantityOrdered : 0)

                              });


            var getSuggestedPo = (from posummary in getPoSummary
                                  join receive in getWarehouseIn
                                  on posummary.ItemCode equals receive.ItemCode
                                  into leftJ1
                                  from receive in leftJ1.DefaultIfEmpty()

                                  group new
                                  {
                                      posummary,
                                      receive,

                                  }
                                  by new
                                  {

                                      posummary.ItemCode,

                                  } into total
                                  select new DtoPoSummaryInventory
                                  {

                                      ItemCode = total.Key.ItemCode,
                                      Ordered = total.Sum(x => x.posummary.Ordered != null ? x.posummary.Ordered : 0) -
                                                total.Sum(x => x.receive.ActualGood != null ? x.receive.ActualGood : 0)

                                  });

            var getMoveOrderoutPerMonth = _context.MoveOrders.Where(x => x.PreparedDate >= StartDate && x.PreparedDate <= EndDate)
                                                              .Where(x => x.IsActive == true)
                                                              .Where(x => x.IsPrepared == true)
                                                              .GroupBy(x => new
                                                              {
                                                                  x.ItemCode,

                                                              }).Select(x => new MoveOrderInventory
                                                              {
                                                                  ItemCode = x.Key.ItemCode,
                                                                  QuantityOrdered = x.Sum(x => x.QuantityOrdered)

                                                              });

            var getBorrowedOutPerMonth = _context.BorrowedIssueDetails.Where(x => x.PreparedDate >= StartDate && x.PreparedDate <= EndDate)
                                                                      .Where(x => x.IsActive == true)
                                                                      .Where(x => x.IsApproved == true)
                                                                      .GroupBy(x => new
                                                                      {

                                                                          x.ItemCode,

                                                                      }).Select(x => new DtoBorrowedIssue
                                                                      {

                                                                          ItemCode = x.Key.ItemCode,
                                                                          Quantity = x.Sum(x => x.Quantity) - x.Sum(x => x.ReturnQuantity),

                                                                      });

            var getAvarageIssuance = (from warehouse in getWarehouseStock
                                      join moveorder in getMoveOrderoutPerMonth
                                      on warehouse.ItemCode equals moveorder.ItemCode
                                      into leftJ1
                                      from moveorder in leftJ1.DefaultIfEmpty()

                                      join borrowed in getBorrowedOutPerMonth
                                      on warehouse.ItemCode equals borrowed.ItemCode
                                      into leftJ2
                                      from borrowed in leftJ2.DefaultIfEmpty()

                                      group new
                                      {
                                          warehouse,
                                          borrowed,
                                          moveorder,
                                      }
                                      by new
                                      {
                                          warehouse.ItemCode

                                      } into total
                                      select new WarehouseInventory
                                      {

                                          ItemCode = total.Key.ItemCode,
                                          ActualGood = (total.Sum(x => x.borrowed.Quantity != null ? x.borrowed.Quantity : 0) + total.Sum(x => x.moveorder.QuantityOrdered != null ? x.moveorder.QuantityOrdered : 0)) / 30

                                      });

            var getReseverUsage = (from warehouse in getWarehouseStock
                                   join ordering in getOrderingReserve
                                   on warehouse.ItemCode equals ordering.ItemCode
                                   into leftJ1
                                   from ordering in leftJ1.DefaultIfEmpty()

                                   group new
                                   {
                                       warehouse,
                                       ordering

                                   }
                                   by new
                                   {
                                       warehouse.ItemCode,

                                   } into total
                                   select new ReserveInventory
                                   {
                                       ItemCode = total.Key.ItemCode,
                                       Reserve = total.Sum(x => x.ordering.QuantityOrdered == null ? 0 : x.ordering.QuantityOrdered)

                                   });


            var inventory = (from material in _context.Materials
                             join posummary in getPoSummary
                              on material.ItemCode equals posummary.ItemCode
                              into leftJ1
                             from posummary in leftJ1.DefaultIfEmpty()

                             join warehouse in getWarehouseIn
                             on material.ItemCode equals warehouse.ItemCode
                             into leftJ2
                             from warehouse in leftJ2.DefaultIfEmpty()

                             join moveorders in getMoveOrderOut
                             on material.ItemCode equals moveorders.ItemCode
                             into leftJ3
                             from moveorders in leftJ3.DefaultIfEmpty()

                             join receiptIn in getReceiptIn
                             on material.ItemCode equals receiptIn.ItemCode
                             into leftJ4
                             from receiptin in leftJ4.DefaultIfEmpty()

                             join issueout in getIssueOut
                             on material.ItemCode equals issueout.ItemCode
                             into leftJ6
                             from issueOut in leftJ6.DefaultIfEmpty()

                             join SOH in getSOH
                             on material.ItemCode equals SOH.ItemCode
                             into leftJ7
                             from SOH in leftJ7.DefaultIfEmpty()

                             join borrow in getBorrowedIssue
                             on material.ItemCode equals borrow.ItemCode
                             into leftJ8
                             from borrow in leftJ8.DefaultIfEmpty()

                             join returned in getReturnedBorrow
                             on material.ItemCode equals returned.ItemCode
                             into leftJ9
                             from returned in leftJ9.DefaultIfEmpty()

                             join reserve in getReserve
                             on material.ItemCode equals reserve.ItemCode
                             into leftJ10
                             from reserve in leftJ10.DefaultIfEmpty()

                             join sudggest in getSuggestedPo
                             on material.ItemCode equals sudggest.ItemCode
                             into leftJ11
                             from sudggest in leftJ11.DefaultIfEmpty()

                             join averageissuance in getAvarageIssuance
                             on material.ItemCode equals averageissuance.ItemCode
                             into leftJ12
                             from averageissuance in leftJ12.DefaultIfEmpty()

                             join usage in getReseverUsage
                             on material.ItemCode equals usage.ItemCode
                             into leftJ13
                             from usage in leftJ13.DefaultIfEmpty()

                             group new
                             {
                        
                                 posummary,
                                 warehouse,
                                 moveorders,
                                 receiptin,
                                 issueOut,
                                 SOH,
                                 borrow,
                                 returned,
                                 reserve,
                                 sudggest,
                                 averageissuance,
                                 usage,

                             }
                             by new
                             {

                                 material.ItemCode,
                                 material.ItemDescription,
                                 material.Uom.UomCode,
                                 material.SubCategory.ItemCategory.ItemCategoryName,
                                 material.BufferLevel,
                                 UnitPrice = posummary.UnitPrice != null ? posummary.UnitPrice : 0,
                                 sudggest = sudggest.Ordered != null ? sudggest.Ordered : 0,
                                 warehouseActualGood = warehouse.ActualGood != null ? warehouse.ActualGood : 0,
                                 receiptin = receiptin.Quantity != null ? receiptin.Quantity : 0,
                                 moveorders = moveorders.QuantityOrdered != null ? moveorders.QuantityOrdered : 0,
                                 issueOut = issueOut.Quantity != null ? issueOut.Quantity : 0,
                                 borrow = borrow.Quantity != null ? borrow.Quantity : 0,
                                 returned = returned.ReturnQuantity != null ? returned.ReturnQuantity : 0,
                                 TotalPrice = posummary.TotalPrice != null ? posummary.TotalPrice : 0,
                                 SOH = SOH.SOH != null ? SOH.SOH : 0,
                                 reserve = reserve.Reserve != null ? reserve.Reserve : 0,
                                 averageissuance = averageissuance.ActualGood != null ? averageissuance.ActualGood : 0,
                                 usage = usage.Reserve != null ? usage.Reserve : 0,


                             }

                              into total
                             select new DtoMRP

                             {
                                   ItemCode = total.Key.ItemCode,
                                 ItemDescription = total.Key.ItemDescription,
                                 Uom = total.Key.UomCode,
                                 ItemCategory = total.Key.ItemCategoryName,
                                 BufferLevel = total.Key.BufferLevel,
                                 Price = total.Key.UnitPrice,
                                 ReceiveIn = total.Key.warehouseActualGood,
                                 MoveOrderOut = total.Key.moveorders,
                                 ReceiptIn = total.Key.receiptin,
                                 IssueOut = total.Key.issueOut,
                                 BorrowedDepartment = total.Key.borrow,
                                 ReturnedBorrowed = total.Key.returned,
                                 TotalPrice = Math.Round(total.Key.TotalPrice, 2),
                                 SOH = total.Key.SOH,
                                 Reserve = total.Key.reserve + total.Key.returned - total.Key.issueOut - total.Key.borrow ,
                                 SuggestedPo = total.Key.sudggest,
                                 AverageIssuance = Math.Round(total.Key.averageissuance, 2),
                                 ReserveUsage = total.Key.usage,
                                 DaysLevel = Math.Round(total.Key.reserve / (total.Key.averageissuance != 0 ? total.Key.averageissuance : 1m)),
                                 BorrowedDifference = total.Key.borrow - total.Key.returned
                             });


            return await PagedList<DtoMRP>.CreateAsync(inventory, userParams.PageNumber, userParams.PageSize);

        }



        public async Task<PagedList<DtoMRP>> GetallItemForInventoryPaginationOrig(UserParams userParams, string search)
        {

            var EndDate = DateTime.Now;
            var StartDate = EndDate.AddDays(-30);

            var getPoSummary = _context.PoSummaries.Where(x => x.IsActive == true)
                                                   .GroupBy(x => new
                                                   {

                                                       x.ItemCode,

                                                   }).Select(x => new PoSummaryDto
                                                   {
                                                       ItemCode = x.Key.ItemCode,
                                                       UnitPrice = x.Sum(x => x.UnitPrice),
                                                       Ordered = x.Sum(x => x.Ordered),
                                                       TotalPrice = x.Average(x => x.UnitPrice)

                                                   });


            var getWarehouseIn = _context.WarehouseReceived.Where(x => x.IsActive == true)
                                                           .Where(x => x.TransactionType == "Receiving")
                                                           .OrderBy(x => x.ActualReceivingDate)
                                                           .GroupBy(x => new
                                                           {

                                                               x.ItemCode,
                                                           }).Select(x => new WarehouseInventory
                                                           {

                                                               ItemCode = x.Key.ItemCode,
                                                               ActualGood = x.Sum(x => x.ActualGood)
                                                           });

            var getMoveOrderOut = _context.MoveOrders.Where(x => x.IsActive == true)
                                                    .Where(x => x.IsPrepared == true)
                                                    .GroupBy(x => new
                                                    {
                                                        x.ItemCode,

                                                    }).Select(x => new MoveOrderInventory
                                                    {

                                                        ItemCode = x.Key.ItemCode,
                                                        QuantityOrdered = x.Sum(x => x.QuantityOrdered),

                                                    });

            var getReceiptIn = _context.WarehouseReceived.Where(x => x.IsActive == true)
                                                         .Where(x => x.TransactionType == "MiscellaneousReceipt")
                                                         .GroupBy(x => new
                                                         {

                                                             x.ItemCode,

                                                         }).Select(x => new DtoRecieptIn
                                                         {

                                                             ItemCode = x.Key.ItemCode,
                                                             Quantity = x.Sum(x => x.ActualGood),

                                                         });


            //var getReceitOut = _context.WarehouseReceived.Where(x => x.IsActive == true)
            //                                             .Where(x => x.TransactionType == "MiscellaneousReceipt")
            //                                             .GroupBy(x => new
            //                                             {

            //                                                 x.ItemCode,
            //                                             }).Select(x => new DtoReceiptOut
            //                                             {

            //                                                 ItemCode = x.Key.ItemCode,
            //                                                 QuantityReject = x.Sum(x => x.TotalReject),
            //                                             });

            var getIssueOut = _context.MiscellaneousIssueDetail.Where(x => x.IsActive == true)
                                                                .Where(x => x.IsTransact == true)
                                                                .GroupBy(x => new
                                                                {

                                                                    x.ItemCode,

                                                                }).Select(x => new DtoIssueInventory
                                                                {
                                                                    ItemCode = x.Key.ItemCode,
                                                                    Quantity = x.Sum(x => x.Quantity),

                                                                });

            var getBorrowedIssue = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                                .Where(x => x.IsApproved == true)
                                                       .GroupBy(x => new
                                                       {

                                                           x.ItemCode,

                                                       }).Select(x => new DtoBorrowedIssue
                                                       {

                                                           ItemCode = x.Key.ItemCode,
                                                           Quantity = x.Sum(x => x.Quantity),

                                                       });

            var getReturnedBorrow = _context.BorrowedIssueDetails.Where(x => x.IsActive == true)
                                                                 .Where(x => x.IsReturned == true)
                                                                 .Where(x => x.IsApprovedReturned == true)
                                                                 .GroupBy(x => new
                                                                 {

                                                                     x.ItemCode,

                                                                 }).Select(x => new DtoBorrowedIssue
                                                                 {

                                                                     ItemCode = x.Key.ItemCode,
                                                                     ReturnQuantity = x.Sum(x => x.ReturnQuantity)

                                                                 });

            var getWarehouseStock = _context.WarehouseReceived.Where(x => x.IsActive == true)
                                                              .GroupBy(x => new
                                                              {
                                                                  x.ItemCode,

                                                              }).Select(x => new WarehouseInventory
                                                              {

                                                                  ItemCode = x.Key.ItemCode,
                                                                  ActualGood = x.Sum(x => x.ActualGood)

                                                              });


            var getOrderingReserve = _context.Orders.Where(x => x.IsActive == true)
                                                      .Where(x => x.IsPrepared == true)
                                                    .GroupBy(x => new
                                                    {
                                                        x.ItemCode,

                                                    }).Select(x => new OrderingInventory
                                                    {
                                                        ItemCode = x.Key.ItemCode,
                                                        QuantityOrdered = x.Sum(x => x.QuantityOrdered),

                                                    });


            var getSOH = (from warehouse in getWarehouseStock
                          join issue in getIssueOut
                          on warehouse.ItemCode equals issue.ItemCode
                          into leftJ1
                          from issue in leftJ1.DefaultIfEmpty()

                          join moveorder in getMoveOrderOut
                          on warehouse.ItemCode equals moveorder.ItemCode
                          into leftJ2
                          from moveorder in leftJ2.DefaultIfEmpty()

                          join borrowed in getBorrowedIssue
                          on warehouse.ItemCode equals borrowed.ItemCode
                          into leftJ3
                          from borrowed in leftJ3.DefaultIfEmpty()

                          join returned in getReturnedBorrow
                          on warehouse.ItemCode equals returned.ItemCode
                          into leftJ4
                          from returned in leftJ4.DefaultIfEmpty()

                          join receipt in getReceiptIn
                          on warehouse.ItemCode equals receipt.ItemCode
                          into leftJ5
                          from receipt in leftJ5.DefaultIfEmpty()

                          group new
                          {

                              warehouse,
                              moveorder,
                              issue,
                              borrowed,
                              returned,
                              receipt,


                          }
                          by new
                          {
                              warehouse.ItemCode

                          } into total

                          select new DtoSOH
                          {

                              ItemCode = total.Key.ItemCode,
                              SOH = total.Sum(x => x.warehouse.ActualGood != null ? x.warehouse.ActualGood : 0) +
                             total.Sum(x => x.returned.ReturnQuantity != null ? x.returned.ReturnQuantity : 0) -
                             total.Sum(x => x.issue.Quantity != null ? x.issue.Quantity : 0) -
                             total.Sum(x => x.borrowed.Quantity != null ? x.borrowed.Quantity : 0) -
                             total.Sum(x => x.moveorder.QuantityOrdered != null ? x.moveorder.QuantityOrdered : 0)

                          });

            var getReserve = (from warehouse in getWarehouseStock
                              join ordering in getOrderingReserve
                              on warehouse.ItemCode equals ordering.ItemCode
                              into leftJ1
                              from ordering in leftJ1.DefaultIfEmpty()

                              group new
                              {

                                  warehouse,
                                  ordering,

                              } by new
                              {

                                  warehouse.ItemCode

                              } into total

                              select new ReserveInventory
                              {

                                  ItemCode = total.Key.ItemCode,
                                  Reserve = total.Sum(x => x.warehouse.ActualGood != null ? x.warehouse.ActualGood : 0) -
                                  (total.Sum(x => x.ordering.QuantityOrdered != null ? x.ordering.QuantityOrdered : 0))

                              });


            var getSuggestedPo = (from posummary in getPoSummary
                                  join receive in getWarehouseIn
                                  on posummary.ItemCode equals receive.ItemCode
                                  into leftJ1
                                  from receive in leftJ1.DefaultIfEmpty()

                                  group new
                                  {
                                      posummary,
                                      receive,

                                  }
                                  by new
                                  {

                                      posummary.ItemCode,

                                  } into total
                                  select new DtoPoSummaryInventory
                                  {

                                      ItemCode = total.Key.ItemCode,
                                      Ordered = total.Sum(x => x.posummary.Ordered != null ? x.posummary.Ordered : 0) -
                                                total.Sum(x => x.receive.ActualGood != null ? x.receive.ActualGood : 0)

                                  });

            var getMoveOrderoutPerMonth = _context.MoveOrders.Where(x => x.PreparedDate >= StartDate && x.PreparedDate <= EndDate)
                                                              .Where(x => x.IsActive == true)
                                                              .Where(x => x.IsPrepared == true)
                                                              .GroupBy(x => new
                                                              {
                                                                  x.ItemCode,

                                                              }).Select(x => new MoveOrderInventory
                                                              {
                                                                  ItemCode = x.Key.ItemCode,
                                                                  QuantityOrdered = x.Sum(x => x.QuantityOrdered)

                                                              });

            var getBorrowedOutPerMonth = _context.BorrowedIssueDetails.Where(x => x.PreparedDate >= StartDate && x.PreparedDate <= EndDate)
                                                                      .Where(x => x.IsActive == true)
                                                                      .Where(x => x.IsApprovedReturned == true)
                                                                      .GroupBy(x => new
                                                                      {

                                                                          x.ItemCode,

                                                                      }).Select(x => new DtoBorrowedIssue
                                                                      {

                                                                          ItemCode = x.Key.ItemCode,
                                                                          Quantity = x.Sum(x => x.Quantity) - x.Sum(x => x.ReturnQuantity),

                                                                      });

            var getAvarageIssuance = (from warehouse in getWarehouseStock
                                      join moveorder in getMoveOrderoutPerMonth
                                      on warehouse.ItemCode equals moveorder.ItemCode
                                      into leftJ1
                                      from moveorder in leftJ1.DefaultIfEmpty()

                                      join borrowed in getBorrowedOutPerMonth
                                      on warehouse.ItemCode equals borrowed.ItemCode
                                      into leftJ2
                                      from borrowed in leftJ2.DefaultIfEmpty()

                                      group new
                                      {
                                          warehouse,
                                          borrowed,
                                          moveorder,
                                      }
                                      by new
                                      {
                                          warehouse.ItemCode

                                      } into total
                                      select new WarehouseInventory
                                      {

                                          ItemCode = total.Key.ItemCode,
                                          ActualGood = (total.Sum(x => x.borrowed.Quantity != null ? x.borrowed.Quantity : 0) + total.Sum(x => x.moveorder.QuantityOrdered != null ? x.moveorder.QuantityOrdered : 0)) / 30

                                      });

            var getReseverUsage = (from warehouse in getWarehouseStock
                                   join ordering in getOrderingReserve
                                   on warehouse.ItemCode equals ordering.ItemCode
                                   into leftJ1
                                   from ordering in leftJ1.DefaultIfEmpty()

                                   group new
                                   {
                                       warehouse,
                                       ordering

                                   }
                                   by new
                                   {
                                       warehouse.ItemCode,

                                   } into total
                                   select new ReserveInventory
                                   {
                                       ItemCode = total.Key.ItemCode,
                                       Reserve = total.Sum(x => x.ordering.QuantityOrdered == null ? 0 : x.ordering.QuantityOrdered)

                                   });


            var inventory = (from material in _context.Materials
                             join posummary in getPoSummary
                              on material.ItemCode equals posummary.ItemCode
                              into leftJ1
                             from posummary in leftJ1.DefaultIfEmpty()

                             join warehouse in getWarehouseIn
                             on material.ItemCode equals warehouse.ItemCode
                             into leftJ2
                             from warehouse in leftJ2.DefaultIfEmpty()

                             join moveorders in getMoveOrderOut
                             on material.ItemCode equals moveorders.ItemCode
                             into leftJ3
                             from moveorders in leftJ3.DefaultIfEmpty()

                             join receiptIn in getReceiptIn
                             on material.ItemCode equals receiptIn.ItemCode
                             into leftJ4
                             from receiptin in leftJ4.DefaultIfEmpty()

                             join issueout in getIssueOut
                             on material.ItemCode equals issueout.ItemCode
                             into leftJ6
                             from issueOut in leftJ6.DefaultIfEmpty()

                             join SOH in getSOH
                             on material.ItemCode equals SOH.ItemCode
                             into leftJ7
                             from SOH in leftJ7.DefaultIfEmpty()

                             join borrow in getBorrowedIssue
                             on material.ItemCode equals borrow.ItemCode
                             into leftJ8
                             from borrow in leftJ8.DefaultIfEmpty()

                             join returned in getReturnedBorrow
                             on material.ItemCode equals returned.ItemCode
                             into leftJ9
                             from returned in leftJ9.DefaultIfEmpty()

                             join reserve in getReserve
                             on material.ItemCode equals reserve.ItemCode
                             into leftJ10
                             from reserve in leftJ10.DefaultIfEmpty()

                             join sudggest in getSuggestedPo
                             on material.ItemCode equals sudggest.ItemCode
                             into leftJ11
                             from sudggest in leftJ11.DefaultIfEmpty()

                             join averageissuance in getAvarageIssuance
                             on material.ItemCode equals averageissuance.ItemCode
                             into leftJ12
                             from averageissuance in leftJ12.DefaultIfEmpty()

                             join usage in getReseverUsage
                             on material.ItemCode equals usage.ItemCode
                             into leftJ13
                             from usage in leftJ13.DefaultIfEmpty()

                             group new
                             {

                                 posummary,
                                 warehouse,
                                 moveorders,
                                 receiptin,
                                 issueOut,
                                 SOH,
                                 borrow,
                                 returned,
                                 reserve,
                                 sudggest,
                                 averageissuance,
                                 usage,


                             }
                             by new
                             {

                                 material.ItemCode,
                                 material.ItemDescription,
                                 material.Uom.UomCode,
                                 material.SubCategory.ItemCategory.ItemCategoryName,
                                 material.BufferLevel,
                                 UnitPrice = posummary.UnitPrice != null ? posummary.UnitPrice : 0,
                                 sudggest = sudggest.Ordered != null ? sudggest.Ordered : 0,
                                 warehouseActualGood = warehouse.ActualGood != null ? warehouse.ActualGood : 0,
                                 receiptin = receiptin.Quantity != null ? receiptin.Quantity : 0,
                                 moveorders = moveorders.QuantityOrdered != null ? moveorders.QuantityOrdered : 0,
                                 issueOut = issueOut.Quantity != null ? issueOut.Quantity : 0,
                                 borrow = borrow.Quantity != null ? borrow.Quantity : 0,
                                 returned = returned.ReturnQuantity != null ? returned.ReturnQuantity : 0,
                                 TotalPrice = posummary.TotalPrice != null ? posummary.TotalPrice : 0,
                                 SOH = SOH.SOH != null ? SOH.SOH : 0,
                                 reserve = reserve.Reserve != null ? reserve.Reserve : 0,
                                 averageissuance = averageissuance.ActualGood != null ? averageissuance.ActualGood : 0,
                                 usage = usage.Reserve != null ? usage.Reserve : 0,

                             }

                              into total
                             select new DtoMRP

                             {
                                 ItemCode = total.Key.ItemCode,
                                 ItemDescription = total.Key.ItemDescription,
                                 Uom = total.Key.UomCode,
                                 ItemCategory = total.Key.ItemCategoryName,
                                 BufferLevel = total.Key.BufferLevel,
                                 Price = total.Key.UnitPrice,
                                 ReceiveIn = total.Key.warehouseActualGood,
                                 MoveOrderOut = total.Key.moveorders,
                                 ReceiptIn = total.Key.receiptin,
                                 IssueOut = total.Key.issueOut,
                                 BorrowedDepartment = total.Key.borrow,
                                 ReturnedBorrowed = total.Key.returned,
                                 TotalPrice = Math.Round(total.Key.TotalPrice, 2),
                                 SOH = total.Key.SOH,
                                 Reserve = total.Key.reserve + total.Key.returned - total.Key.issueOut - total.Key.borrow ,
                                 SuggestedPo = total.Key.sudggest,
                                 AverageIssuance = Math.Round(total.Key.averageissuance, 2),
                                 ReserveUsage = total.Key.usage,
                                 DaysLevel = Math.Round(total.Key.reserve / (total.Key.averageissuance != 0 ? total.Key.averageissuance : 1m)),
                                 BorrowedDifference = total.Key.borrow - total.Key.returned


                             }).Where(x => x.ItemDescription.ToLower()
                               .Contains(search.Trim().ToLower()));


            return await PagedList<DtoMRP>.CreateAsync(inventory, userParams.PageNumber, userParams.PageSize);
        }

            

        

    }
}
