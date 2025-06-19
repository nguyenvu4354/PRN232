namespace ShoppingWeb.Response;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public string ErrorCode { get; set; }

    public ApiResponse(T data, string message = "Operation successful")
    {
        Success = true;
        Data = data;
        Message = message;
        ErrorCode = null;
    }

    // Constructor for error response
    public ApiResponse(string errorMessage, string errorCode = null)
    {
        Success = false;
        Message = errorMessage;
        ErrorCode = errorCode;
        Data = default;
    }

    // Static factory methods for convenience
    public static ApiResponse<T> SuccessResponse(T data, string message = "Operation successful")
        => new ApiResponse<T>(data, message);

    public static ApiResponse<T> ErrorResponse(string errorMessage, string errorCode = null)
        => new ApiResponse<T>(errorMessage, errorCode);
}