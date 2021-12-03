using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlutoDAO.Gov.Application.Exceptions;
using PlutoDAO.Gov.Application.Proposals;
using PlutoDAO.Gov.Application.Proposals.Responses;
using PlutoDAO.Gov.WebApi.Request;

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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(await _proposalService.GetAll());
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
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
    }
}
