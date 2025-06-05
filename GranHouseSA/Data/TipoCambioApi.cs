using GranHouseSA.Models;
using Newtonsoft.Json;

namespace GranHouseSA.Data
{
    public class TipoCambioApi
    {
        private static HttpApi _api;

        public TipoCambio extraerTipoCambio()
        {
            //Reglas de negocio
            try
            { //Intento de conexion con la API

                //Se crea instancia de la API
                _api = new HttpApi();

                //Se obtiene el objeto cliente para consumir la API
                HttpClient client = _api.Inicial();

                //Se utiliza el metodo publico de la API Gometa
                HttpResponseMessage response = client.GetAsync("tdc/tdc.json").Result;

                if (response.IsSuccessStatusCode)
                {
                    //Aqui lee los datos obtenidos del objeto JSON
                    var result = response.Content.ReadAsStringAsync().Result;

                    //Se convierte el objeto JSON al objeto TipoCambio del modelo
                    return JsonConvert.DeserializeObject<TipoCambio>(result);
                }

            }//En caso de error capturamos con la ex
            catch (Exception ex)
            {

            }
            return null;
        }
    }
}
