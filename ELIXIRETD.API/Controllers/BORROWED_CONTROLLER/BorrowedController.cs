﻿using ELIXIRETD.DATA.CORE.ICONFIGURATION;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.DTOs.BORROWED_DTO;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.MODELS.BORROWED_MODEL;
using ELIXIRETD.DATA.DATA_ACCESS_LAYER.REPOSITORIES.BORROWED_REPOSITORY.Excemption;
using ELIXIRETD.DATA.SERVICES;

namespace ELIXIRETD.API.Controllers.BORROWED_CONTROLLER
{

    public class BorrowedController : BaseApiController
    {

        private readonly IUnitOfWork _unitofwork;

        public BorrowedController(IUnitOfWork unitOfWork)
        {
            _unitofwork = unitOfWork;
        }


        [HttpGet]
        [Route("GetAllBorrowedIssueWithPagination")]
        public async Task<ActionResult<IEnumerable<GetAllBorrowedReceiptWithPaginationDto>>> GetAllBorrowedIssueWithPagination([FromQuery] UserParams userParams, [FromQuery] /*bool status*/ bool status, [FromQuery] int empid)
        {
            var issue = await _unitofwork.Borrowed.GetAllBorrowedReceiptWithPagination(userParams, status, empid);

            Response.AddPaginationHeader(issue.CurrentPage, issue.PageSize, issue.TotalCount, issue.TotalPages, issue.HasNextPage, issue.HasPreviousPage);

            var issueResult = new
            {
                issue,
                issue.CurrentPage,
                issue.PageSize,
                issue.TotalCount,
                issue.TotalPages,
                issue.HasNextPage,
                issue.HasPreviousPage
            };

            return Ok(issueResult);

        }


        [HttpGet]
        [Route("GetAllBorrowedIssueWithPaginationOrig")]
        public async Task<ActionResult<IEnumerable<GetAllBorrowedReceiptWithPaginationDto>>> GetAllBorrowedIssueWithPaginationOrig([FromQuery] UserParams userParams, [FromQuery] string search, [FromQuery] bool status, [FromQuery] int empid)
        {
            if (search == null)

                return await GetAllBorrowedIssueWithPagination(userParams, status, empid);

            var issue = await _unitofwork.Borrowed.GetAllBorrowedIssuetWithPaginationOrig(userParams, search, status, empid);

            Response.AddPaginationHeader(issue.CurrentPage, issue.PageSize, issue.TotalCount, issue.TotalPages, issue.HasNextPage, issue.HasPreviousPage);

            var issueResult = new
            {
                issue,
                issue.CurrentPage,
                issue.PageSize,
                issue.TotalCount,
                issue.TotalPages,
                issue.HasNextPage,
                issue.HasPreviousPage
            };

            return Ok(issueResult);

        }


        [HttpPost]
        [Route("AddNewBorrowedIssues")]
        public async Task<IActionResult> AddNewBorrowedIssues([FromBody] BorrowedIssue borrowed)
        {
            borrowed.IsActive = true;
            borrowed.PreparedDate= DateTime.Now;
            borrowed.IsTransact = true;

            borrowed.IsApproved = false; // new Borrowed

            borrowed.StatusApproved = "For Borrowed Approval";

            await _unitofwork.Borrowed.AddBorrowedIssue(borrowed);
            await _unitofwork.CompleteAsync();

            return Ok(borrowed);

        }

        [HttpPost]
        [Route("AddNewBorrowedIssueDetails")]
        public async Task<IActionResult> AddNewBorrowedIssueDetails ([FromBody] BorrowedIssueDetails borrowed)
        {
            borrowed.IsActive= true;
            borrowed.PreparedDate= DateTime.Now;
            borrowed.BorrowedDate= DateTime.Now;

            borrowed.IsApproved = false; // new Borrowed

            await _unitofwork.Borrowed.AddBorrowedIssueDetails(borrowed);
            await _unitofwork.CompleteAsync();

            return Ok("Successfully add new borrowed issue!");
        }



