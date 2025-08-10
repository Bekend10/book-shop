namespace realtime_service.Services.External.Models.CommonModel
{
    public class ApiResponseHandlder<T>
    {
        public int Status { get; set; }
        public string Msg { get; set; }
        public T Data { get; set; }
    }
}
