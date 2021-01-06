using Api.Data.Collections;
using Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InfectadoController : ControllerBase
    {
        Data.MongoDB _mongoDB;
        IMongoCollection<Infectado> _infectadosCollection;

        public InfectadoController(Data.MongoDB mongoDB)
        {
            _mongoDB = mongoDB;
            _infectadosCollection = _mongoDB.DB.GetCollection<Infectado>(typeof(Infectado).Name.ToLower());
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult SalvarInfectado([FromBody] InfectadoDto dto)
        {
            var infectado = new Infectado(dto.DataNascimento, dto.Sexo, dto.cpf, dto.Latitude, dto.Longitude);

            _infectadosCollection.InsertOne(infectado);
            
            return StatusCode(201, "Infectado adicionado com sucesso");
        }

        [HttpGet]
        public ActionResult ObterInfectados()
        {
            var infectados = _infectadosCollection.Find(Builders<Infectado>.Filter.Empty).ToList();
            
            return Ok(infectados);
        }

        [HttpGet]
        [Route("/Infectado/{cpf}")]
        public ActionResult ObterInfectadosPorCpf([FromRoute]string cpf)
        {
            var infectados = _infectadosCollection.Find(Builders<Infectado>.Filter.Eq(o => o.CPF, cpf)).ToList();
            
            return Ok(infectados);
        }

        [HttpPut]
        public ActionResult AlterarInfectado([FromBody] InfectadoDto dto)
        {
            var filter = Builders<Infectado>.Filter;
            _infectadosCollection.UpdateOne(Builders<Infectado>.Filter.Eq(o => o.CPF, dto.cpf),
            Builders<Infectado>.Update.Set("sexo", dto.Sexo));
            _infectadosCollection.UpdateOne(Builders<Infectado>.Filter.Eq(o => o.CPF, dto.cpf),
            Builders<Infectado>.Update.Set("dataNascimento", dto.DataNascimento));
            
            return Ok("Atualizado com sucesso");
        }

        [HttpDelete]
        [Route("/Infectado/{cpf}")]
        public ActionResult DeletarInfectados([FromRoute]string cpf)
        {
            _infectadosCollection.DeleteOne(Builders<Infectado>.Filter.Eq(o => o.CPF, cpf));
            
            return StatusCode(204, "Deletado com sucesso!");
        }
    }
}