        [HttpGet]
        [Route("GetAvailableStocksForBorrowedIssueNoParameters")]
        public async Task<IActionResult> GetAvailableStocksForBorrowedIssueNoParameters()
        {

            var borrow = await _unitofwork.Borrowed.GetAvailableStocksForBorrowedIssueNoParameters();

            return Ok(borrow);
        }


        [HttpGet]
        [Route("GetAllAvailableStocksForBorrowedIsssue")]
        public async Task<IActionResult> GetAllAvailableStocksForBorrowedIsssue([FromQuery] string itemcode)
        {

            var borrow = await _unitofwork.Borrowed.GetAvailableStocksForBorrowedIssue(itemcode);

            return Ok (borrow);
        }


        [HttpPut]
        [Route("CancelItemForTransactBorrow")]
        public async Task<IActionResult> CancelItemForTransactBorrow([FromBody] BorrowedIssueDetails[] borrowed)
        {

            foreach(BorrowedIssueDetails items in borrowed)
            {

                await _unitofwork.Borrowed.Cancelborrowedfortransact(items);
                await _unitofwork.CompleteAsync();

            }
          

            return new JsonResult("Successfully cancelled transaction!");
        }



        [HttpPut]
        [Route("UpdateBorrowedIssuePKey")]
        public async Task<IActionResult> UpdateBorrowedIssuePKey([FromBody] BorrowedIssueDetails[] borrowed)
        {
            foreach(BorrowedIssueDetails items in borrowed)
            {
                items.IsActive= true;
                items.PreparedDate = DateTime.Now;
                items.IsTransact = true;

                await _unitofwork.Borrowed.UpdateIssuePKey(items);
            }

            await _unitofwork.CompleteAsync();

            return Ok(borrowed);
        }



        [HttpGet]
        [Route("GetAllDetailsInBorrowedIssue")]
        public async Task<IActionResult> GetAllDetailsInBorrowedIssue([FromQuery] int id)
        {

            var borrow = await _unitofwork.Borrowed.GetAllDetailsInBorrowedIssue(id);

            return Ok(borrow);
        }

        [HttpGet]
        [Route("GetAllActiveBorrowedIssueTransaction")]
        public async Task<IActionResult> GetAllActiveBorrowedIssueTransaction([FromQuery] int empid)
        {

            var issue = await _unitofwork.Borrowed.GetAllAvailableIssue(empid);

            return Ok(issue);

        }

        [HttpPut]
        [Route("CancelItemCodeInBorrowedIssue")]
        public async Task<IActionResult> CancelItemCodeInBorrowedIssue([FromBody] BorrowedIssueDetails[] borrowed)
        {

            foreach(BorrowedIssueDetails items in borrowed)

            {
                await _unitofwork.Borrowed.CancelIssuePerItemCode(items);
                await _unitofwork.CompleteAsync();
            }
              
            return new JsonResult("Successfully cancelled transaction!");
        }


        [HttpPut]
        [Route("EditReturnedQuantity")]
        public async Task<IActionResult> EditQuantityReturned(BorrowedIssueDetails borrowed)
        {

            var edit = await _unitofwork.Borrowed.EditReturnQuantity(borrowed);



            if (edit == false)
                return BadRequest("Edit failed, please check your input in returned quantity!");


            await _unitofwork.CompleteAsync();

            return Ok("Successfully edit returned quantity!");

        }

        [HttpPut]
        [Route("SaveReturnedQuantity")]
        public async Task<IActionResult> SaveReturnedQuantity([FromBody] BorrowedIssue[] borrowed)
        {

            foreach(BorrowedIssue items in borrowed)
            {
                await _unitofwork.Borrowed.SaveReturnedQuantity(items);
                await _unitofwork.CompleteAsync();

            }

            return Ok(borrowed);
    
        }


