namespace F1Telemetry.Domain.Entities;

public class Season
{
    public int Year { get; set; } // PK
    
    // Propriedade de Navegação: Uma temporada tem vários GPs
    public ICollection<GrandPrix> GrandPrixes { get; set; } = new List<GrandPrix>();
}
