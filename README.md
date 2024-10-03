# Bienal_Chaco_Back
Backend proyecto desarrollo de software 2024
prueba rama dev

#Nueva Arquitectura

- **BienalContext**: Biblioteca de Clases. Un Script por Entidad. Las entidades representan los objetos de negocio de la aplicación. Estas clases son mapeadas a tablas de la base de datos y se utilizan para definir la estructura de los datos que la aplicación manipula.
- **Services**: Biblioteca de Clases. Un script por Servicio. Cada servicio maneja la lógica de negocio y las operaciones que interactúan con servicios externos, como Azure Blob Storage, así como servicios internos que actúan como intermediarios entre la base de datos y las entidades de la aplicación (cada entidad tiene un servicio que lo maneja que implementa una interfaz, para que la entidad dependa de una interfaz y no de su servicio).
- **Contexts**: Biblioteca de Clases. Un script donde se encarga de la conexión con la base de datos, utilizando Entity Framework Core para gestionar el acceso y las operaciones en la base de datos. Además de la conexión va configuraciones como restricciones de la tabla.
- **Requests**: Biblioteca de Clases. Un script por Entidad, donde cada entidad tendrá varios requests. Esta parte de la arquitectura se centra en la gestión de las solicitudes entrantes, definiendo las estructuras de datos necesarias para procesar peticiones de los usuarios o de otros sistemas. Los requests permiten una validación y transformación de datos antes de que estos sean procesados por los servicios, garantizando que solo se manipulen datos válidos y bien formateados