        [HttpGet]
        [Route("GetAllReturnedItem")]
        public async Task<ActionResult<IEnumerable<DtoGetAllReturnedItem>>> GetAllReturnedItem([FromQuery] UserParams userParams, [FromQuery]bool status,[FromQuery] int empid)
        {
            var issue = await _unitofwork.Borrowed.GetAllReturnedItem(userParams, status, empid);

            Response.AddPaginationHeader(issue.CurrentPage, issue.PageSize, issue.TotalCount, issue.TotalPages, issue.HasNextPage, issue.HasPreviousPage);

            var issueResult = new
            {
                issue,
                issue.CurrentPage,
                issue.PageSize,
                issue.TotalCount,
                issue.TotalPages,
                issue.HasNextPage,
                issue.HasPreviousPage
            };

            return Ok(issueResult);

        }


        [HttpGet]
        [Route("GetAllReturnedItemOrig")]
        public async Task<ActionResult<IEnumerable<DtoGetAllReturnedItem>>> GetAllReturnedItemOrig([FromQuery] UserParams userParams, [FromQuery] string search, bool status, int empid)
        {
            if (search == null)

                return await GetAllReturnedItem(userParams, status , empid);

            var issue = await _unitofwork.Borrowed.GetAllReturnedItemOrig(userParams, search , status , empid);

            Response.AddPaginationHeader(issue.CurrentPage, issue.PageSize, issue.TotalCount, issue.TotalPages, issue.HasNextPage, issue.HasPreviousPage);

            var issueResult = new
            {
                issue,
                issue.CurrentPage,
                issue.PageSize,
                issue.TotalCount,
                issue.TotalPages,
                issue.HasNextPage,
                issue.HasPreviousPage
            };

            return Ok(issueResult);

        }


        [HttpGet]
        [Route("ViewBorrowedReturnDetails")]
        public async Task<IActionResult> ViewBorrowedReturnDetails([FromQuery] int id)
        {

            var borrow = await _unitofwork.Borrowed.ViewBorrewedReturnedDetails(id);

            return Ok(borrow);
        }


        // New Update Borrowed


        [HttpGet]
        [Route("GetAllForApprovalBorrowedWithPagination")]
        public async Task<ActionResult<IEnumerable<GetAllForApprovalBorrowedPaginationDTO>>> GetAllForApprovalBorrowedWithPagination([FromQuery] UserParams userParams, [FromQuery] bool status)
        {
            var issue = await _unitofwork.Borrowed.GetAllForApprovalBorrowedWithPagination(userParams, status);

            Response.AddPaginationHeader(issue.CurrentPage, issue.PageSize, issue.TotalCount, issue.TotalPages, issue.HasNextPage, issue.HasPreviousPage);

            var issueResult = new
            {
                issue,
                issue.CurrentPage,
                issue.PageSize,
                issue.TotalCount,
                issue.TotalPages,
                issue.HasNextPage,
                issue.HasPreviousPage
            };

            return Ok(issueResult);

        }


        [HttpGet]
        [Route("GetAllForApprovalBorrowedWithPaginationOrig")]
        public async Task<ActionResult<IEnumerable<GetAllForApprovalBorrowedPaginationDTO>>> GetAllForApprovalBorrowedWithPaginationOrig([FromQuery] UserParams userParams, [FromQuery] string search, [FromQuery] bool status)
        {
            if (search == null)

                return await GetAllForApprovalBorrowedWithPagination(userParams, status);

            var issue = await _unitofwork.Borrowed.GetAllForApprovalBorrowedWithPaginationOrig(userParams, search, status);

            Response.AddPaginationHeader(issue.CurrentPage, issue.PageSize, issue.TotalCount, issue.TotalPages, issue.HasNextPage, issue.HasPreviousPage);

            var issueResult = new
            {
                issue,
                issue.CurrentPage,
                issue.PageSize,
                issue.TotalCount,
                issue.TotalPages,
                issue.HasNextPage,
                issue.HasPreviousPage
            };

            return Ok(issueResult);

        }

        [HttpGet]
        [Route("GetAllForApprovalDetailsInBorrowed")]
        public async Task<IActionResult> GetAllForApprovalDetailsInBorrowed([FromQuery] int id)
        {

            var borrow = await _unitofwork.Borrowed.GetAllForApprovalDetailsInBorrowed(id);

            return Ok(borrow);
        }


