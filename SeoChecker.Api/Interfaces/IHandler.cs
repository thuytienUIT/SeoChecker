using System;
using SeoChecker.Shared.Models;

namespace SeoChecker.Api.Interfaces
{
	public interface IHandler
	{
        Task<string> Handle(SearchRequest request, CancellationToken cancellationToken);
    }
}

