namespace EDO_FOMS.Client.JsResponse
{
    public class JsResult<T>
    {
        public T Data { get; set; }
        public bool Succeed { get; set; }
        public string Message { get; set; }
    }
}
