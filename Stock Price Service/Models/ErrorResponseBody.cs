namespace Stock_Price_Service.Models;

public class ErrorResponseBody
{
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
}

