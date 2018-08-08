using AlumnoLibrary;
using RabbitMQ.Client;
using System;
using System.Text;
using Newtonsoft.Json;

namespace Send
{
    public class Program
    {
        static void Main(string[] args)
        {
            //var factory = new ConnectionFactory() { HostName = "localhost" };
            //using (var connection = factory.CreateConnection())
            //    using (var channel = connection.CreateModel())
            //{
            //    channel.QueueDeclare(queue: "hello",
            //                         durable: false,
            //                         exclusive: false,
            //                         autoDelete: false,
            //                         arguments: null);

            //    string message = "Hello World!";
            //    var body = Encoding.UTF8.GetBytes(message);

            //    channel.BasicPublish(exchange: "",
            //                         routingKey: "hello",
            //                         basicProperties: null,
            //                         body: body);

            //    Console.WriteLine("[x] Sent {0}", message);
            //}
            //}

            CommonService commonService = new CommonService();
            IConnection connection = commonService.GetRabbitMqConnection();
            IModel model = connection.CreateModel();
            SetupSerialisationMessageQueue(model);
            RunSerialisationDemo(model);
        }

        private static void SetupSerialisationMessageQueue(IModel model)
        {
            model.QueueDeclare(CommonService.SerialisationQueueName, true, false, false, null);
        }

        private static void RunSerialisationDemo(IModel model)
        {
            //Alumno al = new Alumno("El", "Guayaba", 288);
            Console.WriteLine("Enter alumno name. Quit with 'q'.");
            while (true)
            {
                string alumnoData = Console.ReadLine();
                if (alumnoData.ToLower() == "q") break;
                string[] namesArray = alumnoData.Split(',');
                Alumno alumno = new Alumno() { Nombre = namesArray[0], Apellidos = namesArray[1], Edad = Convert.ToInt32(namesArray[2]) };

                IBasicProperties basicProperties = model.CreateBasicProperties();
                basicProperties.Persistent = true;
                var jsonAlumno = JsonConvert.SerializeObject(alumno);
                byte[] alumnoBuffer = Encoding.UTF8.GetBytes(jsonAlumno);
                model.BasicPublish("", CommonService.SerialisationQueueName, basicProperties, alumnoBuffer);
            }
        }
    }
}
