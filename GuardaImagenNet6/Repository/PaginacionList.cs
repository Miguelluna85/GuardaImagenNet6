using Microsoft.EntityFrameworkCore;

namespace GuardaImagenNet6.Repository;

public class PaginacionList<T> : List<T>
{
    public int PaginaInicio { get; private set; }
    public int PaginasTotales { get; private set; }

    public PaginacionList(List<T> items, int contador, int paginaInicio, int cantidadregistros)
    {
        PaginaInicio = paginaInicio;
        PaginasTotales = (int)Math.Ceiling(contador / (double)cantidadregistros);

        this.AddRange(items);
    }

    public bool PaginasAnteriores => PaginaInicio > 1;
    public bool PaginasPosteriores => PaginaInicio < PaginasTotales;

    public static async Task<PaginacionList<T>> CrearPaginacion(IQueryable<T> fuente, int paginaInicio, int cantidadregistros)
    {
        var contador = await fuente.CountAsync();
        var items = await fuente
                    .Skip((paginaInicio - 1) * cantidadregistros)
                    .Take(cantidadregistros).ToListAsync();

        return new PaginacionList<T>(items, contador, paginaInicio, cantidadregistros);
    }
}
