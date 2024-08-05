
namespace Models;
public class ServiceResult<TData, TError>
    where TError : struct, Enum
{
    public TData? Data { get; set; }
    public TError Error { get; set; }
    public bool IsSuccess => Error.Equals(default(TError));

    public ServiceResult(TData data) => Data = data;

    public ServiceResult(TError error) => Error = error;
}
