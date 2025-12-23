namespace DownloadAbogados.Domain;

public class Matriculado
{
	public string? Nombre { get; set; }
	public int Matricula { get; set; }
	public string? Fecha { get; set; }
	public string? Domicilio { get; set; }
	public string? Telefono { get; set; }
	public string? Correo { get; set; }

	public override string ToString()
	{
		return $"Nombre: {Nombre}, Matricula: {Matricula}, Fecha: {Fecha}, Domicilio: {Domicilio}, Telefono: {Telefono}, Correo: {Correo}";
	}
}
