using Microsoft.AspNetCore.Mvc;
using EM.Domain.Interface;
using EM.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using EM.Domain.Utilitarios;
using EM.Web.Services;

namespace EM.Web.Controllers;

public class AlunoController(
    IAlunoRepository repositorio,
    ICidadeRepository cidadeRepositorio,
    RelatorioService relatorioService) : Controller
{
    private readonly IAlunoRepository _alunoRepository = repositorio;
    private readonly ICidadeRepository _cidadeRepository = cidadeRepositorio;
    private readonly RelatorioService _relatorioService = relatorioService;

    [HttpGet]
    public IActionResult Index(string busca = "", int? sexo = null, int? cidadeId = null, int? uf = null)
    {
        IEnumerable<Aluno> alunos = _alunoRepository.OtenhaAlunos();
        
        if (!string.IsNullOrWhiteSpace(busca))
        {
            alunos = alunos.Where(a => 
                a.Nome.Contains(busca, StringComparison.OrdinalIgnoreCase) ||
                a.Matricula.ToString().Contains(busca, StringComparison.OrdinalIgnoreCase)
            );
        }

        if (sexo.HasValue)
        {
            alunos = alunos.Where(a => (int)a.Sexo == sexo.Value);
        }

        if (cidadeId.HasValue)
        {
            alunos = alunos.Where(a => a.CidadeId == cidadeId.Value);
        }

        if (uf.HasValue)
        {
            alunos = alunos.Where(a => a.Cidade != null && (int)a.Cidade.UF == uf.Value);
        }

        ViewBag.Busca = busca;
        ViewBag.SexoFiltro = sexo;
        ViewBag.CidadeFiltro = cidadeId;
        ViewBag.UFFiltro = uf;
        ViewBag.SexoList = Enum.GetValues(typeof(Sexo));
        ViewBag.CidadeList = _cidadeRepository.ObtenhaTodasCidades();
        ViewBag.UFList = Enum.GetValues(typeof(UF));
        
        return View(alunos.ToList());
    }
    
    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.IsEdit = false;
        CarregarDadosViewBag(null);
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Aluno aluno)
    {
        if (aluno.Cpf != null && string.IsNullOrWhiteSpace(aluno.Cpf.Value))
        {
            aluno.Cpf = null;            
            ModelState.Remove("Cpf");
            ModelState.Remove("Cpf.Value");
        }
        else if (aluno.Cpf != null && !string.IsNullOrWhiteSpace(aluno.Cpf.Value))
        {
            aluno.Cpf = new CPF(CPF.RemoverFormatacao(aluno.Cpf.Value));
        }
        
        if (!ModelState.IsValid || !ValidarAluno(aluno, null))
        {
            ViewBag.IsEdit = false;
            CarregarDadosViewBag(aluno);
            return View(aluno);
        }
        
        _alunoRepository.AdicionarAluno(aluno);
        return RedirectToAction(nameof(Index));
    }
    
    [HttpGet]
    public IActionResult Edit(int matricula)
    {
        Aluno aluno = _alunoRepository.OtenhaAlunoPorMatricula(matricula);
        if (aluno == null)
        {
            return NotFound();
        }
        
        ViewBag.IsEdit = true;
        CarregarDadosViewBag(aluno);
        return View("Create", aluno);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int matricula, Aluno aluno)
    {
        if (matricula != aluno.Matricula)
        {
            return NotFound();
        }
        
        if (aluno.Cpf != null && string.IsNullOrWhiteSpace(aluno.Cpf.Value))
        {
            aluno.Cpf = null;
            
            ModelState.Remove("Cpf");
            ModelState.Remove("Cpf.Value");
        }
        else if (aluno.Cpf != null && !string.IsNullOrWhiteSpace(aluno.Cpf.Value))
        {
            aluno.Cpf = new CPF(CPF.RemoverFormatacao(aluno.Cpf.Value));
        }
        
        if (!ModelState.IsValid || !ValidarAluno(aluno, matricula))
        {
            ViewBag.IsEdit = true;
            CarregarDadosViewBag(aluno);
            return View("Create", aluno);
        }
        
        _alunoRepository.AtualizarAluno(aluno);
        return RedirectToAction(nameof(Index));
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int matricula)
    {
        _alunoRepository.DeletarAluno(matricula);
        return RedirectToAction(nameof(Index));
    }
    
    [HttpGet]
    public IActionResult GerarRelatorio()
    {
        try
        {
            List<Aluno> alunos = _alunoRepository.OtenhaAlunos();
            byte[] pdfBytes = _relatorioService.GerarRelatorioPDFAlunos(alunos);
            
            Response.Headers.ContentDisposition = $"inline; filename=Relatorio_Alunos_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            return File(pdfBytes, "application/pdf");
        }
        catch (Exception ex)
        {
            string erro = $"Erro ao gerar relatório: {ex.Message}\n\nStack Trace: {ex.StackTrace}";
            if (ex.InnerException != null)
            {
                erro += $"\n\nInner Exception: {ex.InnerException.Message}";
            }
            
            TempData["Erro"] = erro;
            return RedirectToAction(nameof(Index));
        }
    }
    
    private void CarregarDadosViewBag(Aluno? aluno)
    {
        ViewBag.SexoList = new SelectList(Enum.GetValues(typeof(Sexo)), aluno?.Sexo);
        ViewBag.CidadeList = _cidadeRepository.ObtenhaTodasCidades().Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = $"{c.NomeDaCidade} - {c.UF}",
            Selected = aluno?.CidadeId == c.Id
        }).ToList();
    }
    
    private bool ValidarAluno(Aluno aluno, int? matriculaAtual)
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(aluno.Nome) || !Validation.NomeEhValido(aluno.Nome))
        {
            ModelState.AddModelError("Nome", "O nome deve ter entre 3 e 100 caracteres.");
            isValid = false;
        }

        if (aluno.Cpf != null && !string.IsNullOrWhiteSpace(aluno.Cpf.Value))
        {
            if (!aluno.Cpf.EhValido())
            {
                ModelState.AddModelError("Cpf.Value", "CPF inválido.");
                isValid = false;
            }
        }

        Aluno? alunoComMesmaMatricula = _alunoRepository.OtenhaAlunoPorMatricula(aluno.Matricula);
        if (alunoComMesmaMatricula != null)
        {
            if (!matriculaAtual.HasValue || alunoComMesmaMatricula.Matricula != matriculaAtual.Value)
            {
                ModelState.AddModelError("Matricula", $"Já existe um aluno cadastrado com a matrícula {aluno.Matricula}.");
                isValid = false;
            }
        }

        if (aluno.Cpf != null && !string.IsNullOrWhiteSpace(aluno.Cpf.Value))
        {
            Aluno? alunoComMesmoCpf = _alunoRepository.OtenhaAlunoPorCpf(aluno.Cpf.Value);
            if (alunoComMesmoCpf != null)
            {
                if (!matriculaAtual.HasValue || alunoComMesmoCpf.Matricula != matriculaAtual.Value)
                {
                    ModelState.AddModelError("Cpf.Value", $"Já existe um aluno cadastrado com o CPF {aluno.Cpf.CpfFormatado}.");
                    isValid = false;
                }
            }
        }
        
        return isValid;
    }
    
    [HttpGet("/Aluno/ListarAlunos")]
    public IActionResult ListarAlunos()
    {
        try
        {
            IEnumerable<Aluno> alunos = _alunoRepository.OtenhaAlunos();

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
    public IActionResult DeletarAPI(int matricula)
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