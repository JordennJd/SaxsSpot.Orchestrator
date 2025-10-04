using System.Text.Json.Serialization;
using FluentResults;
using MediatR;
using SaxsSpot.Orchestrator.Contracts.Messages;
using SaxsSpot.Orchestrator.Contracts.Models;

namespace SaxsSpot.Orchestrator.Application.Features.Calculation.Command.RunSeriesCalculation;

public record RunSeriesCalculationCommand : CalculateScatteringRequest, IRequest<IResult<Guid>>;