        [HttpPut]
        [Route("ApprovedForBorrowed")]
        public async Task<IActionResult> ApprovedForBorrowed([FromBody] BorrowedIssue[] borrowed)
        {

            foreach (BorrowedIssue items in borrowed)
            {
                await _unitofwork.Borrowed.ApprovedForBorrowed(items);
                await _unitofwork.CompleteAsync();

            }

            return Ok(borrowed);

        }


        [HttpPut]
        [Route("RejectForBorrowed")]
        public async Task<IActionResult> RejectForBorrowed([FromBody] BorrowedIssue[] borrowed)
        {

            foreach (BorrowedIssue items in borrowed)
            {
                await _unitofwork.Borrowed.RejectForBorrowed(items);
                await _unitofwork.CompleteAsync();

            }

            return Ok(borrowed);

        }

        [HttpGet]
        [Route("GetAllRejectBorrowedWithPaginationCustomer")]
        public async Task<ActionResult<IEnumerable<GetRejectBorrowedPagination>>> GetAllRejectBorrowedWithPaginationCustomer([FromQuery] UserParams userParams,[FromQuery] int empid)
        {
            var issue = await _unitofwork.Borrowed.GetAllRejectBorrowedWithPaginationCustomer(userParams, empid);

            Response.AddPaginationHeader(issue.CurrentPage, issue.PageSize, issue.TotalCount, issue.TotalPages, issue.HasNextPage, issue.HasPreviousPage);

            var issueResult = new
            {
                issue,
                issue.CurrentPage,
                issue.PageSize,
                issue.TotalCount,
                issue.TotalPages,
                issue.HasNextPage,
                issue.HasPreviousPage
            };

            return Ok(issueResult);

        }


        [HttpGet]
        [Route("GetAllRejectBorrowedWithPaginationCustomerOrig")]
        public async Task<ActionResult<IEnumerable<GetRejectBorrowedPagination>>> GetAllRejectBorrowedWithPaginationCustomerOrig([FromQuery] UserParams userParams, [FromQuery] string search, [FromQuery] int empid)
        {
            if (search == null)

                return await GetAllRejectBorrowedWithPaginationCustomer(userParams, empid);

            var issue = await _unitofwork.Borrowed.GetAllRejectBorrowedWithPaginationCustomerOrig(userParams, search, empid);

            Response.AddPaginationHeader(issue.CurrentPage, issue.PageSize, issue.TotalCount, issue.TotalPages, issue.HasNextPage, issue.HasPreviousPage);

            var issueResult = new
            {
                issue,
                issue.CurrentPage,
                issue.PageSize,
                issue.TotalCount,
                issue.TotalPages,
                issue.HasNextPage,
                issue.HasPreviousPage
            };

            return Ok(issueResult);

        }





        [HttpGet]
        [Route("GetAllRejectBorrowedWithPagination")]
        public async Task<ActionResult<IEnumerable<GetRejectBorrowedPagination>>> GetAllRejectBorrowedWithPagination([FromQuery] UserParams userParams)
        {
            var issue = await _unitofwork.Borrowed.GetAllRejectBorrowedWithPagination(userParams);

            Response.AddPaginationHeader(issue.CurrentPage, issue.PageSize, issue.TotalCount, issue.TotalPages, issue.HasNextPage, issue.HasPreviousPage);

            var issueResult = new
            {
                issue,
                issue.CurrentPage,
                issue.PageSize,
                issue.TotalCount,
                issue.TotalPages,
                issue.HasNextPage,
                issue.HasPreviousPage
            };

            return Ok(issueResult);

        }


        [HttpGet]
        [Route("GetAllRejectBorrowedWithPaginationOrig")]
        public async Task<ActionResult<IEnumerable<GetRejectBorrowedPagination>>> GetAllRejectBorrowedWithPaginationOrig([FromQuery] UserParams userParams, [FromQuery] string search)
        {
            if (search == null)

                return await GetAllRejectBorrowedWithPagination(userParams);

            var issue = await _unitofwork.Borrowed.GetAllRejectBorrowedWithPaginationOrig(userParams, search);

            Response.AddPaginationHeader(issue.CurrentPage, issue.PageSize, issue.TotalCount, issue.TotalPages, issue.HasNextPage, issue.HasPreviousPage);

            var issueResult = new
            {
                issue,
                issue.CurrentPage,
                issue.PageSize,
                issue.TotalCount,
                issue.TotalPages,
                issue.HasNextPage,
                issue.HasPreviousPage
            };

            return Ok(issueResult);

        }



