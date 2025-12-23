using DownloadAbogados.Domain;

namespace DownloadAbogados.Data.DTOs;

public class MatriculadoDTO: DownloadAbogados.Domain.Matriculado
{
	public MatriculadoDTO(Matriculado matriculado)
	{
		this.Nombre = matriculado.Nombre;
		this.Matricula = matriculado.Matricula;
		this.Fecha = matriculado.Fecha;
		this.Domicilio = matriculado.Domicilio;
		this.Telefono = matriculado.Telefono;
		this.Correo = matriculado.Correo;
		this.Saved = false;
	}

	public bool Saved { get; set; } = false;
}