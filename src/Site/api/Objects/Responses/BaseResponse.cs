namespace Site.api.Objects.Responses
{
    public class BaseResponse
    {
        public bool Success { get; set; }

        public BaseResponse() { }

        public BaseResponse(bool _success)
        {
            Success = _success;
        }
    }
}