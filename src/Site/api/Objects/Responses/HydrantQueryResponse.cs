using System.Collections.Generic;

namespace Site.api.Objects.Responses
{
    public class HydrantQueryResponse: BaseResponse
    {
        public HydrantQueryResponse() { }

        public HydrantQueryResponse(bool _success,
            List<HydrantHeader> _hydrants) : base(_success)
        {
            Hydrants = _hydrants;
        }

        public List<HydrantHeader> Hydrants { get; set; }

    }
}