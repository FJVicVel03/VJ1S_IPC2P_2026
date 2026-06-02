namespace Ejemplo1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=================================");
            Console.WriteLine("EJEMPLO NUMERO 1");
            Console.WriteLine("=================================");

            //Creacion e instanciar objetos de la clase Satelite
            Satelite satelite1 = new Satelite("S001", "Satelite 1", "127.0.0.1");
            Satelite satelite2 = new Satelite("S002", "Satelite 2", "192.168.20");
            Satelite satelite3 = new Satelite("S003", "Satelite 3", "124.123.54");

            Console.WriteLine("Estado Inicial de los satelites creados");
            Console.WriteLine(satelite1.ObtenerDescripcion());
            Console.WriteLine(satelite2.ObtenerDescripcion());
            Console.WriteLine(satelite3.ObtenerDescripcion());
            Console.WriteLine();

            //Modificar los atributos de los satelites
            satelite1.nombre = "Satelite 1 Modificado";
            satelite1.enlaceIP = "127.10.0.1";

            satelite2.nombre = "Satelite 2 Modificado";
            satelite2.enlaceIP = "192.24.20";

            satelite3.nombre = "Satelite 3 Modificado";
            satelite3.enlaceIP = "124.000.00";

            Console.WriteLine("Estado Final de los satelites modificados");
            Console.WriteLine(satelite1.ObtenerDescripcion());
            Console.WriteLine(satelite2.ObtenerDescripcion());
            Console.WriteLine(satelite3.ObtenerDescripcion());
            Console.WriteLine();

            
        }
    }
}