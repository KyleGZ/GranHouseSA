namespace GranHouseSA.Data
{
    public class HttpApi
    {
        public HttpClient Inicial()
        {
            //Se instancia un objeto HttpClient
            var client = new HttpClient();

            //Aquí le indicamos la direccion donde está la API
            client.BaseAddress = new Uri("https://apis.gometa.org");

            return client;
        }
    }
}