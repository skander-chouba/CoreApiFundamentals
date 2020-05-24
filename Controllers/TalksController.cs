using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreApiFundamentals.Models;
using CoreCodeCamp.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoreApiFundamentals.Controllers
{
    [ApiController]
    [Route("api/camps/{moniker}/talks")]
    public class TalksController : ControllerBase
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _link;

        public TalksController(ICampRepository repository, IMapper mapper, LinkGenerator link)
        {
            _repository = repository;
            _mapper = mapper;
            _link = link;
        } 

        [HttpGet]
        public async Task<ActionResult<TalkModel[]>> Get(string moniker)
        {
            try
            {
                var talks = await _repository.GetTalksByMonikerAsync(moniker);
                return _mapper.Map<TalkModel[]>(talks);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TalkModel>>Get(string moniker, int id)
        {
            var talk = await _repository.GetTalkByMonikerAsync(moniker, id);
            return _mapper.Map<TalkModel>(talk);
        }

        [HttpPost]
        public async Task<ActionResult<TalkModel>> Post(string moniker, TalkModel model)
        {
            try
            {
                var camp = await _repository.GetCampAsync(moniker);
                if (camp == null) return BadRequest("Couldn't find the camp");
                var talk = _mapper.Map<Talk>(model);
                talk.Camp = camp;
                if (model.Speaker == null) return BadRequest("Speaker ID is required");
                talk.Speaker = await _repository.GetSpeakerAsync(model.Speaker.SpeakerId);
                if (talk.Speaker == null) return BadRequest("Speaker could not be found");

                _repository.Add(talk);
                if (await _repository.SaveChangesAsync())
                {
                    var url = _link.GetPathByAction(HttpContext, "Get",values: new { moniker, id = talk.TalkId });
                    return Created(url, _mapper.Map<TalkModel>(talk));

                }
                else
                {
                    return BadRequest("Failed to save the new talk");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
