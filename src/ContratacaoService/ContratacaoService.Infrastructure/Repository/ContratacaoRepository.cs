using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContratacaoService.Infrastructure.Data.Repositories;

public class ContratacaoRepository : IContratacaoRepository
{
    private readonly ContratacaoDbContext _context;

    public ContratacaoRepository(ContratacaoDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(Contratacao contratacao)
    {
        await _context.Contratacoes.AddAsync(contratacao);
        await _context.SaveChangesAsync();
    }

    public async Task<Contratacao?> BuscarPorIdAsync(Guid id)
    {
        return await _context.Contratacoes.FindAsync(id);
    }
    
    public async Task<Contratacao?> BuscarPorPropostaIdAsync(Guid propostaId)
    {
        return await _context.Contratacoes.FirstOrDefaultAsync(x => x.PropostaId == propostaId);
    }

    public async Task<IEnumerable<Contratacao>> ListarTodasAsync()
    {
        return await _context.Contratacoes.AsNoTracking().ToListAsync();
    }
}