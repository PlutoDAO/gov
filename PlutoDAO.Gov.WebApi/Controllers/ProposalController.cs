using System;
using System.Threading.Tasks;
using PlutoDAO.Gov.Application.Exceptions;
using PlutoDAO.Gov.Application.Proposals;
using PlutoDAO.Gov.Application.Proposals.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PlutoDAO.Gov.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProposalController : ControllerBase
    {
        private readonly ProposalService _proposalService;

        public ProposalController(ProposalService proposalService)
        {
            _proposalService = proposalService;
        }
        
        [HttpGet("{address}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IProposalResponse))]
        public async Task<IActionResult> Get(string address)
        {
            try
            {
                return Ok(await _proposalService.FindByAddress(address));
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
    }
}
