using Microsoft.AspNetCore.Mvc;
using ProjetoEM.Interfaces;
using ProjetoEM.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjetoEM.Enums;
using ProjetoEM.Validadores;

namespace ProjetoEM.Controllers;

public class CidadeController : Controller
{
    private readonly ICidadeRepository _cidadeRepository;

    public CidadeController(ICidadeRepository repositorio)
    {
        _cidadeRepository = repositorio;
    }

    [HttpGet]
    public IActionResult Index()
    {
        List<Cidade> cidades = _cidadeRepository.ObtenhaTodasCidades();
        return View(cidades);
    }
    
    [HttpGet]
    public IActionResult Create()
    {
        CarregarDadosViewBag(null);
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Cidade cidade)
    {
        if (!ModelState.IsValid || !ValidarCidade(cidade))
        {
            CarregarDadosViewBag(cidade);
            return View(cidade);
        }

        _cidadeRepository.AdicionarCidade(cidade);
        return RedirectToAction(nameof(Index));
    }
    
    [HttpGet]
    public IActionResult Edit(int id)
    {
        Cidade cidade = _cidadeRepository.ObtenhaCidade(id);
        if (cidade == null)
        {
            return NotFound();
        }
        
        CarregarDadosViewBag(cidade);
        return View(cidade);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Cidade cidade)
    {
        if (id != cidade.Id)
        {
            return NotFound();
        }
        
        if (!ModelState.IsValid || !ValidarCidade(cidade))
        {
            CarregarDadosViewBag(cidade);
            return View(cidade);
        }
        
        _cidadeRepository.AtualizarCidade(cidade);
        return RedirectToAction(nameof(Index));
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        _cidadeRepository.DeletarCidade(id);
        return RedirectToAction(nameof(Index));
    }
    
    private void CarregarDadosViewBag(Cidade? cidade)
    {
        ViewBag.UFList = new SelectList(Enum.GetValues(typeof(UF)), cidade?.UF);
    }
    
    private bool ValidarCidade(Cidade cidade)
    {
        if (string.IsNullOrWhiteSpace(cidade.NomeDaCidade) || !ValidadorDaCidade.NomeDaCidadeEhValido(cidade.NomeDaCidade))
        {
            ModelState.AddModelError("NomeDaCidade", "O nome da cidade deve ter entre 3 e 100 caracteres.");
            return false;
        }
        
        return true;
    }
    
    [HttpGet("/Cidade/ObterPorId/{id}")]
    public IActionResult ObterPorId(int id)
    {
        try
        {
            Cidade cidade = _cidadeRepository.ObtenhaCidade(id);

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
            IEnumerable<Cidade> cidades = _cidadeRepository.ObtenhaCidadePorEstado(uf);

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

            Cidade cidadeAdicionada = _cidadeRepository.AdicionarCidade(cidade);
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

            Cidade cidadeExistente = _cidadeRepository.ObtenhaCidade(id);
            if (cidadeExistente == null)
            {
                return NotFound($"Cidade com ID {id} não encontrada!");
            }

            Cidade cidadeAtualizada = _cidadeRepository.AtualizarCidade(cidade);
            return Ok(cidadeAtualizada);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao atualizar cidade: {ex.Message}");
        }
    }

    [HttpDelete("/Cidade/Deletar/{id}")]
    public IActionResult DeletarAPI(int id)
    {
        try
        {
            Cidade cidadeExistente = _cidadeRepository.ObtenhaCidade(id);
            if (cidadeExistente == null)
            {
                return NotFound($"Cidade com ID {id} não encontrada!");
            }

            bool deletado = _cidadeRepository.DeletarCidade(id);
            
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