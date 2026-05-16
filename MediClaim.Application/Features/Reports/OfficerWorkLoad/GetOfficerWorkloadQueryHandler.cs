using MediatR;
using MediClaim.Application.Common.Interfaces;
using MediClaim.Application.Features.Reports.OfficerWorkLoad;
using MediClaim.Application.Repositories;

namespace MediClaim.Application.Features.Reports.OfficerWorkload;

public class GetOfficerWorkloadQueryHandler
    : IRequestHandler<
        GetOfficerWorkloadQuery,
        List<OfficerWorkloadDto>>
{
    private readonly IReportRepository _repository;

    public GetOfficerWorkloadQueryHandler(
        IReportRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<OfficerWorkloadDto>> Handle(
                GetOfficerWorkloadQuery request,
                CancellationToken cancellationToken)
    {
        return await _repository
            .GetOfficerWorkloadAsync(cancellationToken);

    }
}