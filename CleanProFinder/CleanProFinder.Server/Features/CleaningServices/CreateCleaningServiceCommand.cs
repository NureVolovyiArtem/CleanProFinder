﻿using AutoMapper;
using CleanProFinder.Db.DbContexts;
using CleanProFinder.Db.Models;
using CleanProFinder.Server.Extensions;
using CleanProFinder.Server.Features.Base;
using CleanProFinder.Shared.Dto.CleaningServices;
using CleanProFinder.Shared.Errors.ServiceErrors;
using CleanProFinder.Shared.ServiceResponseHandling;
using MediatR;

namespace CleanProFinder.Server.Features.CleaningServices
{
    public class CreateCleaningServiceCommand : CreateCleaningServiceCommandDto, IRequest<ServiceResponse<OwnCleaningServiceFullInfoDto>>
    {
        public class CreateCleaningServiceCommandHandler : BaseHandler<CreateCleaningServiceCommand, ServiceResponse<OwnCleaningServiceFullInfoDto>>
        {
            private readonly ApplicationDbContext _context;
            private readonly IHttpContextAccessor _contextAccessor;
            private readonly ILogger<CreateCleaningServiceCommandHandler> _logger;
            private readonly IMapper _mapper;

            public CreateCleaningServiceCommandHandler(
                ApplicationDbContext context,
                IHttpContextAccessor contextAccessor,
                ILogger<CreateCleaningServiceCommandHandler> logger,
                IMapper mapper)
            {
                _context = context;
                _contextAccessor = contextAccessor;
                _logger = logger;
                _mapper = mapper;
            }

            public override async Task<ServiceResponse<OwnCleaningServiceFullInfoDto>> Handle(CreateCleaningServiceCommand request,
                CancellationToken cancellationToken)
            {
                try
                {
                    return await UnsafeHandleAsync(request, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Create Cleaning Service", ex);
                    return ServiceResponseBuilder.Failure<OwnCleaningServiceFullInfoDto>(ServerError.CreateCleaningServiceError);
                }
            }

            private async Task<ServiceResponse<OwnCleaningServiceFullInfoDto>> UnsafeHandleAsync(CreateCleaningServiceCommand request,
                CancellationToken cancellationToken)
            {
                var validUserId = _contextAccessor.TryGetUserId(out var userId);
                if (validUserId is false)
                {
                    return ServiceResponseBuilder.Failure<OwnCleaningServiceFullInfoDto>(UserError.InvalidAuthorization);
                }

                var newCleaningService = _mapper.Map<CleaningService>(request);
                newCleaningService.ServiceProviderId = userId;

                _context.Add(newCleaningService);
                await _context.SaveChangesAsync(cancellationToken);

                var result = _mapper.Map<OwnCleaningServiceFullInfoDto>(newCleaningService);
                return ServiceResponseBuilder.Success(result);
            }
        }
    }
}
