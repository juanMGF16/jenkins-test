namespace Utilities.Helpers
{
    /// <summary>
    /// Helper para cálculo del dígito de verificación de NITs colombianos
    /// </summary>
    public static class NitHelper
    {
        /// <summary>
        /// Calcula el dígito de verificación de un NIT según el algoritmo oficial
        /// </summary>
        /// <param name="nitBase">NIT sin dígito de verificación</param>
        /// <returns>Dígito de verificación calculado</returns>
        public static int CalcularDV(string nitBase)
        {
            int[] factores = { 71, 67, 59, 53, 47, 43, 41, 37, 29, 23, 19, 17, 13, 7, 3 };
            int suma = 0;
            int nitLength = nitBase.Length;

            for (int i = 0; i < nitLength; i++)
            {
                int digito = int.Parse(nitBase[nitLength - 1 - i].ToString());
                suma += digito * factores[factores.Length - 1 - i];
            }

            int residuo = suma % 11;
            return residuo > 1 ? 11 - residuo : residuo;
        }
    }
}
