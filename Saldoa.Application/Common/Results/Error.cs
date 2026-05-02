namespace Saldoa.Application.Common.Results
{
    public record Error(string Code, string Message, ErrorType Type);
}
