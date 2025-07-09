namespace ShoppingWeb.MvcClient.DTOs.Common
{
    public class ApiResponseDTO<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T Data { get; set; } = default!;
        public string? ErrorCode { get; set; }
    }
}
