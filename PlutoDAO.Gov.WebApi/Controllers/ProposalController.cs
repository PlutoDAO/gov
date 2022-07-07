using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlutoDAO.Gov.Application.Dtos;
using PlutoDAO.Gov.Application.Exceptions;
using PlutoDAO.Gov.Application.Extensions;
using PlutoDAO.Gov.Application.Proposals;
using PlutoDAO.Gov.Application.Proposals.Responses;
using PlutoDAO.Gov.Application.Votes;
using PlutoDAO.Gov.WebApi.Request;
using System;
using System.Threading.Tasks;

namespace PlutoDAO.Gov.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProposalController : ControllerBase
    {
        private readonly ProposalService _proposalService;
        private readonly VoteService _voteService;

        public ProposalController(ProposalService proposalService, VoteService voteService)
        {
            _proposalService = proposalService;
            _voteService = voteService;
        }

        [HttpGet("{proposalId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IProposalResponse))]
        public async Task<IActionResult> Get(string proposalId)
        {
            try
            {
                return Ok(await _proposalService.GetProposal(proposalId));
            }
            catch (ProposalNotFoundException e)
            {
                return Problem(e.Message, null, 404, e.Title, e.Type);
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }

        [HttpGet(Name = nameof(GetList))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ListResponseDto))]
        public async Task<IActionResult> GetList([FromQuery] UrlQueryParameters urlQueryParameters)
        {
            try
            {
                var proposals = await _proposalService.GetList(
                    urlQueryParameters.Limit,
                    urlQueryParameters.Page
                );
                return Ok(GeneratePageLinks(urlQueryParameters, proposals));
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }

        public record UrlQueryParameters(int Limit = 50, int Page = 1);

        private ListResponseDto GeneratePageLinks(
            UrlQueryParameters queryParameters,
            ListResponseDto response
        )
        {
            if (response.CurrentPage > 1)
            {
                var prevRoute = Url.RouteUrl(
                    nameof(GetList),
                    new { limit = queryParameters.Limit, page = queryParameters.Page - 1 }
                );

                response.AddResourceLink(LinkedResourceType.Prev, prevRoute);
            }

            if (response.CurrentPage < response.TotalPages)
            {
                var nextRoute = Url.RouteUrl(
                    nameof(GetList),
                    new { limit = queryParameters.Limit, page = queryParameters.Page + 1 }
                );

                response.AddResourceLink(LinkedResourceType.Next, nextRoute);
            }

            return response;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Save(ProposalRequest request)
        {
            try
            {
                await _proposalService.Save(request);
                return Ok();
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }

        [HttpPost("/{proposalId}/VoteIntent")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        public async Task<IActionResult> GetVoteIntent(VoteIntentRequest request, string proposalId)
        {
            try
            {
                return Ok(await _voteService.Vote(request, proposalId));
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }

        [HttpPost("/{proposalId}/vote")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Vote(DirectVoteRequest request, string proposalId)
        {
            try
            {
                await _voteService.Vote(request, proposalId);
                return Ok();
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
        }
    }
}
