using Microsoft.AspNetCore.Mvc;
using ProjetoEM.Interfaces;
using ProjetoEM.Models;

namespace ProjetoEM.Controllers;

public class CidadeController(ICidadeRepository repositorio) : Controller
{
    private readonly ICidadeRepository _cidadeRepository = repositorio;
    
    [HttpGet("/Cidade/ObterPorId/{id}")]
    public IActionResult ObterPorId(int id)
    {
        try
        {
            var cidade = _cidadeRepository.ObtenhaCidade(id);

            if (cidade == null)
            {
                return NotFound($"Cidade com ID {id} não encontrada!");
            }
            
            return Ok(cidade);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao buscar cidade: {ex.Message}");
        }
    }

    [HttpGet("/Cidade/ListarPorEstado/{uf}")]
    public IActionResult ListarPorEstado(int uf)
    {
        try
        {
            var cidades = _cidadeRepository.ObtenhaCidadePorEstado(uf);

            if (!cidades.Any())
            {
                return NotFound("Não há cidades cadastradas neste estado!");
            }
            
            return Ok(cidades);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao listar cidades por estado: {ex.Message}");
        }
    }

    [HttpPost("/Cidade/Cadastrar")]
    public IActionResult Cadastrar([FromBody] Cidade cidade)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (cidade == null)
            {
                return BadRequest("Dados da cidade são obrigatórios!");
            }

            var cidadeExistente = _cidadeRepository.ObtenhaCidade(cidade.Id);
            if (cidadeExistente != null)
            {
                return Conflict($"Já existe uma cidade cadastrada com o ID {cidade.Id}!");
            }

            var cidadeAdicionada = _cidadeRepository.AdicionarCidade(cidade);
            return CreatedAtAction(nameof(ObterPorId), new { id = cidadeAdicionada.Id }, cidadeAdicionada);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao cadastrar cidade: {ex.Message}");
        }
    }

    [HttpPut("/Cidade/Atualizar/{id}")]
    public IActionResult Atualizar(int id, [FromBody] Cidade cidade)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (cidade == null)
            {
                return BadRequest("Dados da cidade são obrigatórios!");
            }

            if (id != cidade.Id)
            {
                return BadRequest("O ID informado não corresponde à cidade!");
            }

            var cidadeExistente = _cidadeRepository.ObtenhaCidade(id);
            if (cidadeExistente == null)
            {
                return NotFound($"Cidade com ID {id} não encontrada!");
            }

            var cidadeAtualizada = _cidadeRepository.AtualizarCidade(cidade);
            return Ok(cidadeAtualizada);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao atualizar cidade: {ex.Message}");
        }
    }

    [HttpDelete("/Cidade/Deletar/{id}")]
    public IActionResult Deletar(int id)
    {
        try
        {
            var cidadeExistente = _cidadeRepository.ObtenhaCidade(id);
            if (cidadeExistente == null)
            {
                return NotFound($"Cidade com ID {id} não encontrada!");
            }

            var deletado = _cidadeRepository.DeletarCidade(id);
            
            if (deletado)
            {
                return Ok($"Cidade com ID {id} deletada com sucesso!");
            }

            return BadRequest("Erro ao deletar cidade!");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao deletar cidade: {ex.Message}");
        }
    }
}