        [HttpGet]
        [Route("GetAllForApproveReturnedItem")]
        public async Task<ActionResult<IEnumerable<DtoGetAllReturnedItem>>> GetAllForApproveReturnedItem([FromQuery] UserParams userParams,[FromQuery] bool status)
        {
            var issue = await _unitofwork.Borrowed.GetAllForApproveReturnedItem(userParams , status);

            Response.AddPaginationHeader(issue.CurrentPage, issue.PageSize, issue.TotalCount, issue.TotalPages, issue.HasNextPage, issue.HasPreviousPage);

            var issueResult = new
            {
                issue,
                issue.CurrentPage,
                issue.PageSize,
                issue.TotalCount,
                issue.TotalPages,
                issue.HasNextPage,
                issue.HasPreviousPage
            };

            return Ok(issueResult);

        }



        [HttpGet]
        [Route("GetAllForApproveReturnedItemOrig")]
        public async Task<ActionResult<IEnumerable<DtoGetAllReturnedItem>>> GetAllForApproveReturnedItemOrig([FromQuery] UserParams userParams, [FromQuery] string search,[FromQuery] bool status)
        {
            if (search == null)

                return await GetAllForApproveReturnedItem(userParams, status);

            var issue = await _unitofwork.Borrowed.GetAllForApproveReturnedItemOrig(userParams, search, status);

            Response.AddPaginationHeader(issue.CurrentPage, issue.PageSize, issue.TotalCount, issue.TotalPages, issue.HasNextPage, issue.HasPreviousPage);

            var issueResult = new
            {
                issue,
                issue.CurrentPage,
                issue.PageSize,
                issue.TotalCount,
                issue.TotalPages,
                issue.HasNextPage,
                issue.HasPreviousPage
            };

            return Ok(issueResult);

        }


        [HttpPut]
        [Route("ApproveForReturned")]
        public async Task<IActionResult> ApproveForReturned([FromBody] BorrowedIssue[] borrowed)
        {

            foreach (BorrowedIssue items in borrowed)
            {
                await _unitofwork.Borrowed.ApproveForReturned(items);
                await _unitofwork.CompleteAsync();

            }

            return Ok(borrowed);

        }


        [HttpPut]
        [Route("CancelForReturned")]
        public async Task<IActionResult> CancelForReturned([FromBody] BorrowedIssue[] borrowed)
        {

            foreach (BorrowedIssue items in borrowed)
            {
                await _unitofwork.Borrowed.CancelForReturned(items);
                await _unitofwork.CompleteAsync();

            }

            return Ok(borrowed);

        }


        [HttpGet]
        [Route("GetAllDetailsBorrowedTransaction")]
        public async Task<ActionResult<IEnumerable<GetAllDetailsBorrowedTransactionDto>>> GetAllDetailsBorrowedTransaction([FromQuery] UserParams userParams)
        {
            var issue = await _unitofwork.Borrowed.GetAllDetailsBorrowedTransaction(userParams);

            Response.AddPaginationHeader(issue.CurrentPage, issue.PageSize, issue.TotalCount, issue.TotalPages, issue.HasNextPage, issue.HasPreviousPage);

            var issueResult = new
            {
                issue,
                issue.CurrentPage,
                issue.PageSize,
                issue.TotalCount,
                issue.TotalPages,
                issue.HasNextPage,
                issue.HasPreviousPage
            };

            return Ok(issueResult);

        }



