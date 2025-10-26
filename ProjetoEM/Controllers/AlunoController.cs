using Microsoft.AspNetCore.Mvc;
using ProjetoEM.Interfaces;
using ProjetoEM.Models;

namespace ProjetoEM.Controllers;

public class AlunoController(IAlunoRepository repositorio) : Controller
{
    private readonly IAlunoRepository _alunoRepository = repositorio;
    
    [HttpGet("/Aluno/ListarAlunos")]
    public IActionResult ListarAlunos()
    {
        try
        {
            List<Aluno> alunos = _alunoRepository.OtenhaAlunos();

            if (!alunos.Any())
            {
                return NotFound("Não há nenhum aluno matriculado!");
            }
            
            return Ok(alunos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao listar alunos: {ex.Message}");
        }
    }

    [HttpGet("/Aluno/ObterPorMatricula/{matricula}")]
    public IActionResult ObterPorMatricula(int matricula)
    {
        try
        {
            Aluno aluno = _alunoRepository.OtenhaAlunoPorMatricula(matricula);

            if (aluno == null)
            {
                return NotFound($"Aluno com matrícula {matricula} não encontrado!");
            }
            
            return Ok(aluno);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao buscar aluno: {ex.Message}");
        }
    }

    [HttpGet("/Aluno/ListarPorSexo/{sexo}")]
    public IActionResult ListarPorSexo(int sexo)
    {
        try
        {
            IEnumerable<Aluno> alunos = _alunoRepository.OtenhaAlunosPorSexo(sexo);

            if (!alunos.Any())
            {
                return NotFound("Não há alunos cadastrados com o sexo informado!");
            }
            
            return Ok(alunos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao listar alunos por sexo: {ex.Message}");
        }
    }

    [HttpGet("/Aluno/ListarPorCidade/{cidadeId}")]
    public IActionResult ListarPorCidade(int cidadeId)
    {
        try
        {
            IEnumerable<Aluno> alunos = _alunoRepository.OtenhaAlunosPorCidade(cidadeId);

            if (!alunos.Any())
            {
                return NotFound("Não há alunos cadastrados nesta cidade!");
            }
            
            return Ok(alunos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao listar alunos por cidade: {ex.Message}");
        }
    }

    [HttpPost("/Aluno/Cadastrar")]
    public IActionResult Cadastrar([FromBody] Aluno aluno)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (aluno == null)
            {
                return BadRequest("Dados do aluno são obrigatórios!");
            }

            Aluno alunoExistente = _alunoRepository.OtenhaAlunoPorMatricula(aluno.Matricula);
            if (alunoExistente != null)
            {
                return Conflict($"Já existe um aluno cadastrado com a matrícula {aluno.Matricula}!");
            }

            Aluno alunoAdicionado = _alunoRepository.AdicionarAluno(aluno);
            return CreatedAtAction(nameof(ObterPorMatricula), 
                new { matricula = alunoAdicionado.Matricula }, alunoAdicionado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao cadastrar aluno: {ex.Message}");
        }
    }

    [HttpPut("/Aluno/Atualizar/{matricula}")]
    public IActionResult Atualizar(int matricula, [FromBody] Aluno aluno)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (aluno == null)
            {
                return BadRequest("Dados do aluno são obrigatórios!");
            }

            if (matricula != aluno.Matricula)
            {
                return BadRequest("A matrícula informada não corresponde ao aluno!");
            }

            Aluno alunoExistente = _alunoRepository.OtenhaAlunoPorMatricula(matricula);
            if (alunoExistente == null)
            {
                return NotFound($"Aluno com matrícula {matricula} não encontrado!");
            }

            Aluno alunoAtualizado = _alunoRepository.AtualizarAluno(aluno);
            return Ok(alunoAtualizado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao atualizar aluno: {ex.Message}");
        }
    }

    [HttpDelete("/Aluno/Deletar/{matricula}")]
    public IActionResult Deletar(int matricula)
    {
        try
        {
            Aluno alunoExistente = _alunoRepository.OtenhaAlunoPorMatricula(matricula);
            if (alunoExistente == null)
            {
                return NotFound($"Aluno com matrícula {matricula} não encontrado!");
            }

            bool deletado = _alunoRepository.DeletarAluno(matricula);
            
            if (deletado)
            {
                return Ok($"Aluno com matrícula {matricula} deletado com sucesso!");
            }

            return BadRequest("Erro ao deletar aluno!");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro ao deletar aluno: {ex.Message}");
        }
    }
}