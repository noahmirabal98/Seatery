namespace blank.Web.Api.Controllers
{
    [Route("api/profiles")]
    [ApiController]
    public class UserProfilesApiController : BaseApiController
    {
        private IUserProfilesService _service = null;
        private IAuthenticationService<int> _authService = null;

        public UserProfilesApiController(IUserProfilesService service,
           ILogger<UserProfilesApiController> logger,
           IAuthenticationService<int> userProfileService) : base(logger)
        {
            _service = service;
            _authService = userProfileService;
        }

        [HttpPost]
        public ActionResult<ItemResponse<int>> Add(UserProfilesAddRequest model)
        {
            ObjectResult result = null;
            try
            {
                int userId = _authService.GetCurrentUserId();

                int id = _service.Add(model, userId);

                ItemResponse<int> response = new ItemResponse<int>();

                response.Item = id;
                result = Ok(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);

                result = StatusCode(500, response);
            }
           

            return result;
        }

        [HttpGet("paginate")]
        public ActionResult<ItemResponse<Paged<UserProfile>>> GetAllPaginated(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                Paged<UserProfile> page = _service.GetAllPaginated(pageIndex, pageSize);

                if (page == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<UserProfile>> { Item = page };
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

        [HttpGet("current")]
        public ActionResult<ItemResponse<Paged<UserProfile>>> GetByCurrent(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                int userId = _authService.GetCurrentUserId();

                Paged<UserProfile> page = _service.GetByCurrent(pageIndex, pageSize, userId);

                if (page == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<UserProfile>> { Item = page };
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

        [HttpGet("search")]
        public ActionResult<ItemResponse<Paged<UserProfile>>> GetSearchedProfile(int pageIndex, int pageSize, string query)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                int userId = _authService.GetCurrentUserId();

                Paged<UserProfile> page = _service.GetSearchedProfile(pageIndex, pageSize, query);

                if (page == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<UserProfile>> { Item = page };
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

        [HttpPut("{id:int}")]
        public ActionResult<SuccessResponse> Update(UserProfilesUpdateRequest model)
        {
            int code = 200;
            int userId = _authService.GetCurrentUserId();
            BaseResponse response = null;

            try
            {
                _service.Update(model, userId);

                response = new SuccessResponse();
            }
            catch (Exception exc)
            {
                code = 500;
                response = new ErrorResponse(exc.Message);
            }

            return StatusCode(code, response);
        }

        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<UserProfile>> GetById(int id)
        {
            int sCode = 200;
            BaseResponse response = null;

            try
            {
                UserProfile aProfile = _service.GetById(id);

                if (aProfile == null)
                {
                    sCode = 404;
                    response = new ErrorResponse("Resource not found!");
                }
                else
                {
                    response = new ItemResponse<UserProfile> { Item = aProfile };
                }
            }
            catch (SqlException sqlEx)
            {
                sCode = 500;
                response = new ErrorResponse("Resource not found!");
                base.Logger.LogError(sqlEx.ToString());
            }

            return StatusCode(sCode, response);
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
            catch (Exception exc)
            {
                code = 500;
                response = new ErrorResponse(exc.Message);
            }

            return StatusCode(code, response);
        }
    }
}
