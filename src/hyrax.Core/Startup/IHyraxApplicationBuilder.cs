using Microsoft.AspNetCore.Builder;

namespace hyrax.Core.Startup
{
    public interface IHyraxApplicationBuilder//: IApplicationBuilder
    {
        IApplicationBuilder App { get; }
    }
}
