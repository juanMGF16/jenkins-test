using Business.Services.NITValidation.Interfaces;
using System.Net.Http.Json;

namespace Business.Services.NITValidation
{
    /// <summary>
    /// Implementación de <see cref="INitValidationService"/> que utiliza un <see cref="HttpClient"/>
    /// configurado para consultar la existencia de un NIT contra el dataset público de datos.gov.co.
    /// </summary>
    public class NitValidationService : INitValidationService
    {
        private readonly HttpClient _httpClient;

        public NitValidationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://www.datos.gov.co/resource/");
        }

        /// <summary>
        /// Verifica asincrónicamente si un NIT base (sin dígito de verificación) existe en el portal de datos abiertos.
        /// </summary>
        /// <param name="nitBase">La base del NIT a verificar.</param>
        /// <returns>Una tarea que retorna <c>true</c> si la consulta devuelve al menos un resultado (el NIT existe); de lo contrario, <c>false</c>.</returns>
        public async Task<bool> ExistsAsync(string nitBase)
        {
            var result = await _httpClient
                .GetFromJsonAsync<List<NitGovResponse>>($"c82u-588k.json?nit={nitBase}");

            return result != null && result.Count > 0;
        }
    }

    /// <summary>
    /// Representa la estructura de respuesta (DTO) de la consulta a la API de datos.gov.co para la validación de NIT.
    /// </summary>
    public class NitGovResponse
    {
        /// <summary>
        /// El Número de Identificación Tributaria completo (con dígito de verificación) encontrado.
        /// </summary>
        public string? NIT { get; set; }

        /// <summary>
        /// Código de la Cámara de Comercio a la que pertenece el NIT.
        /// </summary>
        public string? Codigo_Camara { get; set; }

        /// <summary>
        /// Número de Matrícula Mercantil asociado al NIT.
        /// </summary>
        public string? Matricula { get; set; }
    }
}
