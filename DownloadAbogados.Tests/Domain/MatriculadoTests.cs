using DownloadAbogados.Data.DTOs;
using DownloadAbogados.Domain;

namespace DownloadAbogados.Tests.Domain;

public class MatriculadoTests
{
	[Fact]
	public void ToString_IncludesAllFields()
	{
		var matriculado = new Matriculado
		{
			Nombre = "Ada",
			Matricula = 5,
			Fecha = "hoy",
			Domicilio = "Calle 1",
			Telefono = "123",
			Correo = "ada@example.test"
		};

		Assert.Equal(
			"Nombre: Ada, Matricula: 5, Fecha: hoy, Domicilio: Calle 1, Telefono: 123, Correo: ada@example.test",
			matriculado.ToString());
	}

	[Fact]
	public void MatriculadoDto_CopiesFieldsAndStartsUnsaved()
	{
		var source = new Matriculado
		{
			Nombre = "Ada",
			Matricula = 5,
			Fecha = "hoy",
			Domicilio = "Calle 1",
			Telefono = "123",
			Correo = "ada@example.test"
		};

		var dto = new MatriculadoDTO(source);

		Assert.Equal(source.Nombre, dto.Nombre);
		Assert.Equal(source.Matricula, dto.Matricula);
		Assert.Equal(source.Fecha, dto.Fecha);
		Assert.Equal(source.Domicilio, dto.Domicilio);
		Assert.Equal(source.Telefono, dto.Telefono);
		Assert.Equal(source.Correo, dto.Correo);
		Assert.False(dto.Saved);
	}
}