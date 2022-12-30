namespace RecordSales.Models
{
    public class JsonResultModel
    {
        public object Collection { get; set; }

        public bool IsSuccess { get; set; }

        public string ErrorMessage { get; set; }
    }
}
