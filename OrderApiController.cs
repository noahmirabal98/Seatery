using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Requests.Orders;
using Sabio.Services;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderApiController : BaseApiController
    {
        private IOrderService _service = null;
        private IAuthenticationService<int> _authService = null;
        public OrderApiController(IOrderService service,
            ILogger<OrderApiController> logger,
            IAuthenticationService<int> authService) : base(logger)
        {
            _service = service;
            _authService = authService;
        }

        [HttpPost]
        public ActionResult<ItemResponse<Order>> Create(OrderAddRequest model)
        {
            int code = 200;
            ObjectResult result = null;
            try
            {
                int userId = _authService.GetCurrentUserId();

                int id = _service.Add(model, userId);
                ItemResponse<int> response = new ItemResponse<int>() { Item = id };
                result = Created201(response);
            }
            catch (Exception ex)
            {
                code = 500;
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(code, response);
            }
            return result;
        }
        [HttpPut("{id:int}")]
        public ActionResult<ItemResponse<int>> Update(OrderUpdateRequest model)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                int userId = _authService.GetCurrentUserId();
                _service.Update(model, userId);

                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }
        [HttpPut("status/{id:int}")]
        public ActionResult<ItemResponse<int>> UpdateStatus(OrderUpdateRequestStatus model)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                int userId = _authService.GetCurrentUserId();
                _service.UpdateStatus(model, userId);

                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpPut("complete/{id:int}")]
        public ActionResult<ItemResponse<int>> UpdateIsCompleted(OrderUpdateRequestStatus model)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                int userId = _authService.GetCurrentUserId();
                _service.UpdateIsComplete(model, userId);

                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<SuccessResponse> Delete(int id)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                _service.Delete(id);

                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }
        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<Order>> Get(int id)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                Order order = _service.Get(id);
                if (order == null)
                {
                    code = 404;
                    response = new ErrorResponse("Order not found");
                }
                else
                {
                    response = new ItemResponse<Order> { Item = order };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse($"Generic Error: {ex.Message}");
            }


            return StatusCode(code, response);
        }
        [HttpGet("{trackingCode}")] 
        public ActionResult<ItemResponse<Order>> GetByTrackingCode(string trackingCode)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                Order order = _service.GetByTrackingCode(trackingCode);
                if (order == null)
                {
                    code = 404;
                    response = new ErrorResponse("Order not found");
                }
                else
                {
                    response = new ItemResponse<Order> { Item = order };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                base.Logger.LogError(ex.ToString());
                response = new ErrorResponse($"Generic Error: {ex.Message}");
            }


            return StatusCode(code, response);
        }
        [HttpGet("paginate")]
        public ActionResult<ItemResponse<Paged<Order>>> GetAll(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                Paged<Order> list = _service.GetAll(pageIndex, pageSize);

                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found");
                }
                else
                {
                    response = new ItemResponse<Paged<Order>> { Item = list };
                };
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);
        }

        [HttpGet("allDetails")]
        public ActionResult<ItemResponse<Paged<Order>>> GetAllDetails (bool isActive, int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                Paged<Order> list = _service.GetAllDetails(isActive, pageIndex, pageSize);

                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found");
                }
                else
                {
                    response = new ItemResponse<Paged<Order>> { Item = list };
                };
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }

        [HttpGet("current")]
        public ActionResult<ItemResponse<Paged<Order>>> GetCurrent(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                int userId = _authService.GetCurrentUserId();
                Paged<Order> list = _service.GetByCreatedBy(pageIndex, pageSize, userId);

                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("App resource not found");
                }
                else
                {
                    response = new ItemResponse<Paged<Order>> { Item = list };
                };
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);
        }

        [HttpGet("current/modified")]
        public ActionResult<ItemResponse<Order>> GetCurrentModified()
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                int userId = _authService.GetCurrentUserId();
                Order order = _service.GetByModifiedBy(userId);
                if (order == null)
                {
                    code = 404;
                    response = new ErrorResponse("No orders found");
                }
                else
                {
                    response = new ItemResponse<Order> { Item = order };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            };

            return StatusCode(code, response);
        }

        [HttpGet("completed")]
        public ActionResult<ItemResponse<Order>> GetCompleted(bool isCompleted)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                int userId = _authService.GetCurrentUserId();
                List<Order> order = _service.GetCompletedByModifiedBy(userId, isCompleted);
                if(order == null)
                {
                    code = 404;
                    response = new ErrorResponse("No Completed Orders");
                }
                else
                {
                    response = new ItemResponse<List<Order>> { Item = order };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }

            return StatusCode(code, response);
        }
    }
}
