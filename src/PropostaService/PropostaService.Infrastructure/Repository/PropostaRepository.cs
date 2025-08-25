using Microsoft.EntityFrameworkCore;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Interfaces;

namespace PropostaService.Infrastructure.Repository;

public class PropostaRepository : IPropostaRepository
{
    private readonly PropostaDbContext _context;

    public PropostaRepository(PropostaDbContext context)
    {
        _context = context;
    }
    
    public async Task CriarAsync(Proposta proposta)
    {
        await _context.Propostas.AddAsync(proposta);
        await _context.SaveChangesAsync();
    }

    public async Task<Proposta?> BuscaPorIdAsync(Guid id)
    {
        return await _context.Propostas.SingleOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Proposta>> BuscaAsync()
    {
        return await _context.Propostas.AsNoTracking().ToListAsync();
    }
    
    public async Task AtualizarAsync(Proposta proposta)
    {
        _context.Propostas.Update(proposta);
        await _context.SaveChangesAsync();
    }
}