        [HttpGet]
        [Route("GetAllDetailsBorrowedTransactionOrig")]
        public async Task<ActionResult<IEnumerable<GetAllDetailsBorrowedTransactionDto>>> GetAllDetailsBorrowedTransactionOrig([FromQuery] UserParams userParams, [FromQuery] string search)
        {
            if (search == null)

                return await GetAllDetailsBorrowedTransaction(userParams);

            var issue = await _unitofwork.Borrowed.GetAllDetailsBorrowedTransactionOrig(userParams, search);

            Response.AddPaginationHeader(issue.CurrentPage, issue.PageSize, issue.TotalCount, issue.TotalPages, issue.HasNextPage, issue.HasPreviousPage);

            var issueResult = new
            {
                issue,
                issue.CurrentPage,
                issue.PageSize,
                issue.TotalCount,
                issue.TotalPages,
                issue.HasNextPage,
                issue.HasPreviousPage
            };

            return Ok(issueResult);

        }

        
        // Update Borrowed


        [HttpPost]
        [Route("AddPendingBorrowedItem")]
        public async Task<IActionResult> AddPendingBorrowedItem([FromBody] BorrowedIssueDetails borrowed)
        {
            borrowed.IsActive = true;
            borrowed.PreparedDate = DateTime.Now;
            borrowed.BorrowedDate = DateTime.Now;

            borrowed.IsTransact = true;

            borrowed.IsApproved = false; // new Borrowed

            await _unitofwork.Borrowed.AddPendingBorrowedItem(borrowed);
            await _unitofwork.CompleteAsync();

            return Ok("Successfully add new borrowed issue!");

        }


        [HttpPut]
        [Route("CloseSaveBorrowed")]
        public async Task<IActionResult> CloseSaveBorrowed([FromBody] BorrowedIssueDetails[] borrowed)
        {
          
            foreach(BorrowedIssueDetails items in borrowed)
            {
                await _unitofwork.Borrowed.CloseSaveBorrowed(items);
                await _unitofwork.CompleteAsync();
            }
          
            return Ok(borrowed);

        }


        [HttpPut]
        [Route("EditBorrowedQuantity")]
        public async Task<IActionResult> EditBorrowedQuantity([FromBody] BorrowedIssueDetails borrowed)
        {

           
                await _unitofwork.Borrowed.EditBorrowedQuantity(borrowed);
                await _unitofwork.CompleteAsync();
            

            return Ok("Successfully edit borrowed issue!");

        }

        [HttpPut]
        [Route("EditBorrowedIssue")]
        public async Task<IActionResult> EditBorrowedIssue([FromBody] BorrowedIssue borrowed)
        {


            await _unitofwork.Borrowed.EditBorrowedIssue(borrowed);
            await _unitofwork.CompleteAsync();


            return Ok("Successfully edit borrowed issue!");

        }




        [HttpGet]
        [Route("ViewAllBorrowedDetails")]
        public async Task<IActionResult> ViewAllBorrowedDetails([FromQuery] int id)
        {

            var borrow = await _unitofwork.Borrowed.ViewAllBorrowedDetails(id);

            return Ok(borrow);
        }


        [HttpPut]
        [Route("CancelAllBorrowed")]
        public async Task<IActionResult> CancelAllBorrowed([FromBody] BorrowedIssue[] borrowed)
        {

            foreach (BorrowedIssue items in borrowed)
            {
                await _unitofwork.Borrowed.CancelAllBorrowed(items);
                await _unitofwork.CompleteAsync();
            }

            return Ok(borrowed);

        }


        [HttpGet]
        [Route("GetTransactedBorrowedDetails")]
        public async Task<IActionResult> GetTransactedBorrowedDetails([FromQuery] int empid)
        {

            var issue = await _unitofwork.Borrowed.GetTransactedBorrowedDetails(empid);

            return Ok(issue);

        }


        [HttpPut]
        [Route("CancelUpdateBorrowed")]
        public async Task<IActionResult> CancelUpdateBorrowed(BorrowedIssueDetails[] borrowed)
        {
            try
            {
                foreach (BorrowedIssueDetails items in borrowed)
                {
                    await _unitofwork.Borrowed.CancelUpdateBorrowed(items);
                    await _unitofwork.CompleteAsync();
                }
                return new JsonResult("Successfully cancel item!");
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message); // Return bad request for the AtLeast1ItemException
            }

        }




    }